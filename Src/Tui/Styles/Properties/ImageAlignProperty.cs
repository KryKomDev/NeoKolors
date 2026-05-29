// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;

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