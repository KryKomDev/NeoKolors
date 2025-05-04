//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

public struct AlignDirection {
    public HorizontalAlign Horizontal { get; }
    public VerticalAlign Vertical { get; }

    public AlignDirection(
        HorizontalAlign horizontal = HorizontalAlign.LEFT,
        VerticalAlign vertical = VerticalAlign.TOP)
    {
        Horizontal = horizontal;
        Vertical = vertical;
    }
}

public enum HorizontalAlign {
    LEFT = 0,
    CENTER = 1,
    RIGHT = 2
}

public enum VerticalAlign {
    TOP = 0,
    CENTER = 1,
    BOTTOM = 2
}