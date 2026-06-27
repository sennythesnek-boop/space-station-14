using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Client.Administration.UI.Migrations;

[UsedImplicitly]
public sealed class MigrationsEui : BaseEui
{
    private readonly MigrationsWindow _window;

    public MigrationsEui()
    {
        _window = new MigrationsWindow();
        _window.OnApprove += id => SendMessage(new MigrationApproveMessage(id));
        _window.OnReject += id => SendMessage(new MigrationRejectMessage(id));
        _window.OnManual += (source, target, scope) => SendMessage(new MigrationManualMessage(source, target, scope));
        _window.OnClose += () => SendMessage(new CloseEuiMessage());
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is MigrationsEuiState s)
            _window.SetState(s);
    }

    public override void Opened()
    {
        _window.OpenCentered();
    }

    public override void Closed()
    {
        _window.Close();
    }
}
