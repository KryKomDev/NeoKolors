# NeoKolors.Common

`NeoKolors.Common` is the foundation library for the NeoKolors ecosystem. It provides optimized primitives, structures, and utilities for working with console colors, styles, ANSI sequences, and advanced graphic rendering formats like Sixel.

---

## Key Features

- **Robust Color Models**: 
  - [NKColor](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKColor.cs): A unified structure representing 24-bit TrueColor (RGB), custom alpha transparency, standard 16 console colors, 256 xterm console colors, or color inheritance.
  - [NKConsoleColor](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKConsoleColor.cs): Extends standard console colors with support for the 6x6x6 RGB color cube and 24 grayscale levels.
  - [NKPalette](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKPalette.cs): Custom terminal palette managers.
- **Memory-Optimized Styling**:
  - [NKStyle](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKStyle.cs): Compresses foreground and background [NKColor](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKColor.cs) states and [TextStyles](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/TextStyles.cs) formatting into a single 64-bit integer (`long`), allowing lightweight and hyper-fast style mutations.
  - [TextStyles](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/TextStyles.cs): Flag enums supporting Bold, Italic, Underline (with multiple [UnderlineType](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/UnderlineType.cs) modes), Faint, Negative, and Strikethrough.
- **AnsiString Parsing**:
  - [AnsiString](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/AnsiString.cs): An optimized text structure that parses raw console strings, preserving text styles and color codes across sub-ranges without breaking when measuring text layout boundaries.
- **Terminal Graphics & Utilities**:
  - [SixelConverter](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/SixelConverter.cs): Encodes images into Sixel format byte streams, allowing graphical visual rendering directly inside compliant terminal windows.
  - [EscapeCodes](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/EscapeCodes.cs): Core database of ANSI sequences for cursor movement, clearing options, styling modifiers, and terminal properties.

---

## Core Types

### 1. NKColor
Encapsulates true color, index color, default console colors, and inheritance values:

```csharp
using NeoKolors.Common;

// 24-bit TrueColor RGB
var rgbColor = NKColor.FromRgb(255, 128, 0);

// Predefined 256 xterm index
var indexedColor = NKColor.FromConsoleColor(NKConsoleColor.LightMagenta);

// Transparent / inherit parent style
var inheritColor = NKColor.Inherit;
```

### 2. NKStyle
Bundles formatting for maximum speed:

```csharp
using NeoKolors.Common;

var style = new NKStyle(
    foreground: NKColor.FromRgb(255, 255, 255),
    background: NKColor.FromRgb(0, 0, 0),
    styles: TextStyles.Bold | TextStyles.Underline
);
```

### 3. AnsiString
Parses ANSI codes to preserve visual segments:

```csharp
using NeoKolors.Common;

if (AnsiString.TryParse("\u001b[31mRed Text\u001b[0m", out var ansiStr))
{
    // Returns 8 (ignores escape codes)
    int length = ansiStr.Length; 
}
```