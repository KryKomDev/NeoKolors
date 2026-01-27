// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct ImageAlignProperty : IStyleProperty<Align, ImageAlignProperty> {
    public Align Value { get; }

    public ImageAlignProperty(Align value) {
        Value = value;
    }

    public ImageAlignProperty() {
        Value = Align.Center;
    }
}