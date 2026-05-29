// NeoKolors
// Copyright (c) krystof 2026

using NeoKolors.Common;

namespace NeoKolors.Tui.Fonts.Coloring;

public class SolidColor : IColoringStrategy {
    private readonly NKColor _color;

    public SolidColor(NKColor color) => _color = color;
        
    public NKColor GetColor(int x, int y, int width, int height) => _color;
}