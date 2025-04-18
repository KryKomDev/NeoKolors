//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Events;

namespace NeoKolors.Tui;

/// <summary>
/// base class for rendered sections
/// </summary>
public interface IView : IRenderable {
    
    /// <summary>
    /// internal views
    /// </summary>
    public IRenderable[] Views { get; }
    
    /// <summary>
    /// delegate called on key press
    /// </summary>
    public Action<object?, KeyEventArgs>? OnKeyPress { get; }
    
    /// <summary>
    /// delegate called on terminal resize
    /// </summary>
    public Action? OnResize { get; }
    
    /// <summary>
    /// delegate called when the application is started
    /// </summary>
    public Action<object?, AppStartEventArgs>? OnStart { get; }
    
    /// <summary>
    /// delegate called when the application is stopped
    /// </summary>
    public Action<object?, EventArgs>? OnStop { get; }
    
    
}