//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument.Exception;

namespace NeoKolors.Settings.Tests;

public class LongArgumentTests {
    
    [Fact]
    public void Set_ShouldWorkProperly() {
        var a = new LongArgument();
        
        // try with double
        a.Set(1);
        Assert.Equal(1, a.Value);
        
        // try with string
        a.Set("4");
        Assert.Equal(4, a.Value);
        
        // try with another argument
        var b = new LongArgument();
        b.Set(7);
        a.Set(b);
        Assert.Equal(7, a.Value);
    }

    [Fact]
    public void Set_ShouldThrowException_ForInvalidString() {
        var a = new LongArgument();
        Assert.Throws<ArgumentInputFormatException>(() => a.Set("invalid"));
    }

    [Fact]
    public void Set_ShouldThrowException_ForOutOfRangeValue() {
        var a = new LongArgument(min: 0, max: 10);
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(-1));
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(11));
    }

    [Fact]
    public void Reset_ShouldWorkProperly() {
        var a = new LongArgument(defaultValue: 1);
        Assert.Equal(1, a.Value);
        a.Set(4);
        Assert.Equal(4, a.Value);
        a.Reset();
        Assert.Equal(1, a.Value);
    }

    [Fact]
    public void Equal_ShouldWorkProperly() {
        var a = new LongArgument();
        var b = new LongArgument(defaultValue: 1);
        
        Assert.False(a.Equals(b));
        
        b.Set(0);
        Assert.False(a.Equals(b));

        var c = new LongArgument();
        Assert.True(c.Equals(a));
    }
}

public class ULongArgumentTests {
    
    [Fact]
    public void Set_ShouldWorkProperly() {
        var a = new ULongArgument();
        
        // try with double
        a.Set(1u);
        Assert.Equal(1u, a.Value);
        
        // try with string
        a.Set("4");
        Assert.Equal(4u, a.Value);
        
        // try with another argument
        var b = new ULongArgument();
        b.Set(7u);
        a.Set(b);
        Assert.Equal(7u, a.Value);
    }

    [Fact]
    public void Set_ShouldThrowException_ForInvalidString() {
        var a = new ULongArgument();
        Assert.Throws<ArgumentInputFormatException>(() => a.Set("invalid"));
    }

    [Fact]
    public void Set_ShouldThrowException_ForOutOfRangeValue() {
        var a = new ULongArgument(defaultValue: 2, min: 2, max: 10);
        Assert.Throws<InvalidArgumentInputTypeException>(() => a.Set(-1));
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(1));
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(11));
    }

    [Fact]
    public void Reset_ShouldWorkProperly() {
        var a = new ULongArgument(defaultValue: 1);
        Assert.Equal(1u, a.Value);
        a.Set(4);
        Assert.Equal(4u, a.Value);
        a.Reset();
        Assert.Equal(1u, a.Value);
    }

    [Fact]
    public void Equal_ShouldWorkProperly() {
        var a = new ULongArgument();
        var b = new ULongArgument(defaultValue: 1u);
        
        Assert.False(a.Equals(b));
        
        b.Set(0u);
        Assert.False(a.Equals(b));

        var c = new ULongArgument();
        Assert.True(c.Equals(a));
    }
}