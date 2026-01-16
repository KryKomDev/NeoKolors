//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Tests;

public class NKCharScreenTests {
    
    [Fact]
    public void Constructor_ShouldInitializeCorrectly() {
        var screen = new NKCharScreen(10, 5);
        Assert.Equal(10, screen.Width);
        Assert.Equal(5, screen.Height);
    }

    [Fact]
    public void Constructor_WithSize_ShouldInitializeCorrectly() {
        // Size is from NeoKolors.Tui (struct Size : IEquatable<Size>)
        var size = new Size(20, 10);
        var screen = new NKCharScreen(size);
        Assert.Equal(20, screen.Width);
        Assert.Equal(10, screen.Height);
    }
}
