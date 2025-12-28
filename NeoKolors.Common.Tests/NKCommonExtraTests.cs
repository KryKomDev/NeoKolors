using Xunit;

namespace NeoKolors.Common.Tests;

public class NKCommonExtraTests {

    [Fact]
    public void NKColor_GetInverse_Rgb() {
        // Arrange
        var color = NKColor.FromRgb(100, 150, 200);

        // Act
        var inverse = color.GetInverse();

        // Assert
        Assert.Equal(ColorType.RGB, inverse.Type);
        Assert.Equal(255 - 100, (byte)(inverse.AsRgb >> 16));
        Assert.Equal(255 - 150, (byte)(inverse.AsRgb >> 8));
        Assert.Equal(255 - 200, (byte)inverse.AsRgb);
    }

    [Fact]
    public void NKColor_GetInverse_Palette() {
        // Arrange
        var color = new NKColor(NKConsoleColor.BLACK); // 0

        // Act
        var inverse = color.GetInverse();

        // Assert
        Assert.Equal(ColorType.CONSOLE_COLOR, inverse.Type);
        Assert.Equal(NKConsoleColor.DARK_GRAY, inverse.AsPalette); // (0 + 8) % 16 = 8
    }

    [Fact]
    public void NKColor_Equality() {
        var c1 = new NKColor(0x123456);
        var c2 = new NKColor(0x123456);
        var c3 = new NKColor(0x654321);

        Assert.True(c1 == c2);
        Assert.False(c1 == c3);
        Assert.True(c1.Equals(c2));
    }

    [Fact]
    public void NKStyle_Constructor_SetsCorrectValues() {
        // Arrange
        var fColor = new NKColor(0xff0000);
        var bColor = new NKColor(0x00ff00);
        var styles = TextStyles.BOLD | TextStyles.ITALIC;

        // Act
        var style = new NKStyle(fColor, bColor, styles);

        // Assert
        Assert.Equal(fColor, style.FColor);
        Assert.Equal(bColor, style.BColor);
        Assert.Equal(styles, style.Styles);
    }

    [Fact]
    public void NKStyle_SetFColor_UpdatesCorrectly() {
        // Arrange
        var style = new NKStyle();
        var color = new NKColor(0x0000ff);

        // Act
        style.SetFColor(color);

        // Assert
        Assert.Equal(color, style.FColor);
    }

    [Fact]
    public void TextStyles_Flags_WorkAsExpected() {
        var styles = TextStyles.BOLD | TextStyles.ITALIC;
        
        Assert.True(styles.HasFlag(TextStyles.BOLD));
        Assert.True(styles.HasFlag(TextStyles.ITALIC));
        Assert.False(styles.HasFlag(TextStyles.UNDERLINE));
    }
}
