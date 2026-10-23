// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A declarative interactive button element with visual hover and press state transitions.
/// </summary>
public class Button : IElement {

    /// <summary>
    /// Gets the inner content of the button.
    /// </summary>
    public IElement Content { get; init; }

    /// <summary>
    /// Callback invoked when the button is clicked.
    /// </summary>
    public Bindable<Action<ElementMouseEventArgs>?> OnClick { get; init; } = default;

    /// <summary>
    /// Style applied when the button is idle.
    /// </summary>
    public Bindable<NKStyle?> NormalStyle { get; init; } = default;

    /// <summary>
    /// Style applied when the cursor is hovering over the button.
    /// </summary>
    public Bindable<NKStyle?> HoverStyle { get; init; } = default;

    /// <summary>
    /// Style applied while the button is pressed down.
    /// </summary>
    public Bindable<NKStyle?> PressStyle { get; init; } = default;

    /// <summary>
    /// Border style enclosing the button.
    /// </summary>
    public Bindable<IBorderStyle?> BorderStyle { get; init; } = NKBorderStyle.GetRounded();

    /// <summary>
    /// Internal padding between the button border and its content.
    /// </summary>
    public Bindable<Spacing> Padding { get; init; } = new Spacing(vertical: Dimension.Chars(0), horizontal: Dimension.Chars(1));

    /// <summary>
    /// Margin outside the button border.
    /// </summary>
    public Bindable<Spacing> Margin { get; init; } = default;

    /// <summary>
    /// Reactive state tracking hover.
    /// </summary>
    public Bindable<bool> IsHovered { get; } = new Signal<bool>(false);

    /// <summary>
    /// Reactive state tracking press.
    /// </summary>
    public Bindable<bool> IsPressed { get; } = new Signal<bool>(false);

    public Button(
        IElement                               content,
        Action<ElementMouseEventArgs>?         onClick     = null,
        NKStyle?                               normalStyle = null,
        NKStyle?                               hoverStyle  = null,
        NKStyle?                               pressStyle  = null,
        IBorderStyle?                          borderStyle = null,
        Spacing?                               padding     = null,
        Spacing?                               margin      = null
    ) {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        if (onClick != null) OnClick = onClick;
        if (normalStyle != null) NormalStyle = normalStyle;
        if (hoverStyle != null) HoverStyle = hoverStyle;
        if (pressStyle != null) PressStyle = pressStyle;
        if (borderStyle != null) BorderStyle = borderStyle;
        if (padding != null) Padding = padding.Value;
        if (margin != null) Margin = margin.Value;
    }

    /// <summary>
    /// Convenience factory method creating a button displaying the specified text.
    /// </summary>
    public static Button FromText(
        string                         text,
        Action<ElementMouseEventArgs>? onClick     = null,
        NKStyle?                       normalStyle = null,
        NKStyle?                       hoverStyle  = null,
        NKStyle?                       pressStyle  = null,
        IBorderStyle?                  borderStyle = null
    ) => new(
        content: new Text(new AnsiString(text)),
        onClick: onClick,
        normalStyle: normalStyle,
        hoverStyle: hoverStyle,
        pressStyle: pressStyle,
        borderStyle: borderStyle
    );

    public Size2D Measure(LayoutContext context) {
        var border = new Border(
            element: Content,
            style: BorderStyle.AsT!,
            padding: Padding.AsT,
            margin: Margin.AsT
        );
        return border.Measure(context);
    }

    public void Render(RenderContext context, Size2D parent) {
        var currentStyle = IsPressed.AsT
            ? (PressStyle.AsT ?? HoverStyle.AsT ?? NormalStyle.AsT)
            : IsHovered.AsT
                ? (HoverStyle.AsT ?? NormalStyle.AsT)
                : NormalStyle.AsT;

        var border = new Border(
            element: Content,
            style: BorderStyle.AsT!,
            padding: Padding.AsT,
            margin: Margin.AsT
        );

        var region = new MouseRegion(
            child: border,
            onClick: OnClick.AsT,
            isHovered: IsHovered,
            isPressed: IsPressed
        );

        if (currentStyle.HasValue) {
            using var _ = context.PushStyle(currentStyle.Value);
            region.Render(context, parent);
        }
        else {
            region.Render(context, parent);
        }
    }
}
