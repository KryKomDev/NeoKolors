// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Extensions.Tests;

public class NumericTests {

    [Fact]
    public void Clamp_ReturnsCorrectValue() {
        Assert.Equal(5, 5.Clamp(0, 10));
        Assert.Equal(0, (-5).Clamp(0, 10));
        Assert.Equal(10, 15.Clamp(0, 10));
    }

    [Fact]
    public void DClamp_ReturnsCorrectValue() {
        // Normal bounds
        Assert.Equal(5, Math.DClamp(5, 0, 10));
        Assert.Equal(0, Math.DClamp(-5, 0, 10));
        Assert.Equal(10, Math.DClamp(15, 0, 10));

        // Swapped bounds
        Assert.Equal(5, Math.DClamp(5, 10, 0));
        Assert.Equal(0, Math.DClamp(-5, 10, 0));
        Assert.Equal(10, Math.DClamp(15, 10, 0));
    }

    [Fact]
    public void TopClamp_ReturnsCorrectValue() {
        Assert.Equal(5, 10.TopClamp(5));
        Assert.Equal(3, 3.TopClamp(5));
        Assert.Equal(-2, 0.TopClamp(-2));
        Assert.Equal(-5, (-5).TopClamp(-2));
    }

    [Fact]
    public void BottomClamp_ReturnsCorrectValue() {
        Assert.Equal(5, 2.BottomClamp(5));
        Assert.Equal(7, 7.BottomClamp(5));
        Assert.Equal(0, (-3).BottomClamp(0));
        Assert.Equal(4, 4.BottomClamp(0));
    }

    [Theory]
    [InlineData(0u, 0)]
    [InlineData(1u, 1)]
    [InlineData(0b1011u, 3)]
    [InlineData(0xFFu, 8)]
    [InlineData(0xFFFFFFFFu, 32)]
    public void PopCount_UInt_ReturnsCorrectCount(uint input, int expected) {
        Assert.Equal(expected, Numeric.PopCount(input));
    }

    [Theory]
    [InlineData((byte)0, 0)]
    [InlineData((byte)1, 1)]
    [InlineData((byte)0b10101010, 4)]
    [InlineData((byte)0b11110000, 4)]
    [InlineData((byte)255, 8)]
    public void PopCount_Byte_ReturnsCorrectCount(byte input, int expected) {
        Assert.Equal(expected, Numeric.PopCount(input));
    }

    [Fact]
    public void Index_ReturnsIndexWithCorrectValue() {
        var idx = 42.Index;
        Assert.Equal(42, idx.Value);
        Assert.False(idx.IsFromEnd);
    }
}