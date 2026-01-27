// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Values;

internal readonly record struct CheckerBckg {
    public NKColor C1 { get; }
    public NKColor C2 { get; }
    public int Width { get; }
    public int Height { get; }
    
    public bool Enabled { get; }

    public CheckerBckg(NKColor c1, NKColor c2, bool enabled = true, int width = 2, int height = 1) {
        C1 = c1;
        C2 = c2;
        Enabled = enabled;
        Width = width;
        Height = height;
    }
}