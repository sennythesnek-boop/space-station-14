using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Client.Administration.UI.TtsConfig;

[UsedImplicitly]
public sealed class TtsConfigEui : BaseEui
{
    private readonly TtsConfigWindow _window;

    public TtsConfigEui()
    {
        _window = new TtsConfigWindow();

        _window.OnSet += (field, value) => SendMessage(new TtsConfigSetMessage(field, value));
        _window.OnSetVoiceGender += (speaker, gender) => SendMessage(new TtsConfigSetVoiceGenderMessage(speaker, gender));
        _window.OnSetAllGenders += gender => SendMessage(new TtsConfigSetAllGenderMessage(gender));
        _window.OnAutoGenders += () => SendMessage(new TtsConfigAutoGenderMessage());
        _window.OnRefreshSpeakers += () => SendMessage(new TtsConfigRefreshSpeakersMessage());
        _window.OnTest += (text, speaker) => SendMessage(new TtsConfigTestMessage(text, speaker));
        _window.OnClose += () => SendMessage(new CloseEuiMessage());
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is TtsConfigEuiState s)
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
