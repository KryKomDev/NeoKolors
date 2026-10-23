// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using SkiaSharp;

namespace NeoKolors.Tui.Core;

public readonly record struct NKImageData {
    public SKImage           Image           { get; init; }
    public SKSamplingOptions SamplingOptions { get; init; }
    public int               ZLayer          { get; init; }
    public Point2D           Position        { get; init; }
    public Size2D            Size            { get; init; }
}