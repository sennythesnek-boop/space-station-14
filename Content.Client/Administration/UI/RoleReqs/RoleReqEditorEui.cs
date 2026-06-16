using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Client.Administration.UI.RoleReqs;

[UsedImplicitly]
public sealed class RoleReqEditorEui : BaseEui
{
    private readonly RoleReqEditorWindow _window;

    public RoleReqEditorEui()
    {
        _window = new RoleReqEditorWindow();

        _window.OnSetTimers += v => SendMessage(new RoleReqSetTimersEnabledMessage(v));
        _window.OnSetOverrides += v => SendMessage(new RoleReqSetOverridesEnabledMessage(v));
        _window.OnEditTime += (job, idx, time) => SendMessage(new RoleReqEditTimeMessage(job, idx, time));
        _window.OnSetInverted += (job, idx, inv) => SendMessage(new RoleReqSetInvertedMessage(job, idx, inv));
        _window.OnRemove += (job, idx) => SendMessage(new RoleReqRemoveMessage(job, idx));
        _window.OnAdd += (job, kind, target, time, inv) => SendMessage(new RoleReqAddMessage(job, kind, target, time, inv));
        _window.OnResetJob += job => SendMessage(new RoleReqResetJobMessage(job));
        _window.OnSaveProfile += name => SendMessage(new RoleReqSaveProfileMessage(name));
        _window.OnLoadProfile += name => SendMessage(new RoleReqLoadProfileMessage(name));
        _window.OnDeleteProfile += name => SendMessage(new RoleReqDeleteProfileMessage(name));
        _window.OnImport += () => SendMessage(new RoleReqImportPrototypeMessage());
        _window.OnClose += () => SendMessage(new CloseEuiMessage());
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is RoleReqEditorState s)
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
