using System.Collections.Generic;
using Content.Server.Silicons.Borgs;
using Content.Shared.Access;
using Content.Shared.Access.Systems;
using Content.Shared.Actions;
using Content.Shared.Mind.Components;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.Silicons.StationAi;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Silicons.StationAi;

public sealed partial class StationAiSystem
{
    [Dependency] private BorgSystem _borg = default!;
    [Dependency] private SharedAccessSystem _accessSystem = default!;
    [Dependency] private SharedActionsSystem _actions = default!;

    private static readonly EntProtoId ShellReturnAction = "ActionStationAiReturnToCore";

    // Gives an AI-controlled shell the run of every door the AI itself could open.
    private static readonly ProtoId<AccessGroupPrototype> AllAccessGroup = "AllAccess";

    private void InitializeShellControl()
    {
        SubscribeLocalEvent<StationAiHeldComponent, StationAiControlShellActionEvent>(OnControlShellAction);
        SubscribeLocalEvent<StationAiHeldComponent, StationAiControlShellMessage>(OnControlShellSelect);

        SubscribeLocalEvent<AiRemoteControllableComponent, StationAiShellReturnEvent>(OnShellReturn);
        SubscribeLocalEvent<AiRemoteControllableComponent, PlayerAttachedEvent>(OnShellPlayerAttached);
        SubscribeLocalEvent<AiRemoteControllableComponent, PlayerDetachedEvent>(OnShellPlayerDetached);
    }

    private void OnControlShellAction(Entity<StationAiHeldComponent> ent, ref StationAiControlShellActionEvent args)
    {
        if (args.Handled || !TryComp<ActorComponent>(ent, out var actor))
            return;

        args.Handled = true;
        _ui.TryToggleUi(ent.Owner, StationAiControlShellUiKey.Key, actor.PlayerSession);
    }

    private void OnControlShellSelect(Entity<StationAiHeldComponent> ent, ref StationAiControlShellMessage args)
    {
        if (!TryGetEntity(args.Target, out var target) || !Exists(target))
            return;

        if (!HasComp<AiRemoteControllableComponent>(target))
            return;

        if (!_mind.TryGetMind(ent.Owner, out var mindId, out var mind))
            return;

        if (mind.VisitingEntity != null)
        {
            _popups.PopupEntity(Loc.GetString("station-ai-shell-already-controlling"), ent, ent.Owner);
            return;
        }

        if (!IsShellAvailable(target.Value))
        {
            _popups.PopupEntity(Loc.GetString("station-ai-shell-unavailable"), ent, ent.Owner);
            return;
        }

        // Mind-visit keeps the AI's core intact, so it can return to it (or is returned automatically
        // if the shell is destroyed) instead of being stranded in the shell.
        _mind.Visit(mindId, target.Value, mind);

        if (TryComp<ActorComponent>(ent, out var actor))
            _ui.CloseUi(ent.Owner, StationAiControlShellUiKey.Key, actor.PlayerSession);
    }

    private void OnShellReturn(Entity<AiRemoteControllableComponent> ent, ref StationAiShellReturnEvent args)
    {
        if (args.Handled || !TryComp<VisitingMindComponent>(ent, out var visiting) || visiting.MindId == null)
            return;

        args.Handled = true;
        _mind.UnVisit(visiting.MindId.Value);
    }

    private void OnShellPlayerAttached(Entity<AiRemoteControllableComponent> ent, ref PlayerAttachedEvent args)
    {
        // A shell only wakes up (full speed + modules + access) while the AI is actually controlling it.
        if (TryComp<BorgChassisComponent>(ent, out var chassis))
            _borg.TryActivate((ent.Owner, chassis));

        // Match the AI's reach: give the shell all-access so it can open every door the AI could.
        _accessSystem.TryAddGroups(ent.Owner, new[] { AllAccessGroup });
        _accessSystem.SetAccessEnabled(ent.Owner, true);

        // The AI can hop back to its core (or to another shell) at any time.
        _actions.AddAction(ent.Owner, ref ent.Comp.ReturnAction, ShellReturnAction);
    }

    private void OnShellPlayerDetached(Entity<AiRemoteControllableComponent> ent, ref PlayerDetachedEvent args)
    {
        if (TryComp<BorgChassisComponent>(ent, out var chassis))
            _borg.SetActive((ent.Owner, chassis), false);

        _accessSystem.SetAccessEnabled(ent.Owner, false);

        if (ent.Comp.ReturnAction is { } returnAction)
            _actions.RemoveAction(ent.Owner, returnAction);
        ent.Comp.ReturnAction = null;
    }

    private List<StationAiShellData> GetShellList()
    {
        var list = new List<StationAiShellData>();

        var query = AllEntityQuery<AiRemoteControllableComponent, MetaDataComponent>();
        while (query.MoveNext(out var uid, out _, out var meta))
        {
            list.Add(new StationAiShellData(GetNetEntity(uid), meta.EntityName, IsShellAvailable(uid)));
        }

        return list;
    }

    private bool IsShellAvailable(EntityUid uid)
    {
        // Unavailable if someone is already in it (a visiting AI or any attached player) or it's down.
        if (HasComp<VisitingMindComponent>(uid))
            return false;

        if (HasComp<ActorComponent>(uid))
            return false;

        return _mobState.IsAlive(uid);
    }
}
