# Tutorial: Creating and Compiling Custom Fonts

NeoKolors features a highly advanced typography engine. By styling characters into glyph templates, you can display banner text, headers, and decorative elements directly inside terminal windows. 

This tutorial details how to use the command-line companion tool **`nkfont`** and XML schemas to design, compile, and embed your own custom TUI fonts.

---

## 1. Setting Up the `nkfont` CLI Tool

`nkfont` is the utility CLI used to scaffold, inspect, and compile fonts.

### Installing the Tool
First, ensure you have the .NET SDK installed. Build and install the global tool by packing it from the repository root:

```bash
# 1. Package the tool
dotnet pack Tools/NKFontTool/NKFontTool.csproj -c Release

# 2. Install globally
dotnet tool install --global --add-source Tools/NKFontTool/bin/Release NKFontTool
```

Verify your installation:
```bash
nkfont list
```

---

## 2. Scaffolding a New Font

Use the `create` command to scaffold the directory structure for a new font:

```bash
nkfont create ./MyCustomFont "MyFont" -a
```

This command will output:
- `Config.xml`: Main metadata specifying letter height, leading, and spacing model.
- `Map.xml`: Mapping rules indicating which symbols map to which source glyph files.
- `Glyphs/`: Folders containing template `.nkg` glyph text files for lowercase, uppercase, digits, and special characters.

---

## 3. Designing Glyph Characters

Open a glyph file (e.g., `Glyphs/uppercase/A.nkg`). A glyph is represented as plain text within grid bounds.

For example, a blocky `A` glyph could look like this:
```text
  ███  
 ██ ██ 
██   ██
███████
██   ██
██   ██
```

### Structuring Glyph Rules
Each glyph source file represents character cells. By default, spaces or empty cells represent the background.

To align symbols or adjust heights, you can define **Baseline offsets** in `Map.xml`:
- Letters like `g` or `y` that drop below the text baseline should specify a negative vertical offset: `Baseline="-2"`.

---

## 4. Configuring Font XML Files

Let's configure the main XML descriptors generated during scaffolding.

### A. Font Metrics (`Config.xml`)
The configuration XML controls core font properties:

```xml
<Font>
    <Name>MyFont</Name>
    <LetterSpacing>1</LetterSpacing>
    <Leading>8</Leading>
    <Ligatures>true</Ligatures>
    
    <!-- Use Variable sizing to automatically adjust widths per character -->
    <Variable WordSpacing="3" Kerning="true" />
</Font>
```

### B. Glyph Mapping (`Map.xml`)
The mappings XML links characters/symbols to their `.nkg` file paths:

```xml
<Mappings>
    <!-- Map single component characters -->
    <Component Symbol="A" File="Glyphs/uppercase/A.nkg" Styles="bold" />
    <Component Symbol="a" File="Glyphs/lowercase/a.nkg" />
    
    <!-- Map custom ligatures (e.g. arrows) -->
    <Ligature Symbol="-&gt;" File="Glyphs/special/arrow_right.nkg" />
    
    <!-- Combine accents automatically with base letters -->
    <AutoCompound Base="´" Align="top" IsBaseMain="false">
        <ApplicableGroup>vowels</ApplicableGroup>
    </AutoCompound>
</Mappings>
```

---

## 5. Compiling the Font

To optimize performance and reduce runtime asset footprints, compile the text directory into a compressed binary `.nkf` file:

```bash
nkfont compile ./MyCustomFont ./MyFont.nkf
```

---

## 6. Testing the Font

You can preview how text renders with your custom font directly in the command prompt before loading it into your app:

```bash
nkfont display ./MyFont.nkf "Hello World!"
```

---

## 7. Packaging and Loading in Your Application

To use the custom font in your C# TUI applications, include it as an **Embedded Resource** in your `.csproj`:

```xml
<ItemGroup>
    <EmbeddedResource Include="MyFont.nkf" />
</ItemGroup>
```

Then, register it in your application setup:

```csharp
using NeoKolors.Tui.Fonts;

// Register the compiled font stream
using (var stream = typeof(Program).Assembly.GetManifestResourceStream("MyApp.MyFont.nkf"))
{
    FontAtlas.Register(stream);
}
```

Now, apply it to a `TextBlock` element:

```csharp
var textBlock = new TextBlock
{
    Font = "MyFont",
    Content = "HELLO TUI"
};
```
