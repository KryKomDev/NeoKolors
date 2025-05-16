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

    public NKStyle StyleTop {
        get => _value.StyleTop;
        set => _value.StyleTop = value;
    }

    public NKStyle StyleBottom {
        get => _value.StyleBottom;
        set => _value.StyleBottom = value;
    }

    public NKStyle StyleLeft {
        get => _value.StyleLeft;
        set => _value.StyleLeft = value;
    }

    public NKStyle StyleRight {
        get => _value.StyleRight;
        set => _value.StyleRight = value;
    }

    public NKStyle StyleTopLeft {
        get => _value.StyleTopLeft;
        set => _value.StyleTopLeft = value;
    }

    public NKStyle StyleTopRight {
        get => _value.StyleTopRight;
        set => _value.StyleTopRight = value;
    }

    public NKStyle StyleBottomLeft {
        get => _value.StyleBottomLeft;
        set => _value.StyleBottomLeft = value;
    }

    public NKStyle StyleBottomRight {
        get => _value.StyleBottomRight;
        set => _value.StyleBottomRight = value;
    }

    public bool IsBorderless => _value.IsBorderless;

    public BorderProperty(BorderStyle value) {
        _value = value;
    }
    
    public BorderProperty() {
        _value = BorderStyle.GetSolid(NKColor.Inherit);
    }
}