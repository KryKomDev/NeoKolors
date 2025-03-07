using NeoKolors.Tui.Style;

namespace NeoKolors.Tui.Elements;

public interface IElement {
    public string[] Selectors { get; }
    public StyleBlock Style { get; protected set; }
    public void UpdateStyle(StyleBlock style);
    public void Render(Rectangle rect);
    public int ComputeHeight(int width);
    public int ComputeWidth(int height);
    public static abstract string GetTag();
}