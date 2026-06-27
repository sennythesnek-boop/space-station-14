using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Barks;

/// <summary>
/// Attached to entities that should play a bark voice when they speak.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SpeechSynthesisComponent : Component
{
    [DataField("voice"), AutoNetworkedField]
    public ProtoId<BarkPrototype>? VoicePrototypeId;
}
