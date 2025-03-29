//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Exceptions;
using static NeoKolors.Tui.Fonts.IGlyph;

namespace NeoKolors.Tui.Fonts;

/// <summary>
/// a glyph mixed from two other glyphs
/// </summary>
public class CompoundGlyph : Glyph {

    public CompoundGlyph(char character, IGlyph first, IGlyph second, sbyte xOffset = 0, sbyte yOffset = 0) : 
        base(character, MixGlyphs(first, second), xOffset, yOffset) { }

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

        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {
                chars[x, y] = ' ';
            }
        }

        for (int y = 0; y < first.Height; y++) {
            for (int x = 0; x < first.Width; x++) {
                chars[-smallerX + first.XOffset + x, -smallerY + first.YOffset + y] = first.Chars[x, y];
            }
        }
        
        for (int y = 0; y < second.Height; y++) {
            for (int x = 0; x < second.Width; x++) {
                chars[-smallerX + second.XOffset + x, -smallerY + second.YOffset + y] = second.Chars[x, y];
            }
        }
        
        return chars;
    }
}