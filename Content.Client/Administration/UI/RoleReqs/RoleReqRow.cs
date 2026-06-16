using System;
using Content.Shared.Administration;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Administration.UI.RoleReqs;

/// <summary>One requirement row inside a job section: a wrapping summary, and (if editable) a time field,
/// a Set button, an Inverted toggle, and a Remove button.</summary>
public sealed class RoleReqRow : BoxContainer
{
    public event Action<TimeSpan>? OnEditTime;
    public event Action<bool>? OnSetInverted;
    public event Action? OnRemove;

    public RoleReqRow(RoleReqEntry entry, bool canEdit)
    {
        Orientation = LayoutOrientation.Horizontal;
        HorizontalExpand = true;
        Margin = new Thickness(16, 1, 0, 1);

        var label = new RichTextLabel { MaxWidth = 300, HorizontalExpand = true };
        label.SetMessage(FormattedMessage.FromUnformatted(entry.Display));
        AddChild(label);

        if (entry.Editable)
        {
            var time = new LineEdit
            {
                Text = RoleReqTimeFormat.Format(entry.Time),
                MinWidth = 78,
                Editable = canEdit,
            };

            var set = new Button
            {
                Text = Loc.GetString("role-req-editor-set"),
                Disabled = !canEdit,
                Margin = new Thickness(4, 0, 0, 0),
            };
            set.OnPressed += _ =>
            {
                if (RoleReqTimeFormat.TryParse(time.Text, out var ts))
                    OnEditTime?.Invoke(ts);
            };

            var inverted = new CheckBox
            {
                Text = Loc.GetString("role-req-editor-inverted"),
                Pressed = entry.Inverted,
                Disabled = !canEdit,
                Margin = new Thickness(6, 0, 0, 0),
            };
            inverted.OnToggled += args => OnSetInverted?.Invoke(args.Pressed);

            AddChild(time);
            AddChild(set);
            AddChild(inverted);
        }

        var remove = new Button
        {
            Text = Loc.GetString("role-req-editor-remove"),
            Disabled = !canEdit,
            Margin = new Thickness(6, 0, 0, 0),
        };
        remove.OnPressed += _ => OnRemove?.Invoke();
        AddChild(remove);
    }
}
