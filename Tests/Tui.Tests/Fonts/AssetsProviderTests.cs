// NeoKolors.Test
// Copyright (c) krystof 2026

using NeoKolors.Tui.Fonts.Assets;

namespace NeoKolors.Tui.Tests;

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
}
