# NeoKolors - Agent Guidelines & Repository Architecture

Welcome to the **NeoKolors** codebase. This document defines key architectural principles, coding standards, build/test workflows, and guidelines for AI agents working in this repository.

---

## 1. Project Overview & Vision

**NeoKolors** is a modern, high-performance C# console graphics, diagnostics, typography, and Text User Interface (TUI) ecosystem. It targets modern .NET runtimes (**`net8.0`**, **`net10.0`**, **`net11.0`**, and **`netstandard2.1`** for core assemblies) using **C# 15**.

### Core Architecture & Packages

| Package / Project | Responsibility | Key Types / Concepts |
| :--- | :--- | :--- |
| **`NeoKolors.Common`** | Low-level primitives, color formats, styles, ANSI sequences | `NKColor` (32-bit compact), `NKStyle` (128-bit explicit struct), `AnsiString`, Sixel graphics |
| **`NeoKolors.Console`** | Cross-platform terminal I/O drivers, logging, exception formatting | Linux POSIX `termios`/`epoll`, Windows API (`ReadConsoleInputW`), SGR 1006 mouse, `NKLogger`, `ExceptionFormatter` |
| **`NeoKolors.Extensions`** | High-performance text & collection helpers | `Chop` (word chopping), `List2D`, naming case converters |
| **`NeoKolors.Settings`** | Type-safe settings builder & parser | Nested option trees, validation, CLI argument merging |
| **`NeoKolors.Tui.Core`** | Core TUI rendering surfaces & screen buffer | `NKCharCanvas` (double-buffered), `NKCharScreen` (diff-based ANSI updates), `HitMap`, border styles |
| **`NeoKolors.Tui.Elements`** | Declarative UI component tree | `Flex` (flexbox), `Grid`, `Table`, `Border`, `Button`, `Text`, `StatusElement`, mouse dispatching |
| **`NeoKolors.Tui.Lifecycle.Core`** | Foundation loop & timing architecture | `ILifecycle`, `LifecycleController`, `FrameClock`, fixed-timestep accumulator, scene stack |
| **`NeoKolors.Tui.Lifecycle.Apps`** | Interactive terminal app host | `AppLifecycleHost`, `NKAppView`, focus tracking, throttled/lazy rendering loops |
| **`NeoKolors.Tui.Lifecycle.Game2D`** | 2D terminal game loop engine | `Game2DLoop`, `Camera2D`, `Render2DPipeline` |
| **`NeoKolors.Tui.Lifecycle.Game3D`** | 3D terminal graphics engine | 3D camera and lifecycle abstractions |
| **`NeoKolors.Tui.Lifecycle.Inline`** | Inline terminal rendering | Inline interactive widgets and progress indicators |
| **`NeoKolors.Tui.Fonts`** | Terminal typography engine | XML V3 definitions, MessagePack binary `.nkf`, Figlet fonts, kerning |
| **`NeoKolors.Tui.Fonts.Assets`** | Pre-compiled built-in font assets | `Bytesized`, `Future` embedded font resources |
| **`NeoKolors.Tools.NKFont`** | Global CLI tool (`nkfont`) | Font creation, XML to `.nkf` compilation, preview, inspection |

---

## 2. Core Engineering Principles

### Performance & Zero-Allocation Mindset
1. **Hot paths must be allocation-free**: Character manipulation, ANSI parsing, double-buffered screen diffing (`NKCharScreen`), and UI layout calculation must avoid heap allocations.
2. **Favor `Span<T>` and `ReadOnlySpan<char>`**: Avoid unnecessary `string` allocations and sub-stringing; use `ReadOnlySpan<char>`, `Memory<T>`, or buffer pooling.
3. **Struct Layouts**: Core styling structs (`NKColor`, `NKStyle`) are designed with explicit bit-packing and minimal footprint. Do not add reference types or enlarge these without careful consideration.
4. **Avoid LINQ in render loops**: Do not use LINQ or closure allocations inside per-frame update/render methods or layout calculation passes.

### Cross-Platform Terminal Support
- Terminal input/output relies on native OS drivers when available (`NK_ENABLE_NATIVE_IO`):
  - Linux: Native POSIX `termios`, `epoll`, asynchronous non-blocking input.
  - Windows: Win32 console input buffers (`ReadConsoleInputW`), VT sequences.
  - Fallback: Standard .NET `System.Console` abstraction when native drivers are unavailable.
- Always preserve terminal state (raw mode restoration, cursor visibility, mouse tracking disable) in `finally` blocks or `IDisposable` implementations.

---

## 3. Build & Test Commands

The solution file is **`NeoKolors.slnx`**.

### Build
```bash
# Build full solution in Debug
dotnet build NeoKolors.slnx -c Debug

# Build full solution in Release
dotnet build NeoKolors.slnx -c Release

# Build specific project
dotnet build Src/Common/NeoKolors.Common.csproj -c Debug
```

### Test
```bash
# Run all tests across all target frameworks (net8.0, net10.0, net11.0)
dotnet test NeoKolors.slnx -c Debug

# Run tests for a specific project
dotnet test Tests/Common.Tests/NeoKolors.Common.Tests.csproj

# Run tests targeting a single framework (fast iteration during development)
dotnet test Tests/Common.Tests/NeoKolors.Common.Tests.csproj -f net10.0
dotnet test Tests/Tui.Tests/Core/NeoKolors.Tui.Core.Tests.csproj -f net10.0

# Run specific test by filter
dotnet test Tests/Common.Tests/NeoKolors.Common.Tests.csproj --filter "FullyQualifiedName~NKColorTests"
```

---

## 4. Coding Standards & Conventions

Defined in [.editorconfig](file:///.editorconfig):
- **C# Version**: C# 15 (`<LangVersion>15</LangVersion>`).
- **Indentation**: 4 spaces, UTF-8 charset.
- **Naming Conventions**:
  - Interfaces: `I` prefix + UpperCamelCase (e.g. `ILifecycle`, `IElement`).
  - Abstract Classes: `A` prefix + UpperCamelCase (e.g. `AElement`, `ALifecycle`).
  - Enums and Enum Members: `ALL_UPPER` (e.g. `NKColorType.RGB`, `StyleFlags.BOLD`).
  - Constants (public/internal): `ALL_UPPER`.
  - Methods and Properties: `UpperCamelCase`.
  - Local variables & parameters: `camelCase`.
  - Private fields: `camelCase` or `_camelCase`.
- **Documentation**: XML documentation generation is enabled (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`). Public APIs should be documented cleanly (`CS1591` warning is suppressed where not strictly required, but descriptive comments on public APIs are encouraged).
- **Conditional Compilation**: Respect defined flags:
  - `NK_RENDERING_PROFILING`: Enables internal render counters and FPS metrics.
  - `NK_ENABLE_NATIVE_IO`: Enables native Linux/Windows console drivers.

---

## 5. File Editing Rules & Best Practices

To avoid common file modification failures:
1. **Always Read Before Editing**: Never attempt an edit from memory. Call `read_file` with line bounds to inspect the exact current code, indentation, and surrounding context.
2. **Anchor with Unique Context**: Choose a `Target` block with 2-3 lines of unique surrounding context before and after the modification so it matches exactly one place in the file.
3. **Preserve Line Endings & Indentation**: This repository enforces CRLF and 4-space indentation. Maintain exact leading whitespace in both `Target` and `Replacement`.
4. **One Contiguous Block per Call**: Never place placeholder comments (e.g., `// ... rest unchanged`) inside `Target` or `Replacement`. Make multiple sequential `edit_file` calls for non-adjacent edits.
5. **Never Wipe Files with `write_file`**: If an `edit_file` call fails, diagnose the whitespace or line mismatch. Do NOT fall back to overwriting the entire file with `write_file(Overwrite: true)`.

