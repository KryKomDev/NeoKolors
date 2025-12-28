// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui;

public sealed record CellInfo(char? Char, NKStyle Style, bool Changed, int ZIndex) {
    public char?   Char    { get; set; } = Char;
    public NKStyle Style   { get; set; } = Style;
    public bool    Changed { get; set; } = Changed;
    public int     ZIndex  { get; set; } = ZIndex;
    
    public static CellInfo Default => new(null, NKStyle.Default, true, 0);
    public static CellInfo GetDefault() => new(null, NKStyle.Default, true, 0);
}