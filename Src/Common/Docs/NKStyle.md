# NKStyle Deep Dive

**[NKStyle](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKStyle.cs)** manages text and cell styling in the NeoKolors ecosystem. It packages foreground colors, background colors, and active text decorations (such as bold, italic, underline, or blinking) into a single optimized 64-bit container.

---

## 1. Internal Bit-Packed Representation

To support high-frequency terminal drawing, `NKStyle` is structured as a bit-packed layout that fits inside a single 64-bit unsigned integer (`ulong`). This minimizes cache misses and simplifies comparison operations.

```csharp
[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
public record struct NKStyle : IFormattable, IParsableValue<NKStyle>, INKParsable<NKStyle>
```

### Bit Fields Layout
The 64 bits of `_raw` are packed as follows:
- **Bits 0–23**: Foreground color (`FColor`) value.
- **Bits 24–47**: Background color (`BColor`) value.
- **Bits 48–55**: Text style flags (`Styles`) mapped from the `TextStyles` enum.
- **Bits 56**: Foreground Color Switch (`FCSw`) indicating whether the color is a custom RGB color or a standard console color.
- **Bits 57**: Background Color Switch (`BCSw`) indicating whether the color is custom RGB or console color.
- **Bits 58–63**: Unused/reserved.

---

## 2. Text Styles Flags (`TextStyles`)

Text styles are represented by the flag enum **[TextStyles](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/TextStyles.cs)**, which maps directly to ANSI graphic rendition escape codes:

```csharp
[Flags]
public enum TextStyles : byte 
{
    None            = 0,
    Bold            = 1 << 0,  // ANSI 1
    Faint           = 1 << 1,  // ANSI 2
    Italic          = 1 << 2,  // ANSI 3
    Underline       = 1 << 3,  // ANSI 4
    Blink           = 1 << 4,  // ANSI 5
    Negative        = 1 << 5,  // ANSI 7
    Invisible       = 1 << 6,  // ANSI 8
    Strikethrough   = 1 << 7   // ANSI 9
}
```

You can toggle styles using standard bitwise operations or fluent chainable extensions:
```csharp
var boldItalic = TextStyles.Bold | TextStyles.Italic;
```

---

## 3. Creating Styles

`NKStyle` instances can be initialized with default parameters, and then modified:

```csharp
using NeoKolors.Common;

// 1. Create a custom style: bold red text on a dark gray background
var customStyle = new NKStyle(
    foreground: NKColor.FromRgb(255, 100, 100),
    background: NKConsoleColor.DARK_GRAY,
    effects: TextStyles.Bold
);

// 2. Modify properties
customStyle.Styles |= TextStyles.Italic; // Add italic effect
```

---

## 4. High-Performance Style Transitions

When rendering strings to the console, writing raw ANSI escape sequences for every single cell is extremely slow. `NKStyle` solves this by computing **minimal difference transition sequences** using the `GetControlChars()` method.

This method compares the style of a previous character cell against the target cell style and outputs the shortest possible ANSI string to achieve the change:

```csharp
var sb = new StringBuilder();
NKStyle currentStyle = cell.Style;

// Appends only the ANSI codes that changed from previousStyle (e.g. just changing colors or toggling bold)
NKStyle.AppendControlChars(sb, previousStyle, currentStyle);
```

### Transition Optimization Rules:
- If both foreground and background colors are identical to the previous cell, no color codes are written.
- If text style effects are disabled (e.g., bold was active and is now inactive), a reset sequence (`\e[0m`) is emitted, and other properties are re-applied.
- If a style is added (e.g., adding italic), only the specific addition code is written (`\e[3m`).
