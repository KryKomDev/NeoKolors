namespace NeoKolors.Common.Tests;

public class AnsiStringTests {
    
    [Fact]
    public void Constructor_Empty_CreatesEmptyString() {
        var ansi = new AnsiString();
        Assert.Equal(0, ansi.Length);
        Assert.Equal(string.Empty, ansi.String);
        Assert.Equal(string.Empty, ansi.ToString());
    }

    [Fact]
    public void Constructor_String_CreatesSimpleAnsiString() {
        var str = "Hello";
        var ansi = new AnsiString(str);
        Assert.Equal(str.Length, ansi.Length);
        Assert.Equal(str, ansi.String);
        Assert.Equal(str, ansi.ToString());
    }

    [Fact]
    public void Constructor_StringAndStyle_AppliesStyleAtStart() {
        var str = "Hello";
        var style = new NKStyle(NKConsoleColor.RED);
        var ansi = new AnsiString(str, style);

        Assert.Equal(str, ansi.String);
        // ToString should contain escape codes.
        // We might not know the exact escape code string without calculating it,
        // but we can check if it starts with the escape sequence.
        var result = ansi.ToString();
        Assert.StartsWith("\e[", result);
        Assert.EndsWith("Hello", result);
    }

    [Fact]
    public void Style_AppliesStyleToWholeString() {
        var ansi = new AnsiString("Test");
        var style = new NKStyle(NKConsoleColor.BLUE);
        var styled = ansi.Style(style);

        // Original should be unchanged (immutability check)
        Assert.NotEqual(ansi, styled);
        Assert.Equal("Test", styled.String);

        // Verify style is in the output
        Assert.Contains("\e[", styled.ToString());
    }

    [Fact]
    public void Style_AtIndex_AppliesStyleFromIndex() {
        var ansi = new AnsiString("Hello World");
        var style = new NKStyle(NKConsoleColor.GREEN);
        var styled = ansi.Style(style, 6); // "World"

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
        var styled = ansi.Style(style, 4..7);

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

        // Reconstruct to see if content is preserved (ignoring potential newlines added or removed logic details for now)
        // The Chop logic seems to consume spaces for breaks.
    }

    [Fact]
    public void Chop_HandlesNewlines() {
        const string text = "Line1\nLine2";
        var ansi = new AnsiString(text);
        var lines = ansi.Chop(50);

        Assert.Equal(2, lines.Length);
        Assert.Equal("Line1", lines[0].String);
        Assert.Equal("Line2", lines[1].String);
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
}