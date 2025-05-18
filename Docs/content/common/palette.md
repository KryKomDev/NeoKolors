+++
date = '2025-05-16T17:12:28+02:00'
draft = true
title = 'NKColorPalette'
+++

```C#
public readonly struct NKColorPalette;
```

A color palette holding instances of `NKColor`

## Constructors

```C#
public NKColorPalette(string url);
```

Creates a new palette from a string with format `"rrggbb-rrggbb-..."`.

```C#
public NKColorPalette(Color[] colors);
public NKColorPalette(NKColor[] colors, bool autoAlpha = true);
```

---

## PrintPalette

```C#
public void PrintPalette();
```

Prints `this` palette to the standard output stream using dots colored by ANSI escape characters.

```C#
public void PrintPalette(Action<int> print);
```
Prints `this` palette to the standard output stream using a custom `Action<int>`.

---

## GeneratePalette

```C#
public static NKColorPalette GeneratePalette(int seed, int colorCount = 10);
```

Generates a color palette using a seed.

### Params
- `seed` - a generation seed for a `Random` instance
- `colorCount` - specifies how many colors will the resulting palette contain.

## GenerateColorAtX

```C#
public static Color GenerateColorAtX(
        (double R, double G, double B) a,
        (double R, double G, double B) b,
        (double R, double G, double B) c,
        (double R, double G, double B) d, 
        double x);
```

Returns the color located at `X` in a graph of `color = A + B * Cos(2 * PI * (Cx + D))`
where `A`, `B`, `C` and `D` are 3d vectors representing a color (all their values should be between 1 and 0).