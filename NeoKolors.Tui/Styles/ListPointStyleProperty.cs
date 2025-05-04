//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

namespace NeoKolors.Tui.Styles;

[StylePropertyName("list-point-style")]
public struct ListPointStyleProperty : IStyleProperty<ListPointStyleProperty, NKStyle> {
    public NKStyle Value { get; }
    
    public ListPointStyleProperty(NKStyle value) {
        Value = value;
    }
    
    public ListPointStyleProperty() {
        Value = new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
    }
}