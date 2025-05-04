//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

namespace NeoKolors.Tui.Styles;

[StylePropertyName("background-color")]
public struct BackgroundColorProperty : IStyleProperty<BackgroundColorProperty, NKColor> {
    
    public NKColor Value { get; }
    
    public BackgroundColorProperty(NKColor value) {
        Value = value;
    }
    
    public BackgroundColorProperty() {
        Value = NKColor.Inherit;
    }
}