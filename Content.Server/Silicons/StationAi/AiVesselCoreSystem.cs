using Content.Server.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.Silicons.StationAi;
using Robust.Shared.Containers;

namespace Content.Server.Silicons.StationAi;

/// <summary>
/// Makes a cyborg AI-controllable while an <see cref="AiVesselCoreComponent"/> item is installed in its
/// brain slot. Installing/removing the core is done through the normal borg brain flow (open panel,
/// click the core onto the chassis), so this just tracks the marker and ejects any controlling AI when
/// the core is pulled.
/// </summary>
public sealed partial class AiVesselCoreSystem : EntitySystem
{
    [Dependency] private MindSystem _mind = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AiVesselCoreComponent, EntGotInsertedIntoContainerMessage>(OnInserted);
        SubscribeLocalEvent<AiVesselCoreComponent, EntGotRemovedFromContainerMessage>(OnRemoved);
    }

    private void OnInserted(Entity<AiVesselCoreComponent> ent, ref EntGotInsertedIntoContainerMessage args)
    {
        if (!IsBrainSlot(args.Container, out var chassis))
            return;

        EnsureComp<AiRemoteControllableComponent>(chassis);
    }

    private void OnRemoved(Entity<AiVesselCoreComponent> ent, ref EntGotRemovedFromContainerMessage args)
    {
        if (!IsBrainSlot(args.Container, out var chassis))
            return;

        // If a station AI is currently inhabiting this body, eject it back to its core before the
        // chassis stops being controllable.
        if (TryComp<VisitingMindComponent>(chassis, out var visiting) && visiting.MindId != null)
            _mind.UnVisit(visiting.MindId.Value);

        RemComp<AiRemoteControllableComponent>(chassis);
    }

    private bool IsBrainSlot(BaseContainer container, out EntityUid chassis)
    {
        chassis = container.Owner;

        return TryComp<BorgChassisComponent>(container.Owner, out var borg) && container.ID == borg.BrainContainerId;
    }
}
