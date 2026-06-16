using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.RichText;

/// <summary>
/// Renders an inline job icon: <c>[chaticon kind="job" key="Captain"]</c>. Used to show a speaker's job icon
/// before their name in radio. Hovering shows the job name.
/// </summary>
[UsedImplicitly]
public sealed partial class ChatIconTag : IMarkupTagHandler
{
    [Dependency] private IEntityManager _entMan = default!;
    [Dependency] private IPrototypeManager _proto = default!;

    public string Name => "chaticon";

    // Target inline height in virtual px, sized to sit with normal chat text.
    private const float IconSize = 14f;

    public bool TryCreateControl(MarkupNode node, [NotNullWhen(true)] out Control? control)
    {
        control = null;

        if (!node.Attributes.TryGetValue("kind", out var kindParam) || !kindParam.TryGetString(out var kind)
            || kind != "job")
            return false;
        if (!node.Attributes.TryGetValue("key", out var keyParam) || !keyParam.TryGetString(out var key))
            return false;

        if (!_proto.TryIndex<JobPrototype>(key, out var job)
            || !_proto.TryIndex<JobIconPrototype>(job.Icon, out var jobIcon))
            return false;

        Texture texture;
        try
        {
            texture = _entMan.System<SpriteSystem>().Frame0(jobIcon.Icon);
        }
        catch
        {
            return false;
        }

        // Scale the icon to a uniform target height regardless of its native rsi size.
        var native = Math.Max(texture.Size.X, texture.Size.Y);
        var scale = native > 0 ? IconSize / native : 1f;

        control = new TextureRect
        {
            Texture = texture,
            TextureScale = new Vector2(scale, scale),
            Stretch = TextureRect.StretchMode.Keep,
            VerticalAlignment = Control.VAlignment.Center,
            ToolTip = job.LocalizedName,
            MouseFilter = Control.MouseFilterMode.Stop,
        };
        return true;
    }
}
