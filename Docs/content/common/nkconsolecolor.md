+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'NKConsoleColor'
+++

```C#
public enum NKConsoleColor : byte;
```

Contains 256 ANSI-predefined colors.

## Layout

| Items     | Content                                                                                                   |
|-----------|-----------------------------------------------------------------------------------------------------------|
| 0   - 15  | 16 colors commonly used in terminals. These are also included in `System.ConsoleColor`                    |
| 16  - 231 | 6 * 6 * 6 color cube colors. The index of a color can be computed with `Index = R * 36 + G * 6 + B + 16`. |
| 232 - 255 | 24 grayscale colors.                                                                                      |
