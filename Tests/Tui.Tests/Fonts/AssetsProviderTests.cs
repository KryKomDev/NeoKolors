// NeoKolors.Test
// Copyright (c) KryKom 2026

using NeoKolors.Tui.Fonts.Assets;

namespace NeoKolors.Tui.Tests.Fonts;

public class AssetsProviderTests {

    [Fact]
    public void AssetsProvider_ShouldProvideBytesizedFont() {
        var font = AssetsProvider.Bytesized;
        Assert.NotNull(font);
        Assert.Equal("Bytesized", font.Info.Name);
    }

    [Fact]
    public void AssetsProvider_ShouldProvideFutureFont() {
        var font = AssetsProvider.Future;
        Assert.NotNull(font);
        Assert.Equal("Future", font.Info.Name);
    }

    [Fact]
    public void AssetsProvider_ShouldProvideDummyFont() {
        var font = AssetsProvider.Dummy;
        Assert.NotNull(font);
        Assert.Equal("Dummy", font.Info.Name);
    }

    [Fact]
    public void AssetsProvider_GetFontStream_ShouldReturnValidStream() {
        using var stream = AssetsProvider.GetFontStream("Future");
        Assert.NotNull(stream);
        Assert.True(stream.Length > 0);
    }

    [Fact]
    public void Bytesized_ShouldRenderSpacesBetweenWordsInMonospacedFont() {
        var font = AssetsProvider.Bytesized;
        Assert.NotNull(font);

        using var canvas = new NKCharCanvas(30, 5);
        font.DrawString("A B", canvas);

        // 'A' starts at x=0 (columns 0..2)
        Assert.Equal('█', canvas.Data[0, 0].Char);

        // In Bytesized, cell advance is 5 (LetterWidth 3 + LetterSpacing 1 + 1)
        // Without space, 'B' would be at x=5.
        // With 1 space, x=5..9 is empty, and 'B' starts at x=10.
        Assert.Equal(' ', canvas.Data[5, 0].Char);
        Assert.Equal('█', canvas.Data[10, 0].Char);

        // Test two spaces: 'B' should start at x=15
        using var canvas2 = new NKCharCanvas(30, 5);
        font.DrawString("A  B", canvas2);
        Assert.Equal('█', canvas2.Data[0, 0].Char);
        Assert.Equal(' ', canvas2.Data[10, 0].Char);
        Assert.Equal('█', canvas2.Data[15, 0].Char);
    }
}
