// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Fonts;

public interface IFont {
    public IFontInfo Info { get; }
    
    public void PlaceString(string str, ICharCanvas canvas);
    public void PlaceString(string str, ICharCanvas canvas, int maxWidth);
    public void PlaceString(
        string str, ICharCanvas canvas, Size bounds,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign verticalAlign =  VerticalAlign.TOP, bool overflow = false);
    
    /// <summary>
    /// Returns the default char font. This font will render a character as it is.
    /// </summary>
    public static DefaultFont Default { get; } = new();
}