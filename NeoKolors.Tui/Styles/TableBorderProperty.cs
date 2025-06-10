//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("table-style")]
public struct TableBorderProperty : IStyleProperty<TableBorderProperty, TableBorderStyle> {
    public TableBorderStyle Value { get; set; }
    
    public TableBorderProperty(TableBorderStyle value) {
        Value = value;
    }

    public TableBorderProperty() {
        Value = TableBorderStyle.GetNormal();
    }
}