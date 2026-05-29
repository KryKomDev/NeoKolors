// NeoKolors
// Copyright (c) krystof 2026

using NeoKolors.Tui.Core;
using FontPropoInfo = NeoKolors.Tui.Fonts.FontProportionsInfo;

namespace NeoKolors.Tui.Fonts;

public class TextRenderingOptions {
    
    public HorizontalAlign    HorizontalAlign { get; set; }
    public VerticalAlign      VerticalAlign   { get; set; }
    public bool               Overflow        { get; set; }
    public bool               JustifyText     { get; set; }
    public WordWrapType       WordWrap        { get; set; }
    public char?              BackgroundChar  { get; set; }
    public IColoringStrategy? Coloring        { get; set; }
    public FontPropoInfo?     Proportions     { get; set; }
    public FontSpacingInfo?   Spacing         { get; set; }

    public TextRenderingOptions(
        HorizontalAlign    horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign      verticalAlign   = VerticalAlign.TOP, 
        bool               overflow        = false,
        bool               justifyText     = false,
        WordWrapType       wordWrap        = WordWrapType.WORD,
        char?              backgroundChar  = null,
        IColoringStrategy? coloring        = null,
        FontPropoInfo?     proportions     = null,
        FontSpacingInfo?   spacing         = null) 
    {
        HorizontalAlign = horizontalAlign;
        VerticalAlign   = verticalAlign;
        Overflow        = overflow;
        JustifyText     = justifyText;
        WordWrap        = wordWrap;
        BackgroundChar  = backgroundChar;
        Coloring        = coloring;
        Proportions     = proportions;
        Spacing         = spacing;
    }
}