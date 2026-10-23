// NeoKolors
// Copyright (c) krystof 2026

using Metriks;

namespace NeoKolors.Tui.Core;

public readonly record struct Sizing {
    public Dimension Width  { get; init; }
    public Dimension Height { get; init; }

    public Sizing(Dimension width, Dimension height) {
        Width  = width;
        Height = height;
    }

    public Sizing(Dimension both) {
        Width  = both;
        Height = both;
    }

    public static Sizing Auto       { get; } = new(Dimension.Auto, Dimension.Auto);
    public static Sizing FitContent { get; } = new(Dimension.FitContent(), Dimension.FitContent());

    public static Sizing FromWidth(Dimension width)   => new() { Width = width };
    public static Sizing FromHeight(Dimension height) => new() { Height = height };
    public static Sizing Fixed(int width, int height) => new(Dimension.Chars(width), Dimension.Chars(height));

    public Size2D ToSize(LayoutContext context, Size2D contentSize = default, bool ungrown = false) {
        int w = Width.Value != null
            ? Width.Resolve(context.Parent.X, context.Screen, contentSize.X, ungrown)
            : contentSize.X;
        int h = Height.Value != null
            ? Height.Resolve(context.Parent.Y, context.Screen, contentSize.Y, ungrown)
            : contentSize.Y;

        return new Size2D(w, h);
    }

    public static implicit operator Sizing(Dimension both) => new(both);
    public static implicit operator Sizing((Dimension Width, Dimension Height) tuple) => new(tuple.Width, tuple.Height);
    public static implicit operator Sizing((int Width, int Height) tuple) => new(Dimension.Chars(tuple.Width), Dimension.Chars(tuple.Height));
}
