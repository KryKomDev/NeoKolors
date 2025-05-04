//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Fonts;

public struct NKImageFontConfig {
    
    /// <summary>
    /// determines the number of columns between individual characters
    /// </summary>
    public int LetterSpacing { get; set; }
    
    /// <summary>
    /// determines the number of columns between words (space glyph size)
    /// </summary>
    public int WordSpacing { get; set; }
    
    /// <summary>
    /// determines the number of rows between lines
    /// </summary>
    public int LineSpacing { get; set; }
    
    /// <summary>
    /// determines the standard size of a glyph
    /// </summary>
    public int LineSize { get; set; }
    
    /// <summary>
    /// defines what to do when a glyph is not found
    /// </summary>
    public UnknownGlyphMode UnknownGlyphMode { get; set; }
    
    /// <summary>
    /// the substitute glyph to use when a glyph is not found,
    /// <see cref="UnknownGlyphMode"/> must be set to <see cref="UnknownGlyphMode.GLYPH"/>
    /// </summary>
    public char SubstituteGlyph { get; set; }
    
    /// <summary>
    /// the width of a single glyph region
    /// </summary>
    public int GlyphWidth { get; set; }
    
    /// <summary>
    /// the height of a single glyph region
    /// </summary>
    public int GlyphHeight { get; set; }
    
    /// <summary>
    /// defines the baseline of the font
    /// </summary>
    public int BaseLine { get; set; }

    /// <summary>
    /// whether the font is monospaced or not
    /// </summary>
    public bool IsMonospaced { get; set; }

    /// <summary>
    /// the distribution of the glyphs in the font
    /// </summary>
    public GlyphDistribution GlyphDistribution { get; set; }

    public NKImageFontConfig(
        int letterSpacing, 
        int wordSpacing,
        int lineSpacing,
        int lineSize,
        int glyphWidth,
        int glyphHeight,
        UnknownGlyphMode unknownGlyphMode = UnknownGlyphMode.SKIP, 
        char substituteGlyph = ' ',
        int baseLine = 0, 
        bool isMonospaced = true, 
        GlyphDistribution? glyphDistribution = null) 
    {
        LetterSpacing = letterSpacing;
        WordSpacing = wordSpacing;
        LineSpacing = lineSpacing;
        LineSize = lineSize;
        UnknownGlyphMode = unknownGlyphMode;
        SubstituteGlyph = substituteGlyph;
        GlyphWidth = glyphWidth;
        GlyphHeight = glyphHeight;
        BaseLine = baseLine;
        IsMonospaced = isMonospaced;
        GlyphDistribution = glyphDistribution ?? new GlyphDistribution();
    }

    public NKImageFontConfig() {
        throw new InvalidOperationException("Cannot create an empty NKImageFontConfig. Use the proper constructor instead.");
    }

    public override string ToString() {
        return $"{UnknownGlyphMode}{(UnknownGlyphMode == UnknownGlyphMode.GLYPH ? SubstituteGlyph : "")}\n" +
               $"{LetterSpacing} {WordSpacing} {LineSpacing} {LineSize}\n" +
               $"{GlyphWidth} {GlyphHeight}\n" +
               $"{IsMonospaced}\n" +
               $"{BaseLine}\n" +
               $"{GlyphDistribution.ToString()}";
    }
}