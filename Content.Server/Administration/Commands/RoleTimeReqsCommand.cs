using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Server)]
public sealed partial class RoleTimeReqsCommand : LocalizedCommands
{
    [Dependency] private EuiManager _euis = default!;

    public override string Command => "roletimereqs";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } admin)
        {
            shell.WriteError(Loc.GetString("cmd-roletimereqs-server"));
            return;
        }

        var ui = new RoleReqEditorEui();
        _euis.OpenEui(ui, admin);
        ui.BuildState();
    }
}
