// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct PositionProperty : IStyleProperty<Position, PositionProperty> {
    public Position Value { get; }

    public PositionProperty(Position value) {
        Value = value;
    }

    public PositionProperty(Dimension x, Dimension y, bool relativeX = false, bool relativeY = false) {
        Value = new Position(x, y, relativeX, relativeY);
    }

    public PositionProperty() {
        Value = new Position();
    }
}