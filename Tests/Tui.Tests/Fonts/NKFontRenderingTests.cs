//
// NeoKolors.Test
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Fonts;
using static NeoKolors.Common.NKTextStyles;

namespace NeoKolors.Tui.Tests.Fonts;

public class NKFontRenderingTests {

    private const string PATH = "NeoKolors.Tui.Tests.Fonts.Assets.Dummy.nkf";

    private static string GetFontPath() {
        // Try to find the font relative to the test project execution
        // We know the source is at NeoKolors.Tui.Tests/Fonts/Assets/Dummy.nkf
        // Current directory is likely .../NeoKolors.Tui.Tests/bin/Debug/net8.0

        var searchPaths = new[] {
            "Fonts/Assets/Dummy.nkf",
            "../../../Fonts/Assets/Dummy.nkf",
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts/Assets/Dummy.nkf"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Fonts/Assets/Dummy.nkf")
        };

        foreach (var path in searchPaths) {
            if (File.Exists(path))
                return Path.GetFullPath(path);
        }

        throw new FileNotFoundException("Could not find Dummy.nkf test font.");
    }

    [Fact]
    public void PlaceString_ShouldRenderSpecificGlyphContent() {
        var       font   = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas = new NKCharCanvas(50, 20);

        // Render 'A' at (0,0)
        font!.DrawString("A", canvas);

        // Based on A.nkg and Dummy1 config:
        // Mask: '.' -> ' ', ' ' -> null. Align: '+' -> null.
        // A.nkg (truncated):
        // Row 0: null, null, null, '_', '_'
        // Row 1: null, null, '/', ' ', ' ', '\\'
        // Row 2: null, '/', ' ', '_', '_', ' ', '\\'
        // Row 3: '/', '_', '/', ' ', ' ', '\\', '_', '\\'

        // Check key points
        Assert.Equal('_', canvas.Data[3, 0].Char);
        Assert.Equal('_', canvas.Data[4, 0].Char);
        Assert.Equal(' ', canvas.Data[2, 0].Char);

        Assert.Equal('/',  canvas.Data[2, 1].Char);
        Assert.Equal(' ',  canvas.Data[3, 1].Char);
        Assert.Equal('\\', canvas.Data[5, 1].Char);

        Assert.Equal('/',  canvas.Data[0, 3].Char);
        Assert.Equal('_',  canvas.Data[1, 3].Char);
        Assert.Equal(' ',  canvas.Data[4, 3].Char);
        Assert.Equal('\\', canvas.Data[7, 3].Char);
    }

    [Fact]
    public void PlaceString_ShouldRenderLigatureExactly() {
        var       font   = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas = new NKCharCanvas(50, 20);

        // Render "->" at (0,0)
        font!.DrawString("->", canvas);

        // arr-right.nkg (6 spaces prefix):
        //       __
        // ._____\.\
        // |______..)
        //       /_/
        // Mask: '.' -> ' ', ' ' -> null.

        // Line 0: null*6, '_', '_'
        Assert.Equal('_', canvas.Data[6, 0].Char);
        Assert.Equal('_', canvas.Data[7, 0].Char);
        Assert.Equal(' ', canvas.Data[5, 0].Char);

        // Line 1: ' ', '_', '_', '_', '_', '_', '\\', ' ', '\\'
        Assert.Equal(' ',  canvas.Data[0, 1].Char);
        Assert.Equal('_',  canvas.Data[1, 1].Char);
        Assert.Equal('\\', canvas.Data[6, 1].Char);
        Assert.Equal(' ',  canvas.Data[7, 1].Char);
        Assert.Equal('\\', canvas.Data[8, 1].Char);

        // Line 2: '|', '_', '_', '_', '_', '_', '_', ' ', ' ', ')'
        Assert.Equal('|', canvas.Data[0, 2].Char);
        Assert.Equal('_', canvas.Data[1, 2].Char);
        Assert.Equal(' ', canvas.Data[7, 2].Char);
        Assert.Equal(')', canvas.Data[9, 2].Char);
    }

    [Fact]
    public void PlaceString_ShouldHandleLineSpacing() {
        var       font   = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas = new NKCharCanvas(50, 20);

        // LineSpacing is 2. A height is 4.
        // First line at y=0. Second line should start at y = 0 + 4 + 2 = 6.
        font!.DrawString("A\nA", canvas);

        // Row 3 of first A is at y=3.
        // Row 0 of second A should be at y=6.

        Assert.Equal('/', canvas.Data[0, 3].Char); // Bottom row of first A
        Assert.Equal(' ', canvas.Data[0, 4].Char); // Line spacing row 1
        Assert.Equal('_', canvas.Data[3, 5].Char); // Top row of second A
    }

    [Fact]
    public void PlaceString_ShouldHandleWordSpacing() {
        var       font   = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas = new NKCharCanvas(100, 20);

        // WordSpacing is 5. Width of A is 8. CharSpacing is 2.
        // "A A" -> A at x=0.
        // x becomes 0 + 8 + 2 = 10.
        // space of 5 -> x becomes 10 + 5 = 15.
        // second A at x = 15.
        font!.DrawString("A A", canvas);

        Assert.Equal('/', canvas.Data[0, 3].Char);  // Leftmost of first A
        Assert.Equal(' ', canvas.Data[8, 3].Char);  // Char spacing start
        Assert.Equal(' ', canvas.Data[12, 3].Char); // Space before second A
        Assert.Equal('/', canvas.Data[13, 3].Char); // Leftmost of second A
    }

    [Fact]
    public void PlaceString_WithAlignment_ShouldAffectPosition() {
        var       font   = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas = new NKCharCanvas(50, 10);

        // Render centered
        font!.DrawString(
            "Test",
            canvas,
            new Rect2D(0, 0, 49, 9),
            new NKStyle(),
            HorizontalAlign.CENTER,
            VerticalAlign.CENTER
        );

        // We expect the text NOT to be at (0,0)
        // Check top-left corner
        Assert.Equal(' ', canvas.Data[0, 0].Char);

        // Check somewhere in the middle (approximate)
        // Center of 50x10 is (25, 5).
        // Text "Test" width is likely > 0.
        // We just check that it's NOT at 0,0 and IS somewhere else.

        bool drawn = false;

        for (int x = 0; x < canvas.Width; x++) {
            for (int y = 0; y < canvas.Height; y++) {
                if (canvas.Data[x, y].Char != ' ') {
                    drawn = true;

                    break;
                }
            }
        }

        Assert.True(drawn, "Canvas should contain centered text.");
    }

    [Fact]
    public void PlaceString_ShouldRenderEWithCaron() {
        var       font   = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas = new NKCharCanvas(10, 10);

        // Render ě with y-offset of 2 so the caron is not clipped off the top of the canvas
        var bounds = new Rect2D(new Point2D(0, 2), new Point2D(10, 10));
        font!.DrawString("ě", canvas, bounds, new NKStyle());

        // Assert that 'ě' is drawn correctly row by row
        // Row 2 should have the caron (\/) centered over the e
        Assert.Equal(' ',  canvas.Data[0, 2].Char);
        Assert.Equal('\\', canvas.Data[1, 2].Char);
        Assert.Equal('/',  canvas.Data[2, 2].Char);
        Assert.Equal(' ',  canvas.Data[3, 2].Char);

        // Row 3 should be the top of the 'e' character ( ___ )
        Assert.Equal(' ', canvas.Data[0, 3].Char);
        Assert.Equal('_', canvas.Data[1, 3].Char);
        Assert.Equal('_', canvas.Data[2, 3].Char);
        Assert.Equal('_', canvas.Data[3, 3].Char);
        Assert.Equal(' ', canvas.Data[4, 3].Char);

        // Row 4 should be the middle of the 'e' character (/ -_)
        Assert.Equal('/', canvas.Data[0, 4].Char);
        Assert.Equal(' ', canvas.Data[1, 4].Char);
        Assert.Equal('-', canvas.Data[2, 4].Char);
        Assert.Equal('_', canvas.Data[3, 4].Char);
        Assert.Equal(')', canvas.Data[4, 4].Char);

        // Row 5 should be the bottom of the 'e' character (\___|)
        Assert.Equal('\\', canvas.Data[0, 5].Char);
        Assert.Equal('_',  canvas.Data[1, 5].Char);
        Assert.Equal('_',  canvas.Data[2, 5].Char);
        Assert.Equal('_',  canvas.Data[3, 5].Char);
        Assert.Equal('|',  canvas.Data[4, 5].Char);
    }

    [Fact]
    public void GetSize_ShouldHandleOverflowingGlyphs() {
        var font = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        Assert.NotNull(font);

        // Assert that measuring works and returns correct dimensions
        var sizeE = font.GetSize("e");
        Assert.Equal(5, sizeE.X); // 'e' width is 5
        Assert.Equal(5, sizeE.Y); // 'e' fits within standard Leading of 5

        var sizeEmpty = font.GetSize("");
        Assert.Equal(0, sizeEmpty.X);
        Assert.Equal(0, sizeEmpty.Y);
    }

    [Fact]
    public void PlaceString_ShouldPreventClippingByAutomaticallyShiftingDown() {
        var       font   = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas = new NKCharCanvas(10, 10);

        // Render ě with y-offset of 0.
        // Even with y-offset of 0, we verify that visual centering or TOP alignment correctly
        // places 'ě' without vertical clipping.
        var bounds = new Rect2D(new Point2D(0, 0), new Point2D(10, 10));
        font!.DrawString("ě", canvas, bounds, new NKStyle());

        // Assert that the caron is fully visible at y = 0
        Assert.Equal('\\', canvas.Data[1, 0].Char);
        Assert.Equal('/',  canvas.Data[2, 0].Char);

        // Assert that the top of 'e' is at y = 1
        Assert.Equal('_', canvas.Data[1, 1].Char);
        Assert.Equal('_', canvas.Data[2, 1].Char);
        Assert.Equal('_', canvas.Data[3, 1].Char);
    }

    [Fact]
    public void PlaceString_ShouldNotApplyUnderlineAndStrikethroughStylesToLettersIfCustomGlyphsAvailable() {
        var font = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        Assert.NotNull(font);
        Assert.NotNull(font.UnderlineGlyph);
        Assert.NotNull(font.StrikethroughGlyph);

        using var canvas = new NKCharCanvas(10, 10);
        var       bounds = new Rect2D(new Point2D(0, 0), new Point2D(10, 10));

        var style = new NKStyle(styles: UNDERLINE | STRIKETHROUGH);
        font.DrawString("e", canvas, bounds, style);

        // Assert that every rendered cell (letters and effects) has underline and strikethrough styles suppressed
        var renderedAny = false;

        for (int x = 0; x < canvas.Width; x++) {
            for (int y = 0; y < canvas.Height; y++) {
                var c = canvas.Data[x, y];

                if (c.Char != ' ') {
                    renderedAny = true;
                    Assert.False(c.Style.Styles.HasFlag(UNDERLINE));
                    Assert.False(c.Style.Styles.HasFlag(STRIKETHROUGH));
                }
            }
        }

        Assert.True(renderedAny);
    }

    [Fact]
    public void PlaceString_ShouldApplyStylesAndEffectsToGaps() {
        var font = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        Assert.NotNull(font);
        Assert.NotNull(font.UnderlineGlyph);
        Assert.NotNull(font.StrikethroughGlyph);

        using var canvas = new NKCharCanvas(20, 10);
        var       bounds = new Rect2D(new Point2D(0, 0), new Point2D(20, 10));

        var style = new NKStyle(styles: UNDERLINE | STRIKETHROUGH);
        font.DrawString("e e", canvas, bounds, style);

        // Ensure that the gap has custom underline and strikethrough glyphs ('-') rendered at correct vertical offsets in canvas
        Assert.Equal('-', canvas.Data[5, 4].Char);
        Assert.Equal('-', canvas.Data[5, 2].Char);

        // Ensure style suppression applies in the gap as well
        Assert.False(canvas.Data[5, 1].Style.Styles.HasFlag(UNDERLINE));
        Assert.False(canvas.Data[5, 1].Style.Styles.HasFlag(STRIKETHROUGH));

        // Now render a partially styled string where the space is not underlined/strikethrough: "{:u}e{:!u} e"
        using var canvas2 = new NKCharCanvas(20, 10);
        var       ansiStr = AnsiString.Parse("{:u}e{:!u} e");
        font.DrawString(ansiStr!, canvas2, bounds, new NKStyle());

        // Column 5 represents the space/gap in "e e", which is NOT underlined or strikethrough.
        // Ensure that the gap does NOT have custom underline or strikethrough cells rendered (they remain ' ').
        Assert.Equal(' ', canvas2.Data[5, 4].Char);
        Assert.Equal(' ', canvas2.Data[5, 2].Char);
    }

    [Fact]
    public void DrawString_InsideRenderContext_ShouldApplyOriginAndClip() {
        var       font    = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas  = new NKCharCanvas(20, 10);
        var       context = new RenderContext(canvas);

        // Shift origin by (5, 2) and clip locally so only global x in [8, 12] is rendered
        using (context.PushScope(offset: new Point2D(5, 2), clip: new Rect2D(3, 0, 7, 6))) {
            font!.DrawString("A", context);
        }

        // "A" top line at (3, 0) and (4, 0) relative to font origin.
        // With context origin (5, 2):
        // (3 + 5, 0 + 2) = (8, 2) -> inside clip [8..11], should be '_'
        // (4 + 5, 0 + 2) = (9, 2) -> inside clip [8..11], should be '_'
        Assert.Equal('_', canvas.Data[8, 2].Char);
        Assert.Equal('_', canvas.Data[9, 2].Char);

        // (2 + 5, 1 + 2) = (7, 3) -> '/' is at x=7 which is outside clip [8..11], should remain ' '
        Assert.Equal(' ', canvas.Data[7, 3].Char);
    }

    [Fact]
    public void DrawString_WithTextRenderingOptions_ShouldApplyColoringAndRender() {
        var       font      = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas    = new NKCharCanvas(50, 20);
        var       testColor = NKColor.FromRgb(0, 255, 128);

        var options = new TextRenderingOptions(
            horizontalAlign: HorizontalAlign.LEFT,
            verticalAlign: VerticalAlign.TOP,
            coloring: ColoringStrategies.Solid(testColor)
        );

        font!.DrawString("A", canvas, options);

        Assert.Equal('_',       canvas.Data[3, 0].Char);
        Assert.Equal(testColor, canvas.Data[3, 0].Style.FColor);
    }

    [Fact]
    public void DrawString_WithLigaturesDisabled_ShouldNotRenderLigature() {
        var       font   = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas = new NKCharCanvas(50, 20);

        var options = new TextRenderingOptions(
            spacing: new FontSpacingInfo(ligatures: false)
        );

        font!.DrawString("->", canvas, options);

        // In ligature mode, row 2 column 9 is ')' from arr-right.nkg.
        // With ligatures disabled, arr-right is not formed, so canvas.Data[9, 2] will be ' '.
        Assert.Equal(' ', canvas.Data[9, 2].Char);
    }

    [Fact]
    public void DrawString_WithCustomLeading_ShouldOverrideLineSpacing() {
        var       font   = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas = new NKCharCanvas(50, 20);

        var options = new TextRenderingOptions(
            spacing: new FontSpacingInfo(leading: 10)
        );

        font!.DrawString("A\nA", canvas, options);

        // First 'A' top is at y=0 -> Data[3, 0] is '_'
        Assert.Equal('_', canvas.Data[3, 0].Char);

        // With leading: 10, second 'A' top is at y=10 -> Data[3, 10] is '_'
        Assert.Equal('_', canvas.Data[3, 10].Char);

        // In default line spacing (5), row 5 would have had the second 'A'. Now row 5 is empty.
        Assert.Equal(' ', canvas.Data[3, 5].Char);
    }

    [Fact]
    public void DrawString_WithWordSpacing_ShouldIncreaseWordGap() {
        var       font          = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvasDefault = new NKCharCanvas(50, 20);
        using var canvasSpaced  = new NKCharCanvas(50, 20);

        font!.DrawString("A A", canvasDefault);

        var options = new TextRenderingOptions(
            spacing: new FontSpacingInfo(wordSpacing: 10)
        );

        font.DrawString("A A", canvasSpaced, options);

        // Find the start of the second 'A' in both canvases
        int firstCharEnd      = 7; // 'A' width
        int secondAColDefault = -1;
        int secondAColSpaced  = -1;

        for (int x = firstCharEnd; x < 50; x++) {
            if (canvasDefault.Data[x, 0].Char == '_') {
                secondAColDefault = x;

                break;
            }
        }

        for (int x = firstCharEnd; x < 50; x++) {
            if (canvasSpaced.Data[x, 0].Char == '_') {
                secondAColSpaced = x;

                break;
            }
        }

        Assert.True(secondAColDefault > 0,                 "Default second 'A' found");
        Assert.True(secondAColSpaced  > 0,                 "Spaced second 'A' found");
        Assert.True(secondAColSpaced  > secondAColDefault, "Spaced second 'A' is further right than default");
    }

    [Fact]
    public void DrawString_WithMonospacedProportions_ShouldSpaceEvenly() {
        var       font   = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas = new NKCharCanvas(50, 20);

        int monoAdvance = 15;

        var options = new TextRenderingOptions(
            proportions: new FontProportionsInfo(new NKFontMonospacedConfig(monoAdvance, monoAdvance, false)),
            spacing: new FontSpacingInfo(letterSpacing: 0)
        );

        font!.DrawString("AA", canvas, options);

        // First 'A' top '_' at index 3
        Assert.Equal('_', canvas.Data[3, 0].Char);

        // Second 'A' offset is glyphWidth + letterSpacing + 1 = 15 + 0 + 1 = 16.
        // Therefore '_' is at 3 + 16 = 19.
        Assert.Equal('_', canvas.Data[3 + 16, 0].Char);
    }

    [Fact]
    public void DrawString_WithJustifyText_ShouldExpandToFillBounds() {
        var       font   = NKFontSerializer.ReadEmbedded<NKFontRenderingTests>(PATH);
        using var canvas = new NKCharCanvas(40, 10);
        var       bounds = new Rect2D(0, 0, 35, 10);

        var options = new TextRenderingOptions(
            justifyText: true
        );

        font!.DrawString("A A", canvas, bounds, options);

        var measuredSize = font.GetSize(new AnsiString("A A"), options, maxWidth: 35);
        Assert.Equal(35, measuredSize.X);
    }
}