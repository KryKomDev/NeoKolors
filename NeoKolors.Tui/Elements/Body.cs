//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Events;
using static NeoKolors.Tui.Elements.ApplicableStylesAttribute.Predefined;

namespace NeoKolors.Tui.Elements;

[ElementName("body")]
[ApplicableStyles(CONTAINER | UNIVERSAL)]
public class Body : Div, IElement, IView {

    public Body(string[] selectors, params IElement[] children) : base(selectors, children) { }
    
    public Body(params IElement[] children) : this([], children) { }
    
    public override void Render(in IConsoleScreen target, Rectangle rect) {
        rect = new Rectangle(0, 0, target.Width - 1, target.Height - 1);
        base.Render(target, rect);
    }
    
    public new int GetWidth(int maxHeight) => throw new InvalidOperationException();
    public new int GetMinWidth(int maxHeight) => throw new InvalidOperationException();
    public new int GetHeight(int maxWidth) => throw new InvalidOperationException();
    public new int GetMinHeight(int maxWidth) => throw new InvalidOperationException();

    public void HandleKeyPress(object? sender, KeyEventArgs args) { }
    public void HandleResize(ResizeEventArgs args) { }
    public void HandleAppStart(object? sender, AppStartEventArgs args) { }
    public void HandleAppStop(object? sender, EventArgs args) { }
}