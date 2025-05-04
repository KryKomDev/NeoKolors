//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("align-self")]
public struct AlignSelfProperty : IStyleProperty<AlignSelfProperty, AlignDirection> {
    
    public AlignDirection Value { get; }
    
    public AlignSelfProperty(AlignDirection value) {
        Value = value;
    }
    
    public AlignSelfProperty() {
        Value = new AlignDirection();
    }
}