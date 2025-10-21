// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts.V2;

public interface IFont {
    public NKFontInfo Info { get; }
    
    public void PlaceString(string str, ICharCanvas canvas);
    public void PlaceString(string str, ICharCanvas canvas, int maxWidth);
}