// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct ListPointProperty : IStyleProperty<ListPointGenerator, ListPointProperty> {
    
    public ListPointGenerator Value { get; }
    
    public ListPointProperty(ListPointGenerator value) => Value = value;
    public ListPointProperty(string point) => Value = _ => point;
    public ListPointProperty() => Value = _ => "*";
    
    public static implicit operator ListPointProperty(ListPointGenerator value) => new(value);
    public static implicit operator ListPointGenerator(ListPointProperty property) => property.Value;
    
    // =========================== UNORDERED =========================== //
    
    public static ListPointProperty Asterisk                => new(_ => "*");
    public static ListPointProperty Dash                    => new(_ => "-");
    public static ListPointProperty AsciiCircle             => new(_ => "o");
    public static ListPointProperty AsciiArow               => new(_ => "->");
    public static ListPointProperty AsciiArowDouble         => new(_ => "=>");
    public static ListPointProperty AsciiSimpleArrow        => new(_ => ">");
    public static ListPointProperty Square                  => new(_ => "■");
    public static ListPointProperty SquareEmpty             => new(_ => "□");
    public static ListPointProperty Triangle                => new(_ => "▶");
    public static ListPointProperty TriangleEmpty           => new(_ => "▷");
    public static ListPointProperty Circle                  => new(_ => "●");
    public static ListPointProperty CircleEmpty             => new(_ => "○");
    public static ListPointProperty DottedCircle            => new(_ => "◌");
    public static ListPointProperty FishEyeCircle           => new(_ => "◉");
    public static ListPointProperty BullseyeCircle          => new(_ => "◎");
    public static ListPointProperty Diamond                 => new(_ => "◆");
    public static ListPointProperty DiamondEmpty            => new(_ => "◇");
    public static ListPointProperty DiamondDouble           => new(_ => "◈");
    public static ListPointProperty ArrowBasic              => new(_ => "⟶");
    public static ListPointProperty ArrowTriangle           => new(_ => "►");
    public static ListPointProperty ArrowTriangleEmpty      => new(_ => "▻");
    public static ListPointProperty ArrowTriangleSmall      => new(_ => "▸");
    public static ListPointProperty ArrowTriangleSmallEmpty => new(_ => "▹");
    public static ListPointProperty ArrowBoldThick          => new(_ => "❱");
    public static ListPointProperty ArrowBoldSimple         => new(_ => "❯");
    public static ListPointProperty ArrowBracketSimple      => new(_ => "⟩");
    public static ListPointProperty ArrowBracketDouble      => new(_ => "⟫");
    public static ListPointProperty ArrowDoubleChevron      => new(_ => "»");
    public static ListPointProperty ArrowSingleChevron      => new(_ => "›");
    public static ListPointProperty Point                   => new(_ => "∙");
    public static ListPointProperty PointEmpty              => new(_ => "◦");
    public static ListPointProperty PointLarger             => new(_ => "•");
    
    
    // ============================ ORDERED ============================ //

    public static ListPointProperty Alphabet           => new(i => ((char)(i - 1 + 'a')).ToString());
    public static ListPointProperty AlphabetDot        => new(i => ((char)(i - 1 + 'a')) + ".");
    public static ListPointProperty AlphabetBracket    => new(i => ((char)(i - 1 + 'a')) + ")");
    public static ListPointProperty Roman              => new(i => i.ToRoman(true));
    public static ListPointProperty RomanDot           => new(i => i.ToRoman(true) + ".");
    public static ListPointProperty RomanBracket       => new(i => i.ToRoman(true) + ")");
    public static ListPointProperty Arabic             => new(i => i.ToString());
    public static ListPointProperty ArabicDot          => new(i => i + ".");
    public static ListPointProperty ArabicBracket      => new(i => i + ")");
    public static ListPointProperty CapAlphabet        => new(i => ((char)(i - 1 + 'A')).ToString());
    public static ListPointProperty CapAlphabetDot     => new(i => ((char)(i - 1 + 'A')) + ".");
    public static ListPointProperty CapAlphabetBracket => new(i => ((char)(i - 1 + 'A')) + ")");
    public static ListPointProperty CapRoman           => new(i => i.ToRoman());
    public static ListPointProperty CapRomanDot        => new(i => i.ToRoman() + ".");
    public static ListPointProperty CapRomanBracket    => new(i => i.ToRoman() + ")");
}