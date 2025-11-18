//
// NeoKolors
// Copyright (c) 2025 KryKom
//

/*
using System.Collections;
using System.Diagnostics;
#pragma warning disable CS0618 // Type or member is obsolete

namespace NeoKolors.Common.Util;

[Obsolete("Use the Metriks library classes instead.")]
public class List2D<T> : List2D, IEnumerable<T> {
    
    private readonly List<List<T>> _matrix; // TODO: optimize this
    
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
    public void RemoveY(int y) {
        if (y < 0 || y >= YSize) throw new IndexOutOfRangeException();
        for (int x = 0; x < XSize; x++)
            _matrix[x].RemoveAt(y);
    }
    
    /// <inheritdoc cref="List2D.RemoveRow()"/>
    public void RemoveY() {
        int y = YSize - 1;
        for (int x = 0; x < XSize; x++) 
            _matrix[x].RemoveAt(y);
    }

    /// <inheritdoc cref="List2D.RemoveCol(int)"/>
    public void RemoveX(int x) {
        if (x < 0 || x >= XSize) throw new IndexOutOfRangeException();
        _matrix.RemoveAt(x);
    }

    /// <inheritdoc cref="List2D.RemoveCol()"/>   
    public void RemoveX() {
        if (XSize != 0)
            _matrix.RemoveAt(XSize - 1);
    }

    /// <summary>
    /// Determines whether all elements in a specific row of the 2D list satisfy a specified condition.
    /// </summary>
    /// <param name="y">The zero-based index of the row to evaluate.</param>
    /// <param name="predicate">The condition to test each element in the row.</param>
    /// <returns>True if all elements in the specified row satisfy the condition; otherwise, false.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown if the specified index is out of range.</exception>
    public bool AllAtY(int y, Predicate<T> predicate) {
        if (y < 0 || y >= YSize) throw new IndexOutOfRangeException();

        for (int x = 0; x < XSize; x++) {
            if (predicate(this[x, y])) continue;
            
            return false;
        }
        
        return true;
    }

    /// <summary>
    /// Determines if all elements in a specified column satisfy the provided predicate.
    /// </summary>
    /// <param name="x">The zero-based index of the column to evaluate.</param>
    /// <param name="predicate">The function used to test each element in the column.</param>
    /// <returns>True if all elements in the column satisfy the predicate; otherwise, false.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified column index is outside the valid range.
    /// </exception>
    public bool AllAtX(int x, Predicate<T> predicate) {
        if (x < 0 || x >= XSize) throw new IndexOutOfRangeException();

        for (int y = 0; y < YSize; y++) {
            if (predicate(this[x, y])) continue;
            
            return false;
        }        
        
        return true;
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
}*/