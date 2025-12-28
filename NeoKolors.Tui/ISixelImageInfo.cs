// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;
using SkiaSharp;

namespace NeoKolors.Tui;

public interface ISixelImageInfo {
    public SKImage Image  { get; }
    public Size2D  Size   { get; }
    public Point2D Offset { get; }
    public int     ZIndex { get; }
}