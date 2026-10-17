// Copyright 2026 Entex Interactive

using Perforce.P4;
using Spectre.Console;
using Spectre.Console.Cli;
using UShell.Perforce.Settings;

namespace UShell.Perforce.Commands
{
    public sealed class ChangesCommand : AsyncCommand<PerforceSettings>
    {
        public override Task<int> ExecuteAsync(
            CommandContext context,
            PerforceSettings settings,
            CancellationToken cancellationToken)
        {
            try
            {
                string address = settings.Host ?? new ServerAddress(string.Empty).Uri;
                string workspace = settings.Workspace ?? Environment.GetEnvironmentVariable("P4CLIENT") ?? string.Empty;
                string user = settings.User ?? Environment.GetEnvironmentVariable("P4USER") ?? string.Empty;

                Server server = new Server(new ServerAddress(address));
                Repository repository = new Repository(server);
                Connection connection = repository.Connection;

                connection.UserName = user;
                connection.Client = new Client
                {
                    Name = workspace
                };

                connection.Connect(new Options());

                try
                {
                    Client client = connection.Client;
                    string root = client.Root;

                    if (string.IsNullOrWhiteSpace(root)) throw new InvalidOperationException($"Workspace '{workspace}' has no configured root.");
                    FileSpec path = new FileSpec(new LocalPath(Path.Combine(root, "...")), null);
                    
                    IList<FileSpec>? results = null;
                    AnsiConsole.Status().Start("Reconciling workspace...", _ => { results = client.ReconcileFiles(new Options(), path, null); });
                    
                    int count = results?.Count ?? 0;
                    AnsiConsole.MarkupLine(count == 0 ? "[yellow]No files with changes found.[/]" : $"[green]{count} files have changes.[/]");
                    return Task.FromResult(0);
                }
                finally
                {
                    connection.Disconnect();
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine(
                    $"[red]Reconcile failed:[/] {Markup.Escape(ex.Message)}");

                return Task.FromResult(1);
            }
        }
    }

    public sealed class ChangesSettings : PerforceSettings
    {
        [CommandArgument(0, "[PATHS]")] public string[] Paths { get; init; } = [];
    }
}