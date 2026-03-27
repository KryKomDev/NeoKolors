//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Tui.Rendering;

namespace NeoKolors.Tui.Tests;

public class CellInfoTests {

    [Fact]
    public void Constructor_ShouldInitializeProperties() {
        var style = new NKStyle(NKConsoleColor.RED, NKConsoleColor.BLUE);
        var cell = new CellInfo('A', style, true, 5);
        
        Assert.Equal('A', cell.Char);
        Assert.Equal(style, cell.Style);
        Assert.True(cell.Changed);
        Assert.Equal(5, cell.ZIndex);
    }

    [Fact]
    public void Equality_ShouldWorkForRecords() {
        var style = new NKStyle(NKConsoleColor.RED, NKConsoleColor.BLUE);
        var cell1 = new CellInfo('A', style, true, 1);
        var cell2 = new CellInfo('A', style, true, 1);
        
        Assert.Equal(cell1, cell2);
    }

    [Fact]
    public void Properties_ShouldBeMutable() {
        var cell = CellInfo.Default;
        cell.Char = 'B';
        cell.ZIndex = 10;
        
        Assert.Equal('B', cell.Char);
        Assert.Equal(10, cell.ZIndex);
    }
}
