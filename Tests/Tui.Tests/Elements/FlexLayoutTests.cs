// NeoKolors.Test
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Tests.Elements;

using CoreDimension = Dimension;
using CoreSpacing = Spacing;
using Element = IElement;

public class FlexLayoutTests {

    [Fact]
    public void Dimension_Resolve_WorksForFixedAndGrow() {
        var screen = new Size2D(120, 40);

        var charsDim = CoreDimension.Chars(15);
        Assert.Equal(15, charsDim.Resolve(100, screen));

        var percentDim = CoreDimension.Percent(50);
        Assert.Equal(50, percentDim.Resolve(100, screen));

        var vwDim = CoreDimension.Vw(50);
        Assert.Equal(60, vwDim.Resolve(100, screen));

        var growDim = new CoreDimension(
            new CoreDimension.Grow(
                1,
                new CoreDimension.Fixed(5,  CoreDimension.Fixed.Units.CHARS),
                new CoreDimension.Fixed(50, CoreDimension.Fixed.Units.CHARS)
            )
        );

        Assert.Equal(5,  growDim.Resolve(100, screen, ungrown: true));
        Assert.Equal(50, growDim.Resolve(100, screen, ungrown: false));
    }

    [Fact]
    public void Spacing_Resolve_ReturnsCorrectOffsetsAndSizes() {
        var screen  = new Size2D(100, 50);
        var parent  = new Size2D(80,  40);
        var context = new LayoutContext(parent, screen);

        var spacing = new CoreSpacing(
            top: CoreDimension.Chars(2),
            bottom: CoreDimension.Chars(3),
            left: CoreDimension.Chars(4),
            right: CoreDimension.Chars(5)
        );

        var resolved = spacing.Resolve(context);
        Assert.Equal(2,                 resolved.Top);
        Assert.Equal(3,                 resolved.Bottom);
        Assert.Equal(4,                 resolved.Left);
        Assert.Equal(5,                 resolved.Right);
        Assert.Equal(9,                 resolved.Horizontal);
        Assert.Equal(5,                 resolved.Vertical);
        Assert.Equal(new Point2D(4, 2), resolved.Offset);
        Assert.Equal(new Size2D(9, 5),  resolved.Size);
    }

    [Fact]
    public void Flex_Row_MeasuresChildrenCorrectly() {
        var screen  = new Size2D(100, 50);
        var parent  = new Size2D(100, 50);
        var context = new LayoutContext(parent, screen);

        var text1 = new Text(content: new AnsiString("Hello"));  // Length = 5, Height = 1
        var text2 = new Text(content: new AnsiString("World!")); // Length = 6, Height = 1

        var flex = new Flex(
            children: new Element[] { text1, text2 },
            direction: FlexDirection.ROW,
            gap: 2
        );

        var measured = flex.Measure(context);

        // 5 + 6 + gap(2) = 13, height = max(1, 1) = 1
        Assert.Equal(13, measured.X);
        Assert.Equal(1,  measured.Y);
    }

    [Fact]
    public void Flex_Column_MeasuresChildrenCorrectly() {
        var screen  = new Size2D(100, 50);
        var parent  = new Size2D(100, 50);
        var context = new LayoutContext(parent, screen);

        var text1 = new Text(content: new AnsiString("Line1"));       // Length = 5, Height = 1
        var text2 = new Text(content: new AnsiString("LongerLine2")); // Length = 11, Height = 1

        var flex = new Flex(
            children: new Element[] { text1, text2 },
            direction: FlexDirection.COLUMN,
            gap: 1
        );

        var measured = flex.Measure(context);

        // Height = 1 + 1 + gap(1) = 3, Width = max(5, 11) = 11
        Assert.Equal(11, measured.X);
        Assert.Equal(3,  measured.Y);
    }

    [Fact]
    public void Flex_Row_RendersWithGrowAndJustify() {
        using var canvas        = new NKCharCanvas(30, 5);
        var       renderContext = new RenderContext(canvas);

        var text1 = new Text(content: new AnsiString("Left"));
        var text2 = new Text(content: new AnsiString("Right"));

        var flex = new Flex(
            children: new Element[] {
                Flex.Item(text1, grow: 1),
                Flex.Item(text2, grow: 1)
            },
            direction: FlexDirection.ROW,
            gap: 0
        );

        flex.Render(renderContext, new Size2D(30, 5));

        // Canvas width is maintained
        Assert.Equal(30, canvas.Width);
    }

    [Fact]
    public void Border_MeasureAndRender_CalculatesCorrectOffsets() {
        var screen  = new Size2D(100, 50);
        var context = new LayoutContext(new Size2D(50, 20), screen);

        var text = new Text(content: new AnsiString("Content")); // 7x1

        var border = new Border(
            element: text,
            padding: new CoreSpacing(CoreDimension.Chars(1)), // +2 horizontally, +2 vertically
            margin: new CoreSpacing(CoreDimension.Chars(2)),  // +4 horizontally, +4 vertically
            style: NKBorderStyle.GetAscii()                   // +2 horizontally, +2 vertically
        );

        var size = border.Measure(context);

        // Content: 7x1
        // + Padding (2x2): 9x3
        // + Border lines (2x2): 11x5
        // + Margin (4x4): 15x9
        Assert.Equal(15, size.X);
        Assert.Equal(9,  size.Y);

        using var canvas        = new NKCharCanvas(30, 20);
        var       renderContext = new RenderContext(canvas);
        border.Render(renderContext, size);

        // Check that border rendered at margin offset (2, 2)
        Assert.NotEqual(default, canvas.Data[2, 2]);
    }

    [Fact]
    public void FlexBox_WithExplicitSizing_MeasureReturnsExactSize() {
        var screen  = new Size2D(120, 40);
        var parent  = new Size2D(100, 30);
        var context = new LayoutContext(parent, screen);

        var box = new FlexBox(
            sizing: (45, 12),
            new Text(new AnsiString("Short"))
        );

        var measured = box.Measure(context);
        Assert.Equal(45, measured.X);
        Assert.Equal(12, measured.Y);
    }

    [Fact]
    public void FlexBox_EmptyWithSizing_ReturnsSizing() {
        var screen  = new Size2D(100, 50);
        var parent  = new Size2D(80, 40);
        var context = new LayoutContext(parent, screen);

        var box = new FlexBox { Sizing = Sizing.Fixed(35, 15) };
        var measured = box.Measure(context);

        Assert.Equal(35, measured.X);
        Assert.Equal(15, measured.Y);
    }

    [Fact]
    public void FlexBox_WithWidthOnlySizing_AdaptsHeightToContent() {
        var screen  = new Size2D(100, 50);
        var parent  = new Size2D(80, 40);
        var context = new LayoutContext(parent, screen);

        var text1 = new Text(new AnsiString("Row1")); // height 1
        var text2 = new Text(new AnsiString("Row2")); // height 1

        var box = FlexBox.Column(
            sizing: Sizing.FromWidth(CoreDimension.Chars(50)),
            text1, text2
        );

        var measured = box.Measure(context);
        Assert.Equal(50, measured.X);
        Assert.Equal(2,  measured.Y); // 1 + 1 = 2
    }

    [Fact]
    public void FlexBox_WithPercentageSizing_ResolvesAgainstParent() {
        var screen  = new Size2D(100, 50);
        var parent  = new Size2D(80, 40);
        var context = new LayoutContext(parent, screen);

        var box = new FlexBox {
            Sizing = new Sizing(CoreDimension.Percent(50), CoreDimension.Percent(25))
        };

        var measured = box.Measure(context);
        Assert.Equal(40, measured.X); // 50% of 80 = 40
        Assert.Equal(10, measured.Y); // 25% of 40 = 10
    }

    [Fact]
    public void FlexBox_Render_ConstrainsContentToSizing() {
        using var canvas        = new NKCharCanvas(80, 20);
        var       renderContext = new RenderContext(canvas);

        var text1 = new Text(new AnsiString("Left"));
        var text2 = new Text(new AnsiString("Right"));

        // Box constrained to width 20, with justify content END
        var box = new FlexBox(
            sizing: (20, 5),
            children: new Element[] { text1, text2 }
        ) {
            Direction = FlexDirection.ROW,
            JustifyContent = JustifyContent.END,
            Gap = 1
        };

        box.Render(renderContext, new Size2D(80, 20));

        // Total content width: "Left"(4) + gap(1) + "Right"(5) = 10
        // Free space in 20 chars = 10. Justify END puts Left at X=10, Right at X=15.
        // It should NOT justify to the outer 80 canvas width.
        Assert.Equal('L', canvas.Data[10, 0].Char); // 'L' at x=10
        Assert.Equal('R', canvas.Data[15, 0].Char); // 'R' at x=15
        Assert.Equal(NKCell.Default, canvas.Data[70, 0]); // x=70 is untouched
    }

    [Fact]
    public void FlexBox_WithGrowWidth_ExpandsToAvailableParentWidth() {
        var screen  = new Size2D(120, 40);
        var parent  = new Size2D(100, 30);
        var context = new LayoutContext(parent, screen);

        // Status bar with GrowTrack / Auto width and 3 items
        var box = new FlexBox(
            sizing: new Sizing(CoreDimension.GrowTrack(), CoreDimension.Chars(1)),
            new Text(new AnsiString("Title")),
            new Text(new AnsiString("Status")),
            new Text(new AnsiString("Quit"))
        ) {
            Direction = FlexDirection.ROW,
            JustifyContent = JustifyContent.SPACE_BETWEEN
        };

        var measured = box.Measure(context);
        Assert.Equal(100, measured.X);
        Assert.Equal(1,   measured.Y);
    }

    [Fact]
    public void Border_WithGrowChild_ExpandsToAvailableParentWidth() {
        var screen  = new Size2D(120, 40);
        var parent  = new Size2D(100, 30);
        var context = new LayoutContext(parent, screen);

        var topBar = new Border(
            element: new Flex(
                direction: FlexDirection.ROW,
                justifyContent: JustifyContent.SPACE_BETWEEN,
                children: new Element[] {
                    new Text(new AnsiString("Title")),
                    new Text(new AnsiString("Status"))
                },
                sizing: new Sizing(CoreDimension.Auto, CoreDimension.Chars(1))
            ),
            style: NKBorderStyle.GetRounded()
        );

        var measured = topBar.Measure(context);
        // Border: 2 overhead horizontally (rounded corners) + child desired (100 - 2 = 98) => 100
        Assert.Equal(100, measured.X);
        Assert.Equal(3,   measured.Y); // 1 content + 2 border = 3
    }

    [Fact]
    public void Border_WithExplicitSizing_MeasureAndRenderRespectsSizing() {
        var screen  = new Size2D(120, 40);
        var parent  = new Size2D(100, 30);
        var context = new LayoutContext(parent, screen);

        var border = new Border(
            element: new Text(new AnsiString("Inside")),
            style: NKBorderStyle.GetRounded(),
            sizing: Sizing.Fixed(50, 6)
        );

        var measured = border.Measure(context);
        Assert.Equal(50, measured.X);
        Assert.Equal(6,  measured.Y);
    }
}