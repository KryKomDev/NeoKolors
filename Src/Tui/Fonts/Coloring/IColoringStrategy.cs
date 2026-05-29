// NeoKolors
// Copyright (c) krystof 2026

using NeoKolors.Common;

namespace NeoKolors.Tui.Fonts;

public interface IColoringStrategy {
    public NKColor GetColor(int x, int y, int width, int height);
}