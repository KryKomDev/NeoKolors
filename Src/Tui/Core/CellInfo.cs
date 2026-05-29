// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Common;

namespace NeoKolors.Tui.Core;

public sealed record CellInfo(char? Char, NKStyle Style, bool Changed, int ZIndex) {
    
    private const int DEFAULT_Z = int.MinValue;

    public char? Char {
        get;
        set {
            if (Char == value)
                return;

            Changed = true;
            field = value;
        }
    } = Char;

    public NKStyle Style {
        get;
        set {
            if (Style == value)
                return;

            Changed = true;
            field = value;
        }
    } = Style;

    public int ZIndex {
        get;
        set {
            if (ZIndex == value)
                return;

            Changed = true;
            field = value;
        }
    } = ZIndex;

    public bool Changed { get; private set; } = Changed;

    public void SetUpdated() => Changed = false;
    
    public static CellInfo Default => GetDefault();
    public static CellInfo Null => GetNull(DEFAULT_Z);

    public static CellInfo GetNull(int z) => new(null, NKStyle.Default, true, z);

    public static CellInfo GetDefault() => new(' ', NKStyle.Default, true, DEFAULT_Z);
}