//
// NeoKolors
// Copyright (c) 2024 KryKom
//


using static System.Console;
using NeoKolors.Console;

namespace NeoKolors.Common;

public class ColorPalette {
    
    public int this[int index] => colors[index];
    private readonly int[] colors;
    
    /// <summary>
    /// returns the stored color palette as a set of integers, where the bytes mean AARRGGBB
    /// </summary>
    public int[] Colors => colors;

    /// <summary>
    /// returns the stored color palette
    /// </summary>
    public System.Drawing.Color[] GetColors => colors.Select(c => System.Drawing.Color.FromArgb(c + (0xff << 24))).ToArray();

    /// <summary>
    /// creates a new ordered color palette from a set of integers, where the bytes mean AARRGGBB
    /// </summary>
    /// <param name="colors">the field of integers representing argb colors</param>
    /// <param name="autoAlpha">sets alpha channel of every color to #ff if true</param>
    public ColorPalette(int[] colors, bool autoAlpha = true) {
        this.colors = colors;

        if (autoAlpha) {
            for (int i = 0; i < this.colors.Length; i++) {
                this.colors[i] |= 0xff << 24;
            }
        }
    }

    /// <summary>
    /// creates a new ordered color palette from a set of colors
    /// </summary>
    public ColorPalette(System.Drawing.Color[] colors) {
        this.colors = new int[colors.Length];

        for (int i = 0; i < colors.Length; i++) {
            this.colors[i] = colors[i].ToArgb();
        }
    }

    /// <summary>
    /// creates a new palette from the format "rrggbb-rrggbb-..."
    /// </summary>
    /// <param name="url">the string</param>
    public ColorPalette(string url) {

        colors = new int[(url.Length + 1) / 7];

        for (int i = 0; i < url.Length; i += 7) {
            string colorRaw = "" + url[i] + url[i + 1] + url[i + 2] + url[i + 3] + url[i + 4] + url[i + 5];

            colors[i / 7] = int.Parse(colorRaw, System.Globalization.NumberStyles.HexNumber);
        }
    }

    /// <summary>
    /// prints a palette to the console
    /// </summary>
    public void PrintPalette() {
        foreach (int c in colors) {
            ConsoleColors.PrintColored("● ", c);
        }
        
        Write("\n");
    }

    /// <summary>
    /// uses a custom action to print a palette 
    /// </summary>
    /// <param name="print">print delegate that prints a single color</param>
    public void PrintPalette(Action<int> print) {
        foreach (var c in colors) {
            print(c);
        }
        
        Write("\n");
    }

    /// <summary>
    /// generates a new random visually pleasing palette 
    /// </summary>
    /// <param name="seed">seed for random</param>
    /// <param name="colorCount">how many colors will the palette contain</param>
    public static ColorPalette GeneratePalette(int seed, int colorCount = 10) {
        ColorPalette palette = new ColorPalette(new int[colorCount]);

        Random rnd = new Random(seed);
        
        var a = (rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle());
        var b = (rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle());
        var c = (rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle());
        var d = (rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle());

        for (int i = 0; i < colorCount; i++) {
            System.Drawing.Color color = GenerateColorAtX(a, b, c, d, (float)i / colorCount);
            palette.colors[i] = color.R << 16 | color.G << 8 | color.B;
        }
        
        return palette;
    }

    /// <summary>
    /// returns the color located at X in a graph of <c>color = A + B * Cos(2 * PI * (Cx + D))</c>
    /// where A, B, C and D are 3d vectors representing a color (all their values should be between 1 and 0)
    /// </summary>
    public static System.Drawing.Color GenerateColorAtX(
        (double R, double G, double B) A, 
        (double R, double G, double B) B,
        (double R, double G, double B) C, 
        (double R, double G, double B) D, double x) 
    {
        System.Drawing.Color result = System.Drawing.Color.FromArgb(
            (byte)((A.R + B.R * Math.Cos(2 * Math.PI * (C.R * x + D.R))) * 255),
            (byte)((A.G + B.G * Math.Cos(2 * Math.PI * (C.G * x + D.G))) * 255),
            (byte)((A.B + B.B * Math.Cos(2 * Math.PI * (C.B * x + D.B))) * 255));
        
        return result;
    }
}