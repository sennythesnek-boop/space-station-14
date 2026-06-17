using System;
using System.Numerics;
using Content.Shared.Movement.Components;
using Content.Shared.Vehicle;
using Robust.Shared.Physics.Components;

namespace Content.Shared.Movement.Systems;

public abstract partial class SharedMoverController
{
    /// <summary>
    /// Car-like movement for a hoverboard: A/D rotate the board, W/S thrust forward/reverse along its facing with
    /// acceleration up to a max speed, and it coasts to a stop with friction when no thrust is applied. Runs only
    /// for entities with <see cref="HoverboardComponent"/>, so normal player movement is untouched.
    /// </summary>
    private void HandleHoverboardMovement(
        Entity<HoverboardComponent> ent,
        InputMoverComponent mover,
        PhysicsComponent physics,
        TransformComponent xform,
        float frameTime)
    {
        var comp = ent.Comp;
        var buttons = mover.HeldMoveButtons;

        // Steering: A turns left (CCW), D turns right (CW).
        var rotInput = 0f;
        if ((buttons & MoveButtons.Left) != 0)
            rotInput += 1f;
        if ((buttons & MoveButtons.Right) != 0)
            rotInput -= 1f;

        if (rotInput != 0f)
            _transform.SetLocalRotation(ent.Owner, xform.LocalRotation + rotInput * comp.RotationSpeed * frameTime, xform);

        // Forward unit vector from the board's facing. ToWorldVec() (not ToVec) so rotation 0 == "south"/(0,-1),
        // matching the engine's sprite-direction convention — the board's nose visually leads the way it drives.
        var forward = _transform.GetWorldRotation(ent.Owner).ToWorldVec();

        // Current speed along the facing (so a turning board keeps its momentum along the new heading).
        var curSpeed = Vector2.Dot(physics.LinearVelocity, forward);

        var fwd = (buttons & MoveButtons.Up) != 0;
        var back = (buttons & MoveButtons.Down) != 0;

        float newSpeed;
        if (fwd && !back)
            newSpeed = MathF.Min(curSpeed + comp.Acceleration * frameTime, comp.MaxSpeed);
        else if (back && !fwd)
            newSpeed = MathF.Max(curSpeed - comp.Acceleration * frameTime, -comp.ReverseMaxSpeed);
        else
        {
            // Coast: bleed speed toward zero.
            var decel = comp.Friction * frameTime;
            newSpeed = MathF.Abs(curSpeed) <= decel ? 0f : curSpeed - MathF.Sign(curSpeed) * decel;
        }

        PhysicsSystem.SetLinearVelocity(ent.Owner, forward * newSpeed, body: physics);
        PhysicsSystem.SetAngularVelocity(ent.Owner, 0f, body: physics);
    }
}
