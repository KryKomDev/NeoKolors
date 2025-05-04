//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

namespace NeoKolors.Tui.Styles;

[StylePropertyName("color")]
public struct ColorProperty : IStyleProperty<ColorProperty, NKColor> {
    
    public NKColor Value { get; }
    
    public ColorProperty(NKColor value) {
        Value = value;
    }
    
    public ColorProperty() {
        Value = NKColor.Default;
    }
}