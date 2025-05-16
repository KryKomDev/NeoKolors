//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Events;

namespace NeoKolors.Tui;

/// <summary>
/// a form of view that can overlap other views or windows
/// </summary>
public class Window : IView {
    
    public IView BaseView { get; set; }
    public void HandleKeyPress(object? sender, KeyEventArgs args) { }
    public void HandleResize(ResizeEventArgs args) { }
    public void HandleAppStart(object? sender, AppStartEventArgs args) { }
    public void HandleAppStop(object? sender, EventArgs args) { }

    public void Render(in IConsoleScreen target) {
        
    }

    public Window(IView baseView) {
        BaseView = baseView;
    }
}