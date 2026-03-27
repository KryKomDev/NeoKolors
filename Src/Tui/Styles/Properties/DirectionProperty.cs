// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct DirectionProperty : IStyleProperty<Direction, DirectionProperty> {
    public Direction Value { get; }
    
    public DirectionProperty(Direction value) {
        Value = value;
    }

    public DirectionProperty() {
        Value = Direction.TOP_TO_BOTTOM;
    }
}