using NeoKolors.Common;
using NeoKolors.ConsoleGraphics.TUI.Style;

namespace NeoKolors.ConsoleGraphics.TUI.Elements;

public class Div : IElement {
    
    public Div(IElement[] content, string[] selectors) {
        Content = content;
        Selectors = selectors;
    }

    public IElement[] Content { get; }
    public string[] Selectors { get; }
    public StyleBlock Style { get; set; }
    
    public void UpdateStyle(StyleBlock style) {
        Style = style;
    }

    public void Draw(Rectangle rect) {
        
    }

    public static string GetTag() => "div";
    
    internal static void RenderBorder(IElement e, Rectangle rect) {
        BorderProperty.Border border = (BorderProperty.Border)e.Style.GetProperty<BorderProperty>();
        Color backgroundColor = (Color)e.Style.GetProperty<BackgroundColorProperty>();
        
        BorderProperty.WriteBorder(rect, border, backgroundColor);
    }

    internal static void SetMargin(IElement e, ref int left, ref int top, ref int right, ref int bottom, Rectangle rect) {
        var m = (MarginProperty.MarginData)e.Style.GetProperty<MarginProperty>();

        left = m.Left.Unit switch {
            SizeValue.UnitType.CHAR => m.Left.Value,
            SizeValue.UnitType.PIXEL => m.Left.Value * 3,
            SizeValue.UnitType.PERCENT => m.Left.Value * rect.Width / 100,
            _ => left
        };

        right = m.Right.Unit switch {
            SizeValue.UnitType.CHAR => m.Right.Value,
            SizeValue.UnitType.PIXEL => m.Right.Value * 3,
            SizeValue.UnitType.PERCENT => m.Right.Value * rect.Width / 100,
            _ => right
        };

        top = m.Top.Unit switch {
            SizeValue.UnitType.CHAR or SizeValue.UnitType.PIXEL => m.Top.Value,
            SizeValue.UnitType.PERCENT => m.Top.Value * rect.Width / 100,
            _ => top
        };

        bottom = m.Bottom.Unit switch {
            SizeValue.UnitType.CHAR or SizeValue.UnitType.PIXEL => m.Bottom.Value,
            SizeValue.UnitType.PERCENT => m.Bottom.Value * rect.Width / 100,
            _ => bottom
        };
    }
}