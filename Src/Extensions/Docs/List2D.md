# List2D Grid Utility

`List2D` and `List2D<T>` (provided by the **Metriks** library dependency) represent structured two-dimensional lists. These structures provide dynamic sizing capabilities and utility helpers to manage cells, coordinates, and viewports on the TUI canvas grid.

---

## 1. Class Definition

```csharp
public class List2D<T> : List2D, IEnumerable<T>
```

---

## 2. Properties

* **`Count`**: Returns the total number of element cells (calculated as `XSize * YSize`).
* **`XSize`**: The horizontal column width.
* **`YSize`**: The vertical row height.
* **`this[int x, int y]`**: Gets or sets the element value at the specified column and row coordinate.

---

## 3. Dynamic Operations

Unlike standard multidimensional arrays (`T[,]`), `List2D` allows adding and removing rows/columns at runtime:

### Adding Rows and Columns
- **`AddRow(List<T> row)`** / **`AddRow(T[] row)`**: Appends a new horizontal row of elements.
- **`AddCol(List<T> col)`** / **`AddCol(T[] col)`**: Appends a new vertical column of elements.

### Removing Rows and Columns
- **`RemoveRow(int index)`** / **`RemoveRow()`**: Removes a row at the specified index, or the last row if omitted.
- **`RemoveCol(int index)`** / **`RemoveCol()`**: Removes a column at the specified index, or the last column if omitted.

---

## 4. Helper Static Methods

The non-generic `List2D` class provides static array manipulation methods:

### Array Resizing
```csharp
// Resizes a 2D array while preserving existing content where possible
List2D.Resize(ref myGridArray, newWidth, newHeight);
```

### Array Filling
- **`Fill<T>(T[,] array, T value)`**: Fills all cells in the grid with a specified value.
- **`SafeFill<T>(T[,] array, T value, int xStart, int yStart, int xEnd, int yEnd)`**: Safely fills a bounding region of cells, automatically clamping index coordinates to fit within the array bounds to prevent out-of-range exceptions.

---

## 5. Usage Example

```csharp
using Metriks;

// 1. Create a two-dimensional grid of integers
var grid = new List2D<int>();

// 2. Add elements
grid.AddRow(new[] { 1, 2, 3 });
grid.AddRow(new[] { 4, 5, 6 });

// 3. Access coordinates
int val = grid[1, 0]; // Returns 2

// 4. Convert directly to standard array
int[,] rawArray = grid.ToArray();
```
