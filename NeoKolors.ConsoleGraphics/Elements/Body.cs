using NeoKolors.Common;
using NeoKolors.ConsoleGraphics.Style;
using NeoKolors.ConsoleGraphics.Style;

namespace NeoKolors.ConsoleGraphics.Elements;

public class Body : Div {
    public Body(IElement[] content) : base(content, []) { }
    public new static string GetTag() => "body";

    public void Draw() {
        var display = (DisplayProperty.DisplayData)Style.GetProperty<DisplayProperty>();
        
        if (display.Type == DisplayProperty.DisplayType.NONE) return;
        
        var margin = (MarginProperty.MarginData)Style.GetProperty<MarginProperty>();
        var padding = (PaddingProperty.PaddingData)Style.GetProperty<PaddingProperty>();
        var border = (BorderProperty.BorderData)Style.GetProperty<BorderProperty>();
        var bgColor = (Color)Style.GetProperty<BackgroundColorProperty>();

        var bRect = ComputeBorderRectangle(
            new Rectangle(0, 0, System.Console.WindowWidth, System.Console.WindowHeight), margin);
        var cRect = ComputeContentRectangle(bRect, padding, border);

        RenderBorder(border, bgColor, bRect);
        
        switch (display.Type) {
            case DisplayProperty.DisplayType.BLOCK:
                RenderBlock(cRect);
                break;
            case DisplayProperty.DisplayType.LIST:
                RenderList(cRect);
                break;
            case DisplayProperty.DisplayType.FLEX:
                RenderFlex(cRect);
                break;
            case DisplayProperty.DisplayType.GRID:
                RenderGrid(cRect);
                break;
        }
    }

    private new void Draw(Rectangle rect) => Draw();
}