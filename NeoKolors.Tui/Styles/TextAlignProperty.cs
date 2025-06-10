//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("text-align")]
public struct TextAlignProperty : IStyleProperty<TextAlignProperty, AlignDirection> {
    public AlignDirection Value { get; }
    
    public TextAlignProperty(AlignDirection value) {
        Value = value;
    }
    
    public TextAlignProperty() {
        Value = new AlignDirection();
    }
}