using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.RoleReqs;

/// <summary>
/// A type-to-search dropdown: a text field that filters a list of options as you type; clicking an option
/// fills the field and selects it. Used to pick a role or department target in the requirement editor.
/// </summary>
public sealed class SearchableOptionBox : BoxContainer
{
    private readonly LineEdit _edit;
    private readonly BoxContainer _list;
    private List<(string Value, string Display)> _options = new();

    /// <summary>The value of the option the user clicked, or null if none is currently selected.</summary>
    public string? SelectedValue { get; private set; }

    public SearchableOptionBox(string placeholder)
    {
        Orientation = LayoutOrientation.Vertical;

        _edit = new LineEdit { PlaceHolder = placeholder, MinWidth = 150 };
        _list = new BoxContainer { Orientation = LayoutOrientation.Vertical, Visible = false };

        AddChild(_edit);
        AddChild(_list);

        _edit.OnTextChanged += _ => Refilter();
    }

    public void SetOptions(IEnumerable<(string Value, string Display)> options)
    {
        _options = options.ToList();
        Clear();
    }

    public void Clear()
    {
        SelectedValue = null;
        _edit.Text = string.Empty;
        _list.RemoveAllChildren();
        _list.Visible = false;
    }

    /// <summary>The chosen value: the clicked option, or an exact text match against an option.</summary>
    public string? Resolve()
    {
        if (SelectedValue != null)
            return SelectedValue;

        var text = _edit.Text.Trim();
        if (text.Length == 0)
            return null;

        foreach (var (value, display) in _options)
        {
            if (string.Equals(display, text, StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, text, StringComparison.OrdinalIgnoreCase))
                return value;
        }

        return null;
    }

    private void Refilter()
    {
        // Typing invalidates any previous click-selection until re-matched.
        SelectedValue = null;
        _list.RemoveAllChildren();

        var query = _edit.Text.Trim();
        if (query.Length == 0)
        {
            _list.Visible = false;
            return;
        }

        var matches = _options
            .Where(o => o.Display.Contains(query, StringComparison.OrdinalIgnoreCase)
                        || o.Value.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(8)
            .ToList();

        foreach (var (value, display) in matches)
        {
            var button = new Button { Text = display, Margin = new Thickness(0, 1, 0, 0) };
            var capturedValue = value;
            var capturedDisplay = display;
            button.OnPressed += _ =>
            {
                _edit.Text = capturedDisplay;
                SelectedValue = capturedValue;
                _list.RemoveAllChildren();
                _list.Visible = false;
            };
            _list.AddChild(button);
        }

        _list.Visible = matches.Count > 0;
    }
}
