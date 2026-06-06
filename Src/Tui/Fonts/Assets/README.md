# NeoKolors.Tui.Fonts.Assets

[![Build Status](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/KryKomDev/9fb6af0ba84bfb5106a78ff35bd8be3c/raw/build-Tui.Fonts.Assets.json&style=for-the-badge&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/build.yml) [![Test Status](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/KryKomDev/9fb6af0ba84bfb5106a78ff35bd8be3c/raw/test-Tui.Fonts.Assets.json&style=for-the-badge&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/build.yml) [![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui.Fonts.Assets?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts.Assets) [![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui.Fonts.Assets?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts.Assets) [![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)

`NeoKolors.Tui.Fonts.Assets` contains the default font definitions and a packaging mechanism for the NeoKolors TUI framework. It provides pre-compiled, out-of-the-box fonts embedded directly into the compiled library and exposes an easy mechanism to load and register them.

## Built-in Fonts

The package includes three default font designs:
1. **Bytesized**: A clean, compact pixel-art styled font ideal for dense information layouts.
2. **Future**: A modern, wide stylized font featuring high-tech and futuristic geometry.
3. **Dummy**: A fallback/skeleton font representation used primarily for testing, debugging, and baseline verification.

## Architecture & Built-in Compilation

Instead of checking in large pre-compiled binary resources, this project structures fonts in human-readable XML format during development. 

### Font Directory Structure
Each font under the project directory (e.g., [Bytesized/](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/Assets/Bytesized)) consists of:
- `manifest.nkfont`: Links the config file and the map file.
- `Config.xml`: Configures variables like letter spacing, word spacing, ligatures, leading, baseline, and alignment markers.
- `Map.xml`: Maps character symbols/ligatures to specific `.nkg` glyph files.
- `.nkg` Files: Plain-text files containing ASCII art representations of character glyph layouts and their alignment points.

### MSBuild Compilation Pipeline
On building the project, a custom MSBuild Target (`CompileBuiltinFonts`) executes:
1. It reflects into the compiled `NeoKolors.Tui.Fonts.dll` assembly to locate the custom compiler task.
2. It processes the raw XML definitions and `.nkg` glyph files inside `Bytesized`, `Future`, and `Dummy`.
3. It validates the configuration and generates compiled MessagePack binary files (`Bytesized.nkf`, `Future.nkf`, and `Dummy.nkf`).
4. These binary files are dynamically added as `EmbeddedResource` files inside the target assembly.

---

## Usage

### AssetsProvider
The [AssetsProvider](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/Assets/AssetsProvider.cs) class provides runtime access to these embedded assets.

#### 1. Automatic Font Registration
If the compiler constant `NK_FONTS_ENABLE_AUTO_REGISTRATION` is defined in your build configuration, `AssetsProvider` uses a C# `[ModuleInitializer]` to automatically register all three built-in fonts into the [FontAtlas](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/FontAtlas.cs) when the assembly is loaded.

#### 2. Manual Registration
To manually load and register these fonts into the global `FontAtlas`:

```csharp
using NeoKolors.Tui.Fonts.Assets;

// Registers "Bytesized", "Future", and "Dummy"
AssetsProvider.RegisterFonts();
```

#### 3. Direct Font Access
You can retrieve the deserialized [NKFont](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Fonts/NKFont.cs) instances directly:

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
