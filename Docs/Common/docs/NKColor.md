# NKColor

```C#
public class NKColor : ICloneable, IEquatable<NKColor>;
```

The `Color` class can hold either `ConsoleColor` or a custom color defined by an `Int32`.
This feature can be useful when working with console.

## Fields

### Color

```C#
public OneOf<int, NKConsoleColor, DefaultColor> Color { get; }
```

### R, G, B

```C#
public byte R;
public byte G;
public byte B;
```

Properties that return the individual color channel values as `byte`.   

### ControlChar

```C#
public string ControlChar;
```

Returns an ANSI escape code for coloring text by the color held by the instance.

### ControlCharB

```C#
public string ControlCharB;
```

Returns an ANSI escape code for coloring background by the color held by the instance.


## Methods

### Write

```C#
public void Write();
```

Prints a colored dot and a hexadecimal representation of the color, if custom, or name,
if palette, to the console.

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

## FromArgb

```c#
public static NKColor FromArgb(int color);
public static NKColor FromArgb(byte r, byte g, byte b);
public static NKColor FromArgb(byte a, byte r, byte g, byte b);
public static NKColor FromArgb(UInt24 color);
```

Creates a new NKColor