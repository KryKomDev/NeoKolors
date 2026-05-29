// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;

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