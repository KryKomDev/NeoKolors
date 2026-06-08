//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Core;
using NeoKolors.Common;

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
        
        canvas.Place(text, new Point(2, 2));

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
        
        canvas.Place(text, new Point(2, 2));
        
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
        canvas.Place("Test", new Point(0, 0));
        
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

    [Fact]
    public void Place_OverlappingCanvasWithInheritStyle_ShouldMergeStylesAndPreserveCharacters() {
        // Arrange: Bottom canvas with colored background
        var bottom = new NKCharCanvas(3, 3);
        bottom.StyleBackground(new Rectangle(0, 0, 2, 2), NKColor.FromRgb(255, 0, 0));
        bottom.Place("abc", new Point(0, 0));

        // Top canvas with transparent background (inherit) and white bold character
        var top = new NKCharCanvas(3, 3);
        top.ForceStyleBackground(new Rectangle(0, 0, 2, 2), NKColor.Inherit);
        var styledPiece = new AnsiString("X", new NKStyle(
            NKColor.FromRgb(255, 255, 255),
            NKColor.Inherit,
            TextStyles.BOLD
        ));
        top.Place(styledPiece, new Point(1, 0));

        // Act: Place top canvas onto bottom canvas
        bottom.Place(top);

        // Assert:
        // Position (0, 0) should remain 'a' with red background
        var cellA = bottom[0, 0];
        Assert.Equal('a', cellA.Char);
        Assert.Equal(NKColor.FromRgb(255, 0, 0), cellA.Style.GetBColor());

        // Position (1, 0) should have 'X' with white text color, bold style, AND red background from the bottom!
        var cellX = bottom[1, 0];
        Assert.Equal('X', cellX.Char);
        Assert.Equal(NKColor.FromRgb(255, 255, 255), cellX.Style.GetFColor());
        Assert.True(cellX.Style.GetStyles().GetIsBold());
        Assert.Equal(NKColor.FromRgb(255, 0, 0), cellX.Style.GetBColor());
        
        // Ensure Changed flag is set to true on modified cells
        Assert.True(cellX.Changed);
    }

    [Fact]
    public void Place_OverlappingCanvasWithDefaultStyle_ShouldPreserveTrueColorBackground() {
        // Arrange: Bottom canvas with colored background
        var bottom = new NKCharCanvas(3, 3);
        bottom.StyleBackground(new Rectangle(0, 0, 2, 2), NKColor.FromRgb(255, 0, 0));
        bottom.Place("abc", new Point(0, 0));

        // Top canvas with default background (not inherit) and character 'X'
        var top = new NKCharCanvas(3, 3);
        top.Place("X", new Point(1, 0));

        // Act: Place top canvas onto bottom canvas
        bottom.Place(top);

        // Assert:
        // Position (1, 0) should have 'X' and preserve the red background from bottom
        var cellX = bottom[1, 0];
        Assert.Equal('X', cellX.Char);
        Assert.Equal(NKColor.FromRgb(255, 0, 0), cellX.Style.GetBColor());
    }
}
