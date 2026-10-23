// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Tui.Core;

public readonly record struct ZIndex {
    public int  Value      { get; init; }
    public bool IsAbsolute { get; init; }

    public ZIndex(int value, bool absolute) {
        Value      = value;
        IsAbsolute = absolute;
    }

    public static ZIndex Absolute(int value) => new(value, true);
    public static ZIndex Relative(int value) => new(value, false);

    public int GetValue(int z) => IsAbsolute ? Value : z + Value;

    public static implicit operator int(ZIndex z) => z.GetValue(0);
    public static implicit operator ZIndex(int z) => new() { Value = z };
}