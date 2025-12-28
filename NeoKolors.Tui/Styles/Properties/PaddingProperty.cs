// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct PaddingProperty : IStyleProperty<Spacing, PaddingProperty> {
    public Spacing Value { get; }
    
    public PaddingProperty(Spacing value) {
        Value = value;
    }

    public PaddingProperty(Dimension left, Dimension right, Dimension top, Dimension bottom) {
        Value = new Spacing(left, right, top, bottom);
    }

    public PaddingProperty(Dimension horizontal, Dimension vertical) {
        Value = new Spacing(horizontal, vertical);
    }

    public PaddingProperty(Dimension padding) {
        Value = new Spacing(padding);
    }
    
    public PaddingProperty() {
        Value = new Spacing();
    }
    
    public Dimension Top    => Value.Top;
    public Dimension Right  => Value.Right;
    public Dimension Bottom => Value.Bottom;
    public Dimension Left   => Value.Left;
}