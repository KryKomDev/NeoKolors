// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct ListPointOffsetProperty : IStyleProperty<int, ListPointOffsetProperty> {
    public int Value { get; }
    
    public ListPointOffsetProperty(int value) => Value = value;
    public ListPointOffsetProperty() => Value = 0;
}