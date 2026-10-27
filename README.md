<div align="center">

<h1>NeoKolors</h1>

<p><strong>A modern, high-performance C# console graphics, diagnostics, typography, and Text User Interface (TUI) ecosystem.</strong></p>

[![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)
[![Latest Release](https://img.shields.io/github/v/release/KryKomDev/NeoKolors?include_prereleases&sort=semver&style=for-the-badge&label=Latest&labelColor=2a313c&color=e051c6)](https://github.com/KryKomDev/NeoKolors/releases)
[![Build & Test](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/ci.yml?branch=main&style=for-the-badge&label=Build%20%26%20Test&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/ci.yml)
[![Documentation](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/docs.yml?branch=main&style=for-the-badge&label=Docs&labelColor=%232a313c)](https://krykomdev.github.io/NeoKolors/)

</div>

---

## Overview

**NeoKolors** is a modular .NET library ecosystem designed for crafting beautiful, responsive, and high-performance terminal user interfaces, console graphics, and CLI tools. Built from the ground up to support modern .NET (targeting .NET Standard 2.1, .NET 8, .NET 10, and preview .NET 11), NeoKolors delivers zero-allocation patterns, native low-level terminal I/O drivers, double-buffered rendering, flexbox layouts, scalable terminal typography, and state-driven game and application lifecycles.

---

## Ecosystem Architecture

The NeoKolors ecosystem is organized into specialized libraries:

| Package | Description | NuGet Latest Release |
| :--- | :--- | :--- |
| **`NeoKolors.Common`** | Core color models (`NKColor`, RGB TrueColor, 256 colors), bit-packed styles (`NKStyle`), Sixel raster graphics, and ANSI strings. | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Common?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Common) |
| **`NeoKolors.Console`** | Low-level cross-platform terminal driver (Linux native `epoll`/`termios`, Windows API, .NET fallback), structured logging (`NKLogger`), and stack trace formatter (`ExceptionFormatter`). | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Console?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Console) |
| **`NeoKolors.Extensions`** | High-performance utilities: word chopping (`Chop`), plain text length calculation, 2D array grid helpers (`List2D`), and naming case converters. | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Extensions?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Extensions) |
| **`NeoKolors.Settings`** | Type-safe settings builder and parser engine with nested node trees, argument validation, and option merging. | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Settings?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Settings) |
| **`NeoKolors.Tui.Core`** | Double-buffered character canvas (`NKCharCanvas`), diff-based screen presenter (`NKCharScreen`), border styles, profiler, and spatial hit-maps (`HitMap`). | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Tui.Core?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Core) |
| **`NeoKolors.Tui.Elements`** | Declarative UI component tree: `Flex`, `Grid`, `Table`, `Border`, `Button`, `List`, `Text`, and mouse event dispatching. | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Tui.Elements?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Elements) |
| **`NeoKolors.Tui.Lifecycle.Core`** | Foundation lifecycle controller (`ILifecycle`, `LifecycleController`), high-precision frame clock (`FrameClock`), fixed-timestep accumulator, and scene stack. | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Tui.Lifecycle.Core?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Lifecycle.Core) |
| **`NeoKolors.Tui.Lifecycle.Apps`** | Interactive application host (`AppLifecycleHost`), view presenter (`NKAppView`), focus tracking, and throttled/lazy rendering loop strategies. | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Tui.Lifecycle.Apps?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Lifecycle.Apps) |
| **`NeoKolors.Tui.Lifecycle.Game2D`** | 2D terminal game loop engine (`Game2DLoop`), 2D camera viewport (`Camera2D`), and multi-stage rendering pipeline (`Render2DPipeline`). | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Tui.Lifecycle.Game2D?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Lifecycle.Game2D) |
| **`NeoKolors.Tui.Lifecycle.Game3D`** | 3D terminal graphics lifecycle and viewport architecture abstractions. | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Tui.Lifecycle.Game3D?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Lifecycle.Game3D) |
| **`NeoKolors.Tui.Fonts`** | Terminal typography engine supporting XML V3 definitions, MessagePack binary `.nkf` compilation, Figlet fonts, and dynamic kerning. | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Tui.Fonts?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts) |
| **`NeoKolors.Tui.Fonts.Assets`** | Pre-compiled built-in font assets (such as `Bytesized` and `Future`) embedded as binary resources. | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Tui.Fonts.Assets?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts.Assets) |
| **`NeoKolors.Tools.NKFont`** | Global CLI tool (`nkfont`) to inspect, create, compile, and preview custom terminal fonts. | [![NuGet](https://img.shields.io/nuget/v/NeoKolors.Tools.NKFont?style=flat-square&color=e051c6)](https://www.nuget.org/packages/NeoKolors.Tools.NKFont) |

---

## Key Features

- **Blazing Fast Output Drivers**:
  - Native Linux terminal driver using direct POSIX `termios`, `epoll` asynchronous notifications, non-blocking I/O, and SIGWINCH signals.
  - Native Windows Console driver utilizing Win32 console input buffers (`ReadConsoleInputW`) and Virtual Terminal sequences.
  - SGR (1006) protocol for high-resolution terminal mouse reporting (click, move, drag, scroll wheel).
- **Memory-Efficient Primitives**:
  - `NKColor`: Compact 32-bit representation supporting RGB 24-bit TrueColor, standard 16 console colors, 256-color palette, and inherit styling.
  - `NKStyle`: Compresses foreground, background, and text styling flags (Bold, Underline, Italic, Strikethrough, Invert, Faint) into a 128-bit explicit layout struct.
  - `NKCharScreen`: Double-buffered screen rendering that computes character and style diffs between frames to output minimal ANSI sequences and eliminate screen flicker.
- **Modern Flexbox & Grid UI Tree**:
  - Declarative layout components: `Flex` with flex direction, justification, cross-axis alignment, and wrap; `Grid` with auto and star sizing; `Border` with multiple built-in border styles (Rounded, Double, Thick, Inset); `Table` with column formatting; `Button` with hover/pressed states; `Text` with word-wrapping.
  - Built-in `HitMap` and `ElementsMouseDispatcher` for accurate mouse event routing directly to UI components.
- **Decoupled Application & Game Loops**:
  - Fixed-timestep accumulator for deterministic physics or game logic (e.g. 60 ticks/sec), completely decoupled from variable frame render rates.
  - Throttled loop strategy with active and background FPS limits (reducing CPU usage when window is inactive) and thread-safe wake-up triggers.
  - Lazy rendering loop strategy for purely event-driven, zero-idle-CPU console applications.
- **Enterprise Diagnostics & Logging**:
  - Asynchronous structured logging (`NKLogger`, `NKDebug`) with binary (`.blog`) and formatted text outputs.
  - Beautiful unhandled exception rendering (`ExceptionFormatter`) with frame highlighting and clean terminal layout.

---

## Installation

Add packages to your project using the .NET CLI:

```bash
# Add common styling and console driver
dotnet add package NeoKolors.Common
dotnet add package NeoKolors.Console

# Add TUI core, elements, and application hosting
dotnet add package NeoKolors.Tui.Core
dotnet add package NeoKolors.Tui.Elements
dotnet add package NeoKolors.Tui.Lifecycle.Apps
```

Or install the `nkfont` CLI tool globally:

```bash
dotnet tool install --global NeoKolors.Tools.NKFont
```

---

## Quickstart Examples

### 1. Styled Terminal Output and Diagnostics

```csharp
using NeoKolors.Common;
using NeoKolors.Console;

// Direct colored and styled output
NKConsole.WriteLine("Hello, NeoKolors!", NKColor.FromRgb(0, 220, 180));

var style = new NKStyle(
    foreground: NKColor.FromRgb(255, 215, 0),
    background: NKColor.FromRgb(30, 30, 40),
    styles: TextStyles.Bold | TextStyles.Underline
);
NKConsole.WriteLine("Important Headline", style);

// Pretty exception formatting
try {
    throw new InvalidOperationException("Failed to load database credentials.");
} catch (Exception ex) {
    ExceptionFormatter.Print(ex);
}
```

---

### 2. Building an Interactive TUI Application

Here is a minimal interactive dashboard application featuring flexbox layout, state signals, and interactive buttons:

```csharp
using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Lifecycle.Apps;

Console.OutputEncoding = Encoding.UTF8;

// 1. Reactive state
var counter = 0;

// 2. Screen, hit map, and lifecycle host
var screen = new NKCharScreen(NKConsole.WindowSize);
var hitMap = new HitMap();
var mouseDispatcher = new ElementsMouseDispatcher();
var host = new AppLifecycleHost();
var loopStrategy = new ThrottledAppLoopStrategy { ActiveFps = 60 };

// 3. UI Builder
IElement BuildUi() {
    var flex = new Flex {
        Direction = FlexDirection.Column,
        Justify = FlexJustify.Center,
        Align = FlexAlign.Center,
        Width = screen.Size.X,
        Height = screen.Size.Y
    };

    var btn = new Button {
        Text = $"  Clicks: {counter} (Click Me)  ",
        NormalStyle = new NKStyle(NKColor.FromRgb(255, 255, 255), NKColor.FromRgb(40, 80, 160), TextStyles.Bold),
        HoverStyle = new NKStyle(NKColor.FromRgb(255, 255, 255), NKColor.FromRgb(60, 120, 240), TextStyles.Bold)
    };

    btn.OnClick += () => {
        counter++;
        view.RootElement = BuildUi();
        view.Invalidate();
        loopStrategy.SignalWakeup();
    };

    var box = new Border(btn) {
        Title = " NeoKolors Interactive App ",
        Style = NKBorderStyle.GetRounded(NKColor.FromRgb(0, 220, 180))
    };

    flex.Add(box);
    return flex;
}

// 4. View Presenter
var view = new NKAppView(BuildUi(), screen, hitMap);

// Wire mouse and keyboard
NKConsole.Mouse += m => {
    mouseDispatcher.Dispatch(hitMap, m);
    view.Invalidate();
    loopStrategy.SignalWakeup();
};

NKConsole.Key += k => {
    if (k.Down && (k.Key == KeyCode.ESCAPE || k.Char == 'q'))
        host.RequestClose();
};

// 5. Run the Application Loop
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

### 3. Creating a 2D Terminal Game

NeoKolors includes a high-performance 2D terminal game loop engine featuring fixed-timestep updates, frame clocks, 2D camera coordinate transforms, and modular rendering pipelines:

```csharp
using NeoKolors.Common;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Lifecycle.Game2D;

var screen = new NKCharScreen(new Size2D(100, 35));
var camera = new Camera2D(new Rect2D(0, 0, 100, 35));
var pipeline = new Render2DPipeline();

// Add custom rendering stages (e.g. background, entities, particles, HUD)
pipeline.AddStage(new MyGameWorldStage());

var gameLoop = new Game2DLoop(screen, camera, pipeline) {
    TargetFps = 60,
    FixedTicksPerSecond = 60
};

gameLoop.OnFixedUpdate += dt => {
    // Deterministic physics / entity logic running at 60 Hz
};

gameLoop.Run();
```

---

## Examples

The repository includes runnable sample projects and applications under the [`Examples/`](Examples/) directory:

- **[ExampleApp.cs](Examples/ExampleApp.cs)**: A complete, reactive TUI desktop dashboard featuring multi-tab navigation, data tables, responsive resizing, interactive buttons, scrolling, and hit-test mouse dispatching.
- **[ExampleGame2D.cs](Examples/ExampleGame2D.cs)**: *Cosmic Invaders*, a full 2D arcade terminal game with particle systems, wave spawners, collision physics, player starship, sound effects, and HUD rendering.
- **[Neofetch](Examples/Neofetch/)**: High-fidelity terminal system information display.
- **[Ascii](Examples/Ascii/)**: ASCII rendering and graphic manipulation utilities.
- **[WeekNumber](Examples/WeekNumber/)**: Calendar and date calculation utility.

To run any example:
```bash
dotnet run Examples/ExampleApp.cs
dotnet run Examples/ExampleGame2D.cs
dotnet run --project Examples/Neofetch/Neofetch.csproj
```

---

## Documentation

Full official documentation, guides, and API reference documentation are available at:
👉 **[https://krykomdev.github.io/NeoKolors/](https://krykomdev.github.io/NeoKolors/)**

---

## License

NeoKolors is open-source software licensed under the [MIT License](LICENSE).
Copyright &copy; 2026 KryKom.
