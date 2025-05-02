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
}