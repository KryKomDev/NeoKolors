# Articles & Documentation Guides

Welcome to the NeoKolors documentation library. Here you will find architectural deep dives, tutorials, and comprehensive guides for every component in the ecosystem.

---

## Documentation Sections

### 1. Foundation Architecture
- [**Solution Project Structure**](Project-Structure.md): Complete dependency graph, project responsibilities, and architecture overview.

### 2. NeoKolors.Common
- [**Common Overview**](../../Src/Common/Docs/Common-Overview.md): Primitives, structures, and utilities for console colors, styles, and graphics.
- [**NKColor Deep Dive**](../../Src/Common/Docs/NKColor.md): 32-bit compact RGB TrueColor, 256-color palette, and default console colors.
- [**NKStyle Deep Dive**](../../Src/Common/Docs/NKStyle.md): 128-bit explicit layout struct packing foreground, background, and text styles.
- [**Sixel Graphics**](../../Src/Common/Docs/SixelConverter.md): Encoding pixel image data directly into terminal Sixel byte streams.
- [**Color Palettes**](../../Src/Common/Docs/NKPalette.md): Custom terminal palette managers and color generators.
- [**NKConsoleColor**](../../Src/Common/Docs/NKConsoleColor.md): 256-color palette mapping, 6x6x6 color cube, and grayscale ramps.
- [**Text Styles and Effects**](../../Src/Common/Docs/TextStyles.md): Bold, Underline, Italic, Strikethrough, Invert, and Faint flags.
- [**Ansi String Style Encoding**](../../Src/Common/Docs/Eng/Ansi-String.md): Parsing and manipulating styled ANSI text without breaking layout boundaries.
- [**Binary Serialization Guide**](../../Src/Common/Docs/Serialization-Guide.md): Protobuf serialization and surrogates for bit-packed structs.

### 3. NeoKolors.Console
- [**Console Architecture & Drivers**](../../Src/Console/Docs/Console-Overview.md): High-performance native Linux (`epoll`/`termios`), Windows (`kernel32.dll`), and managed fallback terminal drivers.
- [**Driver Deep Dive**](../../Src/Console/Driver/Drivers.md): Low-level driver design, asynchronous input loops, SGR mouse protocols, and SIGWINCH resizing.
- [**Logging & Diagnostics**](../../Src/Console/Docs/Logging.md): Asynchronous structured logging (`NKLogger`, `NKDebug`).
- [**Exception Formatting**](../../Src/Console/Docs/Exception-Formatter.md): Visualizing unhandled crash stack traces with clean highlighting.
- [**NKConsole API Reference**](../../Src/Console/Docs/NKConsole.md): Virtual terminal toggles, alternate screen buffer, and input events.
- [**NKLogger Detailed**](../../Src/Console/Docs/NKLogger.md): In-depth logger documentation and architecture.
- [**Logger Configuration**](../../Src/Console/Docs/LoggerConfig.md): Configuring log levels, templates, and writers.
- [**Log File Configuration**](../../Src/Console/Docs/LogFileConfig.md): Datetime partitioning, replace, and append log file strategies.

### 4. NeoKolors.Extensions
- [**Extensions Overview**](../../Src/Extensions/Docs/Extensions-Overview.md): Productivity extension methods for standard .NET types.
- [**List2D Grid Utility**](../../Src/Extensions/Docs/List2D.md): 2D array grid transformations and spatial indexing.
- [**String Extensions**](../../Src/Extensions/Docs/String-Extensions.md): Word wrapping (`Chop`), ANSI measurement, and case conversions.

### 5. NeoKolors.Settings
- [**Settings Overview**](../../Src/Settings/Docs/Settings-Overview.md): Type-safe configuration builder framework with validation, nested nodes, and auto-merging.

### 6. NeoKolors.TUI & Elements
- [**TUI Overview**](../../Src/Tui/Docs/Tui-Overview.md): State-driven Text User Interface architecture.
- [**Quick-Start Tutorial**](../../Src/Tui/Docs/Tutorial-Quickstart.md): Step-by-step tutorial building an interactive TUI counter application.
- [**Application Lifecycle & Hosting**](../../Src/Tui/Docs/Application/Application-Lifecycle.md): Hosting infrastructure, `AppLifecycleHost`, `NKAppView`, and throttled/lazy rendering loops.
- [**Elements Suite**](../../Src/Tui/Elements/README.md): Flexible components: `Flex`, `Grid`, `Table`, `Border`, `Button`, `List`, `Text`, and `ElementsMouseDispatcher`.
- [**Core Canvas & Screen**](../../Src/Tui/Docs/Core/Canvas-and-Screen.md): Double-buffered `NKCharCanvas` and diff-based `NKCharScreen`.
- [**Elements Hierarchy Spec**](../../Src/Tui/Docs/Elements/Type-Hierarchy.md): Control taxonomy and visual state management.
- [**Layout Computation Methods**](../../Src/Tui/Docs/Layout/Layout-Computation-Methods.md): Measuring and arranging visual trees.
- [**Layout Computation Phases**](../../Src/Tui/Docs/Layout/Layout-Computation-Phases.md): Detailed layout evaluation lifecycle.

### 7. Lifecycle Framework
- [**Core Lifecycle & FrameClock**](../../Src/Tui/Lifecycle/Core/README.md): State controller, high-precision frame timing, fixed-timestep physics accumulator, and scene stack.
- [**Application Hosting & Loop Strategies**](../../Src/Tui/Lifecycle/Apps/README.md): Host lifecycle, window focus management, view presenter, and throttled/lazy loop strategies.
- [**2D Game Loop Engine**](../../Src/Tui/Lifecycle/Game2D/README.md): 2D arcade terminal game loop, 2D camera viewport, and multi-stage rendering pipelines.
- [**3D Architecture Plan**](../../Src/Tui/Lifecycle/Game3D/Game3D-Architecture-Plan.md): Technical plan for 3D terminal graphics pipeline and viewport management.

### 8. Typography & Fonts Engine
- [**Fonts Engine Overview**](../../Src/Tui/Docs/Fonts/Fonts-Overview.md): Terminal typography with XML V3 fonts and MessagePack `.nkf` binaries.
- [**Custom Fonts Tutorial**](../../Src/Tui/Docs/Fonts/Tutorial-Custom-Fonts.md): Guide to designing and compiling custom XML fonts.
- [**Builtin Font Compilation Pipeline**](../../Src/Tui/Docs/Fonts/Engineering/Builtin-Font-Compilation.md): Build-time font embedding into `NeoKolors.Tui.Fonts.Assets`.
- [**Font Rendering Pipeline**](../../Src/Tui/Docs/Fonts/Engineering/Font-Rendering.md): Measuring, alignment, placement, and styling.
- [**String Tokenization**](../../Src/Tui/Docs/Fonts/Engineering/String-Tokenization.md): Resolving letter tokens and multi-character ligatures.
- [**XML Font Configuration**](../../Src/Tui/Docs/Fonts/Xml/Xml-Font-Definition-Config.md): Metric configurations and defaults.
- [**XML Font Mapping**](../../Src/Tui/Docs/Fonts/Xml/Xml-Font-Definition-Mapping.md): Character mapping, ligatures, and compound glyphs.
- [**XML Glyph Properties**](../../Src/Tui/Docs/Fonts/Xml/Xml-Glyph-Definition-Properties.md): Masking, alignment points, and anchor replacements.

### 9. Tools & CLI
- [**nkfont CLI Tool Guide**](../../Tools/NKFont/README.md): Command-line utility to create, compile, inspect, and preview custom terminal fonts.
