# NeoKolors.Tui

[![Build & Test](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/ci.yml?branch=main&style=for-the-badge&label=Build%20%26%20Test&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/ci.yml) [![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui) [![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui) [![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)

`NeoKolors.Tui` is a state-driven, component-oriented Text User Interface (TUI) framework for .NET. It provides double-buffered canvas rendering, flexible layout panels, responsive borders, interactive mouse support, and modular application and game lifecycle engines.

---

## Modular Architecture

The TUI ecosystem is divided into focused modules:

- **[NeoKolors.Tui.Core](Core/)**: Double-buffered `NKCharCanvas` and `NKCharScreen`, diff-based ANSI flusher, custom border styling (`NKBorderStyle`), cell layering (`ZLayers`), profiler, and spatial hit-maps (`HitMap`).
- **[NeoKolors.Tui.Elements](Elements/)**: Component tree featuring `Flex`, `Grid`, `Table`, `Border`, `Button`, `List`, `Text`, and mouse event routing (`ElementsMouseDispatcher`).
- **[NeoKolors.Tui.Lifecycle.Core](Lifecycle/Core/)**: Engine foundation with `ILifecycle`, `LifecycleController`, `FrameClock`, `FixedTimestepAccumulator`, `DeltaTime`, and `SceneStack`.
- **[NeoKolors.Tui.Lifecycle.Apps](Lifecycle/Apps/)**: Application hosting infrastructure with `AppLifecycleHost`, `NKAppView`, activation state tracking, and throttled/lazy rendering loops.
- **[NeoKolors.Tui.Lifecycle.Game2D](Lifecycle/Game2D/)**: 2D arcade terminal game loop, 2D camera viewport, and multi-stage rendering pipelines.
- **[NeoKolors.Tui.Lifecycle.Game3D](Lifecycle/Game3D/)**: 3D terminal graphics lifecycle abstractions and architecture plans.
- **[NeoKolors.Tui.Fonts](Fonts/)**: Dynamic typography engine supporting XML V3 definitions, MessagePack binary `.nkf` compilation, Figlet fonts, and font assets.
- **[NeoKolors.Tui.Widgets](Widgets/)**: Standalone console widget controls.

---

## Core Features

- **Zero-Flicker Diff Rendering**:
  `NKCharScreen` maintains front and back character buffers. When `Render()` executes, only changed character cells, foreground/background colors, and style attributes are converted to ANSI sequences, drastically reducing terminal I/O latency and eliminating screen flicker.
- **Declarative Layout Trees**:
  Compose views using `Flex` (row/column orientation, justify content, align items, flex wrapping, gap spacing) and `Grid` (multi-row and multi-column tabular arrangements with Auto and Star sizing).
- **Interactive Mouse Dispatching**:
  Components register visual interaction bounds into a spatial `HitMap`. Mouse clicks, movements, and scroll events are translated into clean component-level callbacks via `ElementsMouseDispatcher`.
- **Decoupled Execution Lifecycles**:
  Use `ThrottledAppLoopStrategy` to throttle active rendering (e.g. 60 FPS) and background rendering (e.g. 10 FPS) when OS terminal window focus is lost. Wake up the render loop immediately on state changes using `SignalWakeup()`.

---

## Getting Started

Building an interactive TUI application:

```csharp
using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Lifecycle.Apps;

Console.OutputEncoding = Encoding.UTF8;

// 1. Setup screen, hit map, and lifecycle host
var screen = new NKCharScreen(NKConsole.WindowSize);
var hitMap = new HitMap();
var mouseDispatcher = new ElementsMouseDispatcher();
var host = new AppLifecycleHost();
var loopStrategy = new ThrottledAppLoopStrategy { ActiveFps = 60 };

int counter = 0;

// 2. Define the UI builder
IElement BuildUi() {
    var container = new Flex {
        Direction = FlexDirection.Column,
        Justify = FlexJustify.Center,
        Align = FlexAlign.Center,
        Width = screen.Size.X,
        Height = screen.Size.Y
    };

    var btn = new Button {
        Text = $"  Clicked {counter} Times  ",
        NormalStyle = new NKStyle(NKColor.FromRgb(255, 255, 255), NKColor.FromRgb(50, 70, 180), TextStyles.Bold),
        HoverStyle = new NKStyle(NKColor.FromRgb(255, 255, 255), NKColor.FromRgb(70, 110, 230), TextStyles.Bold)
    };

    btn.OnClick += () => {
        counter++;
        view.RootElement = BuildUi();
        view.Invalidate();
        loopStrategy.SignalWakeup();
    };

    var box = new Border(btn) {
        Title = " Counter Dashboard ",
        Style = NKBorderStyle.GetRounded(NKColor.FromRgb(0, 220, 180))
    };

    container.Add(box);
    return container;
}

// 3. Create view presenter
var view = new NKAppView(BuildUi(), screen, hitMap);

// Wire inputs
NKConsole.Mouse += m => {
    mouseDispatcher.Dispatch(hitMap, m);
    view.Invalidate();
    loopStrategy.SignalWakeup();
};

NKConsole.Key += k => {
    if (k.Down && (k.Key == KeyCode.ESCAPE || k.Char == 'q'))
        host.RequestClose();
};

// 4. Run the application
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

---

## Documentation

For full guides, architecture diagrams, and tutorials, visit the official documentation:
👉 **[https://krykomdev.github.io/NeoKolors/](https://krykomdev.github.io/NeoKolors/)**
