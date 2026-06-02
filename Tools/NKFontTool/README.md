# NKFontTool (`nkfont`)

`NKFontTool` is a command-line interface (CLI) tool designed to manage, compile, display, inspect, and create custom TUI fonts for the NeoKolors framework.

The tool parses font source definitions (XML configurations and raw `.nkg` text files) and converts them into optimized, pre-validated binary `.nkf` assets.

---

## Installation & Running

### 1. Development Mode
To run the tool directly from source during development:

```bash
dotnet run --project Tools/NKFontTool/NKFontTool.csproj -- [arguments]
```

### 2. Install as a .NET Global Tool
The project is configured to pack as a .NET Tool under the command name `nkfont`. To pack and install it locally:

```bash
# Package the tool
dotnet pack Tools/NKFontTool/NKFontTool.csproj -c Release

# Install globally from the build directory
dotnet tool install --global --add-source Tools/NKFontTool/bin/Release NKFontTool
```

Once installed, you can execute it from anywhere using the command:

```bash
nkfont [arguments]
```

---

## Commands and Usage

### `create` (alias `new`)
Creates a new blank XML font definition folder structure.

```bash
nkfont create <output-dir> [font-name] [options]
```

- **Arguments**:
  - `output-dir` (Required): The output directory where the configuration files and glyph subfolders are initialized.
  - `font-name` (Optional): The internal name of the font. Defaults to the folder name if omitted.
- **Options**:
  - `-a` | `--all-ascii`: Automatically creates directories (`lowercase`, `uppercase`, `digits`, `special`) and populates them with template `.nkg` files for all printable ASCII characters. It also generates the complete `Map.xml` mapping.

**Example**:
```bash
nkfont create ./MyNewFont CoolFont -a
```

---

### `compile`
Compiles an XML font definition directory or zip file into a highly-compressed binary `.nkf` font.

```bash
nkfont compile <xml-path> [output-nkf]
```

- **Arguments**:
  - `xml-path` (Required): The directory path or path to a zip file containing the XML font definitions.
  - `output-nkf` (Optional): The output path for the compiled `.nkf` binary file. If omitted, saves using the font's internal name in the current directory.

**Example**:
```bash
nkfont compile ./MyNewFont ./MyNewFont.nkf
```

---

### `display` (aliases `show`, `render`)
Renders a text string using the specified font directly inside the terminal. This is useful for visual validation during font design.

```bash
nkfont display <font> <text...>
```

- **Arguments**:
  - `font` (Required): A built-in font name (`Bytesized`, `Future`, `Dummy`), a compiled `.nkf` file path, or a path to a raw XML font folder/zip.
  - `text` (Required): The text to display. Supports ANSI coloring codes for styled output.

**Example**:
```bash
# Render using built-in font
nkfont display Future "Hello World"

# Render using custom binary font
nkfont display ./MyNewFont.nkf "Custom Typography"
```

---

### `list`
Lists all available built-in fonts pre-registered in the active session.

```bash
nkfont list
```

---

### `font` (alias `inspect-font`)
Lists all characters, symbols, and ligatures mapped inside a specified font.

```bash
nkfont font <font>
```

- **Arguments**:
  - `font` (Required): A built-in font name, compiled `.nkf` file path, or XML font folder/zip.

**Example**:
```bash
nkfont font Bytesized
```

---

### `glyph` (alias `inspect-glyph`)
Inspects a single glyph inside a font. It details its width, height, alignment points, and prints a visual preview highlighting the glyph's structure and anchor bounds.

```bash
nkfont glyph <font> <character-or-ligature>
```

- **Arguments**:
  - `font` (Required): A built-in font name, compiled `.nkf` file path, or XML font folder/zip.
  - `character-or-ligature` (Required): The character symbol (e.g. `a`) or ligature string (e.g. `fi`) to inspect.

**Example**:
```bash
nkfont glyph Future a
```

---

## Code Reference
- **CLI Logic entrypoint**: [Program.cs](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Tools/NKFontTool/Program.cs)
- **Project Configuration**: [NKFontTool.csproj](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Tools/NKFontTool/NKFontTool.csproj)
