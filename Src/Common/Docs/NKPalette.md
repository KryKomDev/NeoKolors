# Color Palettes and Schemes

**[NKPalette](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKPalette.cs)** is a readonly utility struct that represents a structured collection of at least 5 colors. It acts as the core theme provider for TUI components, layouts, and styled console output.

---

## 1. Structure of a Theme Palette

An `NKPalette` contains a minimum of 5 mapped colors. By convention, these are structured at specific indices to define a UI theme layout:

| Index | Property        | Description                                                                 |
|-------|-----------------|-----------------------------------------------------------------------------|
| **0** | `Base`          | The primary brand or key UI background color.                               |
| **1** | `Background`    | The secondary panel, border, or card background color.                      |
| **2** | `Text`          | The main high-contrast text color (e.g. white or light gray).               |
| **3** | `TextSecondary` | The secondary text color for captions or descriptions.                      |
| **4** | `Accent`        | The highlighting color for hovered/focused buttons, selections, or accents. |

---

## 2. Instantiating a Palette

`NKPalette` offers multiple constructors depending on your color source:

### 2.1 From an Array of Colors
Initialize a palette directly using an array of `NKColor` or `System.Drawing.Color` objects:
```csharp
var myColors = new NKColor[] {
    NKColor.FromRgb(30, 30, 30),   // Base
    NKColor.FromRgb(45, 45, 45),   // Background
    NKColor.FromRgb(240, 240, 240), // Text
    NKColor.FromRgb(180, 180, 180), // TextSecondary
    NKColor.FromRgb(0, 180, 255)    // Accent
};

var palette = new NKPalette(myColors);
```

### 2.2 From URL-Encoded Hyphen Strings
You can load themes directly from a hyphenated hex string (useful when importing palettes from online generation sites):
```csharp
// Format: "rrggbb-rrggbb-rrggbb-rrggbb-rrggbb"
var urlPalette = new NKPalette("1e1e1e-2d2d2d-f0f0f0-b4b4b4-00b4ff");
```

---

## 3. Cosine-Based Visual Palette Generation

To construct visually pleasing color gradients automatically, `NKPalette` includes a seed-based procedural generator that uses Cosine-based color waves:

$$\text{Color}(x) = A + B \times \cos(2\pi \times (C \times x + D))$$

Where $A, B, C,$ and $D$ are 3D vector parameters representing channel weights, scale, frequency, and phase offsets:

```csharp
// Generate a harmonious 10-color scheme using a random seed
NKPalette procedualTheme = NKPalette.GeneratePalette(seed: 42, colorCount: 8);
```

This mathematical approach guarantees that colors in the generated palette blend together harmoniously without producing harsh, clashing tones.

---

## 4. Built-in Palettes (`NKPalettes`)

The static class **[NKPalettes](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/NKPalettes.cs)** contains pre-configured theme options ready to use:

* **`Nord`**: Cool, ice-blue Scandinavian-inspired palette.
* **`OneDark`**: Standard modern IDE dark theme.
* **`Dracula`**: Vibrant gothic purple-accented layout.
* **`Catppuccin`**: Soft pastel-themed palette.
* **`Gruvbox`**: Retro warm-toned pastel palette.

### Usage Example
```csharp
using NeoKolors.Common;

NKPalette theme = NKPalettes.Nord;
Console.WriteLine($"Accent Color: {theme.Accent}");
```
