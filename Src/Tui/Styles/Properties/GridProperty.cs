// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct GridProperty : IStyleProperty<GridDimensions, GridProperty> {
    public GridDimensions Value { get; }
    
    public GridProperty(GridDimensions value) {
        Value = value;
    }
    
    public GridProperty() {
        Value = GridDimensions.Default;
    }
}