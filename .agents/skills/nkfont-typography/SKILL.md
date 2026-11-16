---
name: nkfont-typography
description: >-
  Use this skill when creating, inspecting, compiling, or testing terminal fonts,
  working with XML v3 font schemas, binary .nkf files, or using the nkfont CLI tool.
---

# NeoKolors Typography & `nkfont` CLI Guide

NeoKolors features a dedicated terminal typography engine in `NeoKolors.Tui.Fonts`, pre-compiled font assets in `NeoKolors.Tui.Fonts.Assets`, and a CLI tool in `Tools/NKFont`.

## Font Architecture

1. **Source Definitions (XML v3 + Glyphs)**:
   - `Config.xml`: Font metadata, line height, baseline, kerning pairs, fallback rules.
   - `Map.xml`: Mapping from characters/code points to glyph files (`.nkg`).
   - Glyphs (`*.nkg`): Plain text ASCII/Unicode art representing individual glyphs.
   - Schemas located at `Docs/_site/Schemas/Fonts/v3/`:
     - `Font.Config.xsd`
     - `Font.Map.xsd`
     - `Font.Common.xsd`
2. **Binary Compiled Assets (`.nkf`)**:
   - Compiled MessagePack binary format.
   - Built-in fonts like `Bytesized` and `Future` are embedded in `NeoKolors.Tui.Fonts.Assets`.
3. **Figlet Font Support**:
   - NeoKolors can also load standard `.flf` Figlet font files dynamically.

## Using `nkfont` CLI Tool

### Running in Development
```bash
dotnet run --project Tools/NKFont/NeoKolors.Tools.NKFont.csproj -- [command] [args]
```

### Key Commands

- **Create a new font project**:
  ```bash
  dotnet run --project Tools/NKFont/NeoKolors.Tools.NKFont.csproj -- create ./MyFont NewFont -a
  ```
- **Compile XML source to `.nkf` binary**:
  ```bash
  dotnet run --project Tools/NKFont/NeoKolors.Tools.NKFont.csproj -- compile ./Src/Tui/Fonts/Assets/Bytesized ./Bytesized.nkf
  ```
- **Inspect font properties**:
  ```bash
  dotnet run --project Tools/NKFont/NeoKolors.Tools.NKFont.csproj -- inspect ./Bytesized.nkf
  ```
- **Preview text rendered in the terminal**:
  ```bash
  dotnet run --project Tools/NKFont/NeoKolors.Tools.NKFont.csproj -- display ./Bytesized.nkf "Hello NeoKolors"
  ```

## Testing Font Changes
Run font tests in `Tests/Tui.Tests/Fonts/`:
```bash
dotnet test Tests/Tui.Tests/Fonts/NeoKolors.Tui.Fonts.Tests.csproj -f net10.0
```
