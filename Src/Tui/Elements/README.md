# NeoKolors.Tui.Elements

[![Build & Test](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/ci.yml?branch=main&style=for-the-badge&label=Build%20%26%20Test&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/ci.yml) [![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui.Elements?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Elements) [![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui.Elements?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui.Elements) [![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)

`NeoKolors.Tui.Elements` provides the declarative, component-driven UI element tree for the NeoKolors TUI framework. It includes flexible layout panels, bordered frames, interactive buttons, tabular data grids, lists, styled text blocks, and mouse event dispatching.

---

## Component Suite

| Component | Class | Description |
| :--- | :--- | :--- |
| **`Flex`** | [`Flex` | Web-inspired flexbox layout container supporting row/column directions, justify content, cross-axis alignment, wrapping, and item gap spacing. |
| **`Grid`** | [`Grid` | 2D tabular layout supporting row/column definitions, column/row spanning, and proportional (star), auto, and fixed sizing. |
| **`Border`** | [`Border` | Framing container providing stylized box outlines (`NKBorderStyle`), padding, margin, title labels, and background colors. |
| **`Button`** | [`Button` | Interactive command button supporting hover and pressed visual states, custom child content, and click event delegates. |
| **`Table`** | [`Table` | Rich tabular data visualizer with column definitions, cell formatting, headers, and borders. |
| **`List`** | [`List` | Structured list view supporting bullet/numbered list point generators and selectable items. |
| **`Text`** | [`Text` | High-performance text element supporting plain and ANSI strings, word wrapping, alignment, and trimming. |
| **`MouseRegion`** | [`MouseRegion` | Spatial interaction boundary registered in a `HitMap` to intercept mouse clicks, movements, and scroll events. |
| **`ElementsMouseDispatcher`** | [`ElementsMouseDispatcher` | Dispatches incoming terminal mouse events to the corresponding registered element callbacks. |

---

## Core Usage

### 1. Flexbox Layout (`Flex`)

`Flex` arranges children linearly along a main axis while aligning them across a cross axis:

```csharp
using NeoKolors.Tui.Elements;

var flex = new Flex {
    Direction = FlexDirection.Row,       // Row or Column
    Justify = FlexJustify.SpaceBetween,  // Start, Center, End, SpaceBetween, SpaceAround
    Align = FlexAlign.Center,            // Start, Center, End, Stretch
    Wrap = FlexWrap.NoWrap,
    Gap = 2                              // Space between items in character cells
};

flex.Add(new Text("Left Item"));
flex.Add(new Text("Right Item"));
```

---

### 2. Framed Containers (`Border`)

`Border` wraps any child `IElement` with a stylized outline:

```csharp
using NeoKolors.Common;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;

var innerContent = new Text("System Status: Online");

var box = new Border(innerContent) {
    Title = " Diagnostics ",
    Padding = new Spacing(2, 1),
    Style = NKBorderStyle.GetRounded(NKColor.FromRgb(0, 220, 180))
};
```

---

### 3. Interactive Buttons (`Button`)

`Button` triggers click callbacks and transitions visual styles when the mouse cursor hovers over it:

```csharp
using NeoKolors.Common;
using NeoKolors.Tui.Elements;

var btn = new Button {
    Text = " [ Submit Order ] ",
    NormalStyle = new NKStyle(NKColor.FromRgb(255, 255, 255), NKColor.FromRgb(40, 80, 160), TextStyles.Bold),
    HoverStyle = new NKStyle(NKColor.FromRgb(255, 255, 255), NKColor.FromRgb(60, 120, 240), TextStyles.Bold)
};

btn.OnClick += () => {
    NKConsole.WriteLine("Button clicked!");
};
```

---

### 4. Tabular Data Layout (`Table` and `Grid`)

```csharp
using NeoKolors.Tui.Elements;

var table = new Table();
table.AddColumn("ID", width: 6);
table.AddColumn("Process Name", width: 20);
table.AddColumn("CPU %", width: 10);

table.AddRow("1042", "dotnet", "1.4%");
table.AddRow("2180", "terminal", "0.2%");
```

---

### 5. Mouse Dispatching (`ElementsMouseDispatcher`)

When rendering an element tree using `NKAppView`, elements automatically register their screen coordinates in the view's `HitMap`. Use `ElementsMouseDispatcher` to forward mouse events from `NKConsole`:

```csharp
var hitMap = new HitMap();
var mouseDispatcher = new ElementsMouseDispatcher();

// In your input loop or event handler:
NKConsole.Mouse += mouseEvent => {
    mouseDispatcher.Dispatch(hitMap, mouseEvent);
    view.Invalidate();
};
```
