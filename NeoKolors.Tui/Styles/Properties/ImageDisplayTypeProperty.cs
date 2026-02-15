// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct ImageDisplayTypeProperty : IStyleProperty<ImageDisplayType, ImageDisplayTypeProperty> {
    public ImageDisplayType Value { get; }

    public ImageDisplayTypeProperty(ImageDisplayType value) => Value = value;
    public ImageDisplayTypeProperty() => Value = ImageDisplayType.FIT;
}