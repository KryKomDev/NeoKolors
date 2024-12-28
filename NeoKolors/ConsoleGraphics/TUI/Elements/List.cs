using NeoKolors.Common;
using NeoKolors.ConsoleGraphics.TUI.Style;

namespace NeoKolors.ConsoleGraphics.TUI.Elements;

public class List : Div {

    public new string[] Selectors { get; }
    public new StyleBlock Style { get; set; }
    public IElement[] Contents { get; set; }

    public List(IElement[] contents, string[] selectors) : base(contents, selectors) {
        Contents = contents;
        Selectors = selectors;
        Style = new StyleBlock("*");
    }
    
    public new void UpdateStyle(StyleBlock style) => Style = style;

    public new void Draw(Rectangle rect) {
        var display = (DisplayProperty.DisplayData)Style.GetProperty<DisplayProperty>();
        var margin = (MarginProperty.MarginData)Style.GetProperty<MarginProperty>();
        var padding = (PaddingProperty.PaddingData)Style.GetProperty<PaddingProperty>();
        var border = (BorderProperty.BorderData)Style.GetProperty<BorderProperty>();
        var bgColor = (Color)Style.GetProperty<BackgroundColorProperty>();

        if (display.Type == DisplayProperty.DisplayType.NONE) return;
        
        var bRect = ComputeBorderRectangle(rect, margin);
        var cRect = ComputeContentRectangle(bRect, padding, border);

        RenderBorder(border, bgColor, bRect);
        RenderList(cRect);
    }

    public new static string GetTag() => "list";
}