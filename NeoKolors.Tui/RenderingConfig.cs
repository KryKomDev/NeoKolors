// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui;

public readonly struct RenderingConfig {
    public bool IsLazy { get; }
    public int Limit { get; }
    public bool IsUnlimited { get; }

    private RenderingConfig(bool isLazy, int limit) {
        IsLazy = isLazy;
        Limit = limit;
        
        if (limit < 1) Limit = 1;
        IsUnlimited = false;
    }

    public RenderingConfig() {
        IsLazy = false;
        Limit = 24;
        IsUnlimited = false;
    }

    public static RenderingConfig Lazy() => new(true, 0);
    public static RenderingConfig Limited(int limit) => new(false, limit);
    public static RenderingConfig Unlimited() => new(false, -1);

    public override string ToString() => IsLazy ? "Lazy" : IsUnlimited ? "Unlimited" : $"Limited ({Limit} fps)";
}