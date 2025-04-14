//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common.Tests;

public class StringUtilsTests {
    
    [Fact]
    public void VisibleLength_ShouldReturnCorrectValue() {
        var s = "\e[38;2;1;1;1mHello\e[39m World\b";
        Assert.Equal(12, s.VisibleLength());
    }

    [Fact]
    public void Capitalize_ShouldReturnCorrectValue() {
        var s = "hello, world!";
        Assert.Equal("Hello, World!", s.Capitalize());
        s = "";
        Assert.Equal("", s.Capitalize());
        s = null;
        Assert.Throws<ArgumentNullException>(() => s!.Capitalize());
    }

    [Fact]
    public void CapitalizeFirst_ShouldReturnCorrectValue() {
        var s = "hello, world!";
        Assert.Equal("Hello, world!", s.CapitalizeFirst());
        s = "";
        Assert.Equal("", s.CapitalizeFirst());
        s = null;
        Assert.Throws<ArgumentNullException>(() => s!.CapitalizeFirst());
    }

    [Fact]
    public void Format_ShouldReturnCorrectValue() {
        var s = "hello, {0}".Format("world");
        Assert.Equal("hello, world", s.Format());
    }
}