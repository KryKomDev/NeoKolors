using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.ConsoleGraphics.TUI.Style;

namespace NeoKolors.ConsoleGraphics.TUI.Elements;

public class Text : IElement {
    public string Content { get; }
    public string[] Selectors { get; }
    public StyleBlock Style { get; set; }
    private readonly string[] words;

    private static readonly StyleBlock DEFAULT_STYLES = new("*", 
        new ColorProperty(ConsoleColor.Gray), 
        new BackgroundColorProperty(ConsoleColor.Black),
        new BorderProperty(BorderProperty.BorderStyle.NONE)
    );
    
    public void UpdateStyle(StyleBlock style) {
        Style = style;
    }

    public void Draw(Rectangle rect) {
        
        // style gathering
        var bgColor = (Color)Style.GetProperty<BackgroundColorProperty>();
        var margin = (MarginProperty.MarginData)Style.GetProperty<MarginProperty>();
        var padding = (PaddingProperty.PaddingData)Style.GetProperty<PaddingProperty>();
        var border = (BorderProperty.BorderData)Style.GetProperty<BorderProperty>();
        var textColor = (Color)Style.GetProperty<ColorProperty>();
        var horizontalAlign = (HorizontalAlignData)Style.GetProperty<HorizontalAlignItemsProperty>();
        var verticalAlign = (VerticalAlignData)Style.GetProperty<VerticalAlignItemsProperty>();
        var display = (DisplayProperty.DisplayData)Style.GetProperty<DisplayProperty>();
        
        // if "display: none" do not do anything
        if (display.Type == DisplayProperty.DisplayType.NONE) return;

        // compute content area
        Rectangle cRect = Div.ComputeBorderRectangle(rect, margin);
        
        // border thickness
        int br = border.Style != BorderProperty.BorderStyle.NONE ? 1 : 0;
        
        // render border
        Div.RenderBorder(border, bgColor, cRect);

        Div.SetPadding(padding, out var pr, out var pt, out var pl, out var pb, cRect);

        // calculate max text width
        int textWidth = cRect.Width - pr - pl - br;

        // split text into lines
        string[] lines = ComputeLines(textWidth);
        
        int lineCounter = 0;

        int vo = 0;

        switch (verticalAlign.Value) {
            case VerticalAlignDirection.TOP:
                vo = 0;
                break;
            case VerticalAlignDirection.BOTTOM:
                if (lines.Length > cRect.Height - br * 2) {
                    vo = 0;
                    break;
                }
                
                vo = cRect.Height - br * 2 - lines.Length + 1;
                break;
            case VerticalAlignDirection.CENTER:
                if (lines.Length > cRect.Height - br * 2) {
                    vo = 0;
                    break;
                }

                vo = (cRect.Height - br * 2 - lines.Length) / 2;
                break;
        }

        int endY;

        if (lines.Length > cRect.HigherY - pb - br - (cRect.LowerY + pt + br + vo)) {
            endY = cRect.HigherY - br;
        }
        else {
            endY = cRect.HigherY - pb - br;
        }
        
        for (int i = cRect.LowerY + pt + br + vo; i <= endY; i++) {
            if (lineCounter >= lines.Length) break;
            
            switch (horizontalAlign.Value) {
                case HorizontalAlignDirection.LEFT:
                    System.Console.SetCursorPosition(cRect.LowerX + pl + br, i);
                    break;
                case HorizontalAlignDirection.CENTER:
                    System.Console.SetCursorPosition(cRect.LowerX + pr + br + (textWidth - lines[lineCounter].VisibleLength()) / 2, i);
                    break;
                case HorizontalAlignDirection.RIGHT:
                    System.Console.SetCursorPosition(cRect.LowerX + pr + br + textWidth - lines[lineCounter].VisibleLength(), i);
                    break;
            }
            
            ConsoleColors.PrintColored(lines[lineCounter], textColor, bgColor);
            lineCounter++;
        }
    }

    public int ComputeHeight(int width) {
        var height = (HeightProperty.SizeData)Style.GetProperty<HeightProperty>();

        // height: num
        if (height.Value.IsStatic) return height.Value.ToChars(width);

        var widthData = (WidthProperty.SizeData)Style.GetProperty<WidthProperty>();

        // width: min-content
        if (widthData.Option == SizeValue.SizeOptions.MIN_CONTENT) {
            int max = 0;

            foreach (var w in ProcessWords()) {
                max = Math.Max(w.Length, max);
            }

            return ComputeLines(max).Length;
        }

        // width: max-content
        if (widthData.Option == SizeValue.SizeOptions.MAX_CONTENT) {
            return Int32.MaxValue;
        }

        // height: auto
        var margin = (MarginProperty.MarginData)Style.GetProperty<MarginProperty>();
        var padding = (PaddingProperty.PaddingData)Style.GetProperty<PaddingProperty>();
        var border = (BorderProperty.BorderData)Style.GetProperty<BorderProperty>();

        Div.SetMargin(margin, out int ml, out int mt, out int mr, out int mb, new Rectangle());
        Div.SetPadding(padding, out int pl, out int pt, out int pr, out int pb, new Rectangle());

        int b = border.Style switch {
            BorderProperty.BorderStyle.NONE => 0,
            _ => 2
        };
        
        return ComputeLines(width - ml - mr - pl - pr - b).Length + mb + mt + pb + pt + b;
    }

    public int ComputeWidth(int height) {
        var size = (WidthProperty.SizeData)Style.GetProperty<WidthProperty>();

        if (size.Value.Option == SizeValue.SizeOptions.AUTO)
            return size.Value.ToChars(height);


        throw new NotImplementedException();
    }

    public static string GetTag() => "p";

    public Text(string content, string[] selectors, StyleBlock style) {
        Content = content.Replace('\n', ' ');
        Selectors = selectors;
        Style = style;
        Style.Merge(DEFAULT_STYLES);
        words = ProcessWords();
    }

    private string[] ProcessWords() {
        string[] newWords = Content.Split(' ');
        
        for (int i = 0; i < newWords.Length; i++) {
            newWords[i] = newWords[i].Trim();
        }
        
        List<string> noEmpty = [];

        foreach (var w in newWords) {
            if (w.Length != 0) {
                noEmpty.Add(w);
            }
        }

        for (int i = 0; i < noEmpty.Count; i++) {
            noEmpty[i] = noEmpty[i].Trim();
        }
        
        return noEmpty.ToArray();
    }

    private static (string[] lines, string remainder) SplitWord(string word, int width) {
        List<string> lines = new List<string>();
        string remainder = word;

        while (remainder.VisibleLength() > width) {
            lines.Add(remainder.Substring(0, Math.Min(width, remainder.Length)));
            remainder = remainder.Substring(Math.Min(width, remainder.Length));
        }
        
        return (lines.ToArray(), remainder);
    }

    private string[] ComputeLines(int textWidth) {
        List<string> lines = new List<string>();
        string line = "";
        
        foreach (var w in words) {
            if (w == "<br>") {
                lines.Add(line);
                line = "";
            }
            else if (w.VisibleLength() > textWidth) {
                var l = SplitWord(w, textWidth);
                lines.AddRange(l.lines);
                line = l.remainder;
            }
            else if ((line + (line != "" ? " " + w : w)).VisibleLength() > textWidth) {
                lines.Add(line);
                line = w;
            }
            else {
                line += (line != "" ? " " + w : w);
            }
        }

        if (line != "") {
            lines.Add(line);
        }
        
        return lines.ToArray();
    }
}