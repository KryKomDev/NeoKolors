//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Fonts;

public interface IFontReader {

    /// <summary>
    /// loads the font from a character font file
    /// </summary>
    public IFont ReadFont();
}
