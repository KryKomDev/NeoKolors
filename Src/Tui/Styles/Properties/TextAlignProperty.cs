// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;

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