// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct BorderProperty : IStyleProperty<BorderStyle, BorderProperty> {
    public BorderStyle Value { get; }
    
    public BorderProperty(BorderStyle value) {
        Value = value;
    }

    public BorderProperty() {
        Value = BorderStyle.Borderless;
    }
}