# ColorFormat

```C#
public static class ColorFormat;
```

The diagram below shows the different conversions possible:

```Mermaid
graph TD;
    SkiaSharp.SKColor --SkiaToColor--> System.Drawing.Color;
    SkiaSharp.SKColor --SkiaToInt--> HEX;
    HEX --IntToSkia--> SkiaSharp.SKColor;
    SkiaSharp.SKColor --SkiaToHsv--> HSV;
    System.Drawing.Color --ColorToSkia--> SkiaSharp.SKColor;
    HSV --HsvToSkia--> SkiaSharp.SKColor;
    HEX --IntToColor--> System.Drawing.Color;
    System.Drawing.Color --ColorToInt--> HEX;
    System.Drawing.Color --ColorToHsv--> HSV;
    HSV --HsvToColor--> System.Drawing.Color;
    HSV --HsvToInt--> HEX;
    HEX --IntToHsv--> HSV;
    NKConsoleColor --NKToSystem--> System.ConsoleColor;
    System.ConsoleColor --SystemToNK--> NKConsoleColor;
```

The names in the middle of the lines represent the conversion function.

Note that conversions to and from `NKColor` are not present as there
are conversions available directly inside the `NKColor` class. 