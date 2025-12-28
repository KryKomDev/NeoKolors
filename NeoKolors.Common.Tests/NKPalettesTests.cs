using Xunit;

namespace NeoKolors.Common.Tests;

public class NKPalettesTests {

    [Fact]
    public void GetAllPalettes_ReturnsNonEmptyDictionary() {
        // Act
        NKPalettes.GetAllPalettes(out var palettes, out var maxNameLength);

        // Assert
        Assert.NotEmpty(palettes);
        Assert.True(maxNameLength > 0);
        Assert.Contains("VibrantRainbow", palettes.Keys);
    }

    [Fact]
    public void PredefinedPalettes_AreValid() {
        // Verify a few predefined ones
        Assert.Equal(7, NKPalettes.VermilionGreen.Length);
        Assert.Equal(5, NKPalettes.VibrantRainbow.Length);
        Assert.Equal(8, NKPalettes.GreenExtended.Length);
    }
}
