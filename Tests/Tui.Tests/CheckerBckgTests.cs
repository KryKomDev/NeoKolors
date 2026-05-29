
using NeoKolors.Common;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Tests;

public class CheckerBckgTests {
    [Fact]
    public void StyleCheckerBckg_ShouldBeAnchoredToGlobalCoordinates() {
        var canvas = new NKCharCanvas(10, 10);
        var checker = new CheckerBckg(new NKColor(ConsoleColor.Red), new NKColor(ConsoleColor.Blue), true, 2, 2);
        
        // Pattern for 2x2 should be:
        // (0,0),(1,0),(0,1),(1,1) -> Blue (xs=false, ys=false -> false^false = false)
        // (2,0),(3,0),(2,1),(3,1) -> Red  (xs=true, ys=false -> true^false = true)
        
        // Wait, looking at current code: (xs ^ ys) ? checker.C1 : checker.C2;
        // xs starts false, ys starts false. So (false ^ false) is false -> C2 (Blue). Correct.

        // Test with region starting at (0,0)
        canvas.StyleCheckerBckg(new Rectangle(0, 0, 4, 4), checker.FieldSize, checker.C1, checker.C2);
        
        Assert.Equal(new NKColor(ConsoleColor.Blue), canvas[0, 0].Style.BColor);
        Assert.Equal(new NKColor(ConsoleColor.Blue), canvas[1, 1].Style.BColor);
        Assert.Equal(new NKColor(ConsoleColor.Red), canvas[2, 0].Style.BColor);
        Assert.Equal(new NKColor(ConsoleColor.Red), canvas[0, 2].Style.BColor);
        Assert.Equal(new NKColor(ConsoleColor.Blue), canvas[2, 2].Style.BColor);

        canvas.Clean();

        // Test with region starting at (1,1)
        // If it's anchored globally:
        // (1,1) is still Blue.
        // (2,1) is still Red.
        // (1,2) is still Red.
        // (2,2) is still Blue.
        canvas.StyleCheckerBckg(new Rectangle(1, 1, 3, 3), checker.FieldSize, checker.C1, checker.C2);

        Assert.Equal(new NKColor(ConsoleColor.Blue), canvas[1, 1].Style.BColor);
        Assert.Equal(new NKColor(ConsoleColor.Red), canvas[2, 1].Style.BColor);
        Assert.Equal(new NKColor(ConsoleColor.Red), canvas[1, 2].Style.BColor);
        Assert.Equal(new NKColor(ConsoleColor.Blue), canvas[2, 2].Style.BColor);
    }
}
