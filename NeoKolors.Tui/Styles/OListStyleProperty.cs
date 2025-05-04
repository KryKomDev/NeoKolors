//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.Contracts;
using NeoKolors.Common.Util;

namespace NeoKolors.Tui.Styles;

[StylePropertyName("ol-style")]
public readonly struct OListStyleProperty : IStyleProperty<OListStyleProperty, OListStyleProperty> {
    
    public NumberingStyle Numbering { get; }
    public string Prefix { get; }
    public string Suffix { get; }
    
    public OListStyleProperty Value => this;

    public OListStyleProperty(NumberingStyle numbering, string suffix, string prefix = "") {
        Numbering = numbering;
        Prefix = prefix;
        Suffix = suffix;
    }
    
    public OListStyleProperty() {
        Numbering = NumberingStyle.NUMBER;
        Prefix = ".";
        Suffix = "";
    }
    
    [Pure]
    public string ToString(int number) => $"{Prefix}{ToString(number, Numbering)}{Suffix}";
    
    [Pure]
    private static string ToString(int number, NumberingStyle numbering) => numbering switch {
        NumberingStyle.NUMBER => number.ToString(),
        NumberingStyle.LOWER_ALPHABET => ((char)('a' + number - 1)).ToString(),
        NumberingStyle.UPPER_ALPHABET => ((char)('A' + number - 1)).ToString(),
        NumberingStyle.LOWER_ROMAN => number.ToRoman(lowercase: true),
        NumberingStyle.UPPER_ROMAN => number.ToRoman(lowercase: false),
        _ => throw new ArgumentOutOfRangeException(nameof(numbering), numbering, null)
    };
    
    public static OListStyleProperty NumberParen() => new(NumberingStyle.NUMBER, ")");
    public static OListStyleProperty LowerAlphaParen() => new(NumberingStyle.LOWER_ALPHABET, ")");
    public static OListStyleProperty UpperAlphaParen() => new(NumberingStyle.UPPER_ALPHABET, ")");
    public static OListStyleProperty LowerRomanParen() => new(NumberingStyle.LOWER_ROMAN, ")");
    public static OListStyleProperty UpperRomanParen() => new(NumberingStyle.UPPER_ROMAN, ")");
    public static OListStyleProperty NumberDot() => new(NumberingStyle.NUMBER, ".");
    public static OListStyleProperty LowerAlphaDot() => new(NumberingStyle.LOWER_ALPHABET, ".");
    public static OListStyleProperty UpperAlphaDot() => new(NumberingStyle.UPPER_ALPHABET, ".");
    public static OListStyleProperty LowerRomanDot() => new(NumberingStyle.LOWER_ROMAN, ".");
    public static OListStyleProperty UpperRomanDot() => new(NumberingStyle.UPPER_ROMAN, ".");
}

public enum NumberingStyle {
    NUMBER,
    LOWER_ALPHABET,
    UPPER_ALPHABET,
    LOWER_ROMAN,
    UPPER_ROMAN
}