// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

public class Body : Div {
    protected new static StyleCollection DefaultStyle { get; } = new(Div.DefaultStyle) {
        Width  = Dimension.Stretch,
        Height = Dimension.Stretch,
        
        ReadOnly = true,
    };

    public Body(params IElement[] children) : base(DefaultStyle, children) { }
    public Body(StyleCollection defaultStyle, params IElement[] children) : base(defaultStyle, children) { }
}