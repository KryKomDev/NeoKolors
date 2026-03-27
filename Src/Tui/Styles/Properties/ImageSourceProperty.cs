// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct ImageSourceProperty : IStyleProperty<string, ImageSourceProperty> {
    public string Value { get; }

    public ImageSourceProperty(string value) {
        Value = value;
    }

    public ImageSourceProperty() : this("") { }
}