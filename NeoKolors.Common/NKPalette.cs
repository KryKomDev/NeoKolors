//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Drawing;
using NeoKolors.Common.Util;
using static System.Math;

namespace NeoKolors.Common;

public readonly struct NKPalette : IFormattable {
    
    public int this[int index] => _colors[index];
    private readonly NKColor[] _colors;
    public NKColor[] Colors => _colors;
    public int Length => _colors.Length;
    public string Preview => ToPreview();
    
    public NKColor Base => _colors[0];
    public NKColor Background => _colors[1];
    public NKColor Text => _colors[2];
    public NKColor TextSecondary => _colors[3];
    public NKColor Accent => _colors[4];
    
    
    /// <summary>
    /// creates a new, ordered color palette from a set of integers, where the bytes mean AARRGGBB
    /// </summary>
    /// <param name="colors">the field of integers representing argb colors</param>
    public NKPalette(NKColor[] colors) {
        if (colors.Length < 5) throw new ArgumentException("Any palette must contain at least 5 colors.");
        _colors = colors;
    }

    /// <summary>
    /// creates a new, ordered color palette from a set of colors
    /// </summary>
    public NKPalette(Color[] colors) {
        if (colors.Length < 5) throw new ArgumentException("Any palette must contain at least 5 colors.");
        
        _colors = new NKColor[colors.Length];

        for (int i = 0; i < colors.Length; i++) {
            _colors[i] = colors[i].ToArgb();
        }
    }

    /// <summary>
    /// creates a new palette from the format "rrggbb-rrggbb-..."
    /// </summary>
    /// <param name="url">the string</param>
    public NKPalette(string url) {

        _colors = new NKColor[(url.Length + 1) / 7];

        if (_colors.Length < 5) throw new ArgumentException("Any palette must contain at least 5 colors.");
        
        for (int i = 0; i < url.Length; i += 7) {
            string colorRaw = url.Substring(i, 6);

            _colors[i / 7] = int.Parse(colorRaw, NumberStyles.HexNumber);
        }
    }

    /// <summary>
    /// prints a palette to the console
    /// </summary>
    public void PrintPalette() {
        foreach (int c in _colors) {
            Console.Write("● ".AddColor(c));
        }
        
        Console.Write("\n");
    }

    /// <summary>
    /// uses a custom action to print a palette 
    /// </summary>
    /// <param name="print">print delegate that prints a single color</param>
    public void PrintPalette(Action<NKColor> print) {
        foreach (var c in _colors) {
            print(c);
        }
        
        Console.Write("\n");
    }

    /// <summary>
    /// generates a new random visually pleasing palette 
    /// </summary>
    /// <param name="seed">seed for random</param>
    /// <param name="colorCount">how many colors will the palette contain</param>
    public static NKPalette GeneratePalette(int seed, int colorCount = 10) {
        if (colorCount < 5) throw new ArgumentException("Any palette must contain at least 5 colors.");
        
        var palette = new NKPalette(new NKColor[colorCount]);

        var rnd = new Random(seed);

        var a = (rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
        var b = (rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
        var c = (rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
        var d = (rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
        
        for (int i = 0; i < colorCount; i++) {
            Color color = GenerateColorAtX(a, b, c, d, (float)i / colorCount * 3);
            palette._colors[i] = color.R << 16 | color.G << 8 | color.B;
        }
        
        return palette;
    }

    /// <summary>
    /// returns the color located at X in a graph of <c>color = A + B * Cos(2 * PI * (Cx + D))</c>
    /// where A, B, C and D are 3d vectors representing a color (all their values should be between 1 and 0)
    /// </summary>
    public static Color GenerateColorAtX(
        (double R, double G, double B) a,
        (double R, double G, double B) b,
        (double R, double G, double B) c,
        (double R, double G, double B) d, double x) 
    {
        double re = Cos(2 * PI * (c.R + d.R * 2 * x));
        double gr = Cos(2 * PI * (c.G + d.G * 2 * x));
        double bl = Cos(2 * PI * (c.B + d.B * 2 * x));
        
        
        return Color.FromArgb(
            Normalize(a.R + b.R * re),
            Normalize(a.G + b.G * gr),
            Normalize(a.B + b.B * bl));
    }

    private static byte Normalize(double d) => (byte)((d / 3 + 1f/3f) * 255);

    /// <summary>
    /// Converts the NKPalette instance to its string representation.
    /// </summary>
    /// <returns>A string representation of the palette, where colors are concatenated in their formatted string representation.
    /// Each color is represented using the "#r" format.</returns>
    public override string ToString() => _colors.Join(" ", c => c.ToString("#r"));

    /// <summary>
    /// Converts the colors in the palette to a preview string where each color is represented by a colored bullet symbol.
    /// </summary>
    /// <returns>A string containing colored bullet symbols for each color in the palette.</returns>
    public string ToPreview() => _colors.Join(" ", c => "●".AddColor(c));

    /// <summary>
    /// Converts the color palette into a URL-encoded string representation,
    /// where colors are joined with a hyphen.
    /// </summary>
    /// <returns>A URL-encoded string representing the palette's colors.</returns>
    public string ToUrl() => _colors.Join("-", c => c.ToString("r"));

    /// <summary>
    /// Converts the NKPalette to its string representation using the default format.
    /// </summary>
    /// <returns>A string representation of the palette, where colors are concatenated using the "#r" format.</returns>
    public string ToString(string? format, IFormatProvider? formatProvider) {
        format ??= "p";
        return format switch {
            "p" or "P" or "Preview" => Preview,
            "u" or "U" or "Url" => ToUrl(),
            _ => ToString()
        };
    }
}