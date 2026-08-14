# NKConsoleColor

**[NKConsoleColor](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKConsoleColor.cs)** is an enum representing the 256-color palette index mapping commonly supported by virtual terminal emulators.

---

## 1. Enum Definition

```csharp
public enum NKConsoleColor : byte
```

It is backed by a `byte` type to keep its memory footprint minimal and ensure that it can be stored directly within packed styling structures.

---

## 2. Color Layout Mapping

The 256-color indices are structured as follows:

| Index Range   | Color Content          | Description                                                                                                                                                            |
|---------------|------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **0 - 15**    | Standard System Colors | The 16 legacy console colors commonly supported by all terminals. These map directly to `System.ConsoleColor` values.                                                  |
| **16 - 231**  | 6x6x6 Color Cube       | High-resolution color spacing. The index of any RGB color cube coordinate can be calculated using:<br>`Index = R * 36 + G * 6 + B + 16`<br>where $R, G, B \in [0, 5]$. |
| **232 - 255** | Grayscale Steps        | 24 gradual steps of gray, running from dark gray to light gray (excluding pure black and pure white, which are located in the standard index range).                   |

---

## 3. Usage Example

You can cast from standard integers, or format the enum to print ANSI escape sequences:

```csharp
using NeoKolors.Common;

// 1. Get color by index
NKConsoleColor color = (NKConsoleColor)120; // Specific green/cyan color cube step

// 2. Add color directly to console output
Console.WriteLine("Styled Text".AddColor(color));
```
