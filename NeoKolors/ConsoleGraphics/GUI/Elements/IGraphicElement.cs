//
// NeoKolors
// by KryKom 2024
//

namespace NeoKolors.ConsoleGraphics.GUI.Elements;

/// <summary>
/// base class for all NeoKolors console GUI elements
/// </summary>
public interface IGraphicElement {
    
    /// <summary>
    /// defines the most top-left x-coordinate of the element
    /// </summary>
    int GridX { get; set; }
    
    /// <summary>
    /// defines the most top-left y-coordinate of the element
    /// </summary>
    int GridY { get; set; }
    
    /// <summary>
    /// defines the width of the element
    /// </summary>
    int Width { get; init; }
    
    /// <summary>
    /// defines the height of the element
    /// </summary>
    int Height { get; init; }
    
    /// <summary>
    /// defines the name of the element
    /// </summary>
    string Name { get; init; }
    
    /// <summary>
    /// whether the element is selected or not (should affect only visuals)
    /// </summary>
    bool Selected { get; set; }
    
    /// <summary>
    /// draws the element to console
    /// </summary>
    public void Draw(int x, int y);
    
    /// <summary>
    /// interacts with the element using the key sent to the element
    /// </summary>
    /// <param name="keyInfo">key sent to the element</param>
    public void Interact(ConsoleKeyInfo keyInfo);
    
    /// <summary>
    /// returns the value of the argument represented by the element
    /// </summary>
    public object GetValue();
}