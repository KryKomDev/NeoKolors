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
    
    public new int Count => XSize * YSize;
    public new int XSize => _matrix.Count;
    public new int YSize => _matrix.Count == 0 ? 0 : _matrix[0].Count;
    public new List<T> this[int index] => _matrix[index];

    public new T this[int x, int y] {
        get => _matrix[x][y];
        set => _matrix[x][y] = value;
    }
    
    public List2D() {
        _matrix = new List<List<T>>();
    }
    
    /// <inheritdoc cref="List2D.AddRow(List{object})"/>
    [OverloadResolutionPriority(-1)]
    public void AddRow(List<T> row) {
        if (row.Count != XSize && YSize != 0) throw List2DException.AddInvalidRowSize(XSize, row.Count);
        
        if (YSize == 0)
            for (int x = 0; x < row.Count; x++)
                _matrix.Add([]);
        
        for (int x = 0; x < XSize; x++)
            _matrix[x].Add(row[x]);
    }
    
    /// <inheritdoc cref="List2D.AddRow(object[])"/>
    [OverloadResolutionPriority(1)]
    public void AddRow(T[] row) {
        if (row.Length != XSize && YSize != 0) throw List2DException.AddInvalidRowSize(XSize, row.Length);
        
        if (YSize == 0)
            for (int x = 0; x < row.Length; x++)
                _matrix.Add([]);
        
        for (int x = 0; x < XSize; x++)
            _matrix[x].Add(row[x]);  
    }
    
    /// <inheritdoc cref="List2D.AddCol(List{object})"/>
    [OverloadResolutionPriority(-1)]
    public void AddCol(List<T> col) {
        if (col.Count != YSize && XSize != 0) throw List2DException.AddInvalidColSize(YSize, col.Count);
        _matrix.Add(col);
    }
    
    /// <inheritdoc cref="List2D.AddCol(object[])"/>
    [OverloadResolutionPriority(1)]
    public void AddCol(T[] col) {
        if (col.Length != YSize && XSize != 0) throw List2DException.AddInvalidColSize(YSize, col.Length);
        _matrix.Add(col.ToList());
    }

    /// <inheritdoc cref="List2D.FromArray(object[,])"/>
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
    
    /// <inheritdoc cref="List2D.ToArray"/>
    public new T[,] ToArray() {
        var array = new T[XSize, YSize];
        
        for (int x = 0; x < XSize; x++) {
            for (int y = 0; y < YSize; y++) {
                array[x, y] = this[x, y];
            }
        }
        
        return array;
    }
    
    /// <inheritdoc cref="List2D.RemoveRow(int)"/>
    public new void RemoveRow(int index) {
        if (index < 0 || index >= YSize) throw new IndexOutOfRangeException();
        for (int x = 0; x < XSize; x++)
            _matrix[x].RemoveAt(index);
    }
    
    /// <inheritdoc cref="List2D.RemoveRow()"/>
    public new void RemoveRow() {
        int y = YSize - 1;
        for (int x = 0; x < XSize; x++) 
            _matrix[x].RemoveAt(y);
    }

    /// <inheritdoc cref="List2D.RemoveCol(int)"/>
    public new void RemoveCol(int index) {
        if (index < 0 || index >= XSize) throw new IndexOutOfRangeException();
        _matrix.RemoveAt(index);
    }

    /// <inheritdoc cref="List2D.RemoveCol()"/>   
    public new void RemoveCol() {
        if (XSize != 0)
            _matrix.RemoveAt(XSize - 1);
    }

    
    public new IEnumerator<T> GetEnumerator() {
        for (int x = 0; x < XSize; x++) {
            for (int y = 0; y < YSize; y++) {
                yield return _matrix[x][y];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public static implicit operator T[,](List2D<T> list) => list.ToArray();
    public static implicit operator List2D<T>(T[,] array) => FromArray(array);
}

public class List2D : IEnumerable {
    private readonly List<List<object>> _matrix;

    public int Count => XSize * YSize;
    public int XSize => _matrix.Count;
    public int YSize => _matrix.Count == 0 ? 0 : _matrix[0].Count;
    
    public List<object> this[int index] => _matrix[index];

    public object this[int x, int y] {
        get => _matrix[x][y];
        set => _matrix[x][y] = value;
    }
    
    public List2D() {
        _matrix = new List<List<object>>();
    }

    /// <summary>
    /// Adds a new row to the two-dimensional list. If the list is empty, the new row
    /// initializes the structure with the specified elements. If the list is not empty,
    /// the length of the new row must match the current row size.
    /// </summary>
    /// <param name="row">The list of objects to add as a new row. Must match the current
    /// column count if the structure is non-empty.</param>
    /// <exception cref="List2DException">
    /// Thrown when the length of <paramref name="row"/> does not match the current column count
    /// of the structure.
    /// </exception>
    [OverloadResolutionPriority(-1)]
    public void AddRow(List<object> row) {
        if (row.Count != XSize && YSize != 0) throw List2DException.AddInvalidRowSize(XSize, row.Count);
        
        if (YSize == 0)
            for (int x = 0; x < row.Count; x++)
                _matrix.Add([]);
        
        for (int x = 0; x < XSize; x++)
            _matrix[x].Add(row[x]);
    }

    /// <summary>
    /// Adds a new row to the two-dimensional list. If the list is empty, the provided row
    /// initializes the structure. If the list is not empty, the length of the new row
    /// must match the current column count of the structure.
    /// </summary>
    /// <param name="row">The collection of objects to add as a new row. Must match the current
    /// column count if the structure is non-empty.</param>
    /// <exception cref="List2DException">
    /// Thrown when the length of <paramref name="row"/> does not match the current column count
    /// of the structure.
    /// </exception>
    [OverloadResolutionPriority(1)]
    public void AddRow(object[] row) {
        if (row.Length != XSize && YSize != 0) throw List2DException.AddInvalidRowSize(XSize, row.Length);
        
        if (YSize == 0)
            for (int x = 0; x < row.Length; x++)
                _matrix.Add([]);
        
        for (int x = 0; x < XSize; x++)
            _matrix[x].Add(row[x]);  
    }

    /// <summary>
    /// Adds a new column to the two-dimensional list. If the list is empty, the new column
    /// initializes the structure with the specified elements. If the list is not empty,
    /// the length of the new column must match the current column size.
    /// </summary>
    /// <param name="col">The list of objects to add as a new column. Must match the current
    /// row count if the structure is non-empty.</param>
    /// <exception cref="List2DException">
    /// Thrown when the length of <paramref name="col"/> does not match the current row count
    /// of the structure.
    /// </exception>
    [OverloadResolutionPriority(-1)]
    public void AddCol(List<object> col) {
        if (col.Count != YSize && XSize != 0) throw List2DException.AddInvalidColSize(YSize, col.Count);
        _matrix.Add(col);
    }

    /// <summary>
    /// Adds a new column to the two-dimensional list. If the list is empty, the new column
    /// initializes the structure with the specified elements. If the list is not empty,
    /// the length of the new column must match the current column size.
    /// </summary>
    /// <param name="col">The list of objects to add as a new column. Must match the current
    /// row count if the structure is non-empty.</param>
    /// <exception cref="List2DException">
    /// Thrown when the length of <paramref name="col"/> does not match the current row count
    /// of the structure.
    /// </exception>
    [OverloadResolutionPriority(1)]
    public void AddCol(object[] col) {
        if (col.Length != YSize && XSize != 0) throw List2DException.AddInvalidColSize(YSize, col.Length);
        _matrix.Add(col.ToList());
    }

    /// <summary>
    /// Creates a new instance of the <see cref="List2D"/> class and populates it with elements from the specified two-dimensional array.
    /// Each row of the input array is converted into a corresponding column in the resulting structure.
    /// </summary>
    /// <param name="array">The two-dimensional array of objects to populate the <see cref="List2D"/> structure.
    /// The array's dimensions determine the resulting structure's size.</param>
    /// <returns>A new <see cref="List2D"/> instance initialized with elements from the provided two-dimensional array.</returns>
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

    /// <summary>
    /// Converts the two-dimensional list into a two-dimensional array with the same dimensions
    /// and elements.
    /// </summary>
    /// <returns>A two-dimensional array containing the elements of the list in their respective
    /// positions, matching the list's structure.</returns>
    public object[,] ToArray() {
        var array = new object[XSize, YSize];
        
        for (int x = 0; x < XSize; x++) {
            for (int y = 0; y < YSize; y++) {
                array[x, y] = this[x, y];
            }
        }
        
        return array;
    }

    /// <summary>
    /// Removes a row from the two-dimensional list at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the row to remove.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when <paramref name="index"/> is less than 0 or greater than or equal to the number of rows in the structure.
    /// </exception>
    public void RemoveRow(int index) {
        if (index < 0 || index >= YSize) throw new IndexOutOfRangeException();
        for (int x = 0; x < XSize; x++)
            _matrix[x].RemoveAt(index);
    }

    /// <summary>
    /// Removes the last row from the two-dimensional list. If the list is empty, no action is taken.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to remove a row from an empty structure.
    /// </exception>
    public void RemoveRow() {
        int y = YSize - 1;
        for (int x = 0; x < XSize; x++) 
            _matrix[x].RemoveAt(y);
    }

    /// <summary>
    /// Removes a column at the specified index from the two-dimensional list. The column
    /// is identified and deleted based on its index, and all elements in the column are removed.
    /// </summary>
    /// <param name="index">The zero-based index of the column to be removed. Must be within
    /// the valid range of existing columns.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified <paramref name="index"/> is less than 0 or greater than
    /// or equal to the total number of columns in the list.
    /// </exception>
    public void RemoveCol(int index) {
        if (index < 0 || index >= XSize) throw new IndexOutOfRangeException();
        _matrix.RemoveAt(index);
    }

    /// <summary>
    /// Removes the last column from the two-dimensional list. If the list is empty,
    /// this operation has no effect.
    /// </summary>
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

    /// <summary>
    /// Safely fills a specified 2D array region with the given value, ensuring the region bounds are clamped within the array's dimensions.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The target 2D array to be filled.</param>
    /// <param name="value">The value used to fill the specified region of the array.</param>
    /// <param name="xStart">The starting index of the fill range in the first dimension (rows).</param>
    /// <param name="yStart">The starting index of the fill range in the second dimension (columns).</param>
    /// <param name="xEnd">The ending index of the fill range in the first dimension (rows).</param>
    /// <param name="yEnd">The ending index of the fill range in the second dimension (columns).</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="array"/> is null.</exception>
    public static void SafeFill<T>(T[,]? array, T value, int xStart, int yStart, int xEnd, int yEnd) {
        if (array is null)
            throw new ArgumentNullException(nameof(array));

        xStart = xStart.Clamp(0, array.Len0() - 1);
        xEnd = xEnd.Clamp(0, array.Len0() - 1);
        yStart = yStart.Clamp(0, array.Len1() - 1);
        yEnd = yEnd.Clamp(0, array.Len1() - 1);

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

    /// <summary>
    /// Creates and returns a new empty two-dimensional array of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <returns>A new two-dimensional array with zero rows and zero columns.</returns>
    public static T[,] Empty<T>() => new T[0, 0];
}