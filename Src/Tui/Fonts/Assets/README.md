# NeoKolors.Tui.Fonts.Assets

[![Build & Test](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/ci.yml?branch=main&style=for-the-badge&label=Build%20%26%20Test&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/ci.yml)
[![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui.Fonts.Assets?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts.Assets) 
[![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui.Fonts.Assets?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts.Assets) 
[![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)

`NeoKolors.Tui.Fonts.Assets` contains the default font definitions and a packaging mechanism for the NeoKolors TUI
framework. It provides pre-compiled, out-of-the-box fonts embedded directly into the compiled library and exposes an
easy mechanism to load and register them.

## Built-in Fonts

The package includes three default NKFont designs:

1. **Bytesized**: A clean, compact pixel-art styled font ideal for dense information layouts.
2. **Future**: A modern, wide stylized font featuring high-tech and futuristic geometry.
3. **Dummy**: A fallback/skeleton font representation used primarily for testing, debugging, and baseline verification.

Additionally, four built-in FIGlet (`.flf`) fonts with accompanying metadata are embedded:

4. **Standard**: Classic FIGlet font designed by Glenn Chappell & Ian Chai.
5. **Shadow**: 3D shadowed font designed by Glenn Chappell.
6. **Slant**: Slanted FIGlet font designed by Glenn Chappell.
7. **Block**: Heavy block FIGlet font designed by Glenn Chappell.

FIGlet font metadata is configured via embedded `manifest.xml` (located under `Figlet/`) or per-font sidecar files (`<FontName>.meta.xml`).

## Architecture & Built-in Compilation

Instead of checking in large pre-compiled binary resources, this project structures fonts in human-readable XML format
during development.

### Font Directory Structure

Each font under the project directory (
e.g., [Bytesized/](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/Assets/Bytesized)) consists
of:

- `manifest.nkfont`: Links the config file and the map file.
- `Config.xml`: Configures variables like letter spacing, word spacing, ligatures, leading, baseline, and alignment
  markers.
- `Map.xml`: Maps character symbols/ligatures to specific `.nkg` glyph files.
- `.nkg` Files: Plain-text files containing ASCII art representations of character glyph layouts and their alignment
  points.

### MSBuild Compilation Pipeline

On building the project, a custom MSBuild Target (`CompileBuiltinFonts`) executes before `PrepareResources`:

1. It dynamically loads the referenced `NeoKolors.Tui.Fonts.dll` in-memory (avoiding file locks during development).
2. It invokes [`CompileFontsTask`](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/Build/CompileFontsTask.cs), passing the configured fonts and output options.
3. For each font, the task validates timestamps against source files (skipping recompilation if up-to-date), deserializes XML definitions, and compiles them into binary Protobuf (`.nkf`) assets.
4. Output `.nkf` files are emitted into the configured output folder (default: `nkf/`).
5. Generated assets are automatically registered as `@(EmbeddedResource)` with standard logical names (`NeoKolors.Tui.Fonts.Assets.<FontName>.nkf`) and registered with `@(FileWrites)` so `dotnet clean` removes them cleanly.

---

## Configuring Font Compilation

You can control what fonts to compile and where to output them directly inside [`NeoKolors.Tui.Fonts.Assets.csproj`](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/Assets/NeoKolors.Tui.Fonts.Assets.csproj).

### 1. Setting the Output Directory

Set `<FontOutputDir>` to change where compiled `.nkf` assets are placed relative to the project directory:

```xml
<PropertyGroup>
    <!-- Default is "nkf". Can also be "compiled", "bin/fonts", etc. -->
    <FontOutputDir>nkf</FontOutputDir>
</PropertyGroup>
```

You can also override this from the command line:

```bash
dotnet build -p:FontOutputDir=compiled
```

### 2. Selecting Which Fonts to Compile

Declare the fonts to compile inside an `<ItemGroup>` using `<Font>` items:

```xml
<ItemGroup>
    <Font Include="Bytesized" />
    <Font Include="Future" />
    <Font Include="Dummy" />
</ItemGroup>
```

- **Add a font**: Simply add `<Font Include="MyNewFont" />`.
- **Exclude a font**: Comment out or remove the corresponding `<Font Include="..." />` item.
- **Auto-discovery fallback**: If `<Font>` items are omitted or empty, `CompileFontsTask` automatically detects all subdirectories in the project containing a `manifest.nkfont` or `Config.xml` file.

### 3. Per-Font Customization

Each `<Font>` item supports metadata attributes to customize output naming or placement individually:

```xml
<ItemGroup>
    <!-- Standard font outputting to $(FontOutputDir)/Bytesized.nkf -->
    <Font Include="Bytesized" />

    <!-- Override destination path for a specific font -->
    <Font Include="Future" OutputPath="custom/folder/Future.nkf" />

    <!-- Override output directory for a specific font -->
    <Font Include="Dummy" OutputDir="test_artifacts" />

    <!-- Specify a custom font name override -->
    <Font Include="Fonts/CustomFontFolder" FontName="MyCustomFont" />
</ItemGroup>
```

| Metadata Attribute | Description | Default |
| ------------------ | ----------- | ------- |
| `OutputPath` / `OutputFile` / `TargetFile` | Explicit destination file path or directory for the compiled `.nkf` | `$(FontOutputDir)/<FontName>.nkf` |
| `OutputDir` / `OutputDirectory` | Directory override for this font | `$(FontOutputDir)` |
| `FontName` / `Name` | Override name used for the font asset and embedded resource | Name of source directory |

### 4. Incremental Compilation & Clean Support

- **Up-To-Date Checking**: The task inspects file timestamps of all source files in each font directory against the target `.nkf` file. If no source files have been modified, compilation is skipped.
- **Clean Support**: Generated output files are tracked by MSBuild's `@(FileWrites)`. Running `dotnet clean` automatically removes compiled `.nkf` files.

---

## Usage

### AssetsProvider

The [AssetsProvider](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/Assets/AssetsProvider.cs)
class provides runtime access to these embedded assets.

#### 1. Automatic Font Registration

If the compiler constant `NK_FONTS_ENABLE_AUTO_REGISTRATION` is defined in your build configuration, `AssetsProvider`
uses a C# `[ModuleInitializer]` to automatically register all built-in fonts into
the [FontAtlas](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/FontAtlas.cs) when the assembly
is loaded.

#### 2. Manual Registration

To manually load and register these fonts into the global `FontAtlas`:

```csharp
using NeoKolors.Tui.Fonts.Assets;

// Registers "Bytesized", "Future", and "Dummy"
AssetsProvider.RegisterFonts();
```

#### 3. Direct Font Access

You can retrieve the
deserialized [NKFont](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/NKFont.cs) instances
directly:

```csharp
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Fonts.Assets;

NKFont? futureFont = AssetsProvider.Future;
NKFont? bytesizedFont = AssetsProvider.Bytesized;

if (futureFont != null)
{
    // Ready for measurement or rendering
}
```

#### 4. Stream Access

Stream access is available via `AssetsProvider.GetFontStream(name)`. It resolves standard manifest resource names (`NeoKolors.Tui.Fonts.Assets.<FontName>.nkf`) as well as subfolder-prefixed names (`*.nkf.<FontName>.nkf`).
