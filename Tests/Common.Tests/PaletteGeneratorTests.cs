// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Common.Tests;

public class PaletteGeneratorTests {

    [Theory]
    [InlineData(PaletteGenerator.PaletteType.RANDOM)]
    [InlineData(PaletteGenerator.PaletteType.MONOCHROMATIC)]
    [InlineData(PaletteGenerator.PaletteType.SHADES)]
    [InlineData(PaletteGenerator.PaletteType.ANALOGOUS)]
    [InlineData(PaletteGenerator.PaletteType.COMPLEMENTARY)]
    [InlineData(PaletteGenerator.PaletteType.SPLIT_COMPLEMENTARY)]
    [InlineData(PaletteGenerator.PaletteType.COMPOUND)]
    [InlineData(PaletteGenerator.PaletteType.TRIANGLE)]
    [InlineData(PaletteGenerator.PaletteType.SQUARE)]
    [InlineData(PaletteGenerator.PaletteType.SINUSOIDAL)]
    public void Generate_AllPaletteTypes_CreatesValidPalette(PaletteGenerator.PaletteType type) {
        // Arrange
        var options = new PaletteGenerator.PaletteGeneratorOptions(
            type: type,
            count: 6,
            seed: 42,
            baseColor: NKColor.FromRgb(0x34, 0x98, 0xdb),
            hueVariance: 10,
            satFactor: 0.9,
            lightFactor: 0.95
        );

        // Act
        var palette = PaletteGenerator.Generate(options);

        // Assert
        Assert.Equal(6, palette.Length);
        Assert.NotNull(palette.Colors);
        foreach (var color in palette.Colors) {
            Assert.True(color.IsRgb);
        }
    }

    [Fact]
    public void Generate_DefaultOptions_CreatesValidPalette() {
        // Act
        var palette = PaletteGenerator.Generate();

        // Assert
        Assert.Equal(5, palette.Length);
        Assert.NotNull(palette.Colors);
    }

    [Fact]
    public void Generate_WithDeterministicSeed_ProducesSameResult() {
        // Arrange
        var options1 = new PaletteGenerator.PaletteGeneratorOptions(
            type: PaletteGenerator.PaletteType.SINUSOIDAL,
            count: 8,
            seed: 12345,
            baseColor: NKColor.FromRgb(255, 100, 50)
        );

        var options2 = new PaletteGenerator.PaletteGeneratorOptions(
            type: PaletteGenerator.PaletteType.SINUSOIDAL,
            count: 8,
            seed: 12345,
            baseColor: NKColor.FromRgb(255, 100, 50)
        );

        // Act
        var p1 = PaletteGenerator.Generate(options1);
        var p2 = PaletteGenerator.Generate(options2);

        // Assert
        Assert.Equal(p1.Colors, p2.Colors);
    }

    [Fact]
    public void Generate_WithConsoleBaseColor_Succeeds() {
        // Arrange
        var options = new PaletteGenerator.PaletteGeneratorOptions(
            type: PaletteGenerator.PaletteType.TRIANGLE,
            count: 5,
            seed: 99,
            baseColor: NKConsoleColor.RED
        );

        // Act
        var palette = PaletteGenerator.Generate(options);

        // Assert
        Assert.Equal(5, palette.Length);
    }
}
