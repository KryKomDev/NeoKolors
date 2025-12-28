// NeoKolors
// Copyright (c) 2025 KryKom

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