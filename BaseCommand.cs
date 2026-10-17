// Copyright 2026 Entex Interactive

using Perforce.P4;
using Spectre.Console;
using Spectre.Console.Cli;
using UShell.Perforce.Settings;

namespace UShell.Perforce
{
    public abstract class BaseCommand<TSettings> : AsyncCommand<TSettings> where TSettings : PerforceSettings
    {
        protected Repository Repository { get; private set; } = null!;
        protected Connection Connection { get; private set; } = null!;
        protected Client Client { get; private set; } = null!;

        public sealed override async Task<int> ExecuteAsync(CommandContext context, TSettings settings, CancellationToken cancellationToken)
        {
            try
            {
                string address = settings.Host ?? new ServerAddress(string.Empty).Uri;
                string workspace = settings.Workspace ?? Environment.GetEnvironmentVariable("P4CLIENT") ?? string.Empty;
                string user = settings.User ?? Environment.GetEnvironmentVariable("P4USER") ?? string.Empty;

                Server server = new Server(new ServerAddress(address));
                Repository = new Repository(server);
                Connection = Repository.Connection;

                if (!string.IsNullOrWhiteSpace(user)) Connection.UserName = user;
                if (!string.IsNullOrWhiteSpace(workspace))
                {
                    Connection.Client = new Client
                    {
                        Name = workspace
                    };
                }

                Connection.Connect(new Options());

                Client = Connection.Client ?? throw new InvalidOperationException("No Perforce workspace is selected.");
                return await ExecuteP4Async(context, settings, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                AnsiConsole.MarkupLine("\n[yellow]Command cancelled.[/]");
                return 130;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Perforce command failed:[/] " + $"{Markup.Escape(ex.Message)}");
                return 1;
            }
            finally
            {
                Connection.Disconnect();
            }
        }

        protected abstract Task<int> ExecuteP4Async(CommandContext context, TSettings settings, CancellationToken cancellationToken);
    }
}