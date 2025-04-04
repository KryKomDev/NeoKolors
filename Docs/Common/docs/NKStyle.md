# NKStyle

```C#
public struct NKStyle : ICloneable, IEquatable<NKStyle>;
```

Contains information about console styles (bg / fg color, bold, italic etc.).

## Properties

### Raw

```C#
public ulong Raw { get; private set; }
```

Contains the compressed styles.

| bits    | meaning                                                         |
|---------|-----------------------------------------------------------------|
| 00 - 23 | foreground color                                                |
| 24 - 47 | background color                                                |
| 56 - 61 | bitmap of text styles                                           |
| 62      | toggles terminal palette mode for foreground color (1 = custom) |
| 62      | toggles terminal palette mode for background color (1 = custom) |

#### Palette Color mode and Custom Color mode

Palette color mode indicates that one of the colors from `NKConsoleColor` (or `System.ConsoleColor`)
is in use. `NKConsoleColor` is byte so it takes the least significant byte in the space. If the 9th 
least significant byte is on the color is the default.

Custom color mode indicates that a custom 24bit color is in use. the format is the classic RGB.

### FColor

```C#
public NKColor FColor { get; set; }
```

Represents the color of the text.

### BColor

```C#
public NKColor FColor { get; set; }
```

Represents the color of the background.

### Styles 

```C#
public TextStyles Styles { get; set; }
```

Contains the styles of the text.

### IsFColorCustom

```C#
public bool IsFColorCustom { get; }
```

### IsBColorCustom

```C#
public bool IsBColorCustom { get; }
``` 

### IsFColorDefault

```C#
public bool IsFColorDefault { get; }
```

### IsBColorDefault

```C#
public bool IsBColorDefault { get; }
```

---

## Methods

### SetFColor(NKColor)

```C#
public void SetFColor(NKColor color);
```

Sets the text color using the universal color format.

### SetBColor(NKColor)

```C#
public void SetBColor(NKColor color);
```

Sets the background color using the universal color format.

### SetFColor(UInt24)

```C#
public void SetFColor(UInt24 color);
```

Sets the text color using an RGB.

### SetBColor(UInt24)

```C#
public void SetBColor(UInt24 color);
```

Sets the background color using an RGB.

### SetFColor(NKConsoleColor)

```C#
public void SetFColor(NKConsoleColor color);
```

Sets the text color using a palette color.

### SetBColor(NKConsoleColor)

```C#
public void SetBColor(NKConsoleColor color);
```

Sets the background color using a palette color.

### SetFColor()

```C#
public void SetFColor();
```

Sets the text color to the default. Works only in console.

### SetBColor()

```C#
public void SetBColor();
```

Sets the background color to the default. Works only in console.

### GetFColor()

```C#
public NKColor GetFColor();
```

Returns the text color using the universal color format.

### GetBColor()

```C#
public NKColor GetBColor();
```

Returns the background color using the universal color format.

### SetStyles(TextStyles)

```C#
public void SetStyles(TextStyles styles);
```

Sets the text styles.

### GetStyles()

```C#
public TextStyles GetStyles();
```

Returns the text styles.