// NeoKolors
// Copyright (c) krystof 2026

using Metriks;

namespace NeoKolors.Tui.Core;

public readonly record struct LayoutContext {
    public Size2D Parent { get; init; }
    public Size2D Screen { get; init; }
    
    public LayoutContext(Size2D parent, Size2D screen) {
        Parent = parent;
        Screen = screen;
    }
}