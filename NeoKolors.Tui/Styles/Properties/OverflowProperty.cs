// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct OverflowProperty : IStyleProperty<bool, OverflowProperty> {
    public bool Value { get; }
    
    public OverflowProperty(bool value) {
        Value = value;
    }

    public OverflowProperty() {
        Value = false;
    }
}