// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Extensions.Tests;

public class Numeric {

    [Fact]
    public void Belongs_ReturnsCorrectValue() {
        Assert.True(0.Belongs(0..10));
        Assert.True(10.Belongs(0..10));
        Assert.False(0.Belongs(^0..10));
        Assert.False(10.Belongs(0..^10));
        Assert.False((-1).Belongs(0..10));
        Assert.False(11.Belongs(0..10));
    }
}