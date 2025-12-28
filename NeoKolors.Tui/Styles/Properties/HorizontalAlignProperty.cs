// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct HorizontalAlignProperty : IStyleProperty<HorizontalAlign, HorizontalAlignProperty> {
    public HorizontalAlign Value { get; }
    
    public HorizontalAlignProperty(HorizontalAlign value) {
        Value = value;
    }
    
    public HorizontalAlignProperty() {
        Value = HorizontalAlign.LEFT;
    }
}