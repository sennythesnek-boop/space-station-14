using System.Numerics;
using Content.Shared.ActionBlocker;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Vehicle;

/// <summary>
/// Wires a hoverboard rider's input to the board: when someone buckles, their movement is relayed to the board
/// (so they drive it); when they unbuckle, the relay is cleared and the board stops.
/// </summary>
public sealed partial class SharedHoverboardSystem : EntitySystem
{
    [Dependency] private SharedMoverController _mover = default!;
    [Dependency] private SharedPhysicsSystem _physics = default!;
    [Dependency] private ActionBlockerSystem _blocker = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<HoverboardComponent, StrappedEvent>(OnStrapped);
        SubscribeLocalEvent<HoverboardComponent, UnstrappedEvent>(OnUnstrapped);

        // Buckling normally cancels CanMove (you can't walk while strapped to a chair). For a hoverboard rider we
        // need CanMove to stay on so their input is relayed to the board, so we un-cancel after the buckle system runs.
        SubscribeLocalEvent<HoverboardRiderComponent, UpdateCanMoveEvent>(OnRiderUpdateCanMove,
            after: new[] { typeof(SharedBuckleSystem) });
    }

    private void OnStrapped(Entity<HoverboardComponent> ent, ref StrappedEvent args)
    {
        // Mark the rider first so the CanMove recompute inside SetRelay sees it and leaves movement enabled.
        EnsureComp<HoverboardRiderComponent>(args.Buckle.Owner);

        // Relay the rider's movement input to the board so they drive it (see SharedMoverController.Hoverboard).
        _mover.SetRelay(args.Buckle.Owner, ent.Owner);
    }

    private void OnUnstrapped(Entity<HoverboardComponent> ent, ref UnstrappedEvent args)
    {
        RemComp<HoverboardRiderComponent>(args.Buckle.Owner);

        if (HasComp<RelayInputMoverComponent>(args.Buckle.Owner))
            RemCompDeferred<RelayInputMoverComponent>(args.Buckle.Owner);

        // Recompute now that the rider is no longer a hoverboard rider (re-applies normal movement rules).
        _blocker.UpdateCanMove(args.Buckle.Owner);

        if (TryComp<PhysicsComponent>(ent, out var physics))
            _physics.SetLinearVelocity(ent.Owner, Vector2.Zero, body: physics);
    }

    private void OnRiderUpdateCanMove(EntityUid uid, HoverboardRiderComponent comp, UpdateCanMoveEvent args)
    {
        // Keep the rider able to "move" — their input is forwarded to the board, they don't actually walk off it.
        args.Uncancel();
    }
}
