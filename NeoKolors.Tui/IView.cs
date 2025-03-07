namespace NeoKolors.Tui;

/// <summary>
/// base class for rendered sections
/// </summary>
public interface IView : IRenderable {
    
    /// <summary>
    /// internal views
    /// </summary>
    public IRenderable[] Views { get; }

    public void InteractKey(object? sender, EventArgs args);
}