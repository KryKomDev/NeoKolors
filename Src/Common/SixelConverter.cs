// NeoKolors
// Copyright (c) 2025 KryKom

using System.Text;
using SkiaSharp;

namespace NeoKolors.Common;

/// <summary>
/// Provides extension methods to convert SkiaSharp bitmaps to Sixel strings.
/// </summary>
public static class SixelConverter {
    
    /// <summary>
    /// Converts an SKBitmap to a Sixel string.
    /// </summary>
    /// <param name="bitmap">The bitmap to convert.</param>
    /// <param name="alphaThreshold">The alpha threshold (0-255) above which a pixel is considered opaque. Defaults to 128.</param>
    /// <returns>A string containing the Sixel data.</returns>
    public static string ToSixel(this SKBitmap bitmap, byte alphaThreshold = 128) {
        if (Equals(bitmap, null) || bitmap.IsEmpty) return string.Empty;

        var sb = new StringBuilder();
        
        // Enter Sixel Mode
        // P1 = 0 (macro parameter), P2 = 1 (keep transparent background), P3 = 0 (horizontal grid size)
        // 1;1;width;height set raster attributes
        sb.Append($"\eP0;1;q\"1;1;{bitmap.Width};{bitmap.Height}");

        // 1. Quantize / Build Palette
        var palette = BuildPalette(bitmap, alphaThreshold, out var pixelIndices);

        // 2. Output Palette
        for (int i = 0; i < palette.Count; i++) {
            var c = palette[i];
            // Sixel uses percentages 0-100 for RGB.
            // Formula: value * 100 / 255
            int r = (int)(c.Red   * (100f / 255f));
            int g = (int)(c.Green * (100f / 255f));
            int b = (int)(c.Blue  * (100f / 255f));
            sb.Append($"#{i};2;{r};{g};{b}");
        }

        // 3. Encode Data
        // Sixel encodes 6 rows of pixels at a time.
        for (int y = 0; y < bitmap.Height; y += 6) {
            
            // For each color in the palette, we output the "strip" for that color.
            // Sixel is color-plane oriented within a band.
            // But we can also do it per color if we want, or iterate colors.
            // The standard way is: For each band, iterate through active colors.
            
            // To optimize, we find which colors are actually used in this band?
            // Or just iterate all palette colors? Iterating all 256 is slow if unused.
            // Better: Iterate x, find color, add to buffer for that color.
            
            // We need a buffer for each active color in this band (6 rows).
            // Key: ColorIndex, Value: List of (x, bitmask)
            // Actually, Sixel is: Color Selector -> Data -> Color Selector -> Data...
            // So for this band, we need to construct the sixel line for each color.
            
            var lines = new Dictionary<int, StringBuilder>();
            
            for (int rowOffset = 0; rowOffset < 6; rowOffset++) {
                int currentY = y + rowOffset;
                
                if (currentY >= bitmap.Height) break;

                for (int x = 0; x < bitmap.Width; x++) {
                    int colorIndex = pixelIndices[x, currentY];
                    if (colorIndex == -1) continue; // Transparent

                    if (!lines.ContainsKey(colorIndex)) {
                        lines[colorIndex] = new StringBuilder();
                        // Pad with transparency/skip if we start late? 
                        // No, Sixel cursor doesn't usually advance automatically between colors, 
                        // but within a color, '?' or other chars advance it.
                        // Actually, we usually just fill the whole width for simplicity or use absolute positioning?
                        // Sixel standard: "Active color is painted".
                        // We need to build the bitmask for the whole width for each color.
                    }
                }
            }
            
            // Optimized approach:
            // For this band (y to y+6):
            // We have `bitmap.Width` columns.
            // Each column has 6 pixels.
            // Each pixel has a color index.
            // We transpose this: For each Color Index present in this band, we construct a line of characters.
            
            // 1. Identify active colors in this band.
            // 2. For each active color:
            //    Construct the string of characters.
            //    Each character represents 6 vertical pixels for that color.
            //    If a pixel (x, y+k) has this color, set k-th bit.
            //    Char = 63 + bitmask.
            
            var activeColors = new HashSet<int>();
            for(int r = 0; r < 6 && y+r < bitmap.Height; r++) {
                for(int x=0; x<bitmap.Width; x++) {
                    int idx = pixelIndices[x, y+r];
                    if(idx != -1) activeColors.Add(idx);
                }
            }
            
            // Sort to keep a deterministic order (optional but good)
            var sortedColors = activeColors.OrderBy(k => k).ToList();
            
            foreach (var colorIdx in sortedColors) {
                sb.Append($"#{colorIdx}"); // Select color
                
                int currentRun = 0;
                char lastChar = '\0';
                
                for (int x = 0; x < bitmap.Width; x++) {
                    int mask = 0;
                    
                    for (int r = 0; r < 6; r++) {
                        if (y + r < bitmap.Height && pixelIndices[x, y + r] == colorIdx) 
                            mask |= (1 << r);
                    }
                    
                    char c = (char)(63 + mask); // 63 is '?' which is 0. '@' is 64? Wait.
                    // Sixel offset is 63.
                    // 0 -> ? (63)
                    // 1 -> @ (64)
                    // ...
                    
                    if (c == lastChar && currentRun > 0) {
                        currentRun++;
                    }
                    else {
                        if (currentRun > 0) EncodeRun(sb, currentRun, lastChar);
                        
                        currentRun = 1;
                        lastChar = c;
                    }
                }
                
                if (currentRun > 0) EncodeRun(sb, currentRun, lastChar);
                
                sb.Append('$'); // End of line (CR), return to start of line for next color overlay
            }
            
            sb.Append('-'); // Next band (LF)
        }

        // Exit Sixel Mode
        sb.Append("\e\\");
        
        return sb.ToString();
    }

    private static void EncodeRun(StringBuilder sb, int count, char c) {
        if (count > 3)
            sb.Append($"!{count}{c}");
        else for (int i = 0; i < count; i++)
                sb.Append(c);
    }

    private static List<SKColor> BuildPalette(SKBitmap bmp, byte alphaThreshold, out int[,] pixelIndices) {
        // Simple approach:
        // 1. Check if < 256 unique colors
        // 2. If yes, use them.
        // 3. If no, use 3-3-2 quantization.

        pixelIndices = new int[bmp.Width, bmp.Height];
        var uniqueColors = new Dictionary<SKColor, int>();
        var palette = new List<SKColor>();
        
        bool useFixed = false;
        
        // Pass 1: Collect unique colors (up to limit)
        for (int y = 0; y < bmp.Height; y++) {
            for (int x = 0; x < bmp.Width; x++) {
                var c = bmp.GetPixel(x, y);
                
                if (c.Alpha < alphaThreshold) {
                    pixelIndices[x, y] = -1; // Transparent
                    continue; 
                }

                // Sixel doesn't support alpha, so we only care about RGB for the palette.
                var rgb = new SKColor(c.Red, c.Green, c.Blue);

                if (!uniqueColors.TryGetValue(rgb, out int index)) {
                    if (uniqueColors.Count >= 256) {
                        useFixed = true;
                        return GenerateQuantizedPalette(bmp, alphaThreshold, pixelIndices, useFixed, palette);
                    }
                    
                    index = palette.Count;
                    uniqueColors[rgb] = index;
                    palette.Add(rgb);
                }
                
                pixelIndices[x, y] = index;
            }
        }

        return GenerateQuantizedPalette(bmp, alphaThreshold, pixelIndices, useFixed, palette);
    }

    private static List<SKColor> GenerateQuantizedPalette(SKBitmap bmp, byte alphaThreshold, int[,] pixelIndices, bool useFixed, List<SKColor> palette) {
        if (!useFixed) return palette;
        
        // Fallback to 3-3-2
        palette.Clear();
        
        // ... (rest of 3-3-2 generation)
        for (int i = 0; i < 256; i++) {
            int r = (i & 0xE0) >> 5; 
            int g = (i & 0x1C) >> 2; 
            int b = (i & 0x03);      
                
            byte r8 = (byte)(r * (255f / 7f));
            byte g8 = (byte)(g * (255f / 7f));
            byte b8 = (byte)(b * (255f / 3f));
                
            palette.Add(new SKColor(r8, g8, b8));
        }
            
        // Remap pixels
        for (int y = 0; y < bmp.Height; y++) {
            for (int x = 0; x < bmp.Width; x++) {
                var c = bmp.GetPixel(x, y);
                if (c.Alpha < alphaThreshold) {
                    pixelIndices[x, y] = -1;
                    continue;
                }
                    
                int r = (c.Red   >> 5) & 0x07;
                int g = (c.Green >> 5) & 0x07;
                int b = (c.Blue  >> 6) & 0x03;
                    
                int index = r << 5 | g << 2 | b;
                pixelIndices[x, y] = index;
            }
        }

        return palette;
    }
}
