// Copyright 2026 Entex Interactive

using Spectre.Console.Cli;
using UShell.Core.Interfaces;
using UShell.Perforce.Commands;

namespace UShell.Perforce
{
    public class PerforceModule : ICommandModule
    {
        public void Register(IConfigurator config)
        {
            config.AddBranch("p4", p4 =>
            {
                p4.AddCommand<InfoCommand>("info").WithDescription("Displays the server information.");
                p4.AddCommand<ChangesCommand>("changes").WithDescription("Reconciles the workspace for latest changes.");
                p4.AddCommand<LatestCommand>("latest").WithDescription("Gets the latest changes from the depot.");
            });
        }
    }
}