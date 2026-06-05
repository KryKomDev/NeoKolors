# NeoKolors Common Overview

The `NeoKolors.Common` package contains the fundamental types, structures, and utilities that represent styling, colors, and graphics in the NeoKolors ecosystem. It provides optimized types for text styling, ANSI escape sequence generation, Sixel image formatting, and color palette management.

---

## 1. Core Types

### 1.1 `NKColor`
**[NKColor](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKColor.cs)** represents a color in the NeoKolors system. It supports:
* **RGB Colors**: Full 24-bit TrueColor (`NKColor.FromRgb(r, g, b)`).
* **Hex Colors**: Instantiated from hex codes (`NKColor.FromHex(0xFF5500)`).
* **Console Colors**: Mapping to the 16 standard console colors (`NKConsoleColor`).
* **Inherited Colors**: Special state indicating a cell inherits its color from its parent layout context.

### 1.2 `NKStyle`
**[NKStyle](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKStyle.cs)** combines foreground color, background color, and text decorations into a single immutable styling structure.

```csharp
using NeoKolors.Common;

// Create a style: Bold green text on inherit background
var style = new NKStyle(
    foreground: NKColor.FromRgb(0, 255, 0),
    background: NKColor.Inherit,
    effects: TextStyles.Bold
);
```

### 1.3 `AnsiString` & `AnsiChar`
* **[AnsiChar](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/AnsiChar.cs)**: An individual character paired with its styling properties.
* **[AnsiString](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/AnsiString.cs)**: An immutable, styled string structure. It parses bracketed style markers (e.g. `{:f#red}Hello{/}`) and stores them as boundary-based index markers.

---

## 2. Text Styles & String Effects

* **[TextStyles](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/TextStyles.cs)**: Enum flags for ANSI styles (Bold, Faint, Italic, Underline, Blink, Negative, Invisible, Strikethrough).
* **[StringEffects](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/StringEffects.cs)**: Extension methods to wrap strings in raw ANSI escape codes for quick stdout writing:

```csharp
using NeoKolors.Common;

string boldRedText = "Warning".Bold().Red();
```

---

## 3. Sixel Graphics (`SixelConverter`)

**[SixelConverter](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/SixelConverter.cs)** provides high-performance conversion of raster images (via pixel byte arrays) to Sixel byte streams, enabling hardware-accelerated image drawing on supported terminals.
