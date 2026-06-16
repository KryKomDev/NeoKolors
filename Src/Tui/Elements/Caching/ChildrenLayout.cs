// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Elements.Caching;

public struct ChildrenLayout {
    public int Count { get; }
    public Area2D[] Children { get; }
    
    public ChildrenLayout(Area2D[] children) {
        Count = children.Length;
        Children = children;
    }

    public ChildrenLayout() {
        Count = 0;
        Children = [];
    }
    
    public static ChildrenLayout Empty => new();
}