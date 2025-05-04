//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

namespace NeoKolors.Tui.Styles;

[StylePropertyName("border")]
public struct BorderProperty : IStyleProperty<BorderProperty, BorderStyle> {
    
    private BorderStyle _value;
    public BorderStyle Value => _value;

    public char Vertical {
        get => _value.Vertical;
        set => _value.Vertical = value;
    }

    public char Horizontal {
        get => _value.Horizontal;
        set => _value.Horizontal = value;
    }

    public char TopRight {
        get => _value.TopRight;
        set => _value.TopRight = value;
    }

    public char TopLeft {
        get => _value.TopLeft;
        set => _value.TopLeft = value;
    }

    public char BottomRight {
        get => _value.BottomRight;
        set => _value.BottomRight = value;
    }

    public char BottomLeft {
        get => _value.BottomLeft;
        set => _value.BottomLeft = value;
    }

    public NKColor FColor {
        get => _value.FColor;
        set => _value.FColor = value;
    }

    public NKColor BColor {
        get => _value.BColor;
        set => _value.BColor = value;
    }
    
    public BorderProperty(BorderStyle value) {
        _value = value;
    }
    
    public BorderProperty() {
        _value = BorderStyle.GetSolid(NKColor.Inherit);
    }
}