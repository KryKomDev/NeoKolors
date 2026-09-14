# NKColor Deep Dive

**[NKColor](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKColor.cs)** is the core color representation structure in the NeoKolors ecosystem. It is designed to be high-performance, memory-efficient, and fully compatible with modern terminals (supporting 24-bit TrueColor RGB, 8-bit ANSI colors, and special layout inheritance states).

---

## 1. Memory Layout and Efficiency

Unlike typical color classes that allocate objects on the heap, `NKColor` is implemented as an explicit layout record struct of size `uint` (32 bits). This layout makes it extremely cache-friendly and allows it to be copied by value with zero allocation overhead.

```csharp
[StructLayout(LayoutKind.Explicit, Size = sizeof(uint))]
public readonly record struct NKColor : IFormattable, IParsable<NKColor>
```

The underlying bits are packed as follows:
- **Bits 0–23 (Lower 24 bits)**: Store the color channel information (RGB values or the 8-bit palette index).
- **Bits 24–31 (Upper 8 bits)**: Store the **Color Type** metadata byte.
- Remaining bits are unused or reserved.

---

## 2. Color Types

An `NKColor` instance can represent four distinct color states, defined by the `ColorType` enum:

| State             | ColorType           | Description                                                 | Example / Constructor             |
|-------------------|---------------------|-------------------------------------------------------------|-----------------------------------|
| **Default**       | `DEFAULT` (0)       | Represents the default terminal color state.                | `NKColor.Default`                 |
| **Console Color** | `CONSOLE_COLOR` (1) | Standard 16 console colors mapped via `NKConsoleColor`.     | `new NKColor(NKConsoleColor.Red)` |
| **RGB TrueColor** | `RGB` (2)           | Full 24-bit RGB TrueColor.                                  | `NKColor.FromRgb(255, 128, 0)`    |
| **Inherit**       | `INHERIT` (3)       | Represents an inherited state (bubbles down layout panels). | `NKColor.Inherit`                 |

---

## 3. Pattern Matching and Value Resolution

To prevent boxing when extracting the color type, `NKColor` provides allocation-free pattern matching methods (`Match` and `Switch`):

### Using `Match<T>`
```csharp
string description = myColor.Match(
    @default => "Using default terminal color",
    rgb      => $"TrueColor RGB hex: #{rgb:x6}",
    palette  => $"8-bit Palette color: {palette}",
    inherit  => "Inheriting color from parent"
);
```

---

## 4. Implicit Conversions and Type Casting

`NKColor` supports seamless implicit conversions to and from standard .NET and Console color types:

```csharp
// Implicit conversion from ConsoleColor
NKColor red = ConsoleColor.Red;

// Implicit conversion from uint/int (TrueColor hex)
NKColor orange = 0xFF5500;

// Casting NKColor back to ConsoleColor (will throw InvalidColorCastException if NKColor is RGB/Default/Inherit)
ConsoleColor systemColor = (ConsoleColor)red;
```

---

## 5. String Formatting and ANSI Escape Codes

The `ToString` method is heavily optimized to return the correct ANSI escape sequences for text coloring based on the color's type and the requested formatting flag:

| Format Code                 | Description                                 | Example Output      |
|-----------------------------|---------------------------------------------|---------------------|
| `"p"` / `"Plain"` / `"Raw"` | A plain textual hex or name representation. | `ff5500` or `Red`   |
| `"t"` / `"Text"` / `"Forg"` | Foreground ANSI escape coloring code.       | `\e[38;2;255;85;0m` |
| `"b"` / `"Bckg"`            | Background ANSI escape coloring code.       | `\e[48;2;255;85;0m` |
| `"u"` / `"Underline"`       | Underline decoration ANSI coloring code.    | `\e[58;2;255;85;0m` |

---

## 6. Utilities and Interpolation

### Linear Interpolation (`Lerp`)
You can smoothly transition between two RGB colors using the static `Lerp` method. This is perfect for custom gradients:
```csharp
NKColor start = NKColor.FromRgb(255, 0, 0); // Red
NKColor end   = NKColor.FromRgb(0, 0, 255); // Blue

// Returns a purple color exactly halfway between red and blue
NKColor half = NKColor.Lerp(start, end, 0.5f);
```

### Color Inverse
The `GetInverse()` method returns the bitwise inverse of an RGB color, or maps the opposite index for palette colors:
```csharp
NKColor color = NKColor.FromRgb(255, 255, 0); // Yellow
NKColor inverse = color.GetInverse();          // Blue
```
