// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;
using NeoKolors.Tui.Fonts.Exceptions;
using static NeoKolors.Tui.Fonts.CompoundGlyphAlignmentType;

namespace NeoKolors.Tui.Fonts;

public class NKCompoundGlyph : ICompoundGlyph {
    
    private readonly char?[,] _compound;
    
    public char?[,] Glyph => _compound;
    public int Width => _compound.Len0;
    public int Height => _compound.Len1;
    
    public int BaselineOffset { get; }
    public GlyphAlignmentPointCollection AlignPoints { get; }

    public IGlyph MainGlyph { get; }
    public IGlyph SecondaryGlyph { get; }
    public CompoundGlyphAlignment Alignment { get; }

    public NKCompoundGlyph(IGlyph mainGlyph, IGlyph secondaryGlyph, CompoundGlyphAlignment alignment) {
        MainGlyph = mainGlyph;
        SecondaryGlyph = secondaryGlyph;
        Alignment = alignment;
        BaselineOffset = MainGlyph.BaselineOffset;
        _compound = CombineGlyphs(MainGlyph, SecondaryGlyph, alignment, out var ap);
        AlignPoints = ap;
    }

    private static char?[,] CombineGlyphs(
        IGlyph first,
        IGlyph second,
        CompoundGlyphAlignment alignment,
        out GlyphAlignmentPointCollection alignPoints)
    {
        alignPoints = new GlyphAlignmentPointCollection();
        
        // set the alignment points
        if (alignment.Type != CUSTOM) {
            foreach (var (c, p) in first.AlignPoints)
                if (!alignPoints.ContainsKey(c)) 
                    alignPoints.Add(c, p);
            
            foreach (var (c, p) in second.AlignPoints) 
                if (!alignPoints.ContainsKey(c))
                    alignPoints.Add(c, p);
        }
        
        Point align = alignment.Type switch {
            TOP_LEFT      => (0,                                  0),
            TOP_CENTER    => (first.Width / 2 - second.Width / 2, 0),
            TOP_RIGHT     => (first.Width - second.Width,         0),
            MIDDLE_LEFT   => (0,                                  first.Height / 2 - second.Height / 2),
            CENTER        => (first.Width / 2 - second.Width / 2, first.Height / 2 - second.Height / 2),
            MIDDLE_RIGHT  => (first.Width - second.Width,         first.Height / 2 - second.Height / 2),
            BOTTOM_LEFT   => (0,                                  first.Height - second.Height),
            BOTTOM_CENTER => (first.Width / 2 - second.Width / 2, first.Height - second.Height),
            BOTTOM_RIGHT  => (first.Width - second.Width,         first.Height - second.Height),
            CUSTOM        => GetCustomAlignPoint(first, second, alignment.AlignmentChar, out alignPoints),
            NONE => throw new ArgumentOutOfRangeException(nameof(alignment.Type)),
            _ => throw new ArgumentOutOfRangeException(nameof(alignment.Type))
        };
        
        return CombineGlyphs(first, second, align);
    }
    
    private static Point GetCustomAlignPoint(
        IGlyph first,
        IGlyph second,
        char character,
        out GlyphAlignmentPointCollection alignPoints) 
    {
        if (!first.AlignPoints.ContainsKey(character) || !second.AlignPoints.ContainsKey(character)) 
            throw GlyphAssemblerException.NoMatchingAlignPoint(character);
        
        var fp = first.AlignPoints[character];
        var sp = second.AlignPoints[character];

        alignPoints = new GlyphAlignmentPointCollection();
        
        foreach (var (c, p) in first.AlignPoints) {
            if (!alignPoints.ContainsKey(c))
                alignPoints.Add(c, p);
        }

        foreach (var (c, p) in second.AlignPoints) {
            if (!alignPoints.ContainsKey(c))
                alignPoints.Add(c, p);
        }
        
        return fp - sp;
    }

    /// <summary>
    /// Combines two glyphs into a single compound glyph based on the specified alignment point.
    /// </summary>
    /// <param name="first">The first glyph to combine.</param>
    /// <param name="second">The second glyph to combine.</param>
    /// <param name="align">The offset of the top-left corner of the secondary glyph from the top-left
    /// corner of the main glyph</param>
    /// <returns>A two-dimensional array of characters representing the combined glyph.</returns>
    private static char?[,] CombineGlyphs(IGlyph first, IGlyph second, Point align) {
        
        // prepare size
        var mxs = (second.Width, second.Height) + align;
        var min = Point.Min(align, Point.Zero);
        var max = Point.Max(mxs, (first.Width, first.Height));
        var size = Point.Rect(min, max);
        
        var glyph = new char?[size.Width - 1, size.Height - 1];

        // copy first glyph
        int fxo = align.X < 0 ? -align.X : 0;
        int fyo = align.Y < 0 ? -align.Y : 0;

        for (int x = 0; x < first.Width; x++) {
            for (int y = 0; y < first.Height; y++) {
                glyph[x + fxo, y + fyo] = first.Glyph[x, y];
            }
        }
        
        // copy second glyph
        int sxo = align.X < 0 ? 0 : align.X;
        int syo = align.Y < 0 ? 0 : align.Y;
        
        for (int x = 0; x < second.Width; x++) {
            for (int y = 0; y < second.Height; y++) {
                if (second.Glyph[x, y] == null) continue;
                glyph[x + sxo, y + syo] = second.Glyph[x, y];
            }
        }
        
        return glyph;
    }
}