// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct BackgroundColorProperty : IStyleProperty<NKColor, BackgroundColorProperty> {
    public NKColor Value { get; }
    
    public BackgroundColorProperty(NKColor value) {
        Value = value;
    }
}