//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Console;

namespace NeoKolors.Test.Console;

public class FancyExceptionTests {
    
    [Fact]
    public void StaticCreate_ShouldBeSetUpCorrectly() {

        DivideByZeroException e = new DivideByZeroException();

        try {
            int zero = 0;
            // ReSharper disable once IntDivisionByZero
            int i = 1 / zero;
        }
        catch (DivideByZeroException re) {
            e = re;
        }

        var fe = (FancyException<DivideByZeroException>)Debug.ToFancy(e);
        Assert.Equal(e, fe.OriginalException);
    }

    [Fact]
    public void Catch_ShouldBeCaughtProperly() {
        DivideByZeroException d = new DivideByZeroException();
        FancyException<DivideByZeroException>? f = null;

        try {
            try {
                Debug.Throw(d);
            }
            catch (FancyException<DivideByZeroException> e) {
                f = e;
            }
        }
        catch (Exception) {
            Assert.Fail("Fancy exception was not caught properly.");
        }

        Assert.True(f != null);
    }
}