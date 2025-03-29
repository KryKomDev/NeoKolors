//
// NeoKolors
// Copyright (c) 2025 by KryKom
//

using System.Globalization;
using System.Text;
using NeoKolors.Console;

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
    /// adds a glyph to the font
    /// </summary>
    /// <param name="g">a glyph</param>
    public void AddGlyph(IGlyph g);
    
    /// <summary>
    /// returns the glyph for a character
    /// </summary>
    public IGlyph GetGlyph(char c);

    /// <summary>
    /// splits a character into its base character and its diacritics character
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
}