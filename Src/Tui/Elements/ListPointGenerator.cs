// NeoKolors
// Copyright (c) krystof 2026

using NeoKolors.Common;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Generates a bullet or marker string for a list item given its 1-based index.
/// </summary>
/// <param name="index">The 1-based index of the list item (1, 2, 3, ...).</param>
/// <returns>An AnsiString representing the marker.</returns>
public delegate AnsiString ListPointGenerator(int index);

/// <summary>
/// Predefined list point generators for ordered and unordered lists.
/// </summary>
public static class ListPoints {

    // =========================== UNORDERED =========================== //

    public static ListPointGenerator Bullet        => _ => new AnsiString("•");
    public static ListPointGenerator Circle        => _ => new AnsiString("○");
    public static ListPointGenerator FilledCircle  => _ => new AnsiString("●");
    public static ListPointGenerator Square        => _ => new AnsiString("■");
    public static ListPointGenerator SquareEmpty   => _ => new AnsiString("□");
    public static ListPointGenerator Dash          => _ => new AnsiString("-");
    public static ListPointGenerator Asterisk      => _ => new AnsiString("*");
    public static ListPointGenerator Arrow         => _ => new AnsiString("→");
    public static ListPointGenerator Chevron       => _ => new AnsiString("›");
    public static ListPointGenerator Diamond       => _ => new AnsiString("◆");
    public static ListPointGenerator Triangle      => _ => new AnsiString("▶");

    // ============================ ORDERED ============================ //

    public static ListPointGenerator NumberDot     => i => new AnsiString($"{i}.");
    public static ListPointGenerator NumberParen   => i => new AnsiString($"{i})");
    public static ListPointGenerator NumberBracket => i => new AnsiString($"[{i}]");
    public static ListPointGenerator AlphabetDot   => i => new AnsiString($"{(char)('a' + (i - 1) % 26)}.");
    public static ListPointGenerator AlphabetParen => i => new AnsiString($"{(char)('a' + (i - 1) % 26)})");
    public static ListPointGenerator RomanDot      => i => new AnsiString($"{ToRoman(i)}.");
    public static ListPointGenerator RomanParen    => i => new AnsiString($"{ToRoman(i)})");

    private static string ToRoman(int number) {
        if (number <= 0) return number.ToString();
        string[] thou = ["", "m", "mm", "mmm"];
        string[] hund = ["", "c", "cc", "ccc", "cd", "d", "dc", "dcc", "dccc", "cm"];
        string[] tens = ["", "x", "xx", "xxx", "xl", "l", "lx", "lxx", "lxxx", "xc"];
        string[] ones = ["", "i", "ii", "iii", "iv", "v", "vi", "vii", "viii", "ix"];

        return thou[number / 1000 % 4] +
               hund[number % 1000 / 100] +
               tens[number % 100 / 10] +
               ones[number % 10];
    }
}
