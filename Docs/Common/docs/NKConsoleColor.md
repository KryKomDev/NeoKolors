# NKConsoleColor

```C#
public enum NKConsoleColor : byte;
```

Contains the console palette colors. Unlike the`System.ConsoleColor` 
NKConsoleColor also includes the color cube colors and 24 grayscale colors.

## Basic colors

Items 0 - 15. These colors are also contained in `System.ConsoleColor`. However, they are
ordered differently due to the ANSI color escape sequence format.

## Cube colors

Items 16 - 231. Colors ordered in the 6 x 6 x 6 color cube. 
A colors index can be gotten with formula: `(R / 255) * 36 + (G / 255) * 6 + B + 16`. 
The actual color can differ from the expected color due to different implementations of these colors
in different terminal emulators.

## Grayscale colors

Items 232 - 255. Colors with rising color value and 0 saturation.
The actual color can differ from the expected color due to different implementations of these colors
in different terminal emulators.