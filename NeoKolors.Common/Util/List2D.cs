//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Collections;
using NeoKolors.Common.Exceptions;

#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace NeoKolors.Common.Util;

public class List2D<T> : IEnumerable<T> {
    
    private readonly List<List<T>> _matrix;

    public int Count => XSize * YSize;
    public int XSize => _matrix.Count;
    public int YSize => _matrix.Count == 0 ? 0 : _matrix[0].Count;
    
    public List<T> this[int index] => _matrix[index];
    public T this[int x, int y] => _matrix[x][y];
    
    public List2D() {
        _matrix = new List<List<T>>();
    }
    
    [OverloadResolutionPriority(-1)]
    public void AddRow(List<T> row) {
        if (row.Count != XSize && YSize != 0) throw List2DException.AddInvalidRowSize(XSize, row.Count);
        
        if (YSize == 0)
            for (int x = 0; x < row.Count; x++)
                _matrix.Add([]);
        
        for (int x = 0; x < XSize; x++)
            _matrix[x].Add(row[x]);
    }
    
    [OverloadResolutionPriority(1)]
    public void AddRow(T[] row) {
        if (row.Length != XSize && YSize != 0) throw List2DException.AddInvalidRowSize(XSize, row.Length);
        
        if (YSize == 0)
            for (int x = 0; x < row.Length; x++)
                _matrix.Add([]);
        
        for (int x = 0; x < XSize; x++)
            _matrix[x].Add(row[x]);  
    }
    
    [OverloadResolutionPriority(-1)]
    public void AddCol(List<T> col) {
        if (col.Count != YSize && XSize != 0) throw List2DException.AddInvalidColSize(YSize, col.Count);
        _matrix.Add(col);
    }
    
    [OverloadResolutionPriority(1)]
    public void AddCol(T[] col) {
        if (col.Length != YSize && XSize != 0) throw List2DException.AddInvalidColSize(YSize, col.Length);
        _matrix.Add(col.ToList());
    }

    public static List2D<T> FromArray(T[,] array) {
        List2D<T> list = new();
        
        for (int x = 0; x < array.GetLength(0); x++) {
            List<T> col = new();
            
            for (int y = 0; y < array.GetLength(1); y++) {
                col.Add(array[x, y]);
            }
            
            list.AddCol(col);
        }
        
        return list;
    } 
    
    public T[,] ToArray() {
        var array = new T[XSize, YSize];
        
        for (int x = 0; x < XSize; x++) {
            for (int y = 0; y < YSize; y++) {
                array[x, y] = this[x, y];
            }
        }
        
        return array;
    }

    public void RemoveRow(int index) {
        if (index < 0 || index >= YSize) throw new IndexOutOfRangeException();
        for (int x = 0; x < XSize; x++)
            _matrix[x].RemoveAt(index);
    }
    
    public void RemoveRow() {
        int y = YSize - 1;
        for (int x = 0; x < XSize; x++) 
            _matrix[x].RemoveAt(y);
    }

    public void RemoveCol(int index) {
        if (index < 0 || index >= XSize) throw new IndexOutOfRangeException();
        _matrix.RemoveAt(index);
    }

    public void RemoveCol() {
        if (XSize != 0)
            _matrix.RemoveAt(XSize - 1);
    }
    
    public IEnumerator<T> GetEnumerator() {
        for (int x = 0; x < XSize; x++) {
            for (int y = 0; y < YSize; y++) {
                yield return _matrix[x][y];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}