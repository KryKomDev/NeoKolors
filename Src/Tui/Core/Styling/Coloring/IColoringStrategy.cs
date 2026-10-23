// NeoKolors
// Copyright (c) KryKom 2026

using NeoKolors.Common;

namespace NeoKolors.Tui.Core;

public interface IColoringStrategy {
    public NKColor GetColor(int x, int y, int width, int height);
}