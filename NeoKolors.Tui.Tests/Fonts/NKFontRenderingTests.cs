//
// NeoKolors.Test
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Fonts.Serialization;
using NeoKolors.Common;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Tests;

public class NKFontRenderingTests {

    private const string PATH = "NeoKolors.Tui.Tests.Fonts.BuiltIn.Dummy1.nkf";
    
    private static string GetFontPath() {
        // Try to find the font relative to the test project execution
        // We know the source is at NeoKolors.Tui.Tests/Fonts/BuiltIn/Dummy1.nkf
        // Current directory is likely .../NeoKolors.Tui.Tests/bin/Debug/net8.0
        
        var searchPaths = new[] {
            "Fonts/BuiltIn/Dummy1.nkf",
            "../../../Fonts/BuiltIn/Dummy1.nkf",
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts/BuiltIn/Dummy1.nkf"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Fonts/BuiltIn/Dummy1.nkf")
        };

        foreach (var path in searchPaths) {
            if (File.Exists(path)) return Path.GetFullPath(path);
        }
        
        throw new FileNotFoundException("Could not find Dummy1.nkf test font.");
    }

    [Fact]
    public void PlaceString_ShouldRenderSpecificGlyphContent() {
        var path = GetFontPath();
        var font = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        var canvas = new NKCharCanvas(50, 20);
        
        // Render 'A' at (0,0)
        font!.PlaceString("A", canvas);
        
        // Based on A.nkg and Dummy1 config:
        // Mask: '.' -> ' ', ' ' -> null. Align: '+' -> null.
        // A.nkg (truncated):
        // Row 0: null, null, null, '_', '_'
        // Row 1: null, null, '/', ' ', ' ', '\\'
        // Row 2: null, '/', ' ', '_', '_', ' ', '\\'
        // Row 3: '/', '_', '/', ' ', ' ', '\\', '_', '\\'
        
        // Check key points
        Assert.Equal('_', canvas[3, 0].Char);
        Assert.Equal('_', canvas[4, 0].Char);
        Assert.Null(canvas[2, 0].Char);
        
        Assert.Equal('/', canvas[2, 1].Char);
        Assert.Equal(' ', canvas[3, 1].Char);
        Assert.Equal('\\', canvas[5, 1].Char);
        
        Assert.Equal('/', canvas[0, 3].Char);
        Assert.Equal('_', canvas[1, 3].Char);
        Assert.Equal(' ', canvas[4, 3].Char);
        Assert.Equal('\\', canvas[7, 3].Char);
    }

    [Fact]
    public void PlaceString_ShouldRenderLigatureExactly() {
        var path = GetFontPath();        
        var font = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        var canvas = new NKCharCanvas(50, 20);
        
        // Render "->" at (0,0)
        font!.PlaceString("->", canvas);
        
        // arr-right.nkg (6 spaces prefix):
        //       __
        // ._____\.\
        // |______..)
        //       /_/
        // Mask: '.' -> ' ', ' ' -> null.
        
        // Line 0: null*6, '_', '_'
        Assert.Equal('_', canvas[6, 0].Char);
        Assert.Equal('_', canvas[7, 0].Char);
        Assert.Null(canvas[5, 0].Char);
        
        // Line 1: ' ', '_', '_', '_', '_', '_', '\\', ' ', '\\'
        Assert.Equal(' ', canvas[0, 1].Char);
        Assert.Equal('_', canvas[1, 1].Char);
        Assert.Equal('\\', canvas[6, 1].Char);
        Assert.Equal(' ', canvas[7, 1].Char);
        Assert.Equal('\\', canvas[8, 1].Char);
        
        // Line 2: '|', '_', '_', '_', '_', '_', '_', ' ', ' ', ')'
        Assert.Equal('|', canvas[0, 2].Char);
        Assert.Equal('_', canvas[1, 2].Char);
        Assert.Equal(' ', canvas[7, 2].Char);
        Assert.Equal(')', canvas[9, 2].Char);
    }

    [Fact]
    public void PlaceString_ShouldHandleLineSpacing() {
        var path = GetFontPath();
        var font = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        var canvas = new NKCharCanvas(50, 20);
        
        // LineSpacing is 2. A height is 4.
        // First line at y=0. Second line should start at y = 0 + 4 + 2 = 6.
        font!.PlaceString("A\nA", canvas);
        
        // Row 3 of first A is at y=3.
        // Row 0 of second A should be at y=6.
        
        Assert.Equal('/', canvas[0, 3].Char); // Bottom row of first A
        Assert.Null(canvas[0, 4].Char);       // Line spacing row 1
        Assert.Null(canvas[0, 5].Char);       // Line spacing row 2
        Assert.Equal('_', canvas[3, 6].Char); // Top row of second A
    }

    [Fact]
    public void PlaceString_ShouldHandleWordSpacing() {
        var path = GetFontPath();
        var font = NKFontSerializer.ReadFile(path);
        var canvas = new NKCharCanvas(100, 20);
        var info = (NKFontInfo)font.Info;
        
        // WordSpacing is 5. Width of A is 8. CharSpacing is 2.
        // "A A" -> A at x=0.
        // x becomes 0 + 8 + 2 = 10.
        // space of 5 -> x becomes 10 + 5 = 15.
        // second A at x = 15.
        font.PlaceString("A A", canvas);
        
        Assert.Equal('/', canvas[0, 3].Char);   // Leftmost of first A
        Assert.Null(canvas[8, 3].Char);         // Char spacing start
        Assert.Null(canvas[14, 3].Char);        // Word space end
        Assert.Equal('/', canvas[15, 3].Char);  // Leftmost of second A
    }

    [Fact]
    public void PlaceString_WithAlignment_ShouldAffectPosition() {
        var path = GetFontPath();
        var font = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        var canvas = new NKCharCanvas(50, 10);
        
        // Render centered
        font!.PlaceString("Test", canvas, new Rectangle(0, 0, 49, 9), 
            new NKStyle(), HorizontalAlign.CENTER, VerticalAlign.CENTER);
        
        // We expect the text NOT to be at (0,0)
        // Check top-left corner
        Assert.Null(canvas[0, 0].Char);
        
        // Check somewhere in the middle (approximate)
        // Center of 50x10 is (25, 5).
        // Text "Test" width is likely > 0.
        // We just check that it's NOT at 0,0 and IS somewhere else.
        
        bool drawn = false;
        for (int x = 0; x < canvas.Width; x++) {
            for (int y = 0; y < canvas.Height; y++) {
                if (canvas[x, y].Char.HasValue) {
                    drawn = true;
                    break;
                }
            }
        }
        Assert.True(drawn, "Canvas should contain centered text.");
    }
}
