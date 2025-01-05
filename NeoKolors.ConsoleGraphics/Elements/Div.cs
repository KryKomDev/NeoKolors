using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.ConsoleGraphics.Style;
using NeoKolors.ConsoleGraphics.Style;

namespace NeoKolors.ConsoleGraphics.Elements;

public class Div : IElement {
    
    public Div(IElement[] content, string[] selectors, string? title = null) {
        Content = content;
        Selectors = selectors;
        Title = title;
        Style = new StyleBlock(selectors[0]);
    }

    public IElement[] Content { get; }
    public string? Title { get; }
    
    public string[] Selectors { get; }
    public StyleBlock Style { get; set; }
    
    
    public void UpdateStyle(StyleBlock style) {
        Style = style;
    }

    public void Draw(Rectangle rect) {
        var display = (DisplayProperty.DisplayData)Style.GetProperty<DisplayProperty>();
        var margin = (MarginProperty.MarginData)Style.GetProperty<MarginProperty>();
        var padding = (PaddingProperty.PaddingData)Style.GetProperty<PaddingProperty>();
        var border = (BorderProperty.BorderData)Style.GetProperty<BorderProperty>();
        var bgColor = (Color)Style.GetProperty<BackgroundColorProperty>();

        if (display.Type == DisplayProperty.DisplayType.NONE) return;
        
        var bRect = ComputeBorderRectangle(rect, margin);
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

    protected void RenderBlock(Rectangle rect) {
        int y = rect.LowerY;

        foreach (var e in Content) {
            if (y >= rect.HigherY) break;

            int height = e.ComputeHeight(rect.Width);
            
            if (y + height + 1 >= rect.HigherY) {
                e.Draw(new Rectangle(rect.LowerX, y, rect.HigherX, rect.HigherY));
                break;
            }

            e.Draw(new Rectangle(rect.LowerX, y, rect.HigherX, y + height - 1));
            y += height;
        }
    }

    protected void RenderList(Rectangle rect) {
        // BorderProperty.Border border = new BorderProperty.Border(ConsoleColor.White, BorderProperty.BorderStyle.ASCII);
        // RenderBorder(border, (Color)Style.GetProperty<BackgroundColorProperty>(), rect);
        
        var listStyle = (ListStyleProperty.ListStyleData)Style.GetProperty<ListStyleProperty>();
        
        string point = listStyle.GetPoint();
        int y = rect.LowerY;
        int contentOffset = point.VisibleLength() + 1;

        foreach (var e in Content) {
            if (y >= rect.HigherY) break;
            
            int height = e.ComputeHeight(rect.Width - contentOffset);

            int b = ((BorderProperty.BorderData)e.Style.GetProperty<BorderProperty>()).Style ==
                    BorderProperty.BorderStyle.NONE
                ? 0
                : 1;
            
            SetPadding(
                (PaddingProperty.PaddingData)e.Style.GetProperty<PaddingProperty>()
                , out _, out int p, out _, out _, 
                new Rectangle(rect.LowerX + contentOffset, y, rect.HigherX, Math.Min(rect.HigherY, y + height - 1)));
            SetMargin(
                (MarginProperty.MarginData)e.Style.GetProperty<MarginProperty>()
                , out _, out int m, out _, out _, 
                new Rectangle(rect.LowerX + contentOffset, y, rect.HigherX, Math.Min(rect.HigherY, y + height - 1)));

            int yOffset = b + p + m;

            if (rect.IsInside(rect.LowerX, y + yOffset)) {
                System.Console.SetCursorPosition(rect.LowerX, y + yOffset);
                ConsoleColors.PrintColored(point,
                    ((ListStyleProperty.ListStyleData)Style.GetProperty<ListStyleProperty>()).Color,
                    (Color)Style.GetProperty<BackgroundColorProperty>());
            }

            if (y + height > rect.HigherY) {
                e.Draw(new Rectangle(rect.LowerX + contentOffset, y, rect.HigherX, rect.HigherY));
                break;
            }

            e.Draw(new Rectangle(rect.LowerX + contentOffset, y, rect.HigherX, y + height - 1));
            y += height;
        }
    }

    protected void RenderFlex(Rectangle rect) {
        // BorderProperty.Border border = new BorderProperty.Border(ConsoleColor.White, BorderProperty.BorderStyle.ASCII);
        // RenderBorder(border, (Color)Style.GetProperty<BackgroundColorProperty>(), rect);
        
        var flex = (FlexFlowProperty.FlexFlowData)Style.GetProperty<FlexFlowProperty>();

        if (flex.Wrap == false) {
            uint pieces = 0;

            foreach (var e in Content) {
                pieces += (FlexGrowProperty.FlexGrowData)e.Style.GetProperty<FlexGrowProperty>();
            }

            float single;
            uint i;
            
            switch (flex.Direction) {
                case FlexFlowProperty.FlexDirection.ROW:
                    single = rect.Width / (float)pieces;
                    i = 0;
                    
                    foreach (var e in Content) {
                        uint width = (FlexGrowProperty.FlexGrowData)e.Style.GetProperty<FlexGrowProperty>();
                        e.Draw(new Rectangle(
                            (int)(rect.LowerX + i * single), rect.LowerY, 
                            (int)(rect.LowerX + (i + width) * single) - 1, rect.HigherY));
                        i += width;
                    }
                    
                    break;
                case FlexFlowProperty.FlexDirection.ROW_REVERSE:
                    single = rect.Width / (float)pieces;
                    i = pieces - 1;
                    
                    foreach (var e in Content) {
                        uint width = (FlexGrowProperty.FlexGrowData)e.Style.GetProperty<FlexGrowProperty>();
                        e.Draw(new Rectangle(
                            (int)(rect.LowerX + i * single), rect.LowerY, 
                            (int)(rect.LowerX + (i + width) * single) - 1, rect.HigherY));
                        i -= width;
                    }
                    
                    break;
                case FlexFlowProperty.FlexDirection.COLUMN:
                    single = rect.Height / (float)pieces;
                    i = 0;
                    
                    foreach (var e in Content) {
                        uint height = (FlexGrowProperty.FlexGrowData)e.Style.GetProperty<FlexGrowProperty>();
                        e.Draw(new Rectangle(
                            rect.LowerX, (int)(rect.LowerY + i * single), 
                            rect.HigherX, (int)(rect.LowerY + (i + height) * single) - 1));
                        i += height;
                    }
                    
                    break;
                case FlexFlowProperty.FlexDirection.COLUMN_REVERSE:
                    single = rect.Height / (float)pieces;
                    i = pieces - 1;
                    
                    foreach (var e in Content) {
                        uint height = (FlexGrowProperty.FlexGrowData)e.Style.GetProperty<FlexGrowProperty>();
                        e.Draw(new Rectangle(
                            rect.LowerX, (int)(rect.LowerY + i * single), 
                            rect.HigherX, (int)(rect.LowerY + (i + height) * single) - 1));
                        i -= height;
                    }
                    
                    break;
            }
        }
        
        
    }

    protected void RenderGrid(Rectangle rect) {
        
    }

    public static string GetTag() => "div";
    
    public int ComputeHeight(int width) {
        throw new NotImplementedException();
    }

    public int ComputeWidth(int height) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// see <see cref="BorderProperty.WriteBorder"/>
    /// </summary>
    internal static void RenderBorder(BorderProperty.BorderData borderData, Color backgroundColor, Rectangle rect) => BorderProperty.WriteBorder(rect, borderData, backgroundColor);

    internal static void SetMargin(MarginProperty.MarginData m, out int left, out int top, out int right, out int bottom, Rectangle rect) {
        left = m.Left.ToChars(rect.Width);
        right = m.Right.ToChars(rect.Width);
        top = m.Top.ToChars(rect.Height);
        bottom = m.Bottom.ToChars(rect.Height);
    }

    internal static Rectangle ComputeBorderRectangle(Rectangle rect, MarginProperty.MarginData margin) {
        Rectangle r = rect;
        
        SetMargin(margin, out var ml, out var mt, out var mr, out var mb, rect);

        r.LowerX += ml;
        r.LowerY += mt;
        r.HigherX -= mr;
        r.HigherY -= mb;
        
        return r;
    }
    
    internal static void SetPadding(PaddingProperty.PaddingData m, out int left, out int top, out int right, out int bottom, Rectangle rect) {
        left = m.Left.ToChars(rect.Width);
        right = m.Right.ToChars(rect.Width);
        top = m.Top.ToChars(rect.Height);
        bottom = m.Bottom.ToChars(rect.Height);
    }

    internal static Rectangle ComputeContentRectangle(Rectangle rect, PaddingProperty.PaddingData padding, BorderProperty.BorderData borderData) {
        Rectangle r = rect;

        int b = borderData.Style switch {
            BorderProperty.BorderStyle.NONE => 0,
            _ => 1
        };
        
        SetPadding(padding, out var pl, out var pt, out var pr, out var pb, rect);
    
        r.LowerX += pl + b;
        r.LowerY += pt + b;
        r.HigherX -= (pr + b);
        r.HigherY -= (pb + b);
        
        return r;
    }
}