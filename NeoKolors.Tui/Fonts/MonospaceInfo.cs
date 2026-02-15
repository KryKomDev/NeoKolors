// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Fonts;

public readonly struct MonospaceInfo {

    public ushort CharacterWidth { get; }

    public ushort CharacterHeight { get; }

    public bool AlignToGrid { get; }

    public MonospaceInfo(ushort charWidth, ushort charHeight, bool alignToGrid) {
        CharacterWidth = charWidth;
        CharacterHeight = charHeight;   
        AlignToGrid = alignToGrid;
    }
}