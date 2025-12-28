// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct MarginProperty : IStyleProperty<Spacing, MarginProperty> {
    public Spacing Value { get; }
    
    public MarginProperty(Spacing value) {
        Value = value;
    }

    public MarginProperty(Dimension left, Dimension right, Dimension top, Dimension bottom) {
        Value = new Spacing(left, right, top, bottom);
    }

    public MarginProperty(Dimension horizontal, Dimension vertical) {
        Value = new Spacing(horizontal, vertical);
    }

    public MarginProperty(Dimension margin) {
        Value = new Spacing(margin);
    }
    
    public MarginProperty() {
        Value = Spacing.Zero;
    }
    
    public Dimension Left   => Value.Left;
    public Dimension Right  => Value.Right;
    public Dimension Top    => Value.Top;
    public Dimension Bottom => Value.Bottom;
}