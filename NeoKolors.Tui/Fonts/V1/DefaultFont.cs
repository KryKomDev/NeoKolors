//
// NeoKolors
// Copyright (c)
//

namespace NeoKolors.Tui.Fonts.V1;

/// <summary>
/// represents the default (character) font
/// </summary>
public sealed class DefaultFont : IFont {
    
    internal DefaultFont() { }
    
    public int LetterSpacing => 0;
    public int WordSpacing => 1;
    public int LineSpacing => 0;
    public int LineSize => 1;
    
    public void AddGlyph(IGlyph g) => 
        throw new InvalidOperationException("Default font cannot be modified.");

    public IGlyph GetGlyph(char c) => 
        new NKGlyph(c, new[,]{{c}});
}