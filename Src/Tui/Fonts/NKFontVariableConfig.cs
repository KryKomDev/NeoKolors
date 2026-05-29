// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Tui.Fonts;

public readonly record struct NKFontVariableConfig {
    
    /// <summary>
    /// Specifies whether kerning is enabled for the font.
    /// Kerning adjusts the spacing between characters to improve visual appearance.
    /// </summary>
    public bool Kerning { get; init; }

    public NKFontVariableConfig(bool kerning) {
        Kerning = kerning;
    }
}