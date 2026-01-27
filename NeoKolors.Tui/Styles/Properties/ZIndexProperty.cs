// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct ZIndexProperty : IStyleProperty<int, ZIndexProperty> {
    public int Value { get; }
    
    public ZIndexProperty(int value) => Value = value;
    public ZIndexProperty() => Value = 0;
}