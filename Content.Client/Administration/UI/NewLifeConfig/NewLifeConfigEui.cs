using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Client.Administration.UI.NewLifeConfig;

[UsedImplicitly]
public sealed class NewLifeConfigEui : BaseEui
{
    private readonly NewLifeConfigWindow _window;

    public NewLifeConfigEui()
    {
        _window = new NewLifeConfigWindow();

        _window.OnSetEnabled += value => SendMessage(new NewLifeSetEnabledMessage(value));
        _window.OnSetKeepAntag += value => SendMessage(new NewLifeSetKeepAntagMessage(value));
        _window.OnSetCooldown += value => SendMessage(new NewLifeSetCooldownMessage(value));
        _window.OnSetMax += value => SendMessage(new NewLifeSetMaxMessage(value));
        _window.OnSetActiveProfile += profile => SendMessage(new NewLifeSetActiveProfileMessage(profile));
        _window.OnCreateProfile += name => SendMessage(new NewLifeCreateProfileMessage(name));
        _window.OnDeleteProfile += () => SendMessage(new NewLifeDeleteProfileMessage());
        _window.OnSetEvent += (id, blocked) => SendMessage(new NewLifeSetEventMessage(id, blocked));
        _window.OnClose += () => SendMessage(new CloseEuiMessage());
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is NewLifeConfigEuiState s)
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
