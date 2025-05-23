﻿//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Drawing;
using System.Globalization;
using static System.Math;

namespace NeoKolors.Common;

public readonly struct NKColorPalette {
    
    public int this[int index] => _colors[index];
    private readonly NKColor[] _colors;
    public NKColor[] Colors => _colors;
    public int Length => _colors.Length;

    /// <summary>
    /// returns the stored color palette
    /// </summary>
    public Color[] GetColors => _colors.Select(c => Color.FromArgb(c + (0xff << 24))).ToArray();

    /// <summary>
    /// creates a new ordered color palette from a set of integers, where the bytes mean AARRGGBB
    /// </summary>
    /// <param name="colors">the field of integers representing argb colors</param>
    /// <param name="autoAlpha">sets alpha channel of every color to #ff if true</param>
    public NKColorPalette(NKColor[] colors, bool autoAlpha = true) {
        _colors = colors;

        if (autoAlpha) {
            for (int i = 0; i < _colors.Length; i++) {
                _colors[i] |= 0xff << 24;
            }
        }
    }

    /// <summary>
    /// creates a new ordered color palette from a set of colors
    /// </summary>
    public NKColorPalette(Color[] colors) {
        _colors = new NKColor[colors.Length];

        for (int i = 0; i < colors.Length; i++) {
            _colors[i] = colors[i].ToArgb();
        }
    }

    /// <summary>
    /// creates a new palette from the format "rrggbb-rrggbb-..."
    /// </summary>
    /// <param name="url">the string</param>
    public NKColorPalette(string url) {

        _colors = new NKColor[(url.Length + 1) / 7];

        for (int i = 0; i < url.Length; i += 7) {
            string colorRaw = "" + url[i] + url[i + 1] + url[i + 2] + url[i + 3] + url[i + 4] + url[i + 5];

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
        
        // foreach (int c in colors) {
        //     Console.Write($"{(byte)(c >> 16):X2} ".AddColor((byte)(c >> 16) << 16));
        // }
        //
        // Console.Write("\n");
        //
        // foreach (int c in colors) {
        //     Console.Write($"{(byte)(c >> 8):X2} ".AddColor((byte)(c >> 8) << 8));
        //
        // }
        //
        // Console.Write("\n");
        //
        // foreach (int c in colors) {
        //     Console.Write($"{(byte)(c >> 0):X2} ".AddColor((byte)(c >> 0) << 0));
        //
        // }
        //
        // Console.Write("\n");
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
    public static NKColorPalette GeneratePalette(int seed, int colorCount = 10) {
        NKColorPalette palette = new NKColorPalette(new NKColor[colorCount]);

        Random rnd = new Random(seed);

        var a = (rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
        var b = (rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
        var c = (rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
        var d = (rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
        
        // Console.WriteLine($"{a.Item1}, {a.Item2}, {a.Item3}");
        // Console.WriteLine($"{b.Item1}, {b.Item2}, {b.Item3}");
        // Console.WriteLine($"{c.Item1}, {c.Item2}, {c.Item3}");
        // Console.WriteLine($"{d.Item1}, {d.Item2}, {d.Item3}");
        
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
}