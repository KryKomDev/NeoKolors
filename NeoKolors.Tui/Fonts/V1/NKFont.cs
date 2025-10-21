//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Globalization;
using System.Text;

namespace NeoKolors.Tui.Fonts.V1;

public class NKFont : IFont {

    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(NKFont));
    
    public int LetterSpacing { get; }
    public int WordSpacing { get; }
    public int LineSpacing { get; }
    public int LineSize { get; }

    public MissingGlyphMode MissingGlyphMode { get; }
    public char SubstituteGlyph { get; }
    
    
    public NKFont(
        int letterSpacing = 1, int wordSpacing = 2, int lineSpacing = 1, int lineSize = 3,
        MissingGlyphMode ugm = MissingGlyphMode.SKIP) 
    {
        LetterSpacing = letterSpacing;
        WordSpacing = wordSpacing;
        LineSpacing = lineSpacing;
        LineSize = lineSize;
        if (ugm == MissingGlyphMode.GLYPH) throw FontException.InvalidUnknownGlyphMode();
        MissingGlyphMode = ugm;
    }
    
    public NKFont(
        int letterSpacing = 1, int wordSpacing = 2, int lineSpacing = 1, int lineSize = 3, 
        char substitute = ' ') 
    {
        LetterSpacing = letterSpacing;
        WordSpacing = wordSpacing;
        LineSpacing = lineSpacing;
        LineSize = lineSize;
        MissingGlyphMode = MissingGlyphMode.GLYPH;
        SubstituteGlyph = substitute;
    }

    private Dictionary<char, IGlyph> Glyphs { get; } = new();

    private IGlyph this[char c] {
        get {
            if (Glyphs.TryGetValue(c, out var g)) return g;

            LOGGER.Warn($"Could not find character 'U+{(int)c:x4}'");
            
            return MissingGlyphMode switch {
                MissingGlyphMode.SKIP => NKGlyph.Empty(),
                MissingGlyphMode.CHAR => new NKGlyph(c, $"{c}"),
                MissingGlyphMode.GLYPH => Glyphs[SubstituteGlyph],
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    public void AddGlyph(IGlyph g) => Glyphs.Add(g.Character, g);

    public IGlyph GetGlyph(char c) {
        if (Glyphs.TryGetValue(c, out var g)) return g;

        // if it is not compound
        if ((c + "").Normalize(NormalizationForm.FormD).Length != 2) return this[c];
        
        // if it is compound
        var chars = IFont.Separate(c);
        if (chars.diacritics == '\u030c') chars = (chars.baseChar, 'ˇ');
        if (chars.diacritics == '\u0301') chars = (chars.baseChar, '´');
        if (chars.diacritics == '\u030a') 
            chars = (chars.baseChar, '°');
        return new CompoundGlyph(c, this[chars.baseChar], this[chars.diacritics]);
    }
    
    
    // ------------------------------------------------------ //
    //                        STATIC                          //
    // ------------------------------------------------------ //
    
    
}