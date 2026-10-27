# NeoKolors Documentation

NeoKolors is a modern, high-performance C# library ecosystem providing color utilities, console graphics, native cross-platform terminal drivers, typography engines, and a state-driven Text User Interface (TUI) framework.

---

## Projects in the Ecosystem

### Core Foundation

- **NeoKolors.Common**: Common structures and classes for working with colors, ANSI escape sequences, Sixel graphics, and color palettes. Key types include `NKColor` (32-bit compact RGB TrueColor, 256 colors, inherit values) and `NKStyle` (128-bit explicit layout struct packing foreground, background, and text formatting). It also handles immutable styled `AnsiString` structures and high-performance raster-to-Sixel conversion for rendering images directly in compliant terminals.
- **NeoKolors.Console**: Cross-platform terminal driver, low-level virtual terminal controls, asynchronous mouse and keyboard input capture, structured logging, and unhandled exception formatting. Features native Linux I/O drivers (`epoll`/`termios`), native Windows Console API drivers, and managed .NET fallback drivers.
- **NeoKolors.Extensions**: High-performance extension methods for string manipulation, word wrapping (`Chop`), ANSI printable length measurement, 2D array grid utilities (`List2D`), and case conversion across Camel, Pascal, Snake, Screaming Snake, Kebab, and Train cases.
- **NeoKolors.Settings**: Type-safe settings building and parsing framework. Uses a tree-based structure composed of `SettingsNode`, `SettingsGroup`, and typed `IArgument` variants to define configurations with input validation and auto-merging.

### TUI & Typography

- **NeoKolors.Tui.Core**: Double-buffered character canvas (`NKCharCanvas`), diff-based screen presenter (`NKCharScreen`), customizable border styles (`NKBorderStyle`), cell layering (`ZLayers`), profiler, and spatial hit-maps (`HitMap`).
- **NeoKolors.Tui.Elements**: Component tree featuring declarative flexbox layouts (`Flex`), tabular grids (`Grid`), borders (`Border`), interactive buttons (`Button`), tables (`Table`), lists (`List`), styled text (`Text`), and mouse event routing (`ElementsMouseDispatcher`).
- **NeoKolors.Tui.Fonts**: Typography and text rendering engine supporting XML V3 definitions, MessagePack binary `.nkf` compilation, Figlet fonts, and dynamic kerning.
- **NeoKolors.Tui.Fonts.Assets**: Pre-compiled built-in font assets (such as `Bytesized` and `Future`) embedded as binary resources.
- **NeoKolors.Tui.Widgets**: Standalone interactive widgets (progress bars, spinners, prompts) for standard console streams without full-screen TUI apps.
- **NeoKolors.Tui**: Metapackage combining TUI core, elements, and styling.

### Application & Game Lifecycles

- **NeoKolors.Tui.Lifecycle.Core**: Engine state machine (`ILifecycle`, `LifecycleController`), high-precision frame timing (`FrameClock`), fixed-timestep physics accumulator (`FixedTimestepAccumulator`), and scene stack (`SceneStack`).
- **NeoKolors.Tui.Lifecycle.Apps**: Application hosting infrastructure (`AppLifecycleHost`), view presenter (`NKAppView`), focus tracking, and execution loop strategies (`ThrottledAppLoopStrategy`, `LazyAppLoopStrategy`).
- **NeoKolors.Tui.Lifecycle.Game2D**: Dedicated 2D terminal game loop engine (`Game2DLoop`), 2D camera viewport (`Camera2D`), and multi-stage rendering pipeline (`Render2DPipeline`).
- **NeoKolors.Tui.Lifecycle.Game3D**: 3D terminal graphics lifecycle and viewport architecture abstractions.

### Tools & CLI

- **NeoKolors.Tools.NKFont**: Global .NET CLI tool (`nkfont`) to inspect, create, compile, and preview custom terminal fonts.

---

## Repository and Package Availability

### GitHub Repository

The NeoKolors project is open-source and hosted on GitHub. The repository contains the source code, examples, unit tests, and the issue tracker for the library ecosystem.

* Repository URL: [https://github.com/KryKomDev/NeoKolors](https://github.com/KryKomDev/NeoKolors)

### NuGet Packages

All libraries in the NeoKolors ecosystem are published on NuGet for integration into .NET applications:

```bash
# Foundation
dotnet add package NeoKolors.Common
dotnet add package NeoKolors.Console
dotnet add package NeoKolors.Extensions
dotnet add package NeoKolors.Settings

# TUI Framework
dotnet add package NeoKolors.Tui.Core
dotnet add package NeoKolors.Tui.Elements
dotnet add package NeoKolors.Tui.Lifecycle.Apps
dotnet add package NeoKolors.Tui.Lifecycle.Game2D
dotnet add package NeoKolors.Tui.Fonts
```
