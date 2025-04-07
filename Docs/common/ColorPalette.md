# ColorPalette

```C#
public class ColorPalette;
```

Holds a color palette.

## Properties

### this[int]

```C#
public int this[int index];
```

Returns the HEX-as-int color at the position of index;

### Colors

```C#
public Color[] Colors;
```

Returns all colors stored inside the palette.

### Length

```C#
public int Length;
```

Returns the number of colors stored inside the palette.

## Constructors

```C#
public ColorPalette(Color[] colors, bool autoAlpha = true);
public ColorPalette(System.Drawing.Color[] colors);
```

### ColorPalette(string)

```C#
public ColorPalette(string url);
```

Creates a new palette from the format "rrggbb-rrggbb-...".

## Methods

### PrintPalette

```C#
public void PrintPalette();
public void PrintPalette(Action<int> print);
```

Prints the palette to the console using colorful dots.

The second overload uses a custom action to print a single color.

### GeneratePalette

```C#
public static ColorPalette GeneratePalette(int seed, int colorCount);
```

Randomly generates a new coherent palette using the seed integer.

### GenerateColorAtX

```C#
public static System.Drawing.Color GenerateColorAtX(
        (double R, double G, double B) a,
        (double R, double G, double B) b,
        (double R, double G, double B) c,
        (double R, double G, double B) d, 
        double x);
```

Generates a color at the position of X using a set of variable sinusoids.