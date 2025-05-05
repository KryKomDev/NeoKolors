//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Collections;
using System.Diagnostics;
using NeoKolors.Common.Exceptions;

#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace NeoKolors.Common.Util;

public class List2D<T> : List2D, IEnumerable<T> {
    
    private readonly List<List<T>> _matrix;
    
    public new List<T> this[int index] => _matrix[index];
    public new T this[int x, int y] => _matrix[x][y];
    
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
    
    public new T[,] ToArray() {
        var array = new T[XSize, YSize];
        
        for (int x = 0; x < XSize; x++) {
            for (int y = 0; y < YSize; y++) {
                array[x, y] = this[x, y];
            }
        }
        
        return array;
    }
    
    public new IEnumerator<T> GetEnumerator() {
        for (int x = 0; x < XSize; x++) {
            for (int y = 0; y < YSize; y++) {
                yield return _matrix[x][y];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class List2D : IEnumerable {
    private readonly List<List<object>> _matrix;

    public int Count => XSize * YSize;
    public int XSize => _matrix.Count;
    public int YSize => _matrix.Count == 0 ? 0 : _matrix[0].Count;
    
    public List<object> this[int index] => _matrix[index];
    public object this[int x, int y] => _matrix[x][y];
    
    public List2D() {
        _matrix = new List<List<object>>();
    }
    
    [OverloadResolutionPriority(-1)]
    public void AddRow(List<object> row) {
        if (row.Count != XSize && YSize != 0) throw List2DException.AddInvalidRowSize(XSize, row.Count);
        
        if (YSize == 0)
            for (int x = 0; x < row.Count; x++)
                _matrix.Add([]);
        
        for (int x = 0; x < XSize; x++)
            _matrix[x].Add(row[x]);
    }
    
    [OverloadResolutionPriority(1)]
    public void AddRow(object[] row) {
        if (row.Length != XSize && YSize != 0) throw List2DException.AddInvalidRowSize(XSize, row.Length);
        
        if (YSize == 0)
            for (int x = 0; x < row.Length; x++)
                _matrix.Add([]);
        
        for (int x = 0; x < XSize; x++)
            _matrix[x].Add(row[x]);  
    }
    
    [OverloadResolutionPriority(-1)]
    public void AddCol(List<object> col) {
        if (col.Count != YSize && XSize != 0) throw List2DException.AddInvalidColSize(YSize, col.Count);
        _matrix.Add(col);
    }
    
    [OverloadResolutionPriority(1)]
    public void AddCol(object[] col) {
        if (col.Length != YSize && XSize != 0) throw List2DException.AddInvalidColSize(YSize, col.Length);
        _matrix.Add(col.ToList());
    }

    public static List2D FromArray(object[,] array) {
        List2D list = new();
        
        for (int x = 0; x < array.GetLength(0); x++) {
            List<object> col = [];
            
            for (int y = 0; y < array.GetLength(1); y++) {
                col.Add(array[x, y]);
            }
            
            list.AddCol(col);
        }
        
        return list;
    } 
    
    public object[,] ToArray() {
        var array = new object[XSize, YSize];
        
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

    public IEnumerator GetEnumerator() {
        for (int x = 0; x < XSize; x++) {
            for (int y = 0; y < YSize; y++) {
                yield return _matrix[x][y];
            }
        }
    }

    /// <summary>
    /// Resizes a two-dimensional array to the specified dimensions. If the new dimensions are
    /// larger, the new elements are initialized with the default value for the type. If the dimensions
    /// are smaller, the resulting array contains only the values that fit within the new bounds.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The reference to the array to be resized. Can be null, in which case a new array is created.</param>
    /// <param name="newSizeDim0">The new size for the first dimension of the array. Must be a non-negative value.</param>
    /// <param name="newSizeDim1">The new size for the second dimension of the array. Must be a non-negative value.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="newSizeDim0"/> or <paramref name="newSizeDim1"/> is less than 0.
    /// </exception>
    public static void Resize<T>(ref T[,]? array, int newSizeDim0, int newSizeDim1) {
        if (newSizeDim0 < 0)
            throw new ArgumentOutOfRangeException(nameof(newSizeDim0), "Must be a non-negative number.");

        if (newSizeDim1 < 0) 
            throw new ArgumentOutOfRangeException(nameof(newSizeDim1), "Must be a non-negative number.");
        
        // local copy
        var lArray = array;

        if (lArray is null) {
            array = new T[newSizeDim0, newSizeDim1];
            return;
        }

        // if the new size is the same as the old size, do nothing
        if (lArray.GetLength(0) == newSizeDim0 || lArray.GetLength(1) == newSizeDim1) {
            Debug.Assert(array is not null);
            return;
        }
        
        // copy the actual array
        var newArray = new T[newSizeDim0, newSizeDim1];

        for (int i0 = 0; i0 < Math.Min(newSizeDim0, lArray.GetLength(0)); i0++) {
            for (int i1 = 0; i1 < Math.Min(newSizeDim1, lArray.GetLength(1)); i1++) {
                newArray[i0, i1] = lArray[i0, i1];
            }
        }
        
        // overwrite the old array
        array = newArray;
        Debug.Assert(array is not null);
    }

    /// <summary>
    /// Fills a two-dimensional array with the specified value, overwriting all existing elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to be filled. Must not be null.</param>
    /// <param name="value">The value to assign to every element in the array.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="array"/> is null.
    /// </exception>
    public static void Fill<T>(T[,]? array, T value) {
        if (array is null)
            throw new ArgumentNullException(nameof(array));

        // when Span2D is available, use the System.Array-like Fill method implementation 
        
        for (int i0 = 0; i0 < array.GetLength(0); i0++) {
            for (int i1 = 0; i1 < array.GetLength(1); i1++) {
                array[i0, i1] = value;
            }
        }
    }

    /// <summary>
    /// Fills a specified rectangular region of a two-dimensional array with a provided value.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to be filled. Must not be null.</param>
    /// <param name="value">The value to assign to the specified region of the array.</param>
    /// <param name="xStart">The starting index for the first dimension. Must be a non-negative value and within the bounds of the array.</param>
    /// <param name="yStart">The starting index for the second dimension. Must be a non-negative value and within the bounds of the array.</param>
    /// <param name="xCount">The number of elements to fill in the first dimension. Must be a non-negative value and must fit within the array bounds starting from <paramref name="xStart"/>.</param>
    /// <param name="yCount">The number of elements to fill in the second dimension. Must be a non-negative value and must fit within the array bounds starting from <paramref name="yStart"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="xStart"/>, <paramref name="yStart"/>, <paramref name="xCount"/>, or <paramref name="yCount"/> is invalid
    /// (e.g., negative or out of range for the specified array).
    /// </exception>
    public static void FillCount<T>(T[,]? array, T value, int xStart, int yStart, int xCount, int yCount) {
        if (array is null)
            throw new ArgumentNullException(nameof(array));

        // negative checks
        if (xStart < 0) throw new ArgumentOutOfRangeException(nameof(xStart), "Must be a non-negative number.");
        if (yStart < 0) throw new ArgumentOutOfRangeException(nameof(yStart), "Must be a non-negative number.");
        if (xCount < 0) throw new ArgumentOutOfRangeException(nameof(xCount), "Must be a non-negative number.");
        if (yCount < 0) throw new ArgumentOutOfRangeException(nameof(yCount), "Must be a non-negative number.");
        
        // out of bounds checks
        if ((uint)xStart >= (uint)array.GetLength(0))
            throw new ArgumentOutOfRangeException(nameof(xStart), "Must be a less or equal to array dim0 length.");
        
        if ((uint)yStart >= (uint)array.GetLength(1))
            throw new ArgumentOutOfRangeException(nameof(yStart), "Must be a less or equal to array dim1 length.");
        
        if ((uint)xCount > (uint)(array.GetLength(0) - xStart)) 
            throw new ArgumentOutOfRangeException(nameof(xCount), 
                "Must be less or equal to array dim0 length minus dim0Count.");
        
        if ((uint)yCount > (uint)(array.GetLength(1) - yStart)) 
            throw new ArgumentOutOfRangeException(nameof(yCount), 
                "Must be less or equal to array dim1 length minus dim1Count.");
        
        // when Span2D is available, use the System.Array-like Fill method implementation 
        
        for (int i0 = xStart; i0 < xStart + xCount; i0++) {
            for (int i1 = yStart; i1 < yStart + yCount; i1++) {
                array[i0, i1] = value;
            }
        }
    }

    /// <summary>
    /// Fills a specified subregion within a two-dimensional array with a given value. The subregion is defined
    /// by the provided coordinates, and the method assigns the value to all elements in the specified range.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to be filled. Cannot be null.</param>
    /// <param name="value">The value to assign to each element within the specified subregion.</param>
    /// <param name="xStart">The starting index for the first dimension of the array. Must be a non-negative value
    /// and within the bounds of the array.</param>
    /// <param name="yStart">The starting index for the second dimension of the array. Must be a non-negative value
    /// and within the bounds of the array.</param>
    /// <param name="xEnd">The ending index for the first dimension of the array. Must be a non-negative value
    /// and within the bounds of the array.</param>
    /// <param name="yEnd">The ending index for the second dimension of the array. Must be a non-negative value
    /// and within the bounds of the array.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="array"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="xStart"/>, <paramref name="yStart"/>, <paramref name="xEnd"/>, or <paramref name="yEnd"/>
    /// is negative or when these values exceed the respective dimensions of <paramref name="array"/>.
    /// </exception>
    public static void Fill<T>(T[,]? array, T value, int xStart, int yStart, int xEnd, int yEnd) {
        if (array is null)
            throw new ArgumentNullException(nameof(array));

        // negative checks
        if (xStart < 0) throw new ArgumentOutOfRangeException(nameof(xStart), "Must be a non-negative number.");
        if (yStart < 0) throw new ArgumentOutOfRangeException(nameof(yStart), "Must be a non-negative number.");
        if (xEnd < 0) throw new ArgumentOutOfRangeException(nameof(xEnd), "Must be a non-negative number.");
        if (yEnd < 0) throw new ArgumentOutOfRangeException(nameof(yEnd), "Must be a non-negative number.");
        
        // out of bounds checks
        if ((uint)xStart >= (uint)array.GetLength(0))
            throw new ArgumentOutOfRangeException(nameof(xStart), "Must be a less or equal to array dim0 length.");
        
        if ((uint)yStart >= (uint)array.GetLength(1))
            throw new ArgumentOutOfRangeException(nameof(yStart), "Must be a less or equal to array dim1 length.");
        
        if ((uint)xEnd >= (uint)array.GetLength(0))
            throw new ArgumentOutOfRangeException(nameof(xEnd), "Must be a less or equal to array dim0 length.");
        
        if ((uint)yEnd >= (uint)array.GetLength(1))
            throw new ArgumentOutOfRangeException(nameof(yEnd), "Must be a less or equal to array dim1 length.");
        
        // switch start and end if necessary
        (xStart, xEnd) = xStart < xEnd ? (xStart, xEnd) : (xEnd, xStart);
        (yStart, yEnd) = yStart < yEnd ? (yStart, yEnd) : (yEnd, yStart);
        
        // when Span2D is available, use the System.Array-like Fill method implementation 
        
        for (int i0 = xStart; i0 <= xEnd; i0++) {
            for (int i1 = yStart; i1 <= yEnd; i1++) {
                array[i0, i1] = value;
            }
        }
    }
}