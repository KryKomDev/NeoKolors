//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("align-items")]
public struct AlignItemsProperty : IStyleProperty<AlignItemsProperty, AlignItems> {
    public AlignItems Value { get; }

    public AlignItemsProperty(AlignItems value) {
        Value = value; 
    }

    public AlignItemsProperty() {
        Value = AlignItems.START;
    }
    
    public static implicit operator AlignItemsProperty(AlignItems value) => new(value);
}

public enum AlignItems {
    START,
    CENTER,
    END,
    STRETCH
}