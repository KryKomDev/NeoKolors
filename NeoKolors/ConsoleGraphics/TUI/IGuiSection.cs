using NeoKolors.ConsoleGraphics.TUI.Elements;

namespace NeoKolors.ConsoleGraphics.TUI;

public interface IGuiSection {
    
    /// <summary>
    /// returns the position of the lower-value coordinates defining the section area
    /// </summary>
    public (int x, int y) GetPositionA();
    
    /// <summary>
    /// returns the position of the higher-value coordinates defining the section area
    /// </summary>
    public (int x, int y) GetPositionB();
    
    /// <summary>
    /// sets the actual lower-value coordinate
    /// </summary>
    internal void SetPositionA(int x, int y);
    
    /// <summary>
    /// sets the actual higher-value coordinate
    /// </summary>
    internal void SetPositionB(int x, int y);
    
    /// <summary>
    /// returns whether the section is active
    /// </summary>
    /// <returns>true - active, false - inactive</returns>
    public bool IsActive();
    
    /// <summary>
    /// returns graphical elements contained in the section
    /// </summary>
    public IGraphicElement[] GetArguments();

    /// <summary>
    /// returns sections contained in the sections
    /// </summary>
    /// <returns></returns>
    public IGuiSection[] GetSections();
}