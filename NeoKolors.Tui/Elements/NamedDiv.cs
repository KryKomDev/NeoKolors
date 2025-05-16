//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

[ElementName("ndiv")]
[ApplicableStyles(typeof(Div), "ndiv-title")]
public class NamedDiv : Div {
    
    public NDivTitleProperty TitleStyle => 
        Style["ndiv-title"].Value is NDivTitleProperty style ? style : new NDivTitleProperty().Value;
    
    public string Title { get; }

    public NamedDiv(string title, string[] selectors, params IElement[] content) : base(selectors, content) {
        Title = title;
    }

    public NamedDiv(string title, params IElement[] content) : base([], content) {
        Title = title;
    }

    public override void Render(in IConsoleScreen target, Rectangle rect) {
        base.Render(target, rect);
        if (Border.IsBorderless) return;
        var border = IElement.GetBorderRect(rect, Margin);

        string t = Title;

        if (TitleStyle.Padding) {
            t = t.Length > border.Width - 6 
                ? $" {t[..Math.Max(border.Width - 7, 0)]}… " 
                : $" {t} ";
        }
        else if (t.Length > border.Width - 4) {
            t = $"{t[..Math.Max(border.Width - 5, 0)]}…";
        }

        int xOffset = TitleStyle.Align switch {
            HorizontalAlign.LEFT => 2,
            HorizontalAlign.CENTER => border.Width / 2 - t.Length / 2 + 1,
            HorizontalAlign.RIGHT => border.Width - t.Length - 2,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        target.DrawText(t, border.LowerX + xOffset, border.LowerY, TitleStyle.Style);
    }
}