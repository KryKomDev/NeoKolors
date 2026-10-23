// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Fonts;

namespace NeoKolors.Tui.Elements;

public class Text : IElement {

    public Bindable<AnsiString>            Content     { get; init; }
    public Bindable<IAsciiFont>            Font        { get; init; }
    public Bindable<TextRenderingOptions?> TextOptions { get; init; }
    public Bindable<NKAlign>               TextAlign   { get; init; }
    public Bindable<bool>                  Overflow    { get; init; }
    public Bindable<ZIndex>                ZIndex      { get; init; }
    public Bindable<Sizing>                Sizing      { get; init; }

    public Text(
        Bindable<AnsiString>            content     = default,
        Bindable<IAsciiFont>            font        = default,
        Bindable<TextRenderingOptions?> textOptions = default,
        Bindable<NKAlign>               textAlign   = default,
        Bindable<bool>                  overflow    = default,
        Bindable<ZIndex>                zIndex      = default,
        Bindable<Sizing>                sizing      = default
    ) {
        Content     = content;
        Font        = font;
        TextOptions = textOptions;
        TextAlign   = textAlign;
        Overflow    = overflow;
        ZIndex      = zIndex;
        Sizing      = sizing;
    }

    public void Render(RenderContext context, Size2D parent) {
        if (AnsiString.IsNullOrEmpty(Content))
            return;

        var font     = Font.AsT ?? new DefaultFont();
        var z        = ZIndex.AsT.GetValue(context.ZIndex);
        var overflow = Overflow.AsT;

        using var _ = overflow ? context.PushUnclipped() : default;

        var bounds = new Rect2D(Point2D.Zero, parent);

        if (TextOptions.AsT is not null) {
            font.DrawString(
                Content,
                context,
                bounds,
                TextOptions!,
                z
            );
        }
        else {
            font.DrawString(
                Content,
                context,
                bounds,
                TextAlign.AsT.Horizontal,
                TextAlign.AsT.Vertical,
                overflow,
                z
            );
        }
    }

    public Size2D Measure(LayoutContext context) {
        if (AnsiString.IsNullOrEmpty(Content))
            return Size2D.Zero;

        var font        = Font.AsT ?? new DefaultFont();
        var contentSize = TextOptions.AsT is not null
            ? font.GetSize(Content, TextOptions.AsT)
            : font.GetSize(Content);

        var sizing = Sizing.AsT;
        return sizing != default ? sizing.ToSize(context, contentSize, ungrown: true) : contentSize;
    }
}