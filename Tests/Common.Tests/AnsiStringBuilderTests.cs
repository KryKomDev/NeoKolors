// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Common.Tests;

public class AnsiStringBuilderTests
{
    [Fact]
    public void Constructors_ShouldInitializeCorrectly()
    {
        // Empty constructor
        var builder1 = new AnsiStringBuilder();
        Assert.Equal(0, builder1.Length);
        Assert.Equal(string.Empty, builder1.ToAnsiString().Plain);

        // Capacity constructor
        var builder2 = new AnsiStringBuilder(50);
        Assert.Equal(0, builder2.Length);
        Assert.True(builder2.Capacity >= 50);

        // String constructor
        var builder3 = new AnsiStringBuilder("Hello");
        Assert.Equal(5, builder3.Length);
        Assert.Equal("Hello", builder3.ToAnsiString().Plain);

        // String + capacity constructor
        var builder4 = new AnsiStringBuilder("Hello", 100);
        Assert.Equal(5, builder4.Length);
        Assert.Equal("Hello", builder4.ToAnsiString().Plain);
        Assert.True(builder4.Capacity >= 100);

        // AnsiString constructor
        var style = new NKStyle(NKConsoleColor.RED);
        var ansiStr = new AnsiString("Hello", style);
        var builder5 = new AnsiStringBuilder(ansiStr);
        Assert.Equal(5, builder5.Length);
        Assert.Equal("Hello", builder5.ToAnsiString().Plain);
        Assert.Equal(style, builder5[0].Style);

        // AnsiString + capacity constructor
        var builder6 = new AnsiStringBuilder(ansiStr, 100);
        Assert.Equal(5, builder6.Length);
        Assert.Equal("Hello", builder6.ToAnsiString().Plain);
        Assert.Equal(style, builder6[0].Style);
        Assert.True(builder6.Capacity >= 100);
    }

    [Fact]
    public void Length_PropertyChange_ShouldTruncateOrPad()
    {
        var builder = new AnsiStringBuilder("Hello");

        // Truncate
        builder.Length = 3;
        Assert.Equal(3, builder.Length);
        Assert.Equal("Hel", builder.ToAnsiString().Plain);

        // Pad
        builder.Length = 6;
        Assert.Equal(6, builder.Length);
        Assert.Equal("Hel\0\0\0", builder.ToAnsiString().Plain);
        Assert.Equal(NKStyle.Default, builder[5].Style);
    }

    [Fact]
    public void Indexer_ShouldGetAndSet()
    {
        var builder = new AnsiStringBuilder("Hello");
        var style = new NKStyle(NKConsoleColor.BLUE);

        Assert.Equal('H', builder[0].Char);

        builder[0] = new AnsiChar('X', style);
        Assert.Equal('X', builder[0].Char);
        Assert.Equal(style, builder[0].Style);
    }

    [Fact]
    public void Append_Methods_ShouldAddCorrectly()
    {
        var builder = new AnsiStringBuilder();
        var styleRed = new NKStyle(NKConsoleColor.RED);
        var styleBlue = new NKStyle(NKConsoleColor.BLUE);

        builder.CurrentStyle = styleRed;

        // Append char
        builder.Append('A');
        Assert.Equal('A', builder[0].Char);
        Assert.Equal(styleRed, builder[0].Style);

        // Append char with style override
        builder.Append('B', styleBlue);
        Assert.Equal('B', builder[1].Char);
        Assert.Equal(styleBlue, builder[1].Style);

        // Append char repeating
        builder.Append('C', 2);
        Assert.Equal("ABCC", builder.ToAnsiString().Plain);
        Assert.Equal(styleRed, builder[2].Style);
        Assert.Equal(styleRed, builder[3].Style);

        // Append string
        builder.Append("DEF");
        Assert.Equal("ABCCDEF", builder.ToAnsiString().Plain);
        Assert.Equal(styleRed, builder[6].Style);

        // Append string with style
        builder.Append("GHI", styleBlue);
        Assert.Equal("ABCCDEFGHI", builder.ToAnsiString().Plain);
        Assert.Equal(styleBlue, builder[7].Style);

        // Append substring
        builder.Append("XYZ123", 3, 3); // "123"
        Assert.Equal("ABCCDEFGHI123", builder.ToAnsiString().Plain);
        Assert.Equal(styleRed, builder[10].Style);

        // Append AnsiString
        var ansiStr = new AnsiString("Ansi", styleBlue);
        builder.Append(ansiStr);
        Assert.Equal("ABCCDEFGHI123Ansi", builder.ToAnsiString().Plain);
        Assert.Equal(styleBlue, builder[13].Style);

        // Append primitives
        builder.Append(123);
        builder.Append(true, styleBlue);
        Assert.Contains("123True", builder.ToAnsiString().Plain);
    }

    [Fact]
    public void AppendLine_Methods_ShouldAddNewline()
    {
        var builder = new AnsiStringBuilder();
        builder.AppendLine("Hello");

        var plain = builder.ToAnsiString().Plain;
        Assert.Contains("Hello", plain);
        Assert.Contains(Environment.NewLine, plain);
    }

    [Fact]
    public void Insert_Methods_ShouldInsertCorrectly()
    {
        var builder = new AnsiStringBuilder("Hello");
        var style = new NKStyle(NKConsoleColor.GREEN);

        // Insert char
        builder.Insert(1, 'X', style);
        Assert.Equal("HXello", builder.ToAnsiString().Plain);
        Assert.Equal(style, builder[1].Style);

        // Insert string
        builder.Insert(3, "ABC");
        Assert.Equal("HXeABCllo", builder.ToAnsiString().Plain);

        // Insert AnsiString
        var ansiStr = new AnsiString("World", style);
        builder.Insert(0, ansiStr);
        Assert.Equal("WorldHXeABCllo", builder.ToAnsiString().Plain);
        Assert.Equal(style, builder[0].Style);
    }

    [Fact]
    public void Remove_And_Clear_ShouldModifyBuilder()
    {
        var builder = new AnsiStringBuilder("Hello World");
        builder.Remove(5, 6);
        Assert.Equal("Hello", builder.ToAnsiString().Plain);

        builder.Clear();
        Assert.Equal(0, builder.Length);
    }

    [Fact]
    public void Replace_Char_ShouldReplaceMatchingChars()
    {
        var style = new NKStyle(NKConsoleColor.RED);
        var builder = new AnsiStringBuilder();
        builder.Append('a', style);
        builder.Append('b', style);
        builder.Append('a', style);

        builder.Replace('a', 'x');
        Assert.Equal("xbx", builder.ToAnsiString().Plain);
        Assert.Equal(style, builder[0].Style);
        Assert.Equal(style, builder[2].Style);
    }

    [Fact]
    public void Replace_String_ShouldReplaceMatchingSubstrings()
    {
        var style = new NKStyle(NKConsoleColor.BLUE);
        var builder = new AnsiStringBuilder();
        builder.Append("hello world", style);

        builder.Replace("world", "there");
        Assert.Equal("hello there", builder.ToAnsiString().Plain);
        Assert.Equal(style, builder[6].Style);
        Assert.Equal(style, builder[10].Style);
    }

    [Fact]
    public void Styling_Methods_ShouldApplyCorrectly()
    {
        var builder = new AnsiStringBuilder("Hello World");
        var red = new NKStyle(NKConsoleColor.RED);

        builder.ApplyStyle(red, 0, 5);
        Assert.Equal(red, builder[0].Style);
        Assert.Equal(red, builder[4].Style);
        Assert.Equal(NKStyle.Default, builder[5].Style);
    }

    [Fact]
    public void ImplicitConversion_ShouldWork()
    {
        var builder = new AnsiStringBuilder("Hello");
        AnsiString? ansi = builder;
        Assert.NotNull(ansi);
        Assert.Equal("Hello", ansi.Plain);
    }

    [Fact]
    public void SetFColor_PreservesBackgroundAndStyles()
    {
        var initial = new NKStyle(f: NKConsoleColor.RED, b: NKConsoleColor.BLUE, s: TextStyles.BOLD);
        var builder = new AnsiStringBuilder("Hello", 10).ApplyStyle(initial);

        builder.SetFColor(NKConsoleColor.GREEN);

        Assert.Equal(NKConsoleColor.GREEN, builder[0].Style.FColor.AsPalette);
        Assert.Equal(NKConsoleColor.BLUE, builder[0].Style.BColor.AsPalette);
        Assert.True(builder[0].Style.Styles.HasFlag(TextStyles.BOLD));
    }

    [Fact]
    public void TextStyles_Add_Remove_Toggle_WorkOnBuilder()
    {
        var initial = new NKStyle(f: NKConsoleColor.RED, s: TextStyles.BOLD);
        var builder = new AnsiStringBuilder("Test").ApplyStyle(initial);

        builder.AddStyles(TextStyles.ITALIC);
        Assert.True(builder[0].Style.Styles.HasFlag(TextStyles.BOLD));
        Assert.True(builder[0].Style.Styles.HasFlag(TextStyles.ITALIC));

        builder.RemoveStyles(TextStyles.BOLD);
        Assert.False(builder[0].Style.Styles.HasFlag(TextStyles.BOLD));
        Assert.True(builder[0].Style.Styles.HasFlag(TextStyles.ITALIC));

        builder.ToggleStyles(TextStyles.ITALIC | TextStyles.BOLD);
        Assert.True(builder[0].Style.Styles.HasFlag(TextStyles.BOLD));
        Assert.False(builder[0].Style.Styles.HasFlag(TextStyles.ITALIC));
    }
}
