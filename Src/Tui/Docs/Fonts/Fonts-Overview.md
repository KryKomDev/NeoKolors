# NeoKolors TUI Fonts Overview

The `NeoKolors.Tui.Fonts` package (and its companion asset library `NeoKolors.Tui.Fonts.Assets`) forms the typography and text rendering engine of the NeoKolors TUI ecosystem. It provides parsing, serialization, measurement, and rendering capabilities for stylized ASCII and ANSI fonts on the character grid.

---

## 1. Typography Core

### 1.1 Font Assets
NeoKolors includes built-in stylized fonts that allow drawing large banner text and decorative headers:
* **`Bytesized`**: A compact, block-based font style.
* **`Future`**: A modern sci-fi styled typography look.
* **`Dummy`**: Fallback font style used for diagnostics.

### 1.2 Font Compilation MSBuild Task
The assets project embeds an MSBuild build pipeline task. During project compilation, it automatically packages custom fonts (`.nkf` files) from source assets into optimized embedded resources, ensuring zero-configuration runtime availability.

---

## 2. Font XML Specifications

Custom fonts can be defined using structured XML definitions:
* **[Font Definition Config](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Docs/Fonts/Xml/Xml-Font-Definition-Config.md)**: Declares font metrics, height offsets, tracking spacing, and fallbacks.
* **[Glyph Definition Mapping](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Docs/Fonts/Xml/Xml-Font-Definition-Mapping.md)**: Maps ASCII/Unicode characters to visual rendering blocks (grids of characters).
* **[Glyph Properties](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Docs/Fonts/Xml/Xml-Glyph-Definition-Properties.md)**: Allows masking and styling specific cells of a glyph representation.
