namespace NeoKolors.Extensions.Tests;

public class SpanExtensionsTests {

    [Fact]
    public void Concat_ReadOnlySpan_JoinsCorrectly() {
        var first  = "Hello".AsSpan();
        var second = " World".AsSpan();

        var result = first.Concat(second);

        Assert.Equal("Hello World", result.ToString());
    }

    [Fact]
    public void Concat_String_JoinsCorrectly() {
        var first  = "Hello";
        var second = " World".AsSpan();

        var result = first.Concat(second);

        Assert.Equal("Hello World", result.ToString());
    }
}