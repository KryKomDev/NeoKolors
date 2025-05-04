//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("grid")]
public readonly struct GridProperty : IStyleProperty<GridProperty, GridProperty> {
    public SizeValue[] Rows { get; }
    public SizeValue[] Cols { get; }
    
    public int RowCount => Rows.Length;
    public int ColCount => Cols.Length;
    
    public GridProperty Value => this;

    public GridProperty(SizeValue[] rows, SizeValue[] cols) {
        Rows = rows;
        Cols = cols;
    }

    public GridProperty() {
        Rows = [SizeValue.Auto];
        Cols = [SizeValue.Auto];
    }
}