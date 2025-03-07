namespace NeoKolors.Tui;

/// <summary>
/// a form of view that can overlap other views or windows
/// </summary>
public class Window : IView {
    
    public IRenderable[] Views { get; private set; }

    public void Render() {
        throw new NotImplementedException();
    }
    
    public void InteractKey(object? sender, EventArgs args) {
        throw new NotImplementedException();
    }
}