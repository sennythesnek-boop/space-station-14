using Content.Client.Changelog;
using JetBrains.Annotations;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Systems.EscapeMenu;

/// <summary>
///     Drives a second changelog window for the fork's own changelog, loaded from
///     <c>/IssChangelog</c> instead of the upstream <c>/Changelog</c>. It reuses
///     <see cref="ChangelogWindow"/> so it gets the same Changelog/Rules/Maps/Admin tabs.
/// </summary>
[UsedImplicitly]
public sealed class IssChangelogUIController : UIController
{
    private ChangelogWindow _window = default!;

    public void OpenWindow()
    {
        EnsureWindow();

        _window.OpenCentered();
        _window.MoveToFront();
    }

    private void EnsureWindow()
    {
        if (_window is { Disposed: false })
            return;

        _window = UIManager.CreateWindow<ChangelogWindow>();
        _window.ChangelogDirectory = "/IssChangelog";
        _window.TrackReadId = false;
        _window.Title = Loc.GetString("iss-changelog-window-title");
    }

    public void ToggleWindow()
    {
        EnsureWindow();

        if (_window.IsOpen)
        {
            _window.Close();
        }
        else
        {
            OpenWindow();
        }
    }
}
