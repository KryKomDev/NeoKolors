using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.ConsoleGraphics.TUI.Style;

namespace NeoKolors.ConsoleGraphics.TUI.Elements;

public class Text : IElement {
    public string Content { get; }
    public string[] Selectors { get; }
    public StyleBlock Style { get; set; }
    
    public void UpdateStyle(StyleBlock style) {
        throw new NotImplementedException();
    }

    public void Draw(Rectangle rect) {
        
        Rectangle cRect = rect;
        var bgColor = (Color)Style.GetProperty<BackgroundColorProperty>();
        
        BorderProperty.WriteBorder(rect, new BorderProperty.Border(new Color(ConsoleColor.Black), BorderProperty.BorderStyle.NONE), bgColor);
        
        int mr = 0, ml = 0, mt = 0, mb = 0;
        
        Div.SetMargin(this, ref ml, ref mt, ref mr, ref mb, rect);

        cRect.LowerX += ml;
        cRect.LowerY += mt;
        cRect.HigherX -= mr;
        cRect.HigherY -= mb;
        
        string[] words = ProcessWords();

        List<string> lines = [];
        string line = "";

        // padding and border style
        var p = (PaddingProperty.PaddingData)Style.GetProperty<PaddingProperty>();
        var b = (BorderProperty.Border)Style.GetProperty<BorderProperty>();

        // border thickness
        int br = b.Style != BorderProperty.BorderStyle.NONE ? 1 : 0;

        // calculate right padding
        int pr = p.Right.Unit switch {
            SizeValue.UnitType.CHAR => p.Right.Value,
            SizeValue.UnitType.PIXEL => p.Right.Value * 3,
            SizeValue.UnitType.PERCENT => p.Right.Value * (cRect.Width - br * 2) / 100,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        // calculate left padding
        int pl = p.Right.Unit switch {
            SizeValue.UnitType.CHAR => p.Right.Value,
            SizeValue.UnitType.PIXEL => p.Right.Value * 3,
            SizeValue.UnitType.PERCENT => p.Right.Value * (cRect.Width - br * 2) / 100,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        // calculate max text width
        int textWidth = cRect.Width - pr - pl - br;
        
        // split text into lines
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

        Div.RenderBorder(this, cRect);

        int pt = p.Top.Unit switch {
            SizeValue.UnitType.CHAR => p.Top.Value,
            SizeValue.UnitType.PIXEL => p.Top.Value,
            SizeValue.UnitType.PERCENT => p.Top.Value * (cRect.Height - br * 2) / 100,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        int pb = p.Top.Unit switch {
            SizeValue.UnitType.CHAR => p.Bottom.Value,
            SizeValue.UnitType.PIXEL => p.Bottom.Value,
            SizeValue.UnitType.PERCENT => p.Bottom.Value * (cRect.Height - br * 2) / 100,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var textColor = (Color)Style.GetProperty<ColorProperty>();

        var hai = (HorizontalAlignData)Style.GetProperty<HorizontalAlignItemsProperty>();
        var vai = (VerticalAlignData)Style.GetProperty<VerticalAlignItemsProperty>();

        int lineCounter = 0;

        int vo = 0;

        switch (vai.Data) {
            case VerticalAlignDirection.TOP:
                vo = 0;
                break;
            case VerticalAlignDirection.BOTTOM:
                if (lines.Count > cRect.Height - br * 2) {
                    vo = 0;
                    break;
                }
                
                vo = cRect.Height - br * 2 - lines.Count + 1;
                break;
            case VerticalAlignDirection.CENTER:
                if (lines.Count > cRect.Height - br * 2) {
                    vo = 0;
                    break;
                }

                vo = (cRect.Height - br * 2 - lines.Count) / 2 + 1;
                break;
        }
        
        for (int i = cRect.LowerY + pt + br + vo; i <= cRect.HigherY - pb - br; i++) {
            if (lineCounter >= lines.Count) break;
            
            switch (hai.Data) {
                case HorizontalAlignDirection.LEFT:
                    System.Console.SetCursorPosition(rect.LowerX + pl + br, i);
                    break;
                case HorizontalAlignDirection.CENTER:
                    System.Console.SetCursorPosition(rect.LowerX + pr + br + (textWidth - lines[lineCounter].VisibleLength()) / 2, i);
                    break;
                case HorizontalAlignDirection.RIGHT:
                    System.Console.SetCursorPosition(rect.LowerX + pr + br + textWidth - lines[lineCounter].VisibleLength(), i);
                    break;
            }
            
            ConsoleColors.PrintColored(lines[lineCounter], textColor, bgColor);
            lineCounter++;
        }
    }

    public static string GetTag() => "text";

    public Text(string content, string[] selectors, StyleBlock style) {
        Content = content.Replace('\n', ' ');
        Selectors = selectors;
        Style = style;
    }

    private string[] ProcessWords() {
        string[] words = Content.Split(' ');
        
        for (int i = 0; i < words.Length; i++) {
            words[i] = words[i].Trim();
        }
        
        List<string> noEmpty = [];

        foreach (var w in words) {
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
}