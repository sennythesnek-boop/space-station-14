using System.Linq;
using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Admin)]
public sealed class RoleTimesCommand : LocalizedCommands
{
    [Dependency] private readonly IPlayerLocator _locator = default!;
    [Dependency] private readonly EuiManager _euis = default!;
    [Dependency] private readonly IPlayerManager _players = default!;

    public override string Command => "roletimes";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } admin)
        {
            shell.WriteError(Loc.GetString("cmd-roletimes-server"));
            return;
        }

        if (args.Length != 1)
        {
            shell.WriteError(Loc.GetString("cmd-roletimes-invalid-arguments"));
            return;
        }

        // LookupIdByNameOrIdAsync resolves both online and offline players (by name or user id).
        var located = await _locator.LookupIdByNameOrIdAsync(args[0]);
        if (located == null)
        {
            shell.WriteError(Loc.GetString("cmd-roletimes-invalid-player"));
            return;
        }

        var ui = new RoleTimesEui(located);
        _euis.OpenEui(ui, admin);
        ui.LoadTimes();
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            var options = _players.Sessions.OrderBy(c => c.Name).Select(c => c.Name).ToArray();
            return CompletionResult.FromHintOptions(options, Loc.GetString("cmd-roletimes-completion"));
        }

        return CompletionResult.Empty;
    }
}
