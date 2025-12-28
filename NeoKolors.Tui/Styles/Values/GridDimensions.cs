// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Values;

public readonly struct GridDimensions {
    public Dimension[] Columns { get; }
    public Dimension[] Rows { get; }
    
    public GridDimensions(Dimension[] columns, Dimension[] rows) {
        Columns = columns;
        Rows = rows;
    }
    
    public GridDimensions() {
        Columns = [Dimension.Auto];
        Rows = [Dimension.Auto];
    }
    
    public static GridDimensions Default => new();
}