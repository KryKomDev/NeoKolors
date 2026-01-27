// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;
using SkiaSharp;

namespace NeoKolors.Tui.Rendering;

public readonly record struct SixelImageInfo : ISixelImageInfo {
    public SKBitmap Image    { get; }
    public Size2D   Size     { get; }
    public Size2D   CharSize { get; }
    public Point2D  Offset   { get; }
    public int      ZIndex   { get; }

    public SixelImageInfo(SKBitmap image, Size2D size, Size2D charSize, Point2D offset, int zIndex) {
        Image    = image;
        Size     = size;
        CharSize = charSize;
        Offset   = offset;
        ZIndex   = zIndex;
    }
}