// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Elements;

public class Body : Div {
    public Body(params IElement[] children) : base(children) { }
    
    protected override WidthProperty  DefaultWidth  => new(Dimension.ViewportWidth(100));
    protected override HeightProperty DefaultHeight => new(Dimension.ViewportHeight(100));
}