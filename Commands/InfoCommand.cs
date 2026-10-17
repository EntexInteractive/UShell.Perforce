// Copyright 2026 Entex Interactive

using Perforce.P4;
using Spectre.Console;
using Spectre.Console.Cli;
using UShell.Perforce.Settings;

namespace UShell.Perforce.Commands
{
    public sealed class InfoCommand : BaseCommand<PerforceSettings>
    {
        protected override Task<int> ExecuteP4Async(
            CommandContext context,
            PerforceSettings settings,
            CancellationToken cancellationToken)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[grey]Property[/]")
                .AddColumn("[grey]Value[/]");

            void AddRow(string name, object? value)
            {
                table.AddRow(
                    Markup.Escape(name),
                    Markup.Escape(value?.ToString() ?? "Unknown"));
            }
            
            ServerMetaData metadata = Repository.Server.Metadata;
            ServerVersion? version = metadata?.Version;
            
            AddRow("Client host", Client?.Host);
            AddRow("User name", Connection.UserName);
            AddRow("Client stream", Client?.Stream);
            AddRow("Workspace name", Client?.Name);
            AddRow("Client root", Client?.Root);
            AddRow("Current directory", Environment.CurrentDirectory);
            AddRow("Server address", Repository.Server.Address);
            AddRow("Server date", metadata?.Date);
            AddRow("Server ID", metadata?.Name);
            AddRow("Server license", metadata?.License?.License);
            AddRow("Server root", metadata?.Root);
            AddRow("Server version", version is null ? "Unknown" : $"{version.Product}/{version.Platform}/{version.Major}.{version.Minor} ({version.Date:yyyy/MM/dd})");
            AddRow("Unicode enabled", metadata?.UnicodeEnabled);
            AnsiConsole.Write(table);

            return Task.FromResult(0);
        }
    }
}