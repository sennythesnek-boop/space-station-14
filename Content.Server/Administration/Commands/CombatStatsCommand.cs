using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Logs)]
public sealed partial class CombatStatsCommand : LocalizedCommands
{
    [Dependency] private EuiManager _euis = default!;

    public override string Command => "combatstats";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } admin)
        {
            shell.WriteError(Loc.GetString("cmd-combatstats-server"));
            return;
        }

        var ui = new CombatStatsEui();
        _euis.OpenEui(ui, admin);
        ui.BuildState();
    }
}
