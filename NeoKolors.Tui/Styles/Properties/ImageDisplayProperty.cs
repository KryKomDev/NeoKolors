// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct ImageDisplayProperty : IStyleProperty<ImageDisplayType, ImageDisplayProperty> {
    public ImageDisplayType Value { get; }

    public ImageDisplayProperty(ImageDisplayType value) => Value = value;
    public ImageDisplayProperty() => Value = ImageDisplayType.FIT;
}