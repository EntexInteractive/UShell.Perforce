// Copyright 2026 Entex Interactive

using Perforce.P4;
using Spectre.Console;
using Spectre.Console.Cli;
using UShell.Perforce.Settings;

namespace UShell.Perforce.Commands
{
    public sealed class LatestCommand : AsyncCommand<LatestSettings>
    {
        public override Task<int> ExecuteAsync(CommandContext context, LatestSettings settings, CancellationToken cancellationToken)
        {
            string address = settings.Host ?? new ServerAddress(string.Empty).Uri;
            string workspace = settings.Workspace ?? Environment.GetEnvironmentVariable("P4CLIENT") ?? string.Empty;
            string user = settings.User ?? Environment.GetEnvironmentVariable("P4USER") ?? string.Empty;
            
            Server server = new Server(new ServerAddress(address));
            Repository repository = new Repository(server);
            Connection connection = repository.Connection;
            connection.Client = new Client
            {
                Name = workspace
            };
            
            connection.Connect(new Options());
            
            try
            {
                Client client = connection.Client;

                Console.WriteLine($"P4PORT: {server.Address.Uri}");
                Console.WriteLine($"P4CLIENT: {client.Name}");

                if (settings.DryRun)
                {
                    Options previewOptions = new Options(SyncFilesCmdFlags.Preview);
                    client.SyncFiles(previewOptions);

                    AnsiConsole.MarkupLine("[yellow]Preview complete. No files were synced.[/]");
                    return Task.FromResult(0);
                }

                AnsiConsole.Status().Start("Getting latest changes...", _ => { client.SyncFiles(new Options()); });
                AnsiConsole.MarkupLine("[green]Workspace synced with depot.[/]");
                return Task.FromResult(0);
            }
            finally
            {
                connection.Disconnect();
            }
        }
    }
    
    public sealed class LatestSettings : PerforceSettings
    {
        [CommandOption("-n|--dry-run")]
        public bool DryRun { get; init; }
    }
}