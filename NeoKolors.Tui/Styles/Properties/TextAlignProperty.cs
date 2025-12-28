// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct TextAlignProperty : IStyleProperty<Align, TextAlignProperty> {
    public Align Value { get; }
    
    public TextAlignProperty(Align value) {
        Value = value;
    }
    
    public TextAlignProperty() {
        Value = new Align();
    }
}