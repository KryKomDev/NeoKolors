namespace NeoKolors.Extensions.Tests;

using System.Text;
using NeoKolors.Extensions;

public class MiscExtensionsTests {

    #region Char ToDisplay Tests

    [Fact]
    public void Char_ToDisplay_ReturnsDescriptiveNameForUnprintable() {
        Assert.Equal("NUL", char.ToDisplay('\0'));
        Assert.Equal("ESC", char.ToDisplay('\u001b'));
        Assert.Equal("SPC", char.ToDisplay(' '));
        Assert.Equal("DEL", char.ToDisplay('\xff'));
    }

    [Fact]
    public void Char_ToDisplay_ReturnsCharForPrintable() {
        Assert.Equal("A", char.ToDisplay('A'));
        Assert.Equal("!", char.ToDisplay('!'));
    }

    #endregion

    #region String Substring and Trim Tests

    [Fact]
    public void SafeSubstring_HandlesBoundaries() {
        string s = "Hello";
        Assert.Equal("He", s.SafeSubstring(0, 2));
        Assert.Equal("llo", s.SafeSubstring(2, 10));
        Assert.Equal("", s.SafeSubstring(10, 2));
        Assert.Equal("Hello", s.SafeSubstring(-1, 10));
    }

    [Fact]
    public void SubstringUntil_ReturnsCorrectPortion() {
        string s = "user@domain.com";
        Assert.Equal("user", s.SubstringUntil('@'));
        Assert.Equal("user@domain.com", s.SubstringUntil(':'));
    }

    [Fact]
    public void SubstringAfter_ReturnsCorrectPortion() {
        string s = "user@domain.com";
        Assert.Equal("@domain.com", s.SubstringAfter('@'));
        Assert.Equal("user@domain.com", s.SubstringAfter(':'));
    }

    [Fact]
    public void SubstringBetween_ReturnsCorrectPortion() {
        string s = "prefix[content]suffix";
        Assert.Equal("[content]", s.SubstringBetween('[', ']'));
        Assert.Equal("content", s.SubstringBetween('[', ']', true, true));
        Assert.Equal("prefix[content]suffix", s.SubstringBetween('{', '}')); // Returns original if not found
    }

    [Fact]
    public void Trim_ShortensStringCorrectly() {
        string s = "Hello World";
        Assert.Equal("Hello", s.Trim(5));
        Assert.Equal("Hello World", s.Trim(20));
        Assert.Throws<ArgumentOutOfRangeException>(() => s.Trim(-1));
    }

    [Fact]
    public void Shrink_ShortensFromMiddle() {
        string s = "1234567890";
        // length 5: hl = 2. s[..3] + "…" + s[^2..] => "123" + "…" + "90" = "123…90"
        Assert.Equal("123…90", s.Shrink(5));
        Assert.Equal("1234567890", s.Shrink(10));
    }

    #endregion

    #region String Content Analysis Tests

    [Fact]
    public void ContentAnalysis_ReturnsCorrectResults() {
        Assert.True("abc!123".ContainsSpecial());
        Assert.False("abc123".ContainsSpecial());

        Assert.True("abc123".ContainsNumber());
        Assert.False("abc".ContainsNumber());

        Assert.True("abc123".ContainsLetter());
        Assert.False("123!!!".ContainsLetter());

        Assert.True("abcA".ContainsUpper());
        Assert.False("abca".ContainsUpper());

        Assert.True("abcA".ContainsLower());
        Assert.False("ABCA".ContainsLower());

        Assert.True("_var1".IsValidIdentifier());
        Assert.True("@var".IsValidIdentifier());
        Assert.False("1var".IsValidIdentifier());
        Assert.False("var-1".IsValidIdentifier());

        Assert.True("Word".IsWord());
        Assert.False("Word1".IsWord());

        Assert.True("Capital".IsCapitalWord());
        Assert.False("capital".IsCapitalWord());
        Assert.False("CAPITAL".IsCapitalWord());
    }

    #endregion

    #region Line Padding Tests

    [Fact]
    public void PadLinesLeft_PadsCorrectly() {
        string s = "line1\nline2\nline3";
        string padded = s.PadLinesLeft(2, false);
        Assert.Equal("line1\n  line2\n  line3", padded);

        string paddedFirst = s.PadLinesLeft(2, true);
        Assert.Equal("  line1\n  line2\n  line3", paddedFirst);
    }

    #endregion

    #region StringBuilder Extensions

    [Fact]
    public void StringBuilder_AppendIf_Works() {
        var sb = new StringBuilder();
        sb.AppendIf(true, "yes");
        sb.AppendIf(false, "no");
        Assert.Equal("yes", sb.ToString());
    }

    #endregion
}
