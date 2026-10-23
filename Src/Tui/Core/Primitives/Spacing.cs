// NeoKolors
// Copyright (c) krystof 2026

using Metriks;

namespace NeoKolors.Tui.Core;

public readonly record struct ResolvedSpacing(int Top, int Bottom, int Left, int Right) {
    public int Horizontal => Left + Right;
    public int Vertical   => Top + Bottom;
    public Size2D Size    => new(Horizontal, Vertical);
    public Point2D Offset => new(Left, Top);

    public static ResolvedSpacing Zero { get; } = new(0, 0, 0, 0);
}

public readonly record struct Spacing {
    public Dimension Top    { get; init; }
    public Dimension Bottom { get; init; }
    public Dimension Left   { get; init; }
    public Dimension Right  { get; init; }
    
    public Spacing(Dimension top, Dimension bottom, Dimension left, Dimension right) {
        Top    = top;
        Bottom = bottom;
        Left   = left;
        Right  = right;
    }

    public Spacing(Dimension vertical, Dimension horizontal) {
        Top    = vertical;
        Bottom = vertical;
        Left   = horizontal;
        Right  = horizontal;
    }

    public Spacing(Dimension all) {
        Top    = all;
        Bottom = all;
        Left   = all;
        Right  = all;
    }

    public static Spacing Zero { get; } = new(Dimension.Zero);

    public ResolvedSpacing Resolve(LayoutContext context, bool ungrown = false) {
        int t = Top.Resolve(context.Parent.Y, context.Screen, ungrown: ungrown);
        int b = Bottom.Resolve(context.Parent.Y, context.Screen, ungrown: ungrown);
        int l = Left.Resolve(context.Parent.X, context.Screen, ungrown: ungrown);
        int r = Right.Resolve(context.Parent.X, context.Screen, ungrown: ungrown);

        return new ResolvedSpacing(t, b, l, r);
    }

    public Size2D ToSize(LayoutContext context, bool ungrown = false) {
        return Resolve(context, ungrown).Size;
    }

    public static implicit operator Spacing(Dimension all) => new(all);
}