//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using static NeoKolors.Tui.Fonts.V1.IGlyph;

namespace NeoKolors.Tui.Fonts.V1;

/// <summary>
/// a glyph mixed from two other glyphs
/// </summary>
public class CompoundGlyph : NKGlyph {

    public CompoundGlyph(
        char character, 
        IGlyph first,
        IGlyph second) :
        base(
            character, 
            MixGlyphs(first, second),
            Math.Min(first.XOffset, second.XOffset),
            Math.Min(first.YOffset, second.YOffset)) { }

    private static char[,] MixGlyphs(IGlyph first, IGlyph second) {
        int smallerX = Math.Min(first.XOffset, second.XOffset);
        int greaterX = Math.Max(first.XOffset + first.Width, second.XOffset + second.Width);
        int smallerY = Math.Min(first.YOffset, second.YOffset);
        int greaterY = Math.Max(first.YOffset + first.Height, second.YOffset + second.Height);

        int w = greaterX - smallerX;
        int h = greaterY - smallerY;
        
        if (w > MAX_WIDTH) throw GlyphTooBigException.Width(w);
        if (h > MAX_HEIGHT) throw GlyphTooBigException.Height(h);

        char[,] chars = new char[w, h];

        for (int y = 0; y < first.Height; y++) {
            for (int x = 0; x < first.Width; x++) {
                chars[first.XOffset - smallerX + x, first.YOffset - smallerY + y] = first.Chars[x, y];
            }
        }
        
        for (int y = 0; y < second.Height; y++) {
            for (int x = 0; x < second.Width; x++) {
                if (second.Chars[x, y] != '\0')
                    chars[second.XOffset - smallerX + x, second.YOffset - smallerY + y] = second.Chars[x, y];
            }
        }
        
        return chars;
    }
}