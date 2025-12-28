// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Elements.Caching;

public struct ChildrenLayout {
    public int Count { get; }
    public Rectangle[] Children { get; }
    
    public ChildrenLayout(Rectangle[] children) {
        Count = children.Length;
        Children = children;
    }

    public ChildrenLayout() {
        Count = 0;
        Children = [];
    }
    
    public static ChildrenLayout Empty => new();
}