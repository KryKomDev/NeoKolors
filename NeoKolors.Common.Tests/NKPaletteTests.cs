using System.Drawing;
using Xunit;

namespace NeoKolors.Common.Tests;

public class NKPaletteTests {

    [Fact]
    public void Constructor_WithColors_SetsCorrectValues() {
        // Arrange
        var colors = new NKColor[] {
            NKConsoleColor.BLACK,
            NKConsoleColor.WHITE,
            NKConsoleColor.RED,
            NKConsoleColor.GREEN,
            NKConsoleColor.BLUE
        };

        // Act
        var palette = new NKPalette(colors);

        // Assert
        Assert.Equal(5, palette.Length);
        Assert.Equal(colors, palette.Colors);
        Assert.Equal((NKColor)NKConsoleColor.BLACK, palette.Base);
        Assert.Equal((NKColor)NKConsoleColor.WHITE, palette.Background);
        Assert.Equal((NKColor)NKConsoleColor.RED, palette.Text);
        Assert.Equal((NKColor)NKConsoleColor.GREEN, palette.TextSecondary);
        Assert.Equal((NKColor)NKConsoleColor.BLUE, palette.Accent);
    }

    [Fact]
    public void Constructor_WithTooFewColors_ThrowsArgumentException() {
        // Arrange
        var colors = new NKColor[] { NKConsoleColor.BLACK };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new NKPalette(colors));
    }

    [Fact]
    public void Constructor_WithString_ParsesCorrectly() {
        // Arrange
        var url = "ff0000-00ff00-0000ff-ffffff-000000";

        // Act
        var palette = new NKPalette(url);

        // Assert
        Assert.Equal(5, palette.Length);
        Assert.Equal(0xff0000u, palette[0]);
        Assert.Equal(0x00ff00u, palette[1]);
        Assert.Equal(0x0000ffu, palette[2]);
    }

    [Fact]
    public void ToUrl_ReturnsCorrectFormat() {
        // Arrange
        var palette = new NKPalette("ff0000-00ff00-0000ff-ffffff-000000");

        // Act
        var url = palette.ToUrl();

        // Assert
        Assert.Equal("ff0000-00ff00-0000ff-ffffff-000000", url);
    }

    [Fact]
    public void GeneratePalette_CreatesPaletteWithCorrectLength() {
        // Act
        var palette = NKPalette.GeneratePalette(123, 7);

        // Assert
        Assert.Equal(7, palette.Length);
    }

    [Theory]
    [InlineData("p", true)]
    [InlineData("u", false)]
    public void ToString_WithFormat_ReturnsExpected(string format, bool containsBullet) {
        // Arrange
        var palette = new NKPalette("ff0000-00ff00-0000ff-ffffff-000000");

        // Act
        var result = palette.ToString(format, null);

        // Assert
        if (containsBullet)
            Assert.Contains("●", result);
        else
            Assert.Contains("-", result);
    }
}
