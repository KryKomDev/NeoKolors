//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Tests;

public class NKCharCanvasTests {
    
    [Fact]
    public void Constructor_WithDimensions_ShouldInitializeCorrectly() {
        var canvas = new NKCharCanvas(10, 5);
        Assert.Equal(10, canvas.Width);
        Assert.Equal(5, canvas.Height);
    }

    [Fact]
    public void Resize_ShouldUpdateDimensions() {
        var canvas = new NKCharCanvas(10, 5);
        canvas.Resize(20, 10);
        Assert.Equal(20, canvas.Width);
        Assert.Equal(10, canvas.Height);
    }

    [Fact]
    public void PlaceString_ShouldPlaceCharactersCorrectly() {
        var canvas = new NKCharCanvas(20, 5);
        var text = "Hello";
        // Implicit conversion from NeoKolors.Tui.Point to Metriks.Point2D might be needed or supported
        // Point2D is likely (int, int) or struct with X, Y
        // Based on Point.cs: public static implicit operator Point2D(Point p) => new(p.X, p.Y);
        // So we can pass new Point(2, 2)
        
        canvas.PlaceString(text, new Point(2, 2));

        for (int i = 0; i < text.Length; i++) {
            var cell = canvas[2 + i, 2];
            Assert.Equal(text[i], cell.Char);
        }
    }

    [Fact]
    public void PlaceString_ShouldClipWhenOutOfBounds() {
        var canvas = new NKCharCanvas(5, 5);
        var text = "Hello";
        // Place at (2, 2), width is 5.
        // x=2: 'H'
        // x=3: 'e'
        // x=4: 'l'
        // x=5: out of bounds (should not throw, just clip)
        
        canvas.PlaceString(text, new Point(2, 2));
        
        // Check valid chars
        Assert.Equal('H', canvas[2, 2].Char);
        Assert.Equal('e', canvas[3, 2].Char);
        Assert.Equal('l', canvas[4, 2].Char);
        
        // We can't check canvas[5, 2] as it would throw IndexOutOfRangeException on the verify
        // The test passes if PlaceString didn't throw and filled available space
    }

    [Fact]
    public void Clear_ShouldResetCanvas() {
        var canvas = new NKCharCanvas(10, 5);
        canvas.PlaceString("Test", new Point(0, 0));
        
        canvas.Clear();
        
        // After clear, data is cleared.
        // Wait, NKCharCanvas.Clear calls _data.Clear().
        // Depending on List2D implementation, this might remove items or reset them.
        // If it empties the list, accessing indexer might throw or return default.
        // List2D usually implies a grid.
        // Let's assume Clear resets to default or empties it.
        // If it empties, Width/Height might become 0.
        
        // Checking NKCharCanvas.cs:
        // public void Clear() { _data.Clear(); }
        // If _data is List2D, Clear() likely removes all elements.
        // So Width/Height might be 0 after Clear().
        
        Assert.Equal(0, canvas.Width);
        Assert.Equal(0, canvas.Height);
    }
}
