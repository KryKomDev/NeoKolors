//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Runtime.Versioning;
using NeoKolors.Console;
using NeoKolors.Tui.Events;

namespace NeoKolors.Tui;

/// <summary>
/// a view that contains other views
/// </summary>
public class View : IView {
    
    public IRenderable[] Views { get; private set; }
    public Action<object?, KeyEventArgs>? OnKeyPress { get; } = Aaaah;
    public Action? OnResize { get; }
    public Action<object?, AppStartEventArgs>? OnStart { get; }
    public Action<object?, EventArgs>? OnStop { get; }

    public Rectangle Perimeter { get; private set; }

    public void Render(in IConsoleScreen target) {
        
    }
    
    [SupportedOSPlatform("windows")]
    public static void Aaaah(object? sender, KeyEventArgs args) {
        char c = ((KeyEventArgs)args).PressedKey.KeyChar;
        var ki = ((KeyEventArgs)args).PressedKey;
        NKDebug.Debug($"{ki.Key}, {ki.Modifiers.ToString()}");
        System.Console.Beep((int)(55 * Math.Pow(2d, (c - 'A')/12d)), 200);
    }

}