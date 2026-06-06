# NeoKolors.Tui.Core

[![Build Status](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/KryKomDev/9fb6af0ba84bfb5106a78ff35bd8be3c/raw/build-Tui.Core.json&style=for-the-badge&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/build.yml) [![Test Status](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/KryKomDev/9fb6af0ba84bfb5106a78ff35bd8be3c/raw/test-Tui.Core.json&style=for-the-badge&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/build.yml) [![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui.Core?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Core) [![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui.Core?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui.Core) [![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)

`NeoKolors.Tui.Core` provides the core double-buffered visual canvas, geometry abstractions, border styles, and screen rendering drivers that power the NeoKolors TUI framework. It manages the low-level visual grid of character cells, tracking styling state changes and outputting compiled ANSI update buffers to the console.

---

## Core Features

- **Double-Buffered Grid Canvas**:
  - [ICharCanvas](./ICharCanvas.cs) & [NKCharCanvas](./NKCharCanvas.cs): Represent a mutable 2D grid containing character cells, styling metadata, and Z-Index depth layers. Can dynamically resize, clear, or lay out nested canvases.
- **Console Render Screen**:
  - [ICharScreen](./ICharScreen.cs) & [NKCharScreen](./NKCharScreen.cs): Manage the primary terminal screen state, writing diffed grid segments into a single compiled ANSI escape stream to minimize redraw flicker.
- **Customizable Borders**:
  - [BorderStyle](./BorderStyle.cs): Models border configurations (top, bottom, left, right sides, and corners) with custom style attributes. Includes built-in parsers and predefined styles (such as `Ascii`, `Normal`, `Rounded`, `Thick`, `Double`, `Inset`, and `Outset`).
- **Canvas Drawing Helpers**:
  - [CharCanvasExtensions](./CharCanvasExtensions.cs): Extends the base canvas interface to support drawing borders, rectangles, filling background areas, and styling specific regions.
- **Sixel Image Support**:
  - Directly place pixel images ([ISixelImageInfo](./ISixelImageInfo.cs)) onto canvas layer offsets, translating pixel maps into console coordinates.

---

## Core Types and Abstractions

### 1. ICharCanvas
The interface defining operations for visual grid modification:

- **Width / Height / Size**: Dimensions of the current canvas.
- **this[x, y]**: Retrieves or assigns a [CellInfo](./CellInfo.cs) object containing character and style details.
- **Place()**: Places sub-canvases, character grids, styled strings, or raw character codes at specific offsets and depth layers.
- **PlaceSixel()**: Allocates an image buffer to be drawn on terminal screens.

### 2. BorderStyle
Predefined styles for box borders:

- `GetAscii()`: Prints borders using standard ASCII characters (`+`, `-`, `|`).
- `GetRounded()`: Uses box-drawing characters featuring rounded corners (`╭`, `╮`, `╰`, `╯`).
- `GetThick()` / `GetDouble()`: Pre-configured thick or double-line box margins.
- `GetInset()` / `GetOutset()`: Utilizes shadows and highlights to simulate 3D inset/outset framing.

---

## Usage Examples

### 1. Drawing Styled Rectangles
Using drawing extensions to draw borders and fill backgrounds:

```csharp
using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

// Create a visual canvas
var canvas = new NKCharCanvas(80, 24);

// Define a region and border style
var region = new Rectangle(new Point2D(5, 2), new Point2D(25, 8));
var border = BorderStyle.GetRounded(NKColor.FromRgb(0, 255, 128));

// Draw the rounded border
canvas.PlaceRectangle(region, border);

// Apply a background color to the inner area
var innerRegion = new Rectangle(new Point2D(6, 3), new Point2D(24, 7));
canvas.StyleBackground(innerRegion, NKColor.FromRgb(32, 32, 32));
```

### 2. Rendering the Screen Buffer
Setting up a screen and executing render cycles:

```csharp
using Metriks;
using NeoKolors.Tui.Core;

var screenSize = new Size2D(120, 40);
var screen = new NKCharScreen(screenSize);

// Draw static text
screen.Place("NeoKolors TUI Core Engine", new Point2D(2, 1));

// Render changes to the console stream
screen.Render();
```
