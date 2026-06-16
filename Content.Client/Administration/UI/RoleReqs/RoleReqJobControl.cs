using System;
using System.Collections.Generic;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.RoleReqs;

/// <summary>A collapsible per-job section: header (with reset), the requirement rows, and an "add" row.</summary>
public sealed class RoleReqJobControl : BoxContainer
{
    public event Action<int, TimeSpan>? OnEditTime;
    public event Action<int, bool>? OnSetInverted;
    public event Action<int>? OnRemove;
    public event Action<RoleReqKind, string, TimeSpan, bool>? OnAdd;
    public event Action? OnReset;

    /// <summary>Raised when the section is expanded/collapsed, so the window can remember the state.</summary>
    public event Action<bool>? OnExpandChanged;

    public string JobName { get; }
    public string JobId { get; }

    private readonly BoxContainer _body;
    private readonly Button _toggle;
    private readonly RoleReqJobInfo _info;

    private readonly IReadOnlyList<(string Value, string Display)> _roleOptions;
    private readonly IReadOnlyList<(string Value, string Display)> _deptOptions;

    public RoleReqJobControl(
        RoleReqJobInfo info,
        bool canEdit,
        bool startExpanded,
        IReadOnlyList<(string Value, string Display)> roleOptions,
        IReadOnlyList<(string Value, string Display)> deptOptions)
    {
        _info = info;
        _roleOptions = roleOptions;
        _deptOptions = deptOptions;
        JobName = info.JobName;
        JobId = info.JobId;

        Orientation = LayoutOrientation.Vertical;
        HorizontalExpand = true;

        var header = new BoxContainer { Orientation = LayoutOrientation.Horizontal, HorizontalExpand = true };

        _toggle = new Button { HorizontalExpand = true };
        _toggle.Label.Align = Label.AlignMode.Left;

        var reset = new Button
        {
            Text = Loc.GetString("role-req-editor-reset"),
            Disabled = !canEdit || !info.Overridden,
            Margin = new Thickness(4, 0, 0, 0),
        };
        reset.OnPressed += _ => OnReset?.Invoke();

        header.AddChild(_toggle);
        header.AddChild(reset);
        AddChild(header);

        _body = new BoxContainer { Orientation = LayoutOrientation.Vertical, HorizontalExpand = true, Visible = startExpanded };
        AddChild(_body);

        _toggle.OnPressed += _ =>
        {
            _body.Visible = !_body.Visible;
            UpdateHeader();
            OnExpandChanged?.Invoke(_body.Visible);
        };
        UpdateHeader();

        foreach (var entry in info.Requirements)
        {
            var idx = entry.Index;
            var row = new RoleReqRow(entry, canEdit);
            row.OnEditTime += t => OnEditTime?.Invoke(idx, t);
            row.OnSetInverted += inv => OnSetInverted?.Invoke(idx, inv);
            row.OnRemove += () => OnRemove?.Invoke(idx);
            _body.AddChild(row);
        }

        if (canEdit)
            _body.AddChild(BuildAddRow());
    }

    private void UpdateHeader()
    {
        var arrow = _body.Visible ? "▼" : "►";
        var star = _info.Overridden ? " *" : "";
        _toggle.Text = $"{arrow} {_info.JobName}  ({_info.Requirements.Count}){star}";
    }

    private Control BuildAddRow()
    {
        var row = new BoxContainer
        {
            Orientation = LayoutOrientation.Horizontal,
            HorizontalExpand = true,
            Margin = new Thickness(16, 2, 0, 6),
            VerticalAlignment = VAlignment.Top,
        };

        var kind = new OptionButton();
        kind.AddItem(Loc.GetString("role-req-editor-kind-overall"), (int) RoleReqKind.Overall);
        kind.AddItem(Loc.GetString("role-req-editor-kind-role"), (int) RoleReqKind.Role);
        kind.AddItem(Loc.GetString("role-req-editor-kind-department"), (int) RoleReqKind.Department);
        kind.SelectId((int) RoleReqKind.Overall);

        var target = new SearchableOptionBox(Loc.GetString("role-req-editor-target-placeholder"))
        {
            Margin = new Thickness(4, 0, 0, 0),
        };

        var time = new LineEdit { PlaceHolder = "HH:MM", MinWidth = 70, Margin = new Thickness(4, 0, 0, 0) };
        var inverted = new CheckBox { Text = Loc.GetString("role-req-editor-inverted"), Margin = new Thickness(6, 0, 0, 0) };
        var add = new Button { Text = Loc.GetString("role-req-editor-add"), Margin = new Thickness(6, 0, 0, 0) };

        void UpdateTarget()
        {
            switch ((RoleReqKind) kind.SelectedId)
            {
                case RoleReqKind.Role:
                    target.SetOptions(_roleOptions);
                    target.Visible = true;
                    break;
                case RoleReqKind.Department:
                    target.SetOptions(_deptOptions);
                    target.Visible = true;
                    break;
                default:
                    target.Clear();
                    target.Visible = false;
                    break;
            }
        }

        UpdateTarget();

        kind.OnItemSelected += args =>
        {
            kind.SelectId(args.Id);
            UpdateTarget();
        };

        add.OnPressed += _ =>
        {
            if (!RoleReqTimeFormat.TryParse(time.Text, out var ts))
                return;

            var k = (RoleReqKind) kind.SelectedId;
            string tgt;
            if (k == RoleReqKind.Overall)
            {
                tgt = string.Empty;
            }
            else
            {
                var resolved = target.Resolve();
                if (resolved == null)
                    return;
                tgt = resolved;
            }

            OnAdd?.Invoke(k, tgt, ts, inverted.Pressed);
        };

        row.AddChild(kind);
        row.AddChild(target);
        row.AddChild(time);
        row.AddChild(inverted);
        row.AddChild(add);
        return row;
    }
}
