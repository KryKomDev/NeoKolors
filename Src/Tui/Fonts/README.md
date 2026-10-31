# NeoKolors.Tui.Fonts

[![Build & Test](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/ci.yml?branch=main&style=for-the-badge&label=Build%20%26%20Test&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/ci.yml) [![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui.Fonts?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts) [![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui.Fonts?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts) [![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)

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

- **[IAsciiFont](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/IAsciiFont.cs)**: The base interface defining string measurement and rendering logic onto a canvas, with support for [TextRenderingOptions](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/TextRenderingOptions.cs) for detailed alignment and formatting.
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
if (FontAtlas.TryGet("Future", out var font))
{
    var canvas = new NKCharCanvas(80, 20);
    var bounds = new Rect2D(new Point2D(0, 0), new Point2D(80, 20));

    // Draw the string with Center alignment
    font.DrawString(
        "HELLO WORLD",
        canvas,
        bounds,
        new NKStyle(),
        HorizontalAlign.CENTER,
        VerticalAlign.CENTER
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

### 3. FIGlet Fonts & Metadata Support
NeoKolors supports deserializing FIGlet (`.flf`) fonts directly. Metadata (author, license, custom attributes) can be attached and loaded automatically using a hybrid strategy:

1. **Embedded FIGlet Comments**: Extracted directly from the comment lines in the `.flf` header (e.g. `Author: ...`, `License: ...`, `Date: ...`, `Year: ...`).
2. **Directory Manifest XML** (`manifest.xml` or `Figlet.manifest.xml`): Defines metadata for multiple fonts in a single file:
   ```xml
   <FigletManifest>
       <Font Name="Shadow" Author="Glenn Chappell" LicenseType="Attribution / Permissive" SourceFormat="Figlet" Year="1993">
           <Description>Shadow font by Glenn Chappell</Description>
       </Font>
   </FigletManifest>
   ```
3. **Sidecar XML** (`<FontName>.meta.xml`): Per-font metadata file located next to the `.flf` file or embedded as a resource:
   ```xml
   <Font Author="Glenn Chappell" LicenseType="Attribution">
       <Metadata>
           <Year>1993</Year>
       </Metadata>
   </Font>
   ```
4. **Explicit `FigletMetadata`**: Passed directly to `NKFontSerializer.DeserializeFiglet(..., metadata: customMeta)`.

The resulting [`NKFontInfo`](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/NKFontInfo.cs) exposes:
- `SourceFormat`: A string identifier (`"NKFont"`, `"Figlet"`).
- `Author`, `LicenseType`, `LicenseFile`, `LicenseContent`.
- `Metadata`: An `IReadOnlyDictionary<string, string>` holding arbitrary metadata attributes (accessible via `font.Info.GetMetadata("Year")` with case-insensitive key lookup).

Both `SourceFormat` and `Metadata` are fully preserved during binary `.nkf` serialization.

## MSBuild Compilation Task

The project implements a custom MSBuild task [`CompileFontsTask`](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/Build/CompileFontsTask.cs). This task is invoked during project compilation to automatically convert source XML font structures (containing configs, mapping, and text-based `.nkg` glyphs) into binary Protobuf `.nkf` files to be packaged as embedded resources.

### Task Properties

| Property | Type | Description |
| -------- | ---- | ----------- |
| `SourceDir` | `string` | Base directory containing font source directories (defaults to current directory). |
| `OutputDir` | `string` | Base directory where compiled `.nkf` files are written (defaults to `SourceDir`). |
| `Fonts` | `ITaskItem[]` | List of font items to compile. Supports per-item metadata (`OutputPath`, `OutputDir`, `FontName`). If omitted or empty, auto-discovers font folders in `SourceDir`. |
| `ForceRebuild` | `bool` | When `true`, forces recompilation of all fonts regardless of whether the output files are up-to-date (default: `false`). |
| `OutputFiles` | `ITaskItem[]` (Output) | Emits the generated or verified `.nkf` files with metadata (`FontName`, `LogicalName`, `SourceDir`). |

### Example MSBuild Invocation

```xml
<CompileFontsTask 
    SourceDir="$(MSBuildProjectDirectory)"
    OutputDir="nkf"
    Fonts="@(Font)">
    <Output TaskParameter="OutputFiles" ItemName="CompiledFonts" />
</CompileFontsTask>

<ItemGroup>
    <EmbeddedResource Include="@(CompiledFonts)">
        <LogicalName>$(RootNamespace).%(Filename)%(Extension)</LogicalName>
    </EmbeddedResource>
    <FileWrites Include="@(CompiledFonts)" />
</ItemGroup>
```
