// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct TextColorProperty : IStyleProperty<NKColor, TextColorProperty> {
    public NKColor Value { get; }
    
    public TextColorProperty(NKColor value) {
        Value = value;
    }
}