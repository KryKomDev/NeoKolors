using SkiaSharp;

namespace NeoKolors.Common.Tests;

public class SixelConverterTests {
    
    [Fact]
    public void ToSixel_ReturnsEmpty_ForNullOrEmptyBitmap() {
        var nullSixel = SixelConverter.ToSixel(null!);
        using var emptyBitmap = new SKBitmap();
        var emptySixel = emptyBitmap.ToSixel();

        Assert.Equal(string.Empty, nullSixel);
        Assert.Equal(string.Empty, emptySixel);
    }

    [Fact]
    public void ToSixel_ReturnsValidSixelString_ForSimpleBitmap() {
        using var bitmap = new SKBitmap(10, 10);
        using (var canvas = new SKCanvas(bitmap)) {
            canvas.Clear(SKColors.Red);
        }

        var sixel = bitmap.ToSixel();

        Assert.NotNull(sixel);
        Assert.StartsWith("\eP0;1;q", sixel);
        Assert.EndsWith("\e\\", sixel);
        Assert.Contains("#0;2;100;0;0", sixel); // Red color definition check (approximate)
    }

    [Fact]
    public void ToSixel_HandlesTransparentBitmap() {
        using var bitmap = new SKBitmap(10, 10);
        bitmap.Erase(SKColors.Transparent);

        var sixel = bitmap.ToSixel();

        Assert.NotNull(sixel);
        Assert.StartsWith("\eP0;1;q", sixel);
        Assert.EndsWith("\e\\", sixel);
        
        // Should not contain any color application commands if fully transparent
        // But palette might still be emitted if logic is simple.
        // The current implementation emits palette only for used colors?
        // Let's check: implementation builds palette from pixels. If all transparent (-1), palette is empty.
    }

    [Fact]
    public void ToSixel_RespectsAlphaThreshold() {
        using var bitmap = new SKBitmap(1, 1);
        
        // Semi-transparent color (Alpha = 100)
        bitmap.SetPixel(0, 0, new SKColor(255, 0, 0, 100));

        // With threshold 128, it should be transparent (empty data)
        var sixel128 = bitmap.ToSixel();
        Assert.DoesNotContain("#0", sixel128); // No color 0 defined if no pixels used

        // With threshold 50, it should be opaque
        var sixel50 = bitmap.ToSixel(50);
        Assert.Contains("#0;2;100;0;0", sixel50);
    }

    [Fact]
    public void ToSixel_HandlesMultipleColors() {
        using var bitmap = new SKBitmap(2, 1);
        bitmap.SetPixel(0, 0, SKColors.Red);
        bitmap.SetPixel(1, 0, SKColors.Blue);

        var sixel = bitmap.ToSixel();

        Assert.Contains("#0;2;100;0;0", sixel); // Red
        Assert.Contains("#1;2;0;0;100", sixel); // Blue
    }

    [Fact]
    public void ToSixel_UsesRunLengthEncoding() {
        using var bitmap = new SKBitmap(10, 1);
        using (var canvas = new SKCanvas(bitmap)) {
            canvas.Clear(SKColors.Green);
        }

        var sixel = bitmap.ToSixel();

        // RLE format: !countchar
        // 10 pixels -> !10? (or similar, depending on the bitmask char)
        // Bitmask for top row (1st bit set) is 1. Char = 63 + 1 = 64 ('@').
        // So we expect "!10@"
        Assert.Contains("!10", sixel);
    }

    [Fact]
    public void ToSixel_HandlesMultiBand() {
        
        // Height 12 needs 2 bands (6 rows each)
        using var bitmap = new SKBitmap(1, 12);
        bitmap.Erase(SKColors.White);

        var sixel = bitmap.ToSixel();

        // Should contain the band separator "-"
        Assert.Contains("-", sixel);
    }

    [Fact]
    public void ToSixel_FallbacksToQuantization_ForManyColors() {
        
        using var bitmap = new SKBitmap(17, 16);
        var colorCount = 0;
        for (var y = 0; y < 16; y++)
        for (var x = 0; x < 17; x++) {
            
            // Generate unique colors
            // Just varying R and G slightly
            var r = (byte)(colorCount % 256);
            var g = (byte)(colorCount / 256);
            var b = (byte)((x + y) % 256);

            // Ensure they are actually distinct if possible, or just trust the loop produces enough variance
            // Easier: Use a set to verify we generated enough unique inputs for the test
            // But for the test itself, we just put them in the bitmap.
            // To guarantee uniqueness easily:
            // 256 * r + g? No.
            // Let's just increment R, G, B components.
            // R goes 0-255. G goes 0-255.
            // If we set B=1, we get another 256.
            bitmap.SetPixel(x, y,
                colorCount < 256 ? new SKColor((byte)colorCount, 0, 0) : new SKColor((byte)(colorCount - 256), 10, 0));

            colorCount++;
        }

        var sixel = bitmap.ToSixel();

        // The implementation should handle this without crashing.
        // It switches to a fixed 3-3-2 palette (256 colors).
        Assert.NotNull(sixel);
        Assert.EndsWith("\e\\", sixel);

        // We can verify that we don't have > 256 palette entries defined
        // Palette definitions look like "#256;2;..." if it went over.
        // So check that "#256;" is NOT present (assuming 0-indexed up to 255).
        Assert.DoesNotContain("#256;", sixel);
    }
}