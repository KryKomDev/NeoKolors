# Text Styles and Effects

**[TextStyles](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/TextStyles.cs)** is a bit-flag enum that defines styling effects and text decorations on terminal cells. These map directly to Standard ECMA-48 Graphic Rendition (SGR) escape codes.

---

## 1. Bit-Flag Definitions

```csharp
[Flags]
public enum TextStyles : byte 
{
    None            = 0,
    Bold            = 1 << 0,  // ANSI SGR 1
    Faint           = 1 << 1,  // ANSI SGR 2
    Italic          = 1 << 2,  // ANSI SGR 3
    Underline       = 1 << 3,  // ANSI SGR 4
    Blink           = 1 << 4,  // ANSI SGR 5
    Negative        = 1 << 5,  // ANSI SGR 7
    Invisible       = 1 << 6,  // ANSI SGR 8
    Strikethrough   = 1 << 7   // ANSI SGR 9
}
```

---

## 2. Fluently Applying Styles to Strings

The **[StringEffects](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Common/StringEffects.cs)** class provides fluent, chainable extension methods on standard strings. This allows you to apply styling tags and colors inline for console writing:

```csharp
using NeoKolors.Common;

// Apply multiple decorations and colors
string warningText = "CRITICAL WARNING".Bold().Italic().Red();

Console.WriteLine(warningText);
```

### Supported Fluent Styling Methods:
- `.Bold()`
- `.Faint()`
- `.Italic()`
- `.Underline()` (Supports custom decoration types like curly or dashed)
- `.Blink()`
- `.Negative()`
- `.Strikethrough()`
