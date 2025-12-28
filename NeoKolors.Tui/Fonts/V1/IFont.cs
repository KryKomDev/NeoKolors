//
// NeoKolors
// Copyright (c) 2025 by KryKom
//

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

namespace NeoKolors.Tui.Fonts.V1;

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
                } 
                else {
                    var glyph = font.GetGlyph(currentChar);
                    charWidth = glyph.Width + font.LetterSpacing;
                }

                if (currentWidth + charWidth > maxWidth) {
                    if (lastBreakPosition != -1) {
                        output.Add(s[chunkStart..lastBreakPosition]);
                        currentPosition = lastBreakPosition + 1;
                        
                        while (currentPosition < s.Length && char.IsWhiteSpace(s[currentPosition])) {
                            currentPosition++;
                        }
                    }
                    else {
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
    /// Calculates the size of the given string based on the specified font settings.
    /// </summary>
    /// <param name="s">The input string whose size is to be calculated.</param>
    /// <param name="font">The font used to calculate the size of the string.</param>
    /// <returns>The width and height of the string as a <see cref="Size"/> struct.</returns>
    public static Size GetSize(string s, IFont font) {
        var lines = s.Split('\n');
        int width = 0;

        if (font is DefaultFont) {
            width = lines.Select(l => l.Length).Prepend(width).Max();

            return new Size(width, lines.Length);
        }

        foreach (var line in lines) {
            var lineWidth = 0;
            
            foreach (var c in line) {
                if (c == ' ') {
                    lineWidth += font.WordSpacing;
                } 
                else {
                    var glyph = font.GetGlyph(c);
                    lineWidth += glyph.Width + font.LetterSpacing;
                }
            }
            
            var chars = line.Replace(" ", "").Length;
            lineWidth += (chars - 1) * font.LetterSpacing;
            
            width = Math.Max(width, lineWidth);
        }
        
        return new Size(width, lines.Length * font.LineSize + (lines.Length - 1) * font.LineSpacing);
    }

    /// <summary>
    /// Calculates the minimum size required to render a given text using the specified font.
    /// </summary>
    /// <param name="s">The text string to be measured.</param>
    /// <param name="font">The font used for rendering the text.</param>
    /// <returns>A <see cref="Size"/> object representing the minimum width and height required to render the text.</returns>
    public static Size GetMinSize(string s, IFont font) {
        var words = s.Replace(' ', '\n').Split('\n');
        int width = 0;
        string[] lines;
        int maxLength = words.Select(w => w.Length).Prepend(width).Max();
        
        if (font is DefaultFont) {
            width = maxLength;
            lines = s.Chop(width);
            return new Size(width, lines.Length);
        }
        
        lines = Chop(s, font, maxLength);

        foreach (var l in lines) {
            var lineWidth = 0;
            
            foreach (var c in l) {
                if (c == ' ') {
                    lineWidth += font.WordSpacing;
                } 
                else {
                    var glyph = font.GetGlyph(c);
                    lineWidth += glyph.Width + font.LetterSpacing;
                }
            }
            
            var chars = l.Replace(" ", "").Length;
            lineWidth += (chars - 1) * font.LetterSpacing;
            
            width = Math.Max(width, lineWidth);
        }
        
        return new Size(width, lines.Length * font.LineSize + (lines.Length - 1) * font.LineSpacing);
    }

    /// <summary>
    /// Calculates and retrieves the size required to render the specified text using
    /// the given font and within the maximum allowable width.
    /// </summary>
    /// <param name="s">The text content for which the size is to be calculated.</param>
    /// <param name="font">The font used to render the text.</param>
    /// <param name="maxWidth">The maximum allowable width for rendering the text.</param>
    /// <returns>The calculated size required to render the text.</returns>
    [Pure]
    public static Size GetSize(string s, IFont font, int maxWidth) {
        string[] lines = Chop(s, font, maxWidth);
        int mw = 0;

        // TODO: implement this mf method
        
        foreach (var l in lines) {
            foreach (var c in l) {
                
            }
        }

        return new Size();
    }
    
    /// <summary>
    /// returns a font that behaves the same as if there was no font at all
    /// </summary>
    public static IFont Default => new DefaultFont();
}