namespace NeoKolors.Common.Tests;

public class AnsiStringTests {
    
    [Fact]
    public void Constructor_Empty_CreatesEmptyString() {
        var ansi = new AnsiString();
        Assert.Equal(0, ansi.Length);
        Assert.Equal(string.Empty, ansi.Plain);
        Assert.Equal(string.Empty, ansi.ToString());
    }

    [Fact]
    public void Constructor_String_CreatesSimpleAnsiString() {
        var str = "Hello";
        var ansi = new AnsiString(str);
        Assert.Equal(str.Length, ansi.Length);
        Assert.Equal(str, ansi.Plain);
        Assert.Equal(str, ansi.ToString());
    }

    [Fact]
    public void Constructor_StringAndStyle_AppliesStyleAtStart() {
        var str = "Hello";
        var style = new NKStyle(NKConsoleColor.RED);
        var ansi = new AnsiString(str, style);

        Assert.Equal(str, ansi.Plain);
        // ToString should contain escape codes.
        var result = ansi.ToString();
        Assert.StartsWith("\e[", result);
        Assert.Contains("Hello", result);
    }

    [Fact]
    public void Style_AppliesStyleToWholeString() {
        var ansi = new AnsiString("Test");
        var style = new NKStyle(NKConsoleColor.BLUE);
        var styled = ansi.ApplyStyle(style);

        // Original should be unchanged (immutability check)
        Assert.NotEqual(ansi, styled);
        Assert.Equal("Test", styled.Plain);

        // Verify style is in the output
        Assert.Contains("\e[", styled.ToString());
    }

    [Fact]
    public void Style_AtIndex_AppliesStyleFromIndex() {
        var ansi = new AnsiString("Hello World");
        var style = new NKStyle(NKConsoleColor.GREEN);
        var styled = ansi.ApplyStyle(style, 6); // "World"

        var str = styled.ToString();
        // "Hello " should be plain, "World" should be styled.
        // Index 6 is 'W'.

        // The implementation sorts markers.
        // At index 0 (default), style is Default.
        // At index 6, style becomes Green.

        // We expect something like: Hello \x1b[...mWorld
        Assert.Contains("Hello ", str);
        Assert.Contains("World", str);
        Assert.True(str.IndexOf("\e[", StringComparison.Ordinal) > 0, "Escape sequence should appear after 'Hello '");
    }

    [Fact]
    public void Style_Range_AppliesStyleInRange() {
        var text = "0123456789";
        var ansi = new AnsiString(text);
        var style = new NKStyle(NKConsoleColor.RED);

        // Apply to 4..7 (4,5,6) -> "456"
        var styled = ansi.ApplyStyle(style, 4..7);

        // Expected: 0123 <red> 456 <reset/default> 789

        var str = styled.ToString();
        Assert.Contains("0123", str);
        Assert.Contains("456", str);
        Assert.Contains("789", str);

        // Check for multiple escape sequences (start of red, end of red)
        Assert.True(str.Split("\e[").Length >= 3);
    }

    [Fact]
    public void Equality_ChecksValueAndStyles() {
        var s1 = new AnsiString("Test", new NKStyle(NKConsoleColor.RED));
        var s2 = new AnsiString("Test", new NKStyle(NKConsoleColor.RED));
        var s3 = new AnsiString("Other", new NKStyle(NKConsoleColor.RED));
        var s4 = new AnsiString("Other", new NKStyle(NKConsoleColor.RED));

        Assert.Equal(s1, s2);
        Assert.NotEqual(s1, s3);
        Assert.NotEqual(s1, s4);
        Assert.True(s1 == s2);
        Assert.True(s1 != s3);
    }

    [Fact]
    public void Chop_SplitsStringCorrectly() {
        var text = "Hello world this is a test";
        var ansi = new AnsiString(text);

        var lines = ansi.Chop(11);

        Assert.NotEmpty(lines);
        foreach (var line in lines) {
            Assert.True(line.Length <= 11);
        }
        
        Assert.Equal("Hello world", lines[0].Plain);
        Assert.Equal("this is a", lines[1].Plain);
        Assert.Equal("test", lines[2].Plain);
    }

    [Fact]
    public void Chop_NoSpaces_SplitsAtWidth() {
        var text = "1234567890";
        var ansi = new AnsiString(text);
        var lines = ansi.Chop(3);

        Assert.Equal(4, lines.Length);
        Assert.Equal("123", lines[0].Plain);
        Assert.Equal("456", lines[1].Plain);
        Assert.Equal("789", lines[2].Plain);
        Assert.Equal("0", lines[3].Plain);
    }

    [Fact]
    public void Chop_ExactlyAtWidth_Works() {
        var text = "123 456";
        var ansi = new AnsiString(text);
        var lines = ansi.Chop(3);
        
        // i=0 '1' w=1
        // i=1 '2' w=2
        // i=2 '3' w=3
        // i=3 ' ' w=3. lastSpace = 3. 
        // w is NOT < 3. 
        // lastSpace.HasValue (3) -> lines.Add(chars[0..3]) ("123"), i = 4, lastBreak = 4, lastSpace = null.
        // i=4 '4' w=1
        // i=5 '5' w=2
        // i=6 '6' w=3
        // loop ends.
        // lastBreak != 7 -> lines.Add(chars[4..7]) ("456").
        
        Assert.Equal(2, lines.Length);
        Assert.Equal("123", lines[0].Plain);
        Assert.Equal("456", lines[1].Plain);
    }
    
    [Fact]
    public void Chop_PreservesStyles() {
        var style = new NKStyle(NKConsoleColor.RED);
        var ansi = new AnsiString("Hello World", style);
        
        var lines = ansi.Chop(5);
        
        Assert.Equal(2, lines.Length);
        Assert.Contains("\e[", lines[0].ToString());
        Assert.Contains("\e[", lines[1].ToString());
    }

    [Fact]
    public void Chop_HandlesNewlines() {
        const string text = "Line1\nLine2";
        var ansi = new AnsiString(text);
        var lines = ansi.Chop(50);

        Assert.Equal(2, lines.Length);
        Assert.Equal("Line1", lines[0].Plain);
        Assert.Equal("Line2", lines[1].Plain);
    }

    [Fact]
    public void Enumerator_YieldsAnsiChars() {
        var text = "AB";
        var style = new NKStyle(NKConsoleColor.RED);
        var ansi = new AnsiString(text, style);

        var list = new List<AnsiChar>();
        foreach (var c in ansi) {
            list.Add(c);
        }

        Assert.Equal(2, list.Count);
        Assert.Equal('A', list[0].Char);
        Assert.Equal(style, list[0].Style);
        Assert.Equal('B', list[1].Char);
        Assert.Equal(style, list[1].Style);
    }

    [Fact]
    public void Substring_PreservesStyleCorrectly() {
        var style1 = new NKStyle(NKConsoleColor.RED);
        var style2 = new NKStyle(NKConsoleColor.BLUE);
        var ansi = new AnsiString("AAA", style1) + new AnsiString("BBB", style2);

        // AAA (red) BBB (blue)
        // Substring(2, 2) -> "AB" (A is red, B is blue)
        var sub = ansi.Substring(2, 2);

        Assert.Equal("AB", sub.Plain);
        Assert.Equal(style1, sub.GetStyleAt(0));
        Assert.Equal(style2, sub.GetStyleAt(1));
    }

    [Fact]
    public void Replace_Char_MaintainsStyles() {
        var style = new NKStyle(NKConsoleColor.RED);
        var ansi = new AnsiString("apple", style);
        var replaced = ansi.Replace('p', 'b');

        Assert.Equal("abble", replaced.Plain);
        foreach(var c in replaced) {
            Assert.Equal(style, c.Style);
        }
    }

    [Fact]
    public void Replace_String_AppliesStyleOfFirstMatch() {
        var style1 = new NKStyle(NKConsoleColor.RED);
        var style2 = new NKStyle(NKConsoleColor.BLUE);
        var ansi = new AnsiString("A", style1) + new AnsiString("X", style2) + new AnsiString("B", style1);

        // "AXB" where A, B are Red and X is Blue.
        // Replace "X" with "YYY"
        var replaced = ansi.Replace("X", "YYY");

        Assert.Equal("AYYYB", replaced.Plain);
        Assert.Equal(style1, replaced.GetStyleAt(0)); // A
        Assert.Equal(style2, replaced.GetStyleAt(1)); // Y
        Assert.Equal(style2, replaced.GetStyleAt(2)); // Y
        Assert.Equal(style2, replaced.GetStyleAt(3)); // Y
        Assert.Equal(style1, replaced.GetStyleAt(4)); // B
    }

    [Fact]
    public void Split_ReturnsStyledParts() {
        var style1 = new NKStyle(NKConsoleColor.RED);
        var style2 = new NKStyle(NKConsoleColor.BLUE);
        var ansi = new AnsiString("Red", style1) + "," + new AnsiString("Blue", style2);

        var parts = ansi.Split(',');

        Assert.Equal(2, parts.Length);
        Assert.Equal("Red", parts[0].Plain);
        Assert.Equal(style1, parts[0].GetStyleAt(0));
        Assert.Equal("Blue", parts[1].Plain);
        Assert.Equal(style2, parts[1].GetStyleAt(0));
    }

    [Fact]
    public void Join_CombinesStyledStrings() {
        var s1 = new AnsiString("A", new NKStyle(NKConsoleColor.RED));
        var s2 = new AnsiString("B", new NKStyle(NKConsoleColor.BLUE));

        var joined = AnsiString.Join("-", [s1, s2]);

        Assert.Equal("A-B", joined.Plain);
        Assert.Equal(NKConsoleColor.RED, joined.GetStyleAt(0).FColor.AsPalette);
        Assert.Equal(NKStyle.Default, joined.GetStyleAt(1)); // separator '-'
        Assert.Equal(NKConsoleColor.BLUE, joined.GetStyleAt(2).FColor.AsPalette);
    }

    [Fact]
    public void Insert_MaintainsSurroundingStyles() {
        var style1 = new NKStyle(NKConsoleColor.RED);
        var style2 = new NKStyle(NKConsoleColor.BLUE);
        var ansi = new AnsiString("AA", style1) + new AnsiString("BB", style2);

        // Insert at index 2 (between AA and BB)
        var inserted = ansi.Insert(2, "X");

        Assert.Equal("AAXBB", inserted.Plain);
        Assert.Equal(style1, inserted.GetStyleAt(0));
        Assert.Equal(style1, inserted.GetStyleAt(1));
        Assert.Equal(NKStyle.Default, inserted.GetStyleAt(2)); // "X" is unstyled
        Assert.Equal(style2, inserted.GetStyleAt(3));
    }

    [Fact]
    public void Padding_InheritsCorrectStyles() {
        var style = new NKStyle(NKConsoleColor.RED);
        var ansi = new AnsiString("Hi", style);

        var paddedLeft = ansi.PadLeft(4, '.');
        Assert.Equal("..Hi", paddedLeft.Plain);
        Assert.Equal(style, paddedLeft.GetStyleAt(0)); // Inherits from first char style

        var paddedRight = ansi.PadRight(4, '.');
        Assert.Equal("Hi..", paddedRight.Plain);
        Assert.Equal(style, paddedRight.GetStyleAt(3)); // Inherits from last char style
    }

    [Fact]
    public void Trim_RemovesWhitespaceButKeepsInternalStyles() {
        var style = new NKStyle(NKConsoleColor.GREEN);
        var ansi = new AnsiString("  ") + new AnsiString("Styled", style) + "  ";

        var trimmed = ansi.Trim();

        Assert.Equal("Styled", trimmed.Plain);
        Assert.Equal(style, trimmed.GetStyleAt(0));
    }

    [Fact]
    public void EmptyString_Operations_DoNotThrow() {
        var empty = new AnsiString();

        Assert.Equal(0, empty.Length);
        Assert.Equal(string.Empty, empty.ToUpper().Plain);
        Assert.Equal(string.Empty, empty.Replace("a", "b").Plain);
        Assert.Single(empty.Split(','));
    }

    [Fact]
    public void AddStyle_ComposesStyles() {
        var redStyle = new NKStyle(NKConsoleColor.RED);
        var boldStyle = new NKStyle(s: TextStyles.BOLD);

        var ansi = new AnsiString("Test", redStyle);
        var composed = ansi.AddStyle(boldStyle);

        var finalStyle = composed.GetStyleAt(0);
        Assert.Equal(NKConsoleColor.RED, finalStyle.FColor.AsPalette);
        Assert.True(finalStyle.Styles.HasFlag(TextStyles.BOLD));
    }

    [Fact]
    public void AddStyle_Range_ComposesOnlyInRange() {
        var redStyle = new NKStyle(NKConsoleColor.RED);
        var boldStyle = new NKStyle(s: TextStyles.BOLD);

        var ansi = new AnsiString("Hello World", redStyle);
        var composed = ansi.AddStyle(boldStyle, 6..11); // "World"

        // "Hello " should be just Red
        Assert.Equal(NKConsoleColor.RED, composed.GetStyleAt(0).FColor.AsPalette);
        Assert.False(composed.GetStyleAt(0).Styles.HasFlag(TextStyles.BOLD));

        // "World" should be Red + Bold
        Assert.Equal(NKConsoleColor.RED, composed.GetStyleAt(6).FColor.AsPalette);
        Assert.True(composed.GetStyleAt(6).Styles.HasFlag(TextStyles.BOLD));
    }

    [Fact]
    public void ApplyStyle_OverwritesExistingStyles() {
        var redBold = new NKStyle(NKConsoleColor.RED, s: TextStyles.BOLD);
        var blueOnly = new NKStyle(NKConsoleColor.BLUE);

        var ansi = new AnsiString("Test", redBold);
        var overwritten = ansi.ApplyStyle(blueOnly);
        var finalStyle = overwritten.GetStyleAt(0);
        Assert.Equal(NKConsoleColor.BLUE, finalStyle.FColor.AsPalette);
        Assert.False(finalStyle.Styles.HasFlag(TextStyles.BOLD), "ApplyStyle should have removed Bold");
    }

    [Fact]
    public void Parse_PlainString_ReturnsPlainAnsiString() {
        var parsed = AnsiString.Parse("Hello World");
        Assert.Equal("Hello World", parsed.Plain);
        Assert.Equal(NKStyle.Default, parsed.GetStyleAt(0));
    }

    [Fact]
    public void Parse_WithStyleFlags_AppliesStyleCorrectly() {
        var parsed = AnsiString.Parse("Hello {:b}World{:i}!");
        Assert.Equal("Hello World!", parsed.Plain);
        Assert.Equal(NKStyle.Default, parsed.GetStyleAt(0)); // "Hello "
        Assert.True(parsed.GetStyleAt(6).Styles.HasFlag(TextStyles.BOLD)); // "World"
        Assert.True(parsed.GetStyleAt(11).Styles.HasFlag(TextStyles.ITALIC)); // "!"
    }

    [Fact]
    public void Parse_WithColors_AppliesForegroundAndBackground() {
        var parsed = AnsiString.Parse("{:f#red}Red{:b#dark-cyan} CyanBg");
        Assert.Equal("Red CyanBg", parsed.Plain);
        Assert.Equal(NKConsoleColor.RED, parsed.GetStyleAt(0).FColor.AsPalette);
        Assert.Equal(NKConsoleColor.DARK_CYAN, parsed.GetStyleAt(3).BColor.AsPalette);
        // Foreground red should persist after background change
        Assert.Equal(NKConsoleColor.RED, parsed.GetStyleAt(3).FColor.AsPalette);
    }

    [Fact]
    public void Parse_WithHexRgbColor_AppliesRgbColor() {
        var parsed = AnsiString.Parse("{:f#012345}Hex");
        Assert.Equal("Hex", parsed.Plain);
        Assert.True(parsed.GetStyleAt(0).FColor.IsRgb);
        Assert.Equal(0x012345u, parsed.GetStyleAt(0).FColor.AsRgb);
    }

    [Fact]
    public void Parse_EscapedCurlyBraces_ReturnsLiteralBraces() {
        var parsed = AnsiString.Parse("{{Hello {:b}World}}");
        Assert.Equal("{Hello World}", parsed.Plain);
        Assert.Equal(NKStyle.Default, parsed.GetStyleAt(0)); // "{"
        Assert.True(parsed.GetStyleAt(7).Styles.HasFlag(TextStyles.BOLD)); // "World}"
    }

    [Fact]
    public void TryParse_InvalidFormat_ReturnsFalse() {
        Assert.False(AnsiString.TryParse("Hello {invalid} World", out _));
        Assert.False(AnsiString.TryParse("Hello {:b", out _));
        Assert.False(AnsiString.TryParse("Hello } World", out _));
        Assert.False(AnsiString.TryParse("Hello {:f#invalid} World", out _));
    }

    [Fact]
    public void Parse_WithNegatedStyleFlags_NegatesActiveStyles() {
        var parsed = AnsiString.Parse("Hello {:b}World{:!b}!");
        Assert.Equal("Hello World!", parsed.Plain);
        Assert.Equal(NKStyle.Default, parsed.GetStyleAt(0)); // "Hello "
        Assert.True(parsed.GetStyleAt(6).Styles.HasFlag(TextStyles.BOLD)); // "World"
        Assert.False(parsed.GetStyleAt(11).Styles.HasFlag(TextStyles.BOLD)); // "!"
    }

    [Fact]
    public void Parse_WithCombinedStyleFlagsAndNegation_NegatesOnlySpecified() {
        // First set BOLD and ITALIC, then negate only BOLD
        var parsed = AnsiString.Parse("Hello {:bi}World{:!b}!");
        Assert.Equal("Hello World!", parsed.Plain);
        Assert.Equal(NKStyle.Default, parsed.GetStyleAt(0));
        
        var worldStyle = parsed.GetStyleAt(6);
        Assert.True(worldStyle.Styles.HasFlag(TextStyles.BOLD));
        Assert.True(worldStyle.Styles.HasFlag(TextStyles.ITALIC));

        var exclamStyle = parsed.GetStyleAt(11);
        Assert.False(exclamStyle.Styles.HasFlag(TextStyles.BOLD));
        Assert.True(exclamStyle.Styles.HasFlag(TextStyles.ITALIC));
    }

    [Fact]
    public void TryParse_InvalidNegationFormat_ReturnsFalse() {
        Assert.False(AnsiString.TryParse("Hello {:!} World", out _));
        Assert.False(AnsiString.TryParse("Hello {:!f#red} World", out _));
        Assert.False(AnsiString.TryParse("Hello {:!b#blue} World", out _));
    }
}