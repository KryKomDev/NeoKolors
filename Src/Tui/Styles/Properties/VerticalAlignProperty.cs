// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Styles.Properties;

public struct VerticalAlignProperty : IStyleProperty<VerticalAlign, VerticalAlignProperty> {
    public VerticalAlign Value { get; }
    
    public VerticalAlignProperty(VerticalAlign value) {
        Value = value;
    }
    
    public VerticalAlignProperty() {
        Value = VerticalAlign.TOP;
    }
}