using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

/// <summary>
/// Opens the player records browser, listing every user who ever connected. Optional argument pre-fills
/// the search filter (username substring or a full GUID).
/// </summary>
[AdminCommand(AdminFlags.Moderator)]
public sealed partial class PlayerRecordsCommand : LocalizedCommands
{
    [Dependency] private EuiManager _euis = default!;

    public override string Command => "playerrecords";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } admin)
        {
            shell.WriteError(Loc.GetString("cmd-playerrecords-server"));
            return;
        }

        var filter = args.Length > 0 ? args[0] : string.Empty;

        var ui = new PlayerRecordsEui(filter);
        _euis.OpenEui(ui, admin);
        ui.Load();
    }
}
