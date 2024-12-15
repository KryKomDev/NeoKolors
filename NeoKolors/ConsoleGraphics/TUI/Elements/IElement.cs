using NeoKolors.ConsoleGraphics.TUI.Style;

namespace NeoKolors.ConsoleGraphics.TUI.Elements;

public interface IElement {
    public string[] Selectors { get; }
    public StyleBlock Style { get; protected set; }
    public void UpdateStyle(StyleBlock style);
    public void Draw(Rectangle rect);
    public static abstract string GetTag();
}