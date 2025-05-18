+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'List2D'
+++

`List2D` and its generic counterpart `List2D<T>` are classes that implement two-dimensional
list structures, providing a more flexible alternative to standard two-dimensional
arrays in C#. These classes offer dynamic sizing capabilities and convenient methods
for manipulating 2D data structures.

## List2D

``` csharp
public class List2D<T> : List2D, IEnumerable<T>
```

### Properties
- **Count**: Gets the total number of elements in the list (XSize * YSize)
- **XSize**: Gets the number of columns in the list
- **YSize**: Gets the number of rows in the list
- **this[int index]**: Gets the list of elements at the specified column index
- **this[int x, int y]**: Gets or sets the element at the specified coordinates

### Methods

#### Constructors

``` csharp
public List2D()
```

Creates a new empty two-dimensional list.

#### Adding Elements

``` csharp
public void AddRow(List<T> row)
```

Adds a new row to the list. If the list is empty, initializes the structure with the
row's length.

``` csharp
public void AddRow(T[] row)
```

Adds a new row from an array. Similar to the List overload.

``` csharp
public void AddCol(List<T> col)
```

Adds a new column to the list. If the list is empty, initializes the structure with 

the column's length.
``` csharp
public void AddCol(T[] col)
```

Adds a new column from an array. Similar to the List overload.

#### Removing Elements

``` csharp
public void RemoveRow(int index)
```

Removes the row at the specified index.

``` csharp
public void RemoveRow()
```

Removes the last row from the list.

``` csharp
public void RemoveCol(int index)
```

Removes the column at the specified index.

``` csharp
public void RemoveCol()
```

Removes the last column from the list.

#### Conversion Methods

``` csharp
public static List2D<T> FromArray(T[,] array)
```

Creates a new List2D from a two-dimensional array.

``` csharp
public T[,] ToArray()
```

Converts the List2D to a two-dimensional array.

#### Implicit Operators

``` csharp
public static implicit operator T[,](List2D<T> list)
public static implicit operator List2D<T>(T[,] array)
```

Allows implicit conversion between List2D and T[,].

---

## List2D (Non-generic)

### Class Definition

``` csharp
public class List2D : IEnumerable
```

### Properties

Same as [List2D](#properties) but with `object` type.

---

## Static Utility Methods

### Array Resizing

``` csharp
public static void Resize<T>(ref T[,]? array, int newSizeDim0, int newSizeDim1)
```

Resizes a two-dimensional array to the specified dimensions, preserving existing
elements where possible.

### Array Filling

### Fill&lt;T&gt;(T[,]?, T)

``` csharp
public static void Fill<T>(T[,]? array, T value)
```

Fills the entire array with the specified value.

### Fill&lt;T&gt;(T[,]?, T, int, int, int, int)

``` csharp
public static void Fill<T>(T[,]? array, T value, int xStart, int yStart, int xEnd, int yEnd)
```

Fills a specified region of the array with the given value, using end-point coordinates.

### FillCount

``` csharp
public static void FillCount<T>(T[,]? array, T value, int xStart, int yStart, int xCount, int yCount)
```

Fills a specified region of the array with the given value, using count-based dimensions.

### SafeFill

``` csharp
public static void SafeFill<T>(T[,]? array, T value, int xStart, int yStart, int xEnd, int yEnd)
```

Safely fills a region of the array, clamping coordinates to valid bounds.

### Empty

``` csharp
public static T[,] Empty<T>()
```

Creates and returns an empty two-dimensional array.

---

## Usage Examples
``` csharp
// Creating a new List2D<T>
var list = new List2D<int>();

// Adding rows and columns
list.AddRow(new[] { 1, 2, 3 });
list.AddRow(new List<int> { 4, 5, 6 });
list.AddCol(new[] { 7, 8 });

// Accessing elements
int value = list[1, 1]; // Gets element at x=1, y=1

// Converting to array
int[,] array = list.ToArray();

// Using static utility methods
List2D.Fill(array, 0); // Fill array with zeros
List2D.SafeFill(array, 1, 0, 0, 2, 2); // Fill region safely
```

---

## Exceptions
- `List2DException`: Thrown when attempting to add rows or columns with invalid sizes
- `IndexOutOfRangeException`: Thrown when accessing elements outside the list bounds
- `ArgumentNullException`: Thrown when null arrays are passed to utility methods
- `ArgumentOutOfRangeException`: Thrown when invalid dimensions are specified

---

## Performance Considerations
- The structure uses nested lists internally, providing O(1) access time to elements
- Adding/removing rows and columns is O(n) operation where n is the size of the affected dimension
- The generic version `List2D<T>` is more efficient when working with value types
- Utility methods are optimized for performance while maintaining safety

---

## Thread Safety
The `List2D` and `List2D<T>` classes are not thread-safe by default. Synchronization should be 
implemented externally if required for concurrent access.
