// NeoKolors
// Copyright (c) krystof 2026

using NeoKolors.Common;

namespace NeoKolors.Tui.Fonts.Coloring;

public class VerticalGradient : IColoringStrategy {
    private readonly NKColor[] _colors;
    private readonly float     _tilt;
        
    public VerticalGradient(params NKColor[] colors) {
        _colors = colors;
        _tilt   = 0f;
    }
        
    public VerticalGradient(float tilt, params NKColor[] colors) {
        _colors = colors;
        _tilt   = tilt;
    }

    public NKColor GetColor(int currentX, int currentY, int width, int height) {
        if (height <= 1 && width <= 1) return _colors[0];

        float score = currentY + currentX * _tilt;

        float minScore = Math.Min(0f, (width - 1) * _tilt);
        float maxScore = Math.Max(height - 1, height - 1 + (width - 1) * _tilt);

        float range = maxScore - minScore;
        float fraction = range == 0 ? 0f : (score - minScore) / range;

        return NKColor.GetMultiStopColor(_colors, fraction);
    }
}