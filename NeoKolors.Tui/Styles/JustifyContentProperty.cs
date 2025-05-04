//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("justify-content")]
public struct JustifyContentProperty : IStyleProperty<JustifyContentProperty, JustifyContent> {
    public JustifyContent Value { get; }
    
    public JustifyContentProperty(JustifyContent value) {
        Value = value;
    }
    
    public JustifyContentProperty() {
        Value = JustifyContent.START;
    }
    
    public static implicit operator JustifyContentProperty(JustifyContent value) => new(value);
}

public enum JustifyContent {
    START,         // [=== = ==          ] start
    CENTER,        // [     === = ==     ] center
    END,           // [          === = ==] end
    SPACE_BETWEEN, // [===      =      ==] spaced with even spaces between elements
    SPACE_AROUND,  // [  ===    =    ==  ] spaced with even padding around elements
    SPACE_EVENLY   // [   ===   =   ==   ] spaced with even spaces between elements and container
}