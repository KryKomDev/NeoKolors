//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using SkiaSharp;
using static System.ConsoleColor;
using static System.Math;
using static NeoKolors.Common.NKConsoleColor;

namespace NeoKolors.Common;

/// <summary>
/// contains methods that convert between color formats (<see cref="SKColor"/>, <see cref="NKColor"/>, ARGB, HSV, IntArgb)
/// </summary>
public static class ColorFormat {
    
    public static SKColor ColorToSkia(this System.Drawing.Color c) => new(c.R, c.G, c.B);
    public static System.Drawing.Color SkiaToColor(this SKColor c) => System.Drawing.Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);
    
    /// <summary>
    /// converts a color stored in an int to SKColor
    /// </summary>
    /// <param name="c">source int color</param>
    /// <param name="autoAlpha">if true automatically sets alpha to 255</param>
    public static SKColor IntToSkia(this int c, bool autoAlpha = true) =>
        new((byte)(c >> 16), (byte)(c >> 8), (byte)c, (byte)(autoAlpha ? 255 : (byte)(c >> 24)));

    public static int SkiaToInt(this SKColor c) => c.Alpha << 24 | c.Red << 16 | c.Green << 8 | c.Blue;
    
    public static SKColor HsvToSkia(double hue, double saturation, double value) {
        int hi = Convert.ToInt32(Floor(hue / 60)) % 6;
        double f = hue / 60 - Floor(hue / 60);
        
        value *= 255;
        byte v = Convert.ToByte(value);
        byte p = Convert.ToByte(value * (1 - saturation));
        byte q = Convert.ToByte(value * (1 - f * saturation));
        byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

        return hi switch {
            0 => new SKColor(v, t, p),
            1 => new SKColor(q, v, p),
            2 => new SKColor(p, v, t),
            3 => new SKColor(p, q, v),
            4 => new SKColor(t, p, v),
            _ => new SKColor(v, p, q)
        };
    }

    public static void SkiaToHsv(SKColor c, out double h, out double s, out double v) {
        int max = Max(c.Red, Max(c.Green, c.Blue));
        int min = Min(c.Red, Min(c.Green, c.Blue));

        h = c.Hue;
        s = max == 0 ? 0 : 1d - 1d * min / max;
        v = max / 255d;
    }
    
    /// <summary>
    /// converts a color stored in an int to SKColor
    /// </summary>
    /// <param name="c">source int color</param>
    /// <param name="autoAlpha">if true automatically sets alpha to 255</param>
    public static System.Drawing.Color IntToColor(this int c, bool autoAlpha = true) =>
        System.Drawing.Color.FromArgb((byte)(autoAlpha ? 255 : (byte)(c >> 24)), (byte)(c >> 16), (byte)(c >> 8), (byte)c);

    public static int ColorToInt(this System.Drawing.Color c) => c.A << 24 | c.R << 16 | c.G << 8 | c.B;
    
    /// <summary>
    /// returns the hue, saturation and value of the supplied color 
    /// </summary>
    public static void ColorToHsv(this System.Drawing.Color color, out double hue, out double saturation, out double value) {
        int max = Max(color.R, Max(color.G, color.B));
        int min = Min(color.R, Min(color.G, color.B));

        hue = color.GetHue();
        saturation = max == 0 ? 0 : 1d - 1d * min / max;
        value = max / 255d;
    }

    /// <summary>
    /// returns the hue, saturation and value of the supplied color 
    /// </summary>
    public static (double h, double s, double v) ColorToHsv(this System.Drawing.Color color) {
        ColorToHsv(color, out double h, out double s, out double v);
        return (h, s, v);
    }
    
    /// <summary>
    /// creates an instance of <see cref="NKColor"/> from the hsv values
    /// </summary>
    public static System.Drawing.Color HsvToColor(double hue, double saturation, double value) {
        int hi = Convert.ToInt32(Floor(hue / 60)) % 6;
        double f = hue / 60 - Floor(hue / 60);
        
        value *= 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        return hi switch {
            0 => System.Drawing.Color.FromArgb(v, t, p),
            1 => System.Drawing.Color.FromArgb(q, v, p),
            2 => System.Drawing.Color.FromArgb(p, v, t),
            3 => System.Drawing.Color.FromArgb(p, q, v),
            4 => System.Drawing.Color.FromArgb(t, p, v),
            _ => System.Drawing.Color.FromArgb(v, p, q)
        };
    }
    
    /// <summary>
    /// turns hsv values into an rgb int
    /// </summary>
    public static int HsvToInt(float h, float s, float v){
        var c = HsvToColor(h, s, v);
        return c.R << 16 | c.G << 8 | c.B;
    }

    public static void IntToHsv(this int c, out double h, out double s, out double v) => IntToColor(c).ColorToHsv(out h, out s, out v);
    public static (double h, double s, double v) IntToHsv(this int c) => IntToColor(c).ColorToHsv();

    /// <summary>
    /// converts the basic <see cref="ConsoleColor"/> to the extended <see cref="NKConsoleColor"/>
    /// </summary>
    /// <param name="color">the color to converts</param>
    public static NKConsoleColor SystemToNK(ConsoleColor color) {
        return color switch {
            Black or DarkGreen or DarkMagenta or Gray or DarkGray or Green or Magenta or White => (NKConsoleColor)color,
            DarkBlue => DARK_BLUE,
            DarkCyan => DARK_CYAN,
            DarkRed => DARK_RED,
            DarkYellow => DARK_YELLOW,
            Blue => BLUE,
            Cyan => CYAN,
            Red => RED,
            Yellow => YELLOW,
            _ => WHITE
        };
    }

    /// <summary>
    /// converts the extended <see cref="NKConsoleColor"/> to the basic <see cref="ConsoleColor"/>
    /// </summary>
    /// <param name="color">the color to convert</param>
    /// <exception cref="InvalidColorCastException">
    /// the passed color is not included in the standard set of colors
    /// </exception>
    public static ConsoleColor NKToSystem(NKConsoleColor color) {
        ThrowIf((byte)color >= 16, InvalidColorCastException.NKToSystem(color));
        return color switch {
            BLACK or DARK_GREEN or DARK_MAGENTA or GRAY or DARK_GRAY or GREEN or MAGENTA or WHITE => (ConsoleColor)color,
            DARK_BLUE => DarkBlue,
            DARK_CYAN => DarkCyan,
            DARK_RED => DarkRed,
            DARK_YELLOW => DarkYellow,
            BLUE => Blue,
            CYAN => Cyan,
            RED => Red,
            YELLOW => Yellow,
            _ => White
        };
    }

    public static NKColor SkiaToNK(this SKColor color) =>
        NKColor.FromArgb(color.Red, color.Green, color.Blue);
}