namespace NeoKolors.Extensions.Tests;

using NeoKolors.Extensions;

public class SpanExtensionsTests {

    [Fact]
    public void Concat_ReadOnlySpan_JoinsCorrectly() {
        ReadOnlySpan<char> first = "Hello".AsSpan();
        ReadOnlySpan<char> second = " World".AsSpan();

        var result = first.Concat(second);

        Assert.Equal("Hello World", result.ToString());
    }

    [Fact]
    public void Concat_String_JoinsCorrectly() {
        string first = "Hello";
        ReadOnlySpan<char> second = " World".AsSpan();

        var result = first.Concat(second);

        Assert.Equal("Hello World", result.ToString());
    }
}
