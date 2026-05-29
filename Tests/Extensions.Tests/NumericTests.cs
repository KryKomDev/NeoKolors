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
}