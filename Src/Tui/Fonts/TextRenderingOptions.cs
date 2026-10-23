// NeoKolors
// Copyright (c) KryKom 2026

using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Fonts;

public class TextRenderingOptions {

    /// <summary>
    /// Gets or sets the horizontal alignment option for text rendering.
    /// </summary>
    public HorizontalAlign HorizontalAlign { get; set; }

    /// <summary>
    /// Gets or sets the vertical alignment option for text rendering.
    /// </summary>
    public VerticalAlign VerticalAlign { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether text rendering should allow content to
    ///     exceed the designated bounds. When set to <c>false</c>, rendering will clip text that
    ///     does not fit within the specified boundaries.
    /// </summary>
    public bool Overflow { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether text should be justified when rendered.
    /// </summary>
    public bool JustifyText { get; set; }

    /// <summary>
    ///     Gets or sets the word wrap behavior for text rendering. Determines how text is
    ///     wrapped within the specified bounds.
    /// </summary>
    public WordWrapType WordWrap { get; set; }

    /// <summary>
    ///     Gets or sets the coloring strategy applied when rendering text. The strategy
    ///     dictates how colors are assigned to different parts of the rendered text.
    /// </summary>
    public IColoringStrategy? Coloring { get; set; }

    /// <summary>
    ///     Gets or sets the font proportions information, which determines whether the font
    ///     is monospaced, variable, or includes kerning adjustments. This property encapsulates
    ///     details about the proportion type and its configuration.
    /// </summary>
    public FontProportionsInfo? Proportions { get; set; }

    /// <summary>
    /// Gets or sets the font spacing configuration used during text rendering.
    /// </summary>
    public FontSpacingInfo? Spacing { get; set; }

    public TextRenderingOptions(
        HorizontalAlign      horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign        verticalAlign   = VerticalAlign.TOP,
        bool                 overflow        = false,
        bool                 justifyText     = false,
        WordWrapType         wordWrap        = WordWrapType.WORD,
        IColoringStrategy?   coloring        = null,
        FontProportionsInfo? proportions     = null,
        FontSpacingInfo?     spacing         = null
    ) {
        HorizontalAlign = horizontalAlign;
        VerticalAlign   = verticalAlign;
        Overflow        = overflow;
        JustifyText     = justifyText;
        WordWrap        = wordWrap;
        Coloring        = coloring;
        Proportions     = proportions;
        Spacing         = spacing;
    }
}