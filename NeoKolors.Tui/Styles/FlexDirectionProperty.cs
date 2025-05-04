//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("flex-direction")]
public struct FlexDirectionProperty : IStyleProperty<FlexDirectionProperty, FlexDirection> {
    public FlexDirection Value { get; }
    
    public FlexDirectionProperty(FlexDirection value) {
        Value = value;
    }
    
    public FlexDirectionProperty() {
        Value = FlexDirection.ROW;
    }
    
    public static implicit operator FlexDirectionProperty(FlexDirection value) => new(value);
}

public enum FlexDirection {
    ROW,
    COLUMN,
    ROW_REVERSE,
    COLUMN_REVERSE
}