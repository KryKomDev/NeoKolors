+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'NKColor'
+++

```C#
public class NKColor : ICloneable, IEquatable<NKColor>;
```

The `Color` class can hold either `ConsoleColor` or a custom color defined by an `Int32`.
This feature can be useful when working with console.

---

## Color

```C#
public OneOf<int, NKConsoleColor, DefaultColor, InheritColor> Color { get; private set; }
```

Holds the actual color.

---

## R, G, B, A

```C#
public byte R { get; set; }
public byte G { get; set; }
public byte B { get; set; }
public byte A { get; set; }
```

Properties that return the individual color channel values as `byte`.

### Exceptions
- **`InvalidColorCastException`** - `this` instance is `DefaultColor` or `InheritColor` or `NKConsoleColor` **or**
     tried to set a color channel when `this.Color` was `DefaultColor` or `InheritColor` or `NKConsoleColor`.

---

## Text

```C#
public string Text;
```

Represents the ANSI control string or textual representation
of the foreground color associated with the current `NKColor` instance.

---

## Background

```C#
public string Bckg;
```

Represents the ANSI control string or textual representation
of the background color associated with the current `NKColor` instance.

---

## Write

```C#
public void Write();
```

Writes a visual representation of `this` color to standard output stream.

---

## Casting
```C#
public static implicit operator Color(ConsoleColor color);
public static implicit operator Color(int color);
public static implicit operator Color(UInt24 color);
public static implicit operator ConsoleColor(Color color);
public static implicit operator int(Color color);
public static implicit operator UInt24(Color color);
public static implicit operator System.Drawing.Color(Color color);
```

---

## ToString

```C#
public string ToString(string? format, IFormatProvider? formatProvider);
public string ToString(string? format);
```

### Formats
- `"p"` or `"P"` or `"Plain"` or `"r"` or `"R"` or `"Raw"` - Returns a visual representation of the color.
- `"t"` or `"T"` or `"Text"` or `"f"` or `"F"` or `"Forg"` - Returns an ANSI escape sequence for coloring text.
- `"b"` or `"B"` or `"Bckg"` - Returns an ANSI escape sequence for coloring text.

```C#
public override string ToString();
```

Returns a visual representation of the color.

---

## GetInverse

```C#
public NKColor GetInverse();
```

Returns the inverse color of `this` instance.

---

## PrintColorCube

```c#
public static void PrintColorCube();
```

Prints the 6x6x6 color cube.

---

## FromArgb

```c#
public static NKColor FromArgb(int color);
public static NKColor FromArgb(byte r, byte g, byte b);
public static NKColor FromArgb(byte a, byte r, byte g, byte b);
public static NKColor FromArgb(UInt24 color);
```

Creates a new NKColor