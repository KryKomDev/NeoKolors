// NeoKolors
// Copyright (c) krystof 2026

using NeoKolors.Common;

namespace NeoKolors.Tui.Fonts.Coloring;

public static class ColoringStrategies {
    public static IColoringStrategy Solid(NKColor color) => new SolidColor(color);

    public static IColoringStrategy HGrad(float tilt, params NKColor[] colors) => new HorizontalGradient(tilt, colors);
    public static IColoringStrategy HGrad(            params NKColor[] colors) => new HorizontalGradient(      colors);
    
    public static IColoringStrategy VGrad(float tilt, params NKColor[] colors) => new VerticalGradient(tilt, colors);
    public static IColoringStrategy VGrad(            params NKColor[] colors) => new VerticalGradient(      colors);
}