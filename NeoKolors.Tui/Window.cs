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
    
    public IRenderable[] Views { get; private set; }
    public Action<object?, KeyEventArgs>? OnKeyPress { get; }
    public Action? OnResize { get; }
    public Action<object?, AppStartEventArgs>? OnStart { get; }
    public Action<object?, EventArgs>? OnStop { get; }

    public void Render(in IConsoleScreen target) {
        throw new NotImplementedException();
    }
}