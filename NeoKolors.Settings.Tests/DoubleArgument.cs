using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Exception;

namespace NeoKolors.Test.Settings;

public class DoubleArgumentTests {
    
    [Fact]
    public void Set_ShouldWorkProperly() {
        DoubleArgument a = new DoubleArgument();
        
        // try with double
        a.Set(1.23);
        Assert.Equal(1.23, a.Value);
        
        // try with string
        a.Set("4.56");
        Assert.Equal(4.56, a.Value);
        
        // try with another argument
        var b = new DoubleArgument();
        b.Set(7.89);
        a.Set(b);
        Assert.Equal(7.89, a.Value);
    }

    [Fact]
    public void Set_ShouldThrowException_ForInvalidString() {
        DoubleArgument a = new DoubleArgument();
        Assert.Throws<ArgumentInputFormatException>(() => a.Set("invalid"));
    }

    [Fact]
    public void Set_ShouldThrowException_ForOutOfRangeValue() {
        DoubleArgument a = new DoubleArgument(minValue: 0, maxValue: 10);
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(-1));
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(11));
    }

    [Fact]
    public void Reset_ShouldWorkProperly() {
        DoubleArgument a = new DoubleArgument(defaultValue: 1.23);
        Assert.Equal(1.23, a.Value);
        a.Set(4.56);
        Assert.Equal(4.56, a.Value);
        a.Reset();
        Assert.Equal(1.23, a.Value);
    }

    [Fact]
    public void Equal_ShouldWorkProperly() {
        var a = new DoubleArgument();
        var b = new DoubleArgument(defaultValue: 1.23);
        
        Assert.False(a.Equals(b));
        
        b.Set(0);
        Assert.False(a.Equals(b));

        var c = new DoubleArgument();
        Assert.True(c.Equals(a));
    }
}