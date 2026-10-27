# NeoKolors TUI Overview

The `NeoKolors.Tui` package is a state-driven, component-oriented Text User Interface (TUI) framework for .NET. It is designed to build desktop-grade, interactive terminal applications and 2D games with double-buffered rendering, flexbox layouts, spatial mouse hit-testing, and modular lifecycle management.

---

## 1. Modular Architecture

The framework is composed of specialized modules:

- **`NeoKolors.Tui.Core`**: Double-buffered `NKCharCanvas` and `NKCharScreen`, diff-based ANSI flusher, custom border styling (`NKBorderStyle`), cell layering (`ZLayers`), profiler, and spatial hit-maps (`HitMap`).
- **`NeoKolors.Tui.Elements`**: Declarative UI components including `Flex`, `Grid`, `Table`, `Border`, `Button`, `List`, `Text`, and mouse event dispatching (`ElementsMouseDispatcher`).
- **`NeoKolors.Tui.Lifecycle.Core`**: State-driven lifecycle engine (`ILifecycle`, `LifecycleController`), high-precision frame timing (`FrameClock`), fixed-timestep physics accumulator (`FixedTimestepAccumulator`), and scene management (`SceneStack`).
- **`NeoKolors.Tui.Lifecycle.Apps`**: Application hosting infrastructure (`AppLifecycleHost`, `NKAppView`) with throttled and lazy execution strategies.
- **`NeoKolors.Tui.Lifecycle.Game2D`**: Dedicated 2D game loop, 2D camera viewport, and multi-stage rendering pipelines.
- **`NeoKolors.Tui.Fonts`**: Dynamic terminal typography engine supporting XML V3 definitions and MessagePack binary `.nkf` assets.

---

## 2. Key Concepts

### 2.1 The Component Tree

UI screens are assembled from modular components implementing `IElement`:
- **Layout Panels**: `Flex` (row/column direction, justify content, alignment, gap, wrapping), `Grid` (multi-row and multi-column tabular layout with Star and Auto sizing).
- **Framing & Presentation**: `Border` (customizable outlines, rounded corners, padding, titles), `Table` (formatted column grids), `List` (bullet/numbered lists).
- **Interactive Controls**: `Button` (click handling, hover and pressed styling).
- **Text & Visuals**: `Text` (plain and ANSI strings, word wrapping, alignment).

### 2.2 Layout & Double-Buffered Rendering

Rendering operates in coordinated phases:
1. **Layout & Measure**: Containers compute child dimensions and arrange coordinates based on available viewport boundaries.
2. **Buffer Drawing**: Elements draw character cells and styles onto `NKCharCanvas` or `NKCharScreen`.
3. **Diff Flush**: `NKCharScreen` compares the back buffer against the front buffer, flushing only modified cells as optimized ANSI sequences to eliminate screen flicker.

### 2.3 Interactive Mouse Dispatching

1. When elements are rendered, interactive boundaries are recorded in a `HitMap`.
2. When mouse inputs are received from `NKConsole`, `ElementsMouseDispatcher` resolves the hit target and invokes component callbacks (`OnClick`, `OnHover`, `OnScroll`).

---

## 3. Application Lifecycle Bootstrapping

Interactive applications are hosted using `AppLifecycleHost`, `NKAppView`, and `ThrottledAppLoopStrategy`:

```csharp
using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Lifecycle.Apps;

Console.OutputEncoding = Encoding.UTF8;

// 1. Initialize screen, hit map, and lifecycle host
var screen = new NKCharScreen(NKConsole.WindowSize);
var hitMap = new HitMap();
var mouseDispatcher = new ElementsMouseDispatcher();
var host = new AppLifecycleHost();
var loopStrategy = new ThrottledAppLoopStrategy { ActiveFps = 60 };

// 2. Build the visual UI tree
IElement BuildUi() {
    var flex = new Flex {
        Direction = FlexDirection.Column,
        Justify = FlexJustify.Center,
        Align = FlexAlign.Center,
        Width = screen.Size.X,
        Height = screen.Size.Y
    };

    var btn = new Button {
        Text = " [ Click Me ] ",
        NormalStyle = new NKStyle(NKColor.FromRgb(255, 255, 255), NKColor.FromRgb(40, 80, 160)),
        HoverStyle = new NKStyle(NKColor.FromRgb(255, 255, 255), NKColor.FromRgb(60, 120, 240))
    };

    btn.OnClick += () => {
        btn.Text = " [ Clicked! ] ";
        view.Invalidate();
        loopStrategy.SignalWakeup();
    };

    flex.Add(new Border(btn) {
        Title = " My TUI App ",
        Style = NKBorderStyle.GetRounded(NKColor.FromRgb(0, 220, 180))
    });

    return flex;
}

// 3. Connect view presenter
var view = new NKAppView(BuildUi(), screen, hitMap);

// 4. Wire mouse and keyboard inputs
NKConsole.Mouse += m => {
    mouseDispatcher.Dispatch(hitMap, m);
    view.Invalidate();
    loopStrategy.SignalWakeup();
};

NKConsole.Key += k => {
    if (k.Down && (k.Key == KeyCode.ESCAPE || k.Char == 'q'))
        host.RequestClose();
};

// 5. Execute application loop
try {
    NKConsole.HideCursor();
    NKConsole.EnableAltBuffer();
    host.Start();

    while (host.State != LifecycleState.STOPPED) {
        loopStrategy.Tick(host, view);
    }
} finally {
    NKConsole.DisableAltBuffer();
    NKConsole.ShowCursor();
}
```
