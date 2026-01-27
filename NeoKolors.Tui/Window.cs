//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Console.Mouse;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Rendering;

namespace NeoKolors.Tui;

/// <summary>
/// a form of view that can overlap other views or windows
/// </summary>
public class Window : IView {
    
    public IView BaseView { get; set; }
    public void HandleKeyPress(ConsoleKeyInfo info) { }
    public void HandleResize(ResizeEventArgs args) { }
    public void HandleAppStart(object? sender, AppStartEventArgs args) { }
    public void HandleAppStop(object? sender, EventArgs args) { }
    public void HandleMouseEvent(MouseEventInfo info) { }

    public void Render(ICharCanvas target) {
        
    }

    public Window(IView baseView) {
        BaseView = baseView;
    }
}