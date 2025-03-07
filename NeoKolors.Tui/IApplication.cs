namespace NeoKolors.Tui;

public interface IApplication {
    
    /// <summary>
    /// renders the application
    /// </summary>
    public void Render();
    
    /// <summary>
    /// starts the application life-cycle
    /// </summary>
    public void Start();
    
    /// <summary>
    /// stops the application life-cycle
    /// </summary>
    public void Stop(); 
}