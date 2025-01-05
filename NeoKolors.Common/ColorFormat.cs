//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using SkiaSharp;

namespace NeoKolors.Common;

/// <summary>
/// contains methods that convert between color formats (<see cref="SKColor"/>, <see cref="Color"/>, ARGB, HSV, IntArgb)
/// </summary>
public static class ColorFormat {

    public static SKColor ColorToSkia(this System.Drawing.Color c) => new(c.R, c.G, c.B);
    public static System.Drawing.Color SkiaToColor(this SKColor c) => System.Drawing.Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);
    
    /// <summary>
    /// converts a color stored in an int to SKColor
    /// </summary>
    /// <param name="c">source int color</param>
    /// <param name="autoAlpha">if true automatically sets alpha to 255</param>
    public static SKColor IntToSkia(this int c, bool autoAlpha = true) {
        return new SKColor((byte)(autoAlpha ? 255 : (byte)(c >> 24)), (byte)(c >> 16), (byte)(c >> 8), (byte)c);
    }
    
    /// <summary>
    /// converts a color stored in an int to SKColor
    /// </summary>
    /// <param name="c">source int color</param>
    /// <param name="autoAlpha">if true automatically sets alpha to 255</param>
    public static System.Drawing.Color IntToColor(this int c, bool autoAlpha = true) {
        return System.Drawing.Color.FromArgb((byte)(autoAlpha ? 255 : (byte)(c >> 24)), (byte)(c >> 16), (byte)(c >> 8), (byte)c);
    }
    
    /// <summary>
    /// returns the hue, saturation and value of the supplied color 
    /// </summary>
    public static void ColorToHsv(System.Drawing.Color color, out double hue, out double saturation, out double value) {
        int max = Math.Max(color.R, Math.Max(color.G, color.B));
        int min = Math.Min(color.R, Math.Min(color.G, color.B));

        hue = color.GetHue();
        saturation = max == 0 ? 0 : 1d - 1d * min / max;
        value = max / 255d;
    }

    /// <summary>
    /// returns the hue, saturation and value of the supplied color 
    /// </summary>
    public static (double h, double s, double v) ColorToHsv(System.Drawing.Color color) {
        ColorToHsv(color, out double h, out double s, out double v);
        return (h, s, v);
    }

    /// <summary>
    /// returns the rgb value of the color of supplied hsv values 
    /// </summary>
    public static (int r, int g, int b) HsvToRgb(double hue, double saturation, double value) {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);
        
        value *= 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        return hi switch {
            0 => (v, t, p),
            1 => (q, v, p),
            2 => (p, v, t),
            3 => (p, q, v),
            4 => (t, p, v),
            _ => (v, p, q)
        };
    }
    
    /// <summary>
    /// creates an instance of <see cref="Color"/> from the hsv values
    /// </summary>
    public static System.Drawing.Color HsvToColor(double hue, double saturation, double value) {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);
        
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
        var c = HsvToRgb(h, s, v);
        return c.r << 16 | c.g << 8 | c.b;
    }
}