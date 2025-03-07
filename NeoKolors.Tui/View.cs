//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui;

/// <summary>
/// a view that contains other views
/// </summary>
public class View : IView {
    
    public IRenderable[] Views { get; private set; }
    
    public Rectangle Perimeter { get; private set; }

    public void Render() {
        throw new NotImplementedException();
    }
    
    public void InteractKey(object? sender, EventArgs args) { 
        throw new NotImplementedException();
    }
}