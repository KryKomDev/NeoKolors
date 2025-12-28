// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct EnableGridProperty : IStyleProperty<bool, EnableGridProperty> {
    public bool Value { get; }
    
    public EnableGridProperty(bool value) {
        Value = value;
    }
    
    public EnableGridProperty() {
        Value = false;
    }
}