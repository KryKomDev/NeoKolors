# NeoKolors.Tui.Fonts

[![Build Status](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/build.yml?style=for-the-badge&labelColor=%232a313c&label=build)](https://github.com/KryKomDev/NeoKolors/actions/workflows/build.yml) [![Test Status](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/build.yml?style=for-the-badge&labelColor=%232a313c&label=tests)](https://github.com/KryKomDev/NeoKolors/actions/workflows/build.yml) [![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui.Fonts?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts) [![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui.Fonts?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts) [![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)

`NeoKolors.Tui.Fonts` is the core font engine for the NeoKolors Text User Interface (TUI) framework. It provides parsing, serializing, measuring, and rendering capabilities for stylized ASCII and ANSI fonts. It enables terminal interfaces to render custom typography, word wrap, alignment, and formatting with high performance.

## Key Features

- **Rich Text Rendering**: Full support for rendering plain strings and [AnsiString](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/AnsiString.cs) values to keep colors, formatting, and styles (strikethrough, underline) intact.
- **Custom Font Formats**: Supports both raw XML definition directories (easy to edit) and high-performance pre-compiled MessagePack binary `.nkf` assets.
- **Advanced Spacing Control**: Supports variable-width (with custom word spacing and kerning) and monospaced font configurations.
- **Ligatures & Alignment Points**: Replaces character groups (ligatures) with single composite glyphs and aligns glyphs using anchor markers (e.g. `+` or `*`) to maintain clean baselines.
- **Dynamic Layout & Alignment**: Handles text wrapping, boundary restrictions, horizontal/vertical alignment (Left, Center, Right; Top, Middle, Bottom), and overflow settings.
- **MSBuild Compiler Integration**: Provides a [CompileFontsTask](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/Build/CompileFontsTask.cs) which compiles XML font representations during target builds.

## Core Architecture

The font system is built around several key types:

- **[IAsciiFont](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/IAsciiFont.cs)**: The base interface defining string measurement and rendering logic onto a canvas.
- **[IExtendedAsciiFont](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/IExtendedAsciiFont.cs)**: Extends font placing methods to accept [TextRenderingOptions](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/TextRenderingOptions.cs) for detailed alignment and formatting.
- **[NKFont](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/NKFont.cs)**: The primary implementation of the font system, containing layout, ligature dictionary mapping, character effects (underline, strikethrough), and cell-level rendering.
- **[FontAtlas](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/FontAtlas.cs)**: A central registry where active fonts (such as `Default` or custom loaded fonts) are registered and resolved.
- **[NKFontSerializer](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/Serialization/NKFontSerializer.Xml.V3.cs)**: Handles serialization and deserialization, capable of loading XML folders, packaged zip archives, web URIs, or binary streams.

---

## Getting Started

### 1. Basic Rendering
To render a text string with a custom font onto a canvas:

```csharp
using Metriks;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Fonts;

// Get a font from the Atlas
if (FontAtlas.TryGet("Future", out var font) && font is IExtendedAsciiFont asciiFont)
{
    var canvas = new NKCharCanvas(80, 20);
    var bounds = new Area2D(new Point2D(0, 0), new Point2D(80, 20));

    // Place the string with Center alignment
    asciiFont.PlaceString(
        "HELLO WORLD",
        canvas,
        bounds,
        new NKStyle(),
        HorizontalAlign.CENTER,
        VerticalAlign.MIDDLE
    );
}
```

### 2. Serialization and Compilation
You can load a font from an XML package or compile it into a binary `.nkf` file for fast load times:

```csharp
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Fonts.Serialization;

// Load font from an XML directory
NKFont? xmlFont = NKFontSerializer.DeserializeXml("Paths/MyFontDirectory");

// Serialize font to a binary format (.nkf)
if (xmlFont != null)
{
    NKFontSerializer.SerializeBinary(xmlFont, "MyFont.nkf");
}

// Fast deserialization of the binary font file
NKFont? binaryFont = NKFontSerializer.DeserializeBinary("MyFont.nkf");
```

## MSBuild Compilation Task
The project implements a custom MSBuild task [CompileFontsTask](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/Build/CompileFontsTask.cs). This task is invoked during project compilation to automatically convert source XML font structures (containing configs, mapping, and text-based `.nkg` glyphs) into binary `.nkf` files to be packaged as embedded resources.
