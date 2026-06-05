# Core Canvas and Screen Rendering

The `NeoKolors.Tui.Core` project provides the double-buffered rendering engine that enables flicker-free updates on terminal screens. It manages off-screen cell buffers and translates differences into optimized ANSI streams.

---

## 1. Double-Buffered Rendering

To prevent screen flickering caused by full terminal redrawing, NeoKolors uses a double-buffered grid system:

```
[Off-screen Canvas] ---> (Diff Engine / NKCharScreen) ---> [Terminal Screen Console]
(NKCharCanvas)                                              (Draws only modified cells)
```

1. **Off-Screen Canvas (`NKCharCanvas`)**: A 2D grid containing character cells (`CellInfo`) where UI elements are drawn.
2. **Screen Representation (`NKCharScreen`)**: Manages the visible terminal grid. It compares its current buffer with the updated off-screen canvas, determines which cells have changed, and writes only those updates to the stdout stream.

---

## 2. Core Abstractions

### 2.1 Character Cell (`CellInfo`)
Every position in the grid is represented by a **[CellInfo](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Core/CellInfo.cs)** struct containing:
* `Char`: The actual character (unicode code point) to render.
* `Foreground`: Custom foreground color (`NKColor`).
* `Background`: Custom background color (`NKColor`).
* `Style`: Styled indicators (e.g. Bold, Italic, Underline) represented as flags.

### 2.2 Character Canvas (`ICharCanvas`)
**[ICharCanvas](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Core/ICharCanvas.cs)** represents a mutable grid of cells. It provides:
* Coordinate-based writing (`this[x, y]`).
* Sub-canvas placement and absolute layering offset operations.
* Sixel image allocation boundaries.

### 2.3 Canvas Extensions (`CharCanvasExtensions`)
Provides higher-level drawing functions:
* Drawing borders (rounded, thin, thick, double, inset/outset shadows).
* Drawing filled or hollow rectangles.
* Applying styles/background colors selectively to grid regions.

---

## 3. Basic Canvas Drawing Example

Below is an example showing how to build an off-screen canvas, draw a bordered box, and render it using the screen engine:

```csharp
using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

// 1. Initialize a 80x25 canvas
var canvas = new NKCharCanvas(80, 25);

// 2. Define drawing bounds
var region = new Rectangle(new Point2D(5, 5), new Point2D(40, 15));

// 3. Define border style
var border = BorderStyle.GetRounded(NKColor.FromRgb(0, 255, 0));

// 4. Draw border & apply background style
canvas.PlaceRectangle(region, border);
canvas.StyleBackground(
    new Rectangle(new Point2D(6, 6), new Point2D(39, 14)), 
    NKColor.FromRgb(20, 20, 20)
);

// 5. Render canvas onto the primary screen
var screen = new NKCharScreen(new Size2D(80, 25));
screen.Place(canvas, Point2D.Zero);
screen.Render(); // Outputs ANSI diffs to stdout
```
