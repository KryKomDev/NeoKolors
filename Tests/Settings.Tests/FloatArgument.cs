//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument.Exception;

namespace NeoKolors.Settings.Tests;

public class FloatArgumentTests {
    
    [Fact]
    public void Set_ShouldWorkProperly() {
        var a = new FloatArgument();
        
        // try with double
        a.Set(1.23f);
        Assert.Equal(1.23f, a.Value);
        
        // try with string
        a.Set("4.56");
        Assert.Equal(4.56f, a.Value);
        
        // try with another argument
        var b = new FloatArgument();
        b.Set(7.89f);
        a.Set(b);
        Assert.Equal(7.89f, a.Value);
    }

    [Fact]
    public void Set_ShouldThrowException_ForInvalidString() {
        var a = new FloatArgument();
        Assert.Throws<ArgumentInputFormatException>(() => a.Set("invalid"));
    }

    [Fact]
    public void Set_ShouldThrowException_ForOutOfRangeValue() {
        var a = new FloatArgument(min: 0, max: 10);
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(-1f));
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(11f));
    }

    [Fact]
    public void Reset_ShouldWorkProperly() {
        var a = new FloatArgument(defaultValue: 1.23f);
        Assert.Equal(1.23f, a.Value);
        a.Set(4.56f);
        Assert.Equal(4.56f, a.Value);
        a.Reset();
        Assert.Equal(1.23f, a.Value);
    }

    [Fact]
    public void Equal_ShouldWorkProperly() {
        var a = new FloatArgument();
        var b = new FloatArgument(defaultValue: 1.23f);
        
        Assert.False(a.Equals(b));
        
        b.Set(0);
        Assert.False(a.Equals(b));

        var c = new FloatArgument();
        Assert.True(c.Equals(a));
    }
}