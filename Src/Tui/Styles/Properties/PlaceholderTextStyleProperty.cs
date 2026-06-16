// NeoKolors
// Copyright (c) KryKom 2026

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct PlaceholderTextStyleProperty : IStyleProperty<NKStyle, PlaceholderTextStyleProperty> {
    public NKStyle Value { get; }

    public PlaceholderTextStyleProperty(NKStyle value) {
        Value = value;
    }

    public PlaceholderTextStyleProperty() {
        Value = new NKStyle(s: TextStyles.FAINT);
    }
}