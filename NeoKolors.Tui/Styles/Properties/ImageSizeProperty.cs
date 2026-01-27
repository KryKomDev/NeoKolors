// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct ImageSizeProperty : IStyleProperty<ViewSize, ImageSizeProperty> {
    public ViewSize Value { get; }

    public ImageSizeProperty(ViewSize value) {
        Value = value;
    }

    public ImageSizeProperty() {
        Value = new ViewSize(Dimension.Auto, Dimension.Auto);
    }
    
    public ImageSizeProperty(Dimension dim) {
        Value = new ViewSize(dim);
    }

    public ImageSizeProperty(Dimension horizontal, Dimension vertical) {
        Value = new ViewSize(horizontal, vertical);
    }
}