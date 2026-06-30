using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Server)]
public sealed partial class NewLifeConfigCommand : LocalizedCommands
{
    [Dependency] private EuiManager _euis = default!;

    public override string Command => "newlifeconfig";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } admin)
        {
            shell.WriteError(Loc.GetString("cmd-newlifeconfig-server"));
            return;
        }

        var ui = new NewLifeConfigEui();
        _euis.OpenEui(ui, admin);
        ui.BuildState();
    }
}
