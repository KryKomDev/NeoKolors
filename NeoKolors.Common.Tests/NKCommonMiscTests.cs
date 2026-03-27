namespace NeoKolors.Common.Tests;

public class NKCommonMiscTests {

    #region Extensions

    [Fact]
    public void Extensions_UintRgb_ReturnsCorrectChannels() {
        uint rgb = 0x112233;
        
        Assert.Equal(0x11, rgb.R);
        Assert.Equal(0x22, rgb.G);
        Assert.Equal(0x33, rgb.B);
    }

    #endregion
    
    #region NKColorExtensions

    [Fact]
    public void NKColorExtensions_WorkAsExpected() {
        int rgbInt = 0x123456;
        NKColor c1 = rgbInt.Rgb;
        Assert.Equal(0x123456u, c1.AsRgb);

        uint rgbUint = 0xabcdefu;
        NKColor c2 = rgbUint.Rgb;
        Assert.Equal(0xabcdefu, c2.AsRgb);

        NKColor c3 = NKConsoleColor.RED.NK;
        Assert.Equal(NKConsoleColor.RED, c3.AsPalette);
    }

    #endregion
}
