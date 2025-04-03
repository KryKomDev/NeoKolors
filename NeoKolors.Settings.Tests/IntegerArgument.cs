using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Exception;

namespace NeoKolors.Test.Settings;

public class IntegerArgumentTests {
    
    [Fact]
    public void Set_ShouldWorkProperly() {
        IntegerArgument a = new IntegerArgument();
        
        // try with double
        a.Set(1);
        Assert.Equal(1, a.Value);
        
        // try with string
        a.Set("4");
        Assert.Equal(4, a.Value);
        
        // try with another argument
        var b = new IntegerArgument();
        b.Set(7);
        a.Set(b);
        Assert.Equal(7, a.Value);
    }

    [Fact]
    public void Set_ShouldThrowException_ForInvalidString() {
        IntegerArgument a = new IntegerArgument();
        Assert.Throws<ArgumentInputFormatException>(() => a.Set("invalid"));
    }

    [Fact]
    public void Set_ShouldThrowException_ForOutOfRangeValue() {
        IntegerArgument a = new IntegerArgument(min: 0, max: 10);
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(-1));
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(11));
    }

    [Fact]
    public void Reset_ShouldWorkProperly() {
        IntegerArgument a = new IntegerArgument(defaultValue: 1);
        Assert.Equal(1, a.Value);
        a.Set(4);
        Assert.Equal(4, a.Value);
        a.Reset();
        Assert.Equal(1, a.Value);
    }

    [Fact]
    public void Equal_ShouldWorkProperly() {
        var a = new IntegerArgument();
        var b = new IntegerArgument(defaultValue: 1);
        
        Assert.False(a.Equals(b));
        
        b.Set(0);
        Assert.False(a.Equals(b));

        var c = new IntegerArgument();
        Assert.True(c.Equals(a));
    }
}

public class UIntegerArgumentTests {
    
    [Fact]
    public void Set_ShouldWorkProperly() {
        UIntegerArgument a = new UIntegerArgument();
        
        // try with double
        a.Set(1u);
        Assert.Equal(1u, a.Value);
        
        // try with string
        a.Set("4");
        Assert.Equal(4u, a.Value);
        
        // try with another argument
        var b = new UIntegerArgument();
        b.Set(7u);
        a.Set(b);
        Assert.Equal(7u, a.Value);
    }

    [Fact]
    public void Set_ShouldThrowException_ForInvalidString() {
        UIntegerArgument a = new UIntegerArgument();
        Assert.Throws<ArgumentInputFormatException>(() => a.Set("invalid"));
    }

    [Fact]
    public void Set_ShouldThrowException_ForOutOfRangeValue() {
        UIntegerArgument a = new UIntegerArgument(min: 2, max: 10);
        Assert.Throws<InvalidArgumentInputTypeException>(() => a.Set(-1));
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(1));
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(11));
    }

    [Fact]
    public void Reset_ShouldWorkProperly() {
        UIntegerArgument a = new UIntegerArgument(defaultValue: 1);
        Assert.Equal(1u, a.Value);
        a.Set(4);
        Assert.Equal(4u, a.Value);
        a.Reset();
        Assert.Equal(1u, a.Value);
    }

    [Fact]
    public void Equal_ShouldWorkProperly() {
        var a = new UIntegerArgument();
        var b = new UIntegerArgument(defaultValue: 1u);
        
        Assert.False(a.Equals(b));
        
        b.Set(0u);
        Assert.False(a.Equals(b));

        var c = new UIntegerArgument();
        Assert.True(c.Equals(a));
    }
}