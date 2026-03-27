// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct WrapDirectionProperty : IStyleProperty<Direction, WrapDirectionProperty> {
    public Direction Value { get; }
    
    public WrapDirectionProperty(Direction value) {
        Value = value;
    }

    public WrapDirectionProperty() {
        Value = Direction.TOP_TO_BOTTOM;
    }
}