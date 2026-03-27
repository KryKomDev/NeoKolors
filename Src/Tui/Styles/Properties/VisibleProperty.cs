// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct VisibleProperty : IStyleProperty<bool, VisibleProperty> {
    public bool Value { get; }
    
    public VisibleProperty(bool value) {
        Value = value;
    }

    public VisibleProperty() {
        Value = true;
    }
}