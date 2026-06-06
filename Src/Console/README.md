# NeoKolors.Console

[![Build Status](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/KryKomDev/9fb6af0ba84bfb5106a78ff35bd8be3c/raw/build-Console.json&style=for-the-badge&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/build.yml) [![Test Status](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/KryKomDev/9fb6af0ba84bfb5106a78ff35bd8be3c/raw/test-Console.json&style=for-the-badge&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/build.yml) [![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Console?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Console) [![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Console?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Console) [![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)

`NeoKolors.Console` is the primary output driver, input processor, log management framework, and exception formatter for the NeoKolors ecosystem. It provides low-level terminal controls combined with advanced diagnostics.

---

## Key Features

- **Console Driver (`NKConsole`)**:
  - Handles basic and styled writes ([NKConsole.Out.cs](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Console/NKConsole.Out.cs)) utilizing hexadecimal, [NKColor](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKColor.cs), and [NKStyle](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKStyle.cs).
  - Controls virtual terminal states: alternate screen buffers, custom cursor visibility, bracketed paste mode, and multi-level mouse reporting.
  - Monitors and captures user input events ([NKConsole.In.cs](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Console/NKConsole.In.cs)), wrapping low-level interop keys and focus transitions.
- **Structured Logging (`NKLogger`)**:
  - A highly performant logger supporting standard logging levels (`Trace`, `Debug`, `Info`, `Warn`, `Error`, `Crit`).
  - Supports structured template parameters (`NKDebug.Info("Successfully created {Name}", fontName)`).
  - Features configurable log file backends, including date-time file partitioning and automatic flushing on process exit.
- **Exception Visualization (`ExceptionFormatter`)**:
  - Automatically captures and reformats unhandled stack traces into visually descriptive blocks with highlighting.
  - Can throw custom [FancyException](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Console/FancyException.cs) bounds, ensuring debugging errors display with clean UI layouts in the terminal.

---

## Core Types

### 1. NKConsole Writing
Prints colored strings using styles, RGB values, or custom coloring codes:

```csharp
using NeoKolors.Common;
using NeoKolors.Console;

// Write with Hex color
NKConsole.WriteLine("Important Notice", 0xFF5555);

// Write using a custom style
var style = new NKStyle(NKColor.FromRgb(0, 255, 0), NKColor.Inherit, TextStyles.Bold);
NKConsole.WriteLine("Success Output", style);
```

### 2. Low-Level Terminal Controls
Allows configuring mouse reporting and window events:

```csharp
using NeoKolors.Console;
using NeoKolors.Console.Ansi.Mouse;

// Hide the blinking hardware cursor
NKConsole.HideCursor();

// Open secondary buffer
NKConsole.EnableAltBuffer();

// Listen to mouse actions
NKConsole.Mouse += mouseEvent => {
    NKConsole.WriteLine($"Mouse clicked at: {mouseEvent.Position}");
};
```

### 3. Static Debug Diagnostics & Logger
Static wrappers around the diagnostic backend:

```csharp
using NeoKolors.Console;

// Initialize datetime file logger
NKDebug.Logger.FileConfig = LogFileConfig.NewDatetime("./logs/{0}.log");

// Log diagnostics
NKDebug.Info("Application initialized.");
NKDebug.Warn("Configuration value '{Key}' is missing, using defaults", "Interval");
```

### 4. Intercepting Exceptions
You can override standard unhandled crash logs with clean colored formats:

```csharp
using NeoKolors.Console;

// Enable pretty stack traces globally
NKDebug.EnableExceptionInterruption();

try
{
    throw new InvalidOperationException("Failed to load settings file.");
}
catch (Exception ex)
{
    // Beautiful formatted output in the terminal
    NKDebug.Formatter.PrintException(ex);
}
```