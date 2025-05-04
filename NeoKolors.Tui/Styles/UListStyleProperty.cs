//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("ul-style")]
public struct UListStyleProperty : IStyleProperty<UListStyleProperty, string> {
    
    public string Value { get; }
    
    public UListStyleProperty(string value) {
        Value = value;
    }
    
    public UListStyleProperty() {
        Value = "*";
    }
    
    public static UListStyleProperty Asterisk() => new("*");
    public static UListStyleProperty Dash() => new("-");
    public static UListStyleProperty AsciiCircle() => new("o");
    public static UListStyleProperty AsciiArow() => new("->");
    public static UListStyleProperty AsciiArowDouble() => new("=>");
    public static UListStyleProperty AsciiSimpleArrow() => new(">");
    public static UListStyleProperty Square() => new("■");
    public static UListStyleProperty SquareEmpty() => new("□");
    public static UListStyleProperty Triangle() => new("▶");
    public static UListStyleProperty TriangleEmpty() => new("▷");
    public static UListStyleProperty Circle() => new("●");
    public static UListStyleProperty CircleEmpty() => new("○");
    public static UListStyleProperty DottedCircle() => new("◌");
    public static UListStyleProperty FishEyeCircle() => new("◉");
    public static UListStyleProperty BullseyeCircle() => new("◎");
    public static UListStyleProperty Diamond() => new("◆");
    public static UListStyleProperty DiamondEmpty() => new("◇");
    public static UListStyleProperty DiamondDouble() => new("◈");
    public static UListStyleProperty ArrowBasic() => new("⟶");
    public static UListStyleProperty ArrowTriangle() => new("►");
    public static UListStyleProperty ArrowTriangleEmpty() => new("▻");
    public static UListStyleProperty ArrowTriangleSmall() => new("▸");
    public static UListStyleProperty ArrowTriangleSmallEmpty() => new("▹");
    public static UListStyleProperty ArrowBoldThick() => new("❱");
    public static UListStyleProperty ArrowBoldSimple() => new("❯");
    public static UListStyleProperty ArrowBracketSimple() => new("⟩");
    public static UListStyleProperty ArrowBracketDouble() => new("⟫");
    public static UListStyleProperty ArrowDoubleChevron() => new("»");
    public static UListStyleProperty ArrowSingleChevron() => new("›");
    public static UListStyleProperty Point() => new("∙");
    public static UListStyleProperty PointEmpty() => new("◦");
    public static UListStyleProperty PointLarger() => new("•");
    // •◦
}