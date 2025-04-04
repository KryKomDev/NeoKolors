//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Tests;

public class BoolArgumentTests {

    [Fact]
    public void Set_ShouldWorkProperly() {
        BoolArgument a = new BoolArgument();
        
        // try with bool
        a.Set(true);
        Assert.True(a.Value);
        
        // try with string
        a.Set("faLsE");
        Assert.False(a.Value);
        
        // try with another argument
        var b = new BoolArgument();
        b.Set(true);
        a.Set(b);
        Assert.True(a.Value);
    }

    [Fact]
    public void Reset_ShouldWorkProperly() {
        BoolArgument a = new BoolArgument(defaultValue: true);
        Assert.True(a.Value);
        a.Set(false);
        Assert.False(a.Value);
        a.Reset();
        Assert.True(a.Value);
    }

    [Fact]
    public void Equal_ShouldWorkProperly() {
        var a = new BoolArgument();
        var b = new BoolArgument(true);
        
        Assert.False(a.Equals(b));
        
        b.Set(false);
        Assert.False(a.Equals(b));

        var c = new BoolArgument();
        Assert.True(c.Equals(a));
    }
}