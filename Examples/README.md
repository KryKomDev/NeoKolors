# NeoKolors Examples

This directory contains standalone runnable examples and sample projects demonstrating the capabilities of the NeoKolors ecosystem.

---

## Standalone C# File Examples

These examples use top-level statements and file-based scripts that can be executed directly with `dotnet run`.

### 1. [ExampleApp.cs](./ExampleApp.cs) &mdash; Interactive TUI Dashboard

A feature-rich terminal user interface application demonstrating the full power of `NeoKolors.Tui`:
- **App Lifecycle & View Hosting**: Uses `AppLifecycleHost` with `ThrottledAppLoopStrategy` (60 FPS active, 10 FPS background).
- **Double-Buffered Rendering**: Managed by `NKCharScreen` with zero screen flicker.
- **Interactive Mouse Support**: Implements `HitMap` and `ElementsMouseDispatcher` for accurate mouse click, hover, and scroll handling.
- **Component Layouts**: Built using `Flex` containers, `Border` frames with rounded corners, interactive `Button` controls with hover states, `Table` grids, and `List` views.
- **Reactive State**: Driven by `Signal<T>` reactive primitives.
- **Responsive Terminal Resizing**: Automatically handles console window resize events and recalculates layouts.
- **Diagnostics**: Structured binary logging (`.blog`) and formatted text log outputs.

**Run Example:**
```bash
dotnet run Examples/ExampleApp.cs
```

---

### 2. [ExampleGame2D.cs](./ExampleGame2D.cs) &mdash; *Cosmic Invaders* 2D Terminal Game

A complete, retro-styled 2D arcade space shooter demonstrating the 2D game engine:
- **Game Engine**: Uses `Game2DLoop` with a `FixedTimestepAccumulator` (60 Hz fixed updates) decoupled from rendering.
- **2D Camera**: `Camera2D` viewport mapping world coordinates to terminal character coordinates.
- **Modular Pipeline**: Multi-stage `Render2DPipeline` executing distinct render passes:
  1. *StarfieldStage*: Multi-layer parallax scrolling starfield.
  2. *EntitiesStage*: Player spaceship, alien invaders, and projectile lasers.
  3. *ParticlesStage*: High-velocity particle explosion effects with lifetime decay and color fading.
  4. *HudAndPresentStage*: Real-time score, shields, energy bars, wave counter, and Game Over banner.
- **Input System**: Keyboard controls for spaceship thrusters and laser cannons.

**Run Example:**
```bash
dotnet run Examples/ExampleGame2D.cs
```

---

## Example Projects

### 3. [Neofetch](./Neofetch/) &mdash; System Information Display

Demonstrates terminal graphic layouts, styled strings (`AnsiString`), and RGB true color rendering to present OS, kernel, uptime, and system hardware specifications alongside stylized ASCII artwork.

**Run Project:**
```bash
dotnet run --project Examples/Neofetch/Neofetch.csproj
```

---

### 4. [Ascii](./Ascii/) &mdash; ASCII Art and Typography

Demonstrates loading, scaling, and printing stylized ASCII typography and character graphics onto the console buffer using `NeoKolors.Common` and `NeoKolors.Console`.

**Run Project:**
```bash
dotnet run --project Examples/Ascii/Ascii.csproj
```

---

### 5. [WeekNumber](./WeekNumber/) &mdash; Date & Calendar Utility

A lightweight CLI utility demonstrating console text formatting and colorization to calculate and display the current ISO week number and calendar metrics.

**Run Project:**
```bash
dotnet run --project Examples/WeekNumber/WeekNumber.csproj
```

---

### 6. [Testing](./Testing/) &mdash; Development Sandbox

An internal experimental playground used to verify new layout components, drivers, and diagnostic features during library development.

**Run Project:**
```bash
dotnet run --project Examples/Testing/Testing.csproj
```
