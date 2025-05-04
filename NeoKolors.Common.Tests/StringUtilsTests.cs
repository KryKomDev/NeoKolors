//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Collections;
using NeoKolors.Common.Util;

namespace NeoKolors.Common.Tests;

public class StringUtilsTests {
    
    [Fact]
    public void VisibleLength_ShouldReturnCorrectValue() {
        var s = "\e[38;2;1;1;1mHello\e[39m World\b";
        Assert.Equal(12, s.VisibleLength());
    }

    [Fact]
    public void Capitalize_ShouldReturnCorrectValue() {
        var s = "hello, world!";
        Assert.Equal("Hello, World!", s.Capitalize());
        s = "";
        Assert.Equal("", s.Capitalize());
        s = null;
        Assert.Throws<ArgumentNullException>(() => s!.Capitalize());
    }

    [Fact]
    public void CapitalizeFirst_ShouldReturnCorrectValue() {
        var s = "hello, world!";
        Assert.Equal("Hello, world!", s.CapitalizeFirst());
        s = "";
        Assert.Equal("", s.CapitalizeFirst());
        s = null;
        Assert.Throws<ArgumentNullException>(() => s!.CapitalizeFirst());
    }

    [Fact]
    public void Format_ShouldReturnCorrectValue() {
        var s = "hello, {0}".Format("world");
        Assert.Equal("hello, world", s.Format());
    }
    
    [Fact]
    public void Join_WithSimpleCollection_JoinsCorrectly() {
        var numbers = new[] { 1, 2, 3, 4, 5 };
        string result = numbers.Join(", ");
        Assert.Equal("1, 2, 3, 4, 5", result);
    }

    [Fact]
    public void Join_WithEmptyCollection_ReturnsEmptyString() {
        var empty = Array.Empty<int>();
        string result = empty.Join(", ");
        Assert.Equal("", result);
    }

    [Fact]
    public void Join_WithNullObjects_IncludesEmptyStrings() {
        var items = new object?[] { "first", null, "third" };
        string result = items.Join("|");
        Assert.Equal("first||third", result);
    }

    [Fact]
    public void Join_WithCustomObjects_UsesToString() {
        var points = new[] {
            new Point(1, 2),
            new Point(3, 4)
        };
        
        string result = points.Join("; ");
        Assert.Equal("(1,2); (3,4)", result);
    }

    [Fact]
    public void Join_WithEmptySeparator_ConcatenatesStrings() {
        var letters = new[] { "a", "b", "c" };
        string result = letters.Join("");
        Assert.Equal("abc", result);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("-")]
    [InlineData(" -> ")]
    public void Join_WithDifferentSeparators_JoinsCorrectly(string separator) {
        var words = new[] { "one", "two", "three" };
        string result = words.Join(separator);
        Assert.Equal($"one{separator}two{separator}three", result);
    }

    [Fact]
    public void Join_WithCustomStringifier_ProcessesElementsCorrectly() {
        var numbers = new[] { 1, 2, 3 };
        string result = numbers.Join(", ", n => $"Number{n}");
        Assert.Equal("Number1, Number2, Number3", result);
    }

    [Fact]
    public void Join_WithNullStringifier_HandlesNullsCorrectly() {
        var items = new[] { "a", "b", null, "d" };
        string result = items.Join("|", s => s);
        Assert.Equal("a|b||d", result);
    }

    [Fact]
    public void Join_WithCustomStringifierReturningNull_HandlesNullsCorrectly() {
        var numbers = new[] { 1, 2, 3 };
        string result = numbers.Join(",", n => n % 2 == 0 ? null : n.ToString());
        Assert.Equal("1,,3", result);
    }

    [Fact]
    public void Join_WithComplexObjects_UsesCustomStringifier() {
        var points = new[] {
            new Point(1, 2),
            new Point(3, 4),
            null
        };
        
        string result = points.Join("; ", p => p is null ? "null" : $"Point({p.X},{p.Y})");
        Assert.Equal("Point(1,2); Point(3,4); null", result);
    }

    [Fact]
    public void Join_WithMixedTypes_JoinsCorrectly() {
        var items = new ArrayList { 1, "two", 3.0, true };
        string result = items.Join(", ");
        Assert.Equal("1, two, 3, True", result);
    }

    private record Point(int X, int Y) {
        public override string ToString() => $"({X},{Y})";
    }

    [Theory]
    [InlineData(1, "I")]
    [InlineData(4, "IV")]
    [InlineData(9, "IX")]
    [InlineData(49, "XLIX")]
    [InlineData(99, "XCIX")]
    [InlineData(499, "CDXCIX")]
    [InlineData(999, "CMXCIX")]
    [InlineData(3999, "MMMCMXCIX")]
    [InlineData(2024, "MMXXIV")]
    public void ToRoman_BasicConversions_ReturnsCorrectRomanNumerals(int number, string expected) {
        Assert.Equal(expected, number.ToRoman());
    }

    [Theory]
    [InlineData(1, "i")]
    [InlineData(4, "iv")]
    [InlineData(9, "ix")]
    [InlineData(49, "xlix")]
    [InlineData(99, "xcix")]
    [InlineData(499, "cdxcix")]
    [InlineData(999, "cmxcix")]
    [InlineData(3999, "mmmcmxcix")]
    [InlineData(2024, "mmxxiv")]
    public void ToRoman_LowercaseConversions_ReturnsCorrectLowercaseRomanNumerals(int number, string expected) {
        Assert.Equal(expected, number.ToRoman(lowercase: true));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(4000)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void ToRoman_InvalidValues_ThrowsArgumentOutOfRangeException(int number) {
        Assert.Throws<ArgumentOutOfRangeException>(() => number.ToRoman());
    }

    [Fact]
    public void ToRoman_AllSingleDigits_ReturnsCorrectRomanNumerals() {
        var expectedValues = new Dictionary<int, string> {
            { 1, "I" },
            { 2, "II" },
            { 3, "III" },
            { 4, "IV" },
            { 5, "V" },
            { 6, "VI" },
            { 7, "VII" },
            { 8, "VIII" },
            { 9, "IX" }
        };

        foreach (var pair in expectedValues) {
            Assert.Equal(pair.Value, pair.Key.ToRoman());
        }
    }

    [Theory]
    [InlineData(10, "X")]
    [InlineData(20, "XX")]
    [InlineData(30, "XXX")]
    [InlineData(40, "XL")]
    [InlineData(50, "L")]
    [InlineData(60, "LX")]
    [InlineData(70, "LXX")]
    [InlineData(80, "LXXX")]
    [InlineData(90, "XC")]
    public void ToRoman_TensValues_ReturnsCorrectRomanNumerals(int number, string expected) {
        Assert.Equal(expected, number.ToRoman());
    }

    [Theory]
    [InlineData(100, "C")]
    [InlineData(200, "CC")]
    [InlineData(300, "CCC")]
    [InlineData(400, "CD")]
    [InlineData(500, "D")]
    [InlineData(600, "DC")]
    [InlineData(700, "DCC")]
    [InlineData(800, "DCCC")]
    [InlineData(900, "CM")]
    public void ToRoman_HundredsValues_ReturnsCorrectRomanNumerals(int number, string expected) {
        Assert.Equal(expected, number.ToRoman());
    }

    [Theory]
    [InlineData(1000, "M")]
    [InlineData(2000, "MM")]
    [InlineData(3000, "MMM")]
    public void ToRoman_ThousandsValues_ReturnsCorrectRomanNumerals(int number, string expected) {
        Assert.Equal(expected, number.ToRoman());
    }
}