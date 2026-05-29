// NeoKolors
// Copyright (c) krystof 2026

using NeoKolors.Common;

namespace NeoKolors.Tui.Fonts.Coloring;

public class HorizontalGradient : IColoringStrategy {
    private readonly NKColor[] _colors;
    private readonly float     _tilt;

    public HorizontalGradient(params NKColor[] colors) {
        _colors = colors;
        _tilt   = 0f;
    }

    public HorizontalGradient(float tilt, params NKColor[] colors) {
        _colors = colors;
        _tilt   = tilt;
    }

    public NKColor GetColor(int currentX, int currentY, int width, int height) {
        if (width <= 1 && height <= 1) return _colors[0];

        float score = currentX + currentY * _tilt;

        float minScore = Math.Min(0f, (height - 1) * _tilt);
        float maxScore = Math.Max(width - 1, width - 1 + (height - 1) * _tilt);

        float range = maxScore - minScore;
        float fraction = range == 0 ? 0f : (score - minScore) / range;

        return NKColor.GetMultiStopColor(_colors, fraction);
    }
}