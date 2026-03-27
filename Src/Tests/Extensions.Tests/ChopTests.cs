//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Extensions.Tests;

public class ChopTests {
    [Fact]
    public void Chop_SimpleString_ReturnsCorrectChunks() {
        string input = "abcdefghij";
        string[] result = input.Chop(5);
        Assert.Equal(["abcde", "fghij"], result);
    }

    [Fact]
    public void Chop_WithSpaces_WrapsAtSpace() {
        string input = "hello world";
        string[] result = input.Chop(7);
        Assert.Equal(["hello", "world"], result);
    }

    [Fact]
    public void Chop_LongWord_ForceChops() {
        string input = "supercalifragilistic";
        string[] result = input.Chop(5);
        Assert.Equal(["super", "calif", "ragil", "istic"], result);
    }

    [Fact]
    public void Chop_EmptyString_ReturnsEmptyArray() {
        string input = "";
        string[] result = input.Chop(5);
        Assert.Empty(result);
    }

    [Fact]
    public void Chop_NullString_ReturnsEmptyArray() {
        string? input = null;
        string[] result = input!.Chop(5);
        Assert.Empty(result);
    }

    [Fact]
    public void Chop_WithNewlines_RespectsNewlines() {
        string input = "line1\nline2";
        string[] result = input.Chop(10);
        Assert.Equal(["line1", "line2"], result);
    }

    [Fact]
    public void Chop_WithMultipleNewlines_RespectsNewlines() {
        string input = "a\n\nb";
        string[] result = input.Chop(10);
        Assert.Equal(["a", "", "b"], result);
    }

    [Fact]
    public void Chop_TrailingSpace_DiscardedAtBreak() {
        string input = "abc ";
        string[] result = input.Chop(3);
        Assert.Equal(["abc"], result);
    }

    [Fact]
    public void Chop_MultipleSpaces_PreservedExceptBreak() {
        string input = "abc  def";
        string[] result = input.Chop(4);
        // "abc " fits in 4 chars. The second space at index 4 makes it 5 chars, so it breaks there.
        Assert.Equal(["abc ", "def"], result);
    }
}