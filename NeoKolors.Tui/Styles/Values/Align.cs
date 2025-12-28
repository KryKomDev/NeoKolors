// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Values;

public struct Align {
    public HorizontalAlign Horizontal { get; }
    public VerticalAlign   Vertical   { get; }
    
    public Align(HorizontalAlign horizontal, VerticalAlign vertical) {
        Horizontal = horizontal;
        Vertical   = vertical;
    }
}

public enum HorizontalAlign {
    LEFT,
    CENTER,
    RIGHT
}

public enum VerticalAlign {
    TOP,
    CENTER,
    BOTTOM
}