using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.ConsoleGraphics.TUI.Style;
using NeoKolors.Settings.ArgumentTypes;

namespace NeoKolors.ConsoleGraphics.TUI.Elements.Interactive;

public class BoolInteractiveElement : IInteractiveElement<BoolArgument> {

    public BoolArgument Argument { get; }
    public string[] Selectors { get; }
    public StyleBlock Style { get; set; }
    public string Title { get; }

    public BoolInteractiveElement(string title, string[] selectors, BoolArgument argument, StyleBlock style) {
        Title = title;
        Selectors = selectors;
        Argument = argument;
        Style = style;
    }

    public void Interact(ConsoleKeyInfo keyInfo) {
        switch (keyInfo.Key) {
            case ConsoleKey.N or ConsoleKey.RightArrow:
                Argument.Set(true);
                break;
            case ConsoleKey.Y or ConsoleKey.LeftArrow:
                Argument.Set(false);
                break;
        }
        
        // Ctrl + R
        if (keyInfo.KeyChar == 0x00000012) Argument.Reset();
    }

    public void UpdateStyle(StyleBlock style) => Style = style;

    public void Draw(Rectangle rect) {
        var display = (DisplayProperty.DisplayData)Style.GetProperty<DisplayProperty>();

        if (display.Type == DisplayProperty.DisplayType.NONE) return;

        var margin = (MarginProperty.MarginData)Style.GetProperty<MarginProperty>();
        var border = (BorderProperty.BorderData)Style.GetProperty<BorderProperty>();
        var padding = (PaddingProperty.PaddingData)Style.GetProperty<PaddingProperty>();
        var background = (Color)Style.GetProperty<BackgroundColorProperty>();

        var bRect = Div.ComputeBorderRectangle(rect, margin);
        var cRect = Div.ComputeContentRectangle(bRect, padding, border);

        Div.RenderBorder(border, background, bRect);

        int inputWidth = Title.Length + 8;

        var alignH = (HorizontalAlignData)Style.GetProperty<HorizontalAlignItemsProperty>();
        var alignV = (VerticalAlignData)Style.GetProperty<VerticalAlignItemsProperty>();

        int xOffset = 0;
        int yOffset = 0;

        switch (alignH.Value) {
            case HorizontalAlignDirection.LEFT:
                xOffset = 0;
                break;
            case HorizontalAlignDirection.CENTER:
                xOffset = Math.Max(cRect.Width - inputWidth, 0) / 2;
                break;
            case HorizontalAlignDirection.RIGHT:
                xOffset = cRect.Width - inputWidth;
                break;
        }

        switch (alignV.Value) {
            case VerticalAlignDirection.TOP:
                yOffset = 0;
                break;
            case VerticalAlignDirection.CENTER:
                yOffset = cRect.Height / 2;
                break;
            case VerticalAlignDirection.BOTTOM:
                yOffset = cRect.Height;
                break;
        }

        System.Console.SetCursorPosition(cRect.LowerX + xOffset, cRect.LowerY + yOffset);

        // if (Selected)
            ConsoleColors.PrintColored($"{Title}: ", 0xffffff);
        // else
        // System.Console.Write($"{Title}: ");

        var checkbox = (CheckboxProperty.CheckboxData)Style.GetProperty<CheckboxProperty>();
        
        ConsoleColors.PrintComplexColored(checkbox.GetString(Argument.Get()));
    }

    public int ComputeHeight(int width) {
        return 1;
    }

    public int ComputeWidth(int height) {
        return 1;
    }

    public static string GetTag() {
        return "bool";
    }
}