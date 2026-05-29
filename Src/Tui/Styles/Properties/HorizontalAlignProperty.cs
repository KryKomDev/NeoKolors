// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;

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