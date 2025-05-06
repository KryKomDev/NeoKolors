//
// NeoKolors
// Copyright (c) 2025 by KryKom
//

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

namespace NeoKolors.Tui.Fonts;

public interface IFont {
    
    /// <summary>
    /// determines the number of columns between individual characters
    /// </summary>
    public int LetterSpacing { get; }
    
    /// <summary>
    /// determines the number of columns between words (space glyph size)
    /// </summary>
    public int WordSpacing { get; }
    
    /// <summary>
    /// determines the number of rows between lines
    /// </summary>
    public int LineSpacing { get; }
    
    /// <summary>
    /// determines the standard size of a glyph
    /// </summary>
    public int LineSize { get; }
    
    /// <summary>
    /// adds a glyph to the font
    /// </summary>
    /// <param name="g">a glyph</param>
    public void AddGlyph(IGlyph g);
    
    /// <summary>
    /// returns the glyph for a character
    /// </summary>
    public IGlyph GetGlyph(char c);
    
    /// <summary>
    /// splits a character into its base character and its diacritic character
    /// </summary>
    /// <param name="input">the character to split</param>
    public static (char baseChar, char diacritics) Separate(char input) {
        string s = (input + "").Normalize(NormalizationForm.FormD);
        char baseChar = '\0';
        char diacritics = '\0';
        
        foreach (char c in s) {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                diacritics = c;
            else
                baseChar = c;
        }

        return (baseChar, diacritics);
    }
    
    /// <summary>
    /// chops the string into multiple lines so that no line of the text is longer than the maxWidth
    /// when the font is applied
    /// </summary>
    /// <param name="s">the string to be chopped</param>
    /// <param name="font">the font to be applied</param>
    /// <param name="maxWidth">maximum width of a single line</param>
    /// <exception cref="ArgumentException">maxWidth is less than or equal to 0</exception>
    [Pure]
    public static string[] Chop(string s, IFont font, int maxWidth) {
        if (string.IsNullOrWhiteSpace(s)) return [];
        if (maxWidth <= 0) throw new ArgumentException("Maximum width must be positive", nameof(maxWidth));

        List<string> output = [];
        int currentPosition = 0;
    
        while (currentPosition < s.Length) {
            int chunkStart = currentPosition;
            int currentWidth = 0;
            int lastBreakPosition = -1;

            while (currentPosition < s.Length) {
                char currentChar = s[currentPosition];
            
                if (currentChar is '\n' or '\r') {
                    if (currentPosition > chunkStart) {
                        output.Add(s[chunkStart..currentPosition]);
                    }
                    currentPosition++;
                    if (currentChar == '\r' && currentPosition < s.Length && s[currentPosition] == '\n') {
                        currentPosition++;
                    }
                    chunkStart = currentPosition;
                    break;
                }

                int charWidth;
                if (currentChar == ' ') {
                    charWidth = font.WordSpacing;
                } else {
                    IGlyph glyph = font.GetGlyph(currentChar);
                    charWidth = glyph.Width + font.LetterSpacing;
                }

                if (currentWidth + charWidth > maxWidth) {
                    if (lastBreakPosition != -1) {
                        output.Add(s[chunkStart..lastBreakPosition]);
                        currentPosition = lastBreakPosition + 1;
                        while (currentPosition < s.Length && char.IsWhiteSpace(s[currentPosition])) {
                            currentPosition++;
                        }
                    } else {
                        output.Add(s[chunkStart..currentPosition]);
                        currentPosition++;
                    }
                    chunkStart = currentPosition;
                    break;
                }

                if (char.IsWhiteSpace(currentChar)) {
                    lastBreakPosition = currentPosition;
                }

                currentWidth += charWidth;
                currentPosition++;
            }

            if (currentPosition == s.Length && currentPosition > chunkStart) {
                output.Add(s[chunkStart..currentPosition]);
            }
        }

        return output.ToArray();
    }

    /// <summary>
    /// returns a font that behaves the same as if there was no font at all
    /// </summary>
    public static IFont Default => new DefaultFont();
}