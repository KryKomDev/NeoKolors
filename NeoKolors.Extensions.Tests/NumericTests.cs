// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Extensions.Tests;

public class NumericTests {

    [Fact]
    public void Belongs_ReturnsCorrectValue() {
        // Standard inclusive range
        Assert.True(0.Belongs(0..10));
        Assert.True(10.Belongs(0..10));
        Assert.True(5.Belongs(0..10));
        Assert.False((-1).Belongs(0..10));
        Assert.False(11.Belongs(0..10));

        // Range with "exclusive" start (simulated by FromEnd/^)
        // ^0 -> Start + 1. So ^0..10 becomes 1..10
        Assert.False(0.Belongs(^0..10)); 
        Assert.True(1.Belongs(^0..10));
        Assert.True(10.Belongs(^0..10));

        // Range with "exclusive" end (simulated by FromEnd/^)
        // ^10 -> End - 1. So 0..^10 becomes 0..9
        Assert.True(0.Belongs(0..^10));
        Assert.True(9.Belongs(0..^10));
        Assert.False(10.Belongs(0..^10));
    }

    [Fact]
    public void Operator_Equality_ReturnsCorrectValue() {
        Assert.True(5 == (0..10));
        Assert.False(11 == (0..10));
    }

    [Fact]
    public void Operator_Inequality_ReturnsCorrectValue() {
        Assert.False(5 != (0..10));
        Assert.True(11 != (0..10));
    }

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