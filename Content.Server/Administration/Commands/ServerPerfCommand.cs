using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Debug)]
public sealed partial class ServerPerfCommand : LocalizedCommands
{
    [Dependency] private EuiManager _euis = default!;

    public override string Command => "serverperf";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } admin)
        {
            shell.WriteError(Loc.GetString("cmd-serverperf-server"));
            return;
        }

        var ui = new ServerPerfEui();
        _euis.OpenEui(ui, admin);
    }
}
