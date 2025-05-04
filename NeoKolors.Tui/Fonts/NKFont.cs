//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Text;
using NeoKolors.Console;
using NeoKolors.Tui.Exceptions;

namespace NeoKolors.Tui.Fonts;

public class NKFont : IFont {
    
    public int LetterSpacing { get; }
    public int WordSpacing { get; }
    public int LineSpacing { get; }
    public int LineSize { get; }

    public UnknownGlyphMode UnknownGlyphMode { get; }
    public char SubstituteGlyph { get; }
    
    
    public NKFont(
        int letterSpacing = 1, int wordSpacing = 2, int lineSpacing = 1, int lineSize = 3,
        UnknownGlyphMode ugm = UnknownGlyphMode.SKIP) 
    {
        LetterSpacing = letterSpacing;
        WordSpacing = wordSpacing;
        LineSpacing = lineSpacing;
        LineSize = lineSize;
        if (ugm == UnknownGlyphMode.GLYPH) throw FontException.InvalidUnknownGlyphMode();
        UnknownGlyphMode = ugm;
    }
    
    public NKFont(
        int letterSpacing = 1, int wordSpacing = 2, int lineSpacing = 1, int lineSize = 3, 
        char substitute = ' ') 
    {
        LetterSpacing = letterSpacing;
        WordSpacing = wordSpacing;
        LineSpacing = lineSpacing;
        LineSize = lineSize;
        UnknownGlyphMode = UnknownGlyphMode.GLYPH;
        SubstituteGlyph = substitute;
    }

    private Dictionary<char, IGlyph> Glyphs { get; } = new();

    private IGlyph this[char c] {
        get {
            if (Glyphs.TryGetValue(c, out var g)) return g;

            NKDebug.Warn($"Could not find character 'U+{(int)c:x4}'");
            
            return UnknownGlyphMode switch {
                UnknownGlyphMode.SKIP => NKGlyph.Empty(),
                UnknownGlyphMode.DEFAULT => new NKGlyph(c, $"{c}"),
                UnknownGlyphMode.GLYPH => Glyphs[SubstituteGlyph],
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
}