// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct WrapProperty : IStyleProperty<bool, WrapProperty> {
    public bool Value { get; }
    
    public WrapProperty(bool value) {
        Value = value;
    }
    
    public WrapProperty() {
        Value = false;
    }
}