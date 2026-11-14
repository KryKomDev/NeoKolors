// NeoKolors
// Copyright (c) krystof 2026

using System.Drawing;

namespace NeoKolors.Common;

public static class PaletteGenerator {

    /// <summary>
    ///     Specifies the various types of color palette generation strategies available in the
    ///     PaletteGenerator.
    /// </summary>
    public enum PaletteType {

        /// <summary>
        ///     Generates a color palette by randomly selecting colors with no specific pattern or
        ///     constraint.
        /// </summary>
        RANDOM,

        /// <summary>
        ///     Generates a color palette where each color is based on variations of a single base
        ///     color,
        ///     typically by adjusting its saturation and lightness while keeping the hue constant.
        /// </summary>
        MONOCHROMATIC,

        /// <summary>
        ///     Generates a color palette by producing various shades of a single base color.
        ///     The resulting palette consists of colors with varying levels of lightness or darkness.
        /// </summary>
        SHADES,

        /// <summary>
        ///     Generates a color palette using colors that are adjacent to each other on the color
        ///     wheel,
        ///     resulting in a harmonious and visually cohesive set of colors.
        /// </summary>
        ANALOGOUS,

        /// <summary>
        ///     Generates a color palette based on complementary color theory, where the selected
        ///     colors
        ///     are directly opposite each other on the color wheel, creating high contrast and visual
        ///     balance.
        /// </summary>
        COMPLEMENTARY,

        /// <summary>
        ///     Generates a color palette by selecting three colors: the base color and two colors that
        ///     are adjacent
        ///     to the complementary color of the base. This method creates a visually balanced and
        ///     less contrasting scheme
        ///     compared to a direct complementary palette.
        /// </summary>
        SPLIT_COMPLEMENTARY,

        /// <summary>
        ///     Generates a color palette using a combination of multiple harmonious color schemes,
        ///     balancing contrasts and similarities to produce a visually appealing result.
        /// </summary>
        COMPOUND,

        /// <summary>
        ///     Generates a color palette based on a triangular color harmony,
        ///     where three evenly spaced hues form a triangle on the color wheel.
        /// </summary>
        TRIANGLE,

        /// <summary>
        ///     Creates a color palette by selecting colors spaced evenly across four equidistant
        ///     points on the color wheel,
        ///     forming a square shape in the HSV (Hue, Saturation, Value) color space.
        /// </summary>
        SQUARE,

        /// <summary>
        ///     Generates a color palette by applying sinusoidal functions to create smooth, wave-like
        ///     transitions
        ///     in color value variations, suitable for producing visually harmonic color gradients.
        /// </summary>
        SINUSOIDAL
    }

    public record struct PaletteGeneratorOptions {
        public PaletteType Type        { get; init; }
        public int         Count       { get; init; }
        public int         Seed        { get; init; }
        public NKColor     Base        { get; init; }
        public int         HueVariance { get; init; }
        public double      SatFactor   { get; init; }
        public double      LightFactor { get; init; }

        public PaletteGeneratorOptions(
            PaletteType type,
            int         count,
            int         seed,
            NKColor     baseColor,
            int         hueVariance = 0,
            double      satFactor   = 1,
            double      lightFactor = 1
        ) {
            Type        = type;
            Count       = count;
            Seed        = seed;
            Base        = baseColor;
            HueVariance = hueVariance;
            SatFactor   = satFactor;
            LightFactor = lightFactor;
        }
    }

    public static NKPalette Generate(PaletteGeneratorOptions options = default) {
        var count = options.Count < 5 ? 5 : options.Count;
        var rnd   = options.Seed  != 0 ? new Random(options.Seed) : new Random();

        var satFactor   = Math.Abs(options.SatFactor)   < 0.0001 ? 1.0 : options.SatFactor;
        var lightFactor = Math.Abs(options.LightFactor) < 0.0001 ? 1.0 : options.LightFactor;

        double baseH, baseS, baseV;

        if (options.Base.IsDefault || options.Base.IsInherit) {
            baseH = 210.0;
            baseS = 0.75;
            baseV = 0.85;
        }
        else {
            var (r, g, b) = ToRgb(options.Base);
            var sysColor = Color.FromArgb(r, g, b);
            sysColor.ColorToHsv(out baseH, out baseS, out baseV);

            if (baseS < 0.1 && options.Type != PaletteType.MONOCHROMATIC && options.Type != PaletteType.SHADES && options.Type != PaletteType.RANDOM)
                baseS = 0.75;
        }

        var colors = new NKColor[count];

        switch (options.Type) {
            case PaletteType.RANDOM: {
                for (var i = 0; i < count; i++) {
                    var h = rnd.NextDouble() * 360.0;
                    var s = rnd.NextDouble() * satFactor;
                    var v = rnd.NextDouble() * lightFactor;
                    colors[i] = FromHsv(h, s, v);
                }

                break;
            }

            case PaletteType.MONOCHROMATIC: {
                for (var i = 0; i < count; i++) {
                    var t = count == 1 ? 0 : (double)i / (count - 1);
                    var h = baseH + GetVariance(rnd, options.HueVariance);
                    var s = Math.Clamp(baseS * (1.0 - 0.6 * (i % 2 == 1 ? t : t * 0.5)) * satFactor,   0.1, 1.0);
                    var v = Math.Clamp(baseV * (0.3 + 0.7 * t)                          * lightFactor, 0.1, 1.0);
                    colors[i] = FromHsv(h, s, v);
                }

                break;
            }

            case PaletteType.SHADES: {
                for (var i = 0; i < count; i++) {
                    var t = count == 1 ? 0 : (double)i / (count - 1);
                    var h = baseH + GetVariance(rnd, options.HueVariance);
                    var s = Math.Clamp(baseS            * satFactor,   0.0, 1.0);
                    var v = Math.Clamp((0.2 + 0.75 * t) * lightFactor, 0.0, 1.0);
                    colors[i] = FromHsv(h, s, v);
                }

                break;
            }

            case PaletteType.ANALOGOUS: {
                var spread = 60.0;

                for (var i = 0; i < count; i++) {
                    var t  = count == 1 ? 0 : (double)i / (count - 1);
                    var th = -spread                    / 2.0 + spread * t;
                    var h  = baseH                            + th + GetVariance(rnd, options.HueVariance);
                    var s  = Math.Clamp(baseS * satFactor,   0.0, 1.0);
                    var v  = Math.Clamp(baseV * lightFactor, 0.0, 1.0);
                    colors[i] = FromHsv(h, s, v);
                }

                break;
            }

            case PaletteType.COMPLEMENTARY: {
                var compH = baseH + 180.0;

                for (var i = 0; i < count; i++) {
                    var t  = count == 1 ? 0 : (double)i / (count - 1);
                    var th = i % 2 == 0 ? baseH : compH;
                    var h  = th + GetVariance(rnd, options.HueVariance);
                    var s  = Math.Clamp((0.6 + 0.4 * (1.0 - t)) * satFactor,   0.0, 1.0);
                    var v  = Math.Clamp((0.4 + 0.6 * t)         * lightFactor, 0.0, 1.0);
                    colors[i] = FromHsv(h, s, v);
                }

                break;
            }

            case PaletteType.SPLIT_COMPLEMENTARY: {
                double[] splitHues = [baseH, baseH + 150.0, baseH + 210.0];

                for (var i = 0; i < count; i++) {
                    var t  = count == 1 ? 0 : (double)i / (count - 1);
                    var th = splitHues[i % splitHues.Length];
                    var h  = th + GetVariance(rnd, options.HueVariance);
                    var s  = Math.Clamp((0.6  + 0.4  * (i % 2 == 0 ? 1.0 : 0.7)) * satFactor,   0.0, 1.0);
                    var v  = Math.Clamp((0.35 + 0.65 * t)                        * lightFactor, 0.0, 1.0);
                    colors[i] = FromHsv(h, s, v);
                }

                break;
            }

            case PaletteType.COMPOUND: {
                double[] compoundHues = [baseH, baseH + 30.0, baseH + 150.0, baseH + 180.0, baseH + 210.0];

                for (var i = 0; i < count; i++) {
                    var t  = count == 1 ? 0 : (double)i / (count - 1);
                    var th = compoundHues[i % compoundHues.Length];
                    var h  = th + GetVariance(rnd, options.HueVariance);
                    var s  = Math.Clamp((0.5  + 0.5  * (1.0 - 0.2 * (i % 3))) * satFactor,   0.0, 1.0);
                    var v  = Math.Clamp((0.35 + 0.65 * t)                     * lightFactor, 0.0, 1.0);
                    colors[i] = FromHsv(h, s, v);
                }

                break;
            }

            case PaletteType.TRIANGLE: {
                double[] triadicHues = [baseH, baseH + 120.0, baseH + 240.0];

                for (var i = 0; i < count; i++) {
                    var t  = count == 1 ? 0 : (double)i / (count - 1);
                    var th = triadicHues[i % 3];
                    var h  = th + GetVariance(rnd, options.HueVariance);
                    var s  = Math.Clamp((0.6  + 0.4  * (1.0 - 0.2 * (i / 3f))) * satFactor,   0.0, 1.0);
                    var v  = Math.Clamp((0.35 + 0.65 * t)                      * lightFactor, 0.0, 1.0);
                    colors[i] = FromHsv(h, s, v);
                }

                break;
            }

            case PaletteType.SQUARE: {
                double[] squareHues = [baseH, baseH + 90.0, baseH + 180.0, baseH + 270.0];

                for (var i = 0; i < count; i++) {
                    var t  = count == 1 ? 0 : (double)i / (count - 1);
                    var th = squareHues[i % 4];
                    var h  = th + GetVariance(rnd, options.HueVariance);
                    var s  = Math.Clamp((0.6  + 0.4  * (1.0 - 0.2 * (i / 4f))) * satFactor,   0.0, 1.0);
                    var v  = Math.Clamp((0.35 + 0.65 * t)                      * lightFactor, 0.0, 1.0);
                    colors[i] = FromHsv(h, s, v);
                }

                break;
            }

            case PaletteType.SINUSOIDAL: {
                for (var i = 0; i < count; i++) {
                    var t     = count        == 1 ? 0 : (double)i              / (count - 1);
                    var phase = options.Seed != 0 ? options.Seed % 100 / 100.0 * Math.PI * 2 : 0;

                    var h = baseH + 60.0 * Math.Sin(Math.PI * 2 * t + phase)
                        + GetVariance(rnd, options.HueVariance);

                    var s =
                        Math.Clamp(
                            (0.6 + 0.35 * Math.Sin(Math.PI * 2 * t + Math.PI / 3 + phase))
                            * satFactor,
                            0.0,
                            1.0
                        );

                    var v = Math.Clamp(
                        (0.55 + 0.4 * Math.Cos(Math.PI * 2 * t + phase))
                        * lightFactor,
                        0.0,
                        1.0
                    );

                    colors[i] = FromHsv(h, s, v);
                }

                break;
            }

            default: throw new ArgumentOutOfRangeException(nameof(options.Type), options.Type, "Unsupported palette type.");
        }

        return new NKPalette(colors);
    }

    private static (byte R, byte G, byte B) ToRgb(NKColor color) {
        if (color.IsRgb) {
            var hex = color.AsRgb;

            return ((byte)(hex >> 16), (byte)(hex >> 8), (byte)hex);
        }

        if (!color.IsPalette)
            return (0x34, 0x98, 0xdb);

        var p = (byte)color.AsPalette;

        switch (p) {
            case < 16: {
                return p switch {
                    0  => (0, 0, 0),
                    1  => (128, 0, 0),
                    2  => (0, 128, 0),
                    3  => (128, 128, 0),
                    4  => (0, 0, 128),
                    5  => (128, 0, 128),
                    6  => (0, 128, 128),
                    7  => (192, 192, 192),
                    8  => (128, 128, 128),
                    9  => (255, 0, 0),
                    10 => (0, 255, 0),
                    11 => (255, 255, 0),
                    12 => (0, 0, 255),
                    13 => (255, 0, 255),
                    14 => (0, 255, 255),
                    15 => (255, 255, 255),
                    _  => throw new ArgumentOutOfRangeException()
                };
            }
            case <= 231: {
                var idx  = p - 16;
                var rIdx = idx     / 36;
                var gIdx = idx / 6 % 6;
                var bIdx = idx     % 6;
                var r    = (byte)(rIdx == 0 ? 0 : 55 + rIdx * 40);
                var g    = (byte)(gIdx == 0 ? 0 : 55 + gIdx * 40);
                var b    = (byte)(bIdx == 0 ? 0 : 55 + bIdx * 40);

                return (r, g, b);
            }
            case >= 232: {
                var v = (byte)(8 + (p - 232) * 10);

                return (v, v, v);
            }
        }
    }

    private static NKColor FromHsv(double h, double s, double v) {
        h %= 360.0;

        if (h < 0)
            h += 360.0;

        s = Math.Clamp(s, 0.0, 1.0);
        v = Math.Clamp(v, 0.0, 1.0);
        var sysColor = ColorFormat.HsvToColor(h, s, v);

        return NKColor.FromRgb(sysColor.R, sysColor.G, sysColor.B);
    }

    private static double GetVariance(Random rnd, int variance) {
        if (variance <= 0)
            return 0.0;

        return (rnd.NextDouble() * 2.0 - 1.0) * variance;
    }
}