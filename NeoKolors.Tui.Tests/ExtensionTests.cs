//
// NeoKolors.Test
// Copyright (c) 2026 KryKom
//

using NeoKolors.Tui.Extensions;

namespace NeoKolors.Tui.Tests;

public class ExtensionTests {

    [Fact]
    public void DimensionUnits_ShouldWork() {
        Dimension d1 = 10.Ch;
        Dimension d2 = 20.Px;
        Dimension d3 = 50.Pc;
        Dimension d4 = 5.Vw;
        Dimension d5 = 5.Vh;
        
        Assert.Equal(10, d1.ToScalar(100));
        Assert.Equal(20, d2.ToScalar(100));
        Assert.Equal(50, d3.ToScalar(100));
        Assert.True(d4.IsNumber);
        Assert.True(d5.IsNumber);
    }
}
