using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

/// <summary>
/// Opens the migration oversight tool: review automatic/pending migrations and manually transfer data
/// between players. Available to moderators; migrating <i>admin status</i> still requires the Admin flag
/// (enforced in <see cref="MigrationsEui"/>) so it can't be used for privilege escalation.
/// </summary>
[AdminCommand(AdminFlags.Moderator)]
public sealed partial class MigrationsCommand : LocalizedCommands
{
    [Dependency] private EuiManager _euis = default!;

    public override string Command => "migrations";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } admin)
        {
            shell.WriteError(Loc.GetString("cmd-migrations-server"));
            return;
        }

        var ui = new MigrationsEui();
        _euis.OpenEui(ui, admin);
        ui.Load();
    }
}
