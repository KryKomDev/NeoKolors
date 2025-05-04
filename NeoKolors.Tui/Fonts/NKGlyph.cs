//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Exceptions;
using static NeoKolors.Tui.Fonts.IGlyph;

namespace NeoKolors.Tui.Fonts;

/// <summary>
/// represents a single font glyph
/// </summary>
public class NKGlyph : IGlyph {
    
    public char Character { get; }

    /// <summary>
    /// the width of the glyph
    /// </summary>
    public byte Width { get; }
    
    /// <summary>
    /// the height of the glyph
    /// </summary>
    public byte Height { get; }

    /// <summary>
    /// returns the x-offset of the character, (+) -> right, (-) -> left
    /// </summary>
    public sbyte XOffset { get; }
    
    /// <summary>
    /// returns the y-offset of the character, (+) -> down, (-) -> up
    /// </summary>
    public sbyte YOffset { get; }
    
    /// <summary>
    /// returns the individual characters of the glyph,
    /// the first index represents left / right and the second up / down,
    /// [0, 0] is the top left corner
    /// </summary>
    public char[,] Chars { get; }

    /// <inheritdoc cref="IGlyph.GetLines"/>
    public string[] GetLines() {
        string[] lines = new string[Height];
        
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                lines[y] += Chars[x, y];
            }
        }

        return lines;
    }


    public NKGlyph(char character, char[,] glyph, sbyte xOffset = 0, sbyte yOffset = 0) {
        Character = character;
        Chars = glyph;

        int w = Chars.GetLength(0);
        int h = Chars.GetLength(1);
        
        if (w > MAX_WIDTH) throw GlyphTooBigException.Width(w);
        if (h > MAX_HEIGHT) throw GlyphTooBigException.Height(h);
        
        Width = (byte)w;
        Height = (byte)h;

        XOffset = xOffset;
        YOffset = yOffset;
    }

    /// <summary>
    /// creates a new glyph from a string, newlines shall separate individual lines of the glyph
    /// </summary>
    /// <param name="character">the character the glyph is supposed to represent</param>
    /// <param name="glyph">the string from which the glyph will be created</param>
    /// <param name="xOffset">x offset of the glyph</param>
    /// <param name="yOffset">y offset of the glyph</param>
    /// <param name="wm">whitespace mode</param>
    /// <exception cref="GlyphTooBigException">
    /// if the glyph is too big, see <see cref="IGlyph.MAX_WIDTH"/> and <see cref="IGlyph.MAX_HEIGHT"/>
    /// </exception>
    public NKGlyph(char character, string glyph, 
        sbyte xOffset = 0, sbyte yOffset = 0, 
        WhitespaceMode wm = WhitespaceMode.TRANSPARENT) 
    {
        Character = character;
        
        string[] lines = glyph.Split('\n');

        int w = 0;
        int h = lines.Length;

        foreach (var l in lines) if (w < l.Length) w = l.Length;
        
        if (w > MAX_WIDTH) throw GlyphTooBigException.Width(w);
        if (h > MAX_HEIGHT) throw GlyphTooBigException.Height(h);
        
        Width = (byte)w;
        Height = (byte)h;

        Chars = new char[Width, Height];

        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                if (x >= lines[y].Length) Chars[x, y] = wm != WhitespaceMode.OVERLAP ? '\0' : ' ';
                else Chars[x, y] = lines[y][x];
            }
        }
        
        XOffset = xOffset;
        YOffset = yOffset;
    }


    /// <summary>
    /// creates a new glyph from a string, newlines shall separate individual lines of the glyph
    /// </summary>
    /// <param name="character">the character the glyph is supposed to represent</param>
    /// <param name="glyph">the string from which the glyph will be created</param>
    /// <param name="xOffset">x offset of the glyph</param>
    /// <param name="yOffset">y offset of the glyph</param>
    /// <param name="wm">whitespace mode</param>
    /// <exception cref="GlyphTooBigException">
    /// if the glyph is too big, see <see cref="IGlyph.MAX_WIDTH"/> and <see cref="IGlyph.MAX_HEIGHT"/>
    /// </exception>
    public NKGlyph(char character, string[] glyph, 
        sbyte xOffset = 0, sbyte yOffset = 0, 
        WhitespaceMode wm = WhitespaceMode.TRANSPARENT) 
    {
        int maxWidth = 0;
        
        foreach (var l in glyph) maxWidth = l.Length > maxWidth ? l.Length : maxWidth;
        
        if (maxWidth > MAX_WIDTH) throw GlyphTooBigException.Width(maxWidth);
        if (glyph.Length > MAX_HEIGHT) throw GlyphTooBigException.Height(glyph.Length);

        Width = (byte)maxWidth;
        Height = (byte)glyph.Length;
        
        Chars = new char[Width, Height];

        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                if (x >= glyph[y].Length) Chars[x, y] = wm != WhitespaceMode.OVERLAP ? '\0' : ' ';
                else Chars[x, y] = glyph[y][x];
            }
        }

        XOffset = xOffset;
        YOffset = yOffset;
        Character = character;
    }


    /// <summary>
    /// creates a new glyph from a string representing a single glyph
    /// </summary>
    /// <param name="character">the character the glyph is supposed to represent</param>
    /// <param name="glyph">the glyph string</param>
    /// <param name="xOffset">x offset of the glyph</param>
    /// <param name="yOffset">y offset of the glyph</param>
    /// <returns>new glyph</returns>
    public static IGlyph FromString(char character, string glyph, sbyte xOffset, sbyte yOffset) => 
        new NKGlyph(character, glyph, xOffset, yOffset);

    public void PrintGlyph() {
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                System.Console.Write(Chars[x, y]);
            }
            System.Console.WriteLine();
        }
    }

    /// <summary>
    /// returns an empty glyph
    /// </summary>
    public static NKGlyph Empty() => new(' ', " ");
}