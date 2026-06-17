using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Admin)]
public sealed partial class VotingConfigCommand : LocalizedCommands
{
    [Dependency] private EuiManager _euis = default!;

    public override string Command => "votingconfig";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } admin)
        {
            shell.WriteError(Loc.GetString("cmd-votingconfig-server"));
            return;
        }

        var ui = new VoteConfigEui();
        _euis.OpenEui(ui, admin);
        ui.BuildState();
    }
}
