// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts;

public readonly struct VariableInfo {
    
    public bool Kerning { get; }
    public bool LineKerning { get; }
    public int WordSpacing { get; }
    public int EmptyLineHeight { get; }

    public VariableInfo(bool kerning, bool lineKerning, int wordSpacing, int emptyLineHeight) {
        Kerning = kerning;
        LineKerning = lineKerning;
        WordSpacing = wordSpacing;
        EmptyLineHeight = emptyLineHeight;
    }
}