// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Values;

public struct Spacing {
    public Dimension Left { get; }
    public Dimension Right { get; }
    public Dimension Top { get; }
    public Dimension Bottom { get; }
    
    public Spacing(Dimension left, Dimension right, Dimension top, Dimension bottom) {
        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
    }
    
    public Spacing(Dimension value) : this(value, value, value, value) { }
    
    public Spacing(Dimension horizontal, Dimension vertical) : this(horizontal, horizontal, vertical, vertical) { }
    
    public Spacing() : this(Dimension.Zero) { }
    public static Spacing Zero => new();
}