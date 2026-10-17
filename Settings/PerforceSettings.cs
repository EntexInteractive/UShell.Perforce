// Copyright 2026 Entex Interactive

using Spectre.Console.Cli;

namespace UShell.Perforce.Settings
{
    public class PerforceSettings : CommandSettings
    {
        [CommandOption("-p|--host <PORT>")]
        public string? Host { get; init; }

        [CommandOption("-u|--user <USER>")]
        public string? User { get; init; }

        [CommandOption("-c|--workspace <CLIENT>")]
        public string? Workspace { get; init; }
    }
}