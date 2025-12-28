// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;
using SkiaSharp;

namespace NeoKolors.Tui;

public readonly record struct SixelImageInfo : ISixelImageInfo {
    public SKImage Image  { get; }
    public Size2D  Size   { get; }
    public Point2D Offset { get; }
    public int     ZIndex { get; }

    public SixelImageInfo(SKImage image, Size2D size, Point2D offset, int zIndex) {
        Image = image;
        Size = size;
        Offset = offset;
        ZIndex = zIndex;
    }
}