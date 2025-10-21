//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Exceptions;
using NeoKolors.Common.Util;

namespace NeoKolors.Common.Tests;

public class List2DTests {
    
    [Fact]
    public void NewList_IsEmpty() {
        var list = new List2D<int>();
        
        Assert.Equal(0, list.Count);
        Assert.Equal(0, list.XSize);
        Assert.Equal(0, list.YSize);
    }

    [Fact]
    public void AddRow_ToEmptyList_CreatesCorrectStructure() {
        var list = new List2D<int>();
        int[] row = [1, 2, 3];
        
        list.AddRow(row);
        
        Assert.Equal(3, list.XSize);
        Assert.Equal(1, list.YSize);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void AddRow_WithInvalidSize_ThrowsException() {
        var list = new List2D<int>();
        list.AddRow((int[])[1, 2, 3]);
        
        Assert.Throws<List2DException>(() => list.AddRow((int[])[1, 2]));
    }

    [Fact]
    public void AddCol_ToEmptyList_CreatesCorrectStructure() {
        var list = new List2D<int>();
        int[] col = [1, 2, 3];
        
        list.AddCol(col);
        
        Assert.Equal(1, list.XSize);
        Assert.Equal(3, list.YSize);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void AddCol_WithInvalidSize_ThrowsException() {
        var list = new List2D<int>();
        list.AddCol((int[])[1, 2, 3]);
        
        Assert.Throws<List2DException>(() => list.AddCol((int[])[1, 2]));
    }

    [Fact]
    public void Indexer_ReturnsCorrectValues() {
        var list = new List2D<int>();
        list.AddRow((int[])[1, 2, 3]);
        list.AddRow((int[])[4, 5, 6]);
        
        Assert.Equal(1, list[0, 0]);
        Assert.Equal(5, list[1, 1]);
        Assert.Equal(6, list[2, 1]);
    }

    [Fact]
    public void RemoveRow_RemovesCorrectRow() {
        var list = new List2D<int>();
        list.AddRow((int[])[1, 2, 3]);
        list.AddRow((int[])[4, 5, 6]);
        list.AddRow((int[])[7, 8, 9]);
        
        list.RemoveY(1);
        
        Assert.Equal(3, list.XSize);
        Assert.Equal(2, list.YSize);
        Assert.Equal(1, list[0, 0]);
        Assert.Equal(7, list[0, 1]);
    }

    [Fact]
    public void RemoveCol_RemovesCorrectColumn() {
        var list = new List2D<int>();
        list.AddRow((int[])[1, 2, 3]);
        list.AddRow((int[])[4, 5, 6]);
        
        list.RemoveX(1);
        
        Assert.Equal(2, list.XSize);
        Assert.Equal(2, list.YSize);
        Assert.Equal(1, list[0, 0]);
        Assert.Equal(3, list[1, 0]);
    }

    [Fact]
    public void FromArray_CreatesCorrectList2D() {
        int[,] array = {
            {1, 2},
            {3, 4},
            {5, 6}
        };
        
        var list = List2D<int>.FromArray(array);
        
        Assert.Equal(3, list.XSize);
        Assert.Equal(2, list.YSize);
        Assert.Equal(1, list[0, 0]);
        Assert.Equal(6, list[2, 1]);
    }

    [Fact]
    public void ToArray_CreatesCorrectArray() {
        var list = new List2D<int>();
        list.AddRow((int[])[1, 2, 3]);
        list.AddRow((int[])[4, 5, 6]);
        
        var array = list.ToArray();
        
        Assert.Equal(3, array.GetLength(0));
        Assert.Equal(2, array.GetLength(1));
        Assert.Equal(1, array[0, 0]);
        Assert.Equal(6, array[2, 1]);
    }

    [Fact]
    public void Enumeration_YieldsCorrectSequence() {
        var list = new List2D<int>();
        list.AddRow((int[])[1, 2]);
        list.AddRow((int[])[3, 4]);

        int[,] expected = new int[2, 2];
        expected[0, 0] = 1;
        expected[0, 1] = 3;
        expected[1, 0] = 2;
        expected[1, 1] = 4;
        Assert.Equal(expected, list.ToArray());
    }

    [Fact]
    public void RemoveLastRow_RemovesCorrectly() {
        var list = new List2D<int>();
        list.AddRow((int[])[1, 2, 3]);
        list.AddRow((int[])[4, 5, 6]);
        
        list.RemoveY();
        
        Assert.Equal(3, list.XSize);
        Assert.Equal(1, list.YSize);
        Assert.Equal(1, list[0, 0]);
        Assert.Equal(3, list[2, 0]);
    }

    [Fact]
    public void RemoveLastCol_RemovesCorrectly() {
        var list = new List2D<int>();
        list.AddRow((int[])[1, 2, 3]);
        list.AddRow((int[])[4, 5, 6]);
        
        list.RemoveX();
        
        Assert.Equal(2, list.XSize);
        Assert.Equal(2, list.YSize);
        Assert.Equal(1, list[0, 0]);
        Assert.Equal(5, list[1, 1]);
    }
}