// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct ListPointProperty : IStyleProperty<ListPointGenerator, ListPointProperty> {
    
    public ListPointGenerator Value { get; }
    
    public ListPointProperty(ListPointGenerator value) => Value = value;
    public ListPointProperty(AnsiString point) => Value = _ => point;
    public ListPointProperty() : this("*") { }
    
    public static implicit operator ListPointProperty(ListPointGenerator value) => new(value);
    public static implicit operator ListPointGenerator(ListPointProperty property) => property.Value;
    
    // =========================== UNORDERED =========================== //
    
    public static ListPointProperty Asterisk                => new("*");
    public static ListPointProperty Dash                    => new("-");
    public static ListPointProperty AsciiCircle             => new("o");
    public static ListPointProperty AsciiArow               => new("->");
    public static ListPointProperty AsciiArowDouble         => new("=>");
    public static ListPointProperty AsciiSimpleArrow        => new(">");
    public static ListPointProperty Square                  => new("■");
    public static ListPointProperty SquareEmpty             => new("□");
    public static ListPointProperty Triangle                => new("▶");
    public static ListPointProperty TriangleEmpty           => new("▷");
    public static ListPointProperty Circle                  => new("●");
    public static ListPointProperty CircleEmpty             => new("○");
    public static ListPointProperty DottedCircle            => new("◌");
    public static ListPointProperty FishEyeCircle           => new("◉");
    public static ListPointProperty BullseyeCircle          => new("◎");
    public static ListPointProperty Diamond                 => new("◆");
    public static ListPointProperty DiamondEmpty            => new("◇");
    public static ListPointProperty DiamondDouble           => new("◈");
    public static ListPointProperty ArrowBasic              => new("⟶");
    public static ListPointProperty ArrowTriangle           => new("►");
    public static ListPointProperty ArrowTriangleEmpty      => new("▻");
    public static ListPointProperty ArrowTriangleSmall      => new("▸");
    public static ListPointProperty ArrowTriangleSmallEmpty => new("▹");
    public static ListPointProperty ArrowBoldThick          => new("❱");
    public static ListPointProperty ArrowBoldSimple         => new("❯");
    public static ListPointProperty ArrowBracketSimple      => new("⟩");
    public static ListPointProperty ArrowBracketDouble      => new("⟫");
    public static ListPointProperty ArrowDoubleChevron      => new("»");
    public static ListPointProperty ArrowSingleChevron      => new("›");
    public static ListPointProperty Point                   => new("∙");
    public static ListPointProperty PointEmpty              => new("◦");
    public static ListPointProperty PointLarger             => new("•");
    
    
    // ============================ ORDERED ============================ //

    public static ListPointProperty Alphabet           => new(i => ((char)(i - 1 + 'a')).ToString());
    public static ListPointProperty AlphabetDot        => new(i => ((char)(i - 1 + 'a')) + ".");
    public static ListPointProperty AlphabetBracket    => new(i => ((char)(i - 1 + 'a')) + ")");
    public static ListPointProperty Roman              => new(i => string.ToRoman(i, true));
    public static ListPointProperty RomanDot           => new(i => string.ToRoman(i, true) + ".");
    public static ListPointProperty RomanBracket       => new(i => string.ToRoman(i, true) + ")");
    public static ListPointProperty Arabic             => new(i => i.ToString());
    public static ListPointProperty ArabicDot          => new(i => i + ".");
    public static ListPointProperty ArabicBracket      => new(i => i + ")");
    public static ListPointProperty CapAlphabet        => new(i => ((char)(i - 1 + 'A')).ToString());
    public static ListPointProperty CapAlphabetDot     => new(i => ((char)(i - 1 + 'A')) + ".");
    public static ListPointProperty CapAlphabetBracket => new(i => ((char)(i - 1 + 'A')) + ")");
    public static ListPointProperty CapRoman           => new(i => string.ToRoman(i, true));
    public static ListPointProperty CapRomanDot        => new(i => string.ToRoman(i, true) + ".");
    public static ListPointProperty CapRomanBracket    => new(i => string.ToRoman(i, true) + ")");
}