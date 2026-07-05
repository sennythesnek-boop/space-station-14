using Content.Server.Administration.Managers;
using Content.Server.Administration.Performance;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Eui;

namespace Content.Server.Administration;

/// <summary>
/// Server side of the server performance admin window (<c>serverperf</c>).
/// Pushes a fresh snapshot once per second while open. Read-only.
/// </summary>
public sealed partial class ServerPerfEui : BaseEui
{
    [Dependency] private IAdminManager _admins = default!;
    [Dependency] private IEntityManager _entity = default!;

    private PerformanceMonitorSystem _monitor = default!;

    public ServerPerfEui()
    {
        IoCManager.InjectDependencies(this);
    }

    public override void Opened()
    {
        base.Opened();
        _admins.OnPermsChanged += OnPermsChanged;
        _monitor = _entity.System<PerformanceMonitorSystem>();
        _monitor.OnSample += OnSample;
        StateDirty();
    }

    public override void Closed()
    {
        base.Closed();
        _admins.OnPermsChanged -= OnPermsChanged;
        _monitor.OnSample -= OnSample;
    }

    private void OnSample()
    {
        if (!CanView())
        {
            Close();
            return;
        }

        StateDirty();
    }

    private void OnPermsChanged(AdminPermsChangedEventArgs args)
    {
        if (args.Player == Player && !CanView())
            Close();
    }

    private bool CanView() => _admins.HasAdminFlag(Player, AdminFlags.Debug);

    public override EuiStateBase GetNewState() => _monitor.GetState();
}
