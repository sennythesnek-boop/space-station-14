using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Server)]
public sealed class TtsConfigCommand : LocalizedCommands
{
    [Dependency] private readonly EuiManager _euis = default!;

    public override string Command => "ttsconfig";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } admin)
        {
            shell.WriteError(Loc.GetString("cmd-ttsconfig-server"));
            return;
        }

        var ui = new TtsConfigEui();
        _euis.OpenEui(ui, admin);
        ui.BuildState();
    }
}
