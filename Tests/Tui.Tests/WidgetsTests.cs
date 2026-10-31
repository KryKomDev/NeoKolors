// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Fonts.Assets;
using NeoKolors.Tui.Lifecycle.Inline;

namespace NeoKolors.Tui.Tests;

public class WidgetsTests {
    [Fact]
    public void SpinnerElement_AdvancesFrames_OnTick() {
        var style = SpinnerStyle.Dots;
        var spinner = new SpinnerElement(style);

        Assert.Equal(0, spinner.FrameIndex);

        // Step manually
        spinner.Step();
        Assert.Equal(1, spinner.FrameIndex);

        // Tick by interval
        spinner.Tick(style.Interval);
        Assert.Equal(2, spinner.FrameIndex);

        // Tick multiple intervals
        spinner.Tick(style.Interval * 3);
        Assert.Equal(5, spinner.FrameIndex);

        // Wrap around
        spinner.Tick(style.Interval * style.Frames.Count);
        Assert.Equal(5, spinner.FrameIndex);
    }

    [Fact]
    public void SpinnerElement_MeasuresAndRenders() {
        var spinner = new SpinnerElement(SpinnerStyle.Dots);
        var layoutCtx = new LayoutContext(new Size2D(80, 25), new Size2D(80, 25));
        var size = spinner.Measure(layoutCtx);

        Assert.Equal(SpinnerStyle.Dots.Frames[0].Length, size.X);
        Assert.Equal(1, size.Y);

        var canvas = new NKCharCanvas(10, 1);
        var renderCtx = new RenderContext(canvas);
        spinner.Render(renderCtx, size);

        // Assert that the first cell has been populated
        var cell = canvas.Data[0, 0];
        Assert.NotEqual('\0', cell.Char);
    }

    [Fact]
    public void ProgressBarElement_CalculatesCorrectLayout() {
        var progress = new Signal<double>(0.5);
        var bar = new ProgressBarElement(progress) {
            Width = 20,
            ShowPercentage = true
        };

        var layoutCtx = new LayoutContext(new Size2D(80, 25), new Size2D(80, 25));
        var size = bar.Measure(layoutCtx);

        Assert.Equal(20, size.X);
        Assert.Equal(1, size.Y);
    }

    [Fact]
    public void ProgressBarElement_RendersFilledAndEmptyCells() {
        var bar = new ProgressBarElement(0.5) {
            Width = 10,
            ShowPercentage = false,
            FilledChar = '#',
            EmptyChar = '-'
        };

        var canvas = new NKCharCanvas(10, 1);
        var renderCtx = new RenderContext(canvas);
        bar.Render(renderCtx, new Size2D(10, 1));

        // 50% of 10 columns = 5 filled, 5 empty
        for (int x = 0; x < 5; x++) {
            Assert.Equal('#', canvas.Data[x, 0].Char);
        }
        for (int x = 5; x < 10; x++) {
            Assert.Equal('-', canvas.Data[x, 0].Char);
        }
    }

    [Fact]
    public void StatusElement_TransitionsBetweenStates() {
        var status = new StatusElement("Processing item");
        var layoutCtx = new LayoutContext(new Size2D(80, 25), new Size2D(80, 25));

        Assert.False(status.IsCompleted.AsT);
        Assert.False(status.IsFailed.AsT);

        var canvas = new NKCharCanvas(40, 1);
        var renderCtx = new RenderContext(canvas);
        status.Render(renderCtx, status.Measure(layoutCtx));

        // Mark as completed
        status.IsCompleted = true;
        status.SuccessText = "Finished processing!";

        Assert.True(status.IsCompleted.AsT);
        var completedSize = status.Measure(layoutCtx);
        Assert.True(completedSize.X > 0);
    }

    [Fact]
    public void NKInlineScreen_TracksRenderedHeight() {
        var screen = new NKInlineScreen(40, 3);
        Assert.Equal(0, screen.RenderedHeight);

        screen.Render();
        Assert.Equal(3, screen.RenderedHeight);

        screen.Finish(persist: true);
        Assert.Equal(0, screen.RenderedHeight);
    }

    [Fact]
    public async Task InlineLifecycleHost_ExecutesWorkloadSuccessfully() {
        var host = new InlineLifecycleHost {
            TargetFps = 30,
            PersistOnExit = false,
            AutoHideCursor = false,
            DisableAutoWrap = false
        };

        var text = new Text(new AnsiString("Inline lifecycle test"));
        bool executed = false;

        await host.RunAsync(text, ctx => {
            executed = true;
            ctx.Invalidate();
            Assert.True(ctx.ElapsedTime >= TimeSpan.Zero);
            return Task.CompletedTask;
        });

        Assert.True(executed);
    }

    [Fact]
    public async Task CompositeUI_RendersNormalElementsInline() {
        var progress = new Signal<double>(0.25);
        var status = new Signal<string>("Compiling sources...");

        // Compose normal elements: Border, Flex, Text with ProgressBar and Spinner
        var root = new Border(
            element: new Flex(
                direction: FlexDirection.COLUMN,
                children: new IElement[] {
                    new Flex(
                        direction: FlexDirection.ROW,
                        gap: 1,
                        children: new IElement[] {
                            new SpinnerElement(),
                            new Text((Func<AnsiString>)(() => new AnsiString(status.Value)))
                        }
                    ),
                    new ProgressBarElement {
                        Value = progress,
                        Width = 30
                    }
                }
            ),
            style: NKBorderStyle.GetRounded()
        );

        var host = new InlineLifecycleHost {
            TargetFps = 20,
            PersistOnExit = false,
            AutoHideCursor = false,
            DisableAutoWrap = false
        };

        await host.RunAsync(root, async ctx => {
            progress.Value = 0.75;
            status.Value = "Linking assemblies...";
            ctx.Invalidate();
            await Task.Yield();
        });

        Assert.Equal(0.75, progress.Value);
    }

    [Fact]
    public void Font_DrawString_PreservesStylesInBoundingBox_WithInlineScreen() {
        AssetsProvider.RegisterFonts();
        var font = AssetsProvider.Bytesized;
        Assert.NotNull(font);
        var str = AnsiString.Parse("{:n}O");
        Assert.Equal("O", str.Plain);
        Assert.True(str[0].Style.Styles.GetIsNegative());

        using var canvas = new NKCharCanvas(10, 10);
        font.DrawString(str, canvas);

        // Check cells of 'O'
        for (int y = 0; y < 3; y++) {
            for (int x = 0; x < 3; x++) {
                var c = canvas.Data[x, y];
                Assert.True(c.Style.Styles.GetIsNegative(), $"Cell [{x}, {y}] with char '{c.Char}' has style {c.Style.Styles}");
            }
        }

        var bgStr = AnsiString.Parse("{:b#FF0000}O");
        using var bgCanvas = new NKCharCanvas(10, 10);
        font.DrawString(bgStr, bgCanvas);

        for (int y = 0; y < 3; y++) {
            for (int x = 0; x < 3; x++) {
                var c = bgCanvas.Data[x, y];
                Assert.True(c.Style.BColor.IsRgb, $"Cell [{x}, {y}] with char '{c.Char}' does not have RGB background color");
            }
        }

        // Test with NKInlineScreen
        var screen = new NKInlineScreen(10, 10);
        screen.Draw(bgCanvas, 0, 0, 0);
        for (int y = 0; y < 3; y++) {
            for (int x = 0; x < 3; x++) {
                var c = screen.Data[x, y];
                Assert.True(c.Style.BColor.IsRgb, $"InlineScreen cell [{x}, {y}] with char '{c.Char}' does not have RGB background color");
            }
        }
    }

    [Fact]
    public void Font_Dummy_DrawString_PreservesStylesInBoundingBox_WithInlineScreen() {
        AssetsProvider.RegisterFonts();
        var font = AssetsProvider.Dummy;
        Assert.NotNull(font);
        var str = AnsiString.Parse("{:n}A");
        Assert.Equal("A", str.Plain);
        Assert.True(str[0].Style.Styles.GetIsNegative());

        var measured = font.GetSize(str);
        Assert.Equal(8, measured.X);
        Assert.Equal(5, measured.Y);

        using var canvas = new NKCharCanvas(15, 10);
        font.DrawString(str, canvas);

        // All 8x5 cells in the bounding box must have the negative style
        for (int y = 0; y < 5; y++) {
            for (int x = 0; x < 8; x++) {
                var c = canvas.Data[x, y];
                Assert.True(c.Style.Styles.GetIsNegative(), $"Dummy cell [{x}, {y}] with char '{c.Char}' has style {c.Style.Styles}");
            }
        }

        // Test with background color
        var bgStr = AnsiString.Parse("{:b#00FF00}A");
        using var bgCanvas = new NKCharCanvas(15, 10);
        font.DrawString(bgStr, bgCanvas);

        for (int y = 0; y < 5; y++) {
            for (int x = 0; x < 8; x++) {
                var c = bgCanvas.Data[x, y];
                Assert.True(c.Style.BColor.IsRgb, $"Dummy cell [{x}, {y}] with char '{c.Char}' does not have RGB background color");
            }
        }

        // Test with NKInlineScreen
        var screen = new NKInlineScreen(15, 10);
        screen.Draw(bgCanvas, 0, 0, 0);
        for (int y = 0; y < 5; y++) {
            for (int x = 0; x < 8; x++) {
                var c = screen.Data[x, y];
                Assert.True(c.Style.BColor.IsRgb, $"InlineScreen Dummy cell [{x}, {y}] with char '{c.Char}' does not have RGB background color");
            }
        }
    }

    [Fact]
    public void Font_Dummy_MultiCharWithStyles_PreservesWhitespaceBoundingBoxes() {
        AssetsProvider.RegisterFonts();
        var font = AssetsProvider.Dummy;
        Assert.NotNull(font);
        var str = AnsiString.Parse("{:n}A {:b#FF0000}B");
        var size = font.GetSize(str);

        using var canvas = new NKCharCanvas(size.X, size.Y);
        font.DrawString(str, canvas);

        var aWidth = font.GetSize(AnsiString.Parse("A")).X;
        var bWidth = font.GetSize(AnsiString.Parse("B")).X;

        // 'A' bounding box cells must have negative style
        for (int y = 0; y < 5; y++) {
            for (int x = 0; x < aWidth; x++) {
                var c = canvas.Data[x, y];
                Assert.True(c.Style.Styles.GetIsNegative(), $"Dummy 'A' cell [{x}, {y}] with char '{c.Char}' has style {c.Style.Styles}");
            }
        }

        // Space between A and B must also preserve negative style across its height
        for (int y = 0; y < 5; y++) {
            for (int x = aWidth; x < size.X - bWidth; x++) {
                var c = canvas.Data[x, y];
                Assert.True(c.Style.Styles.GetIsNegative(), $"Dummy space cell [{x}, {y}] with char '{c.Char}' has style {c.Style.Styles}");
            }
        }

        // 'B' bounding box cells must have RGB background color
        for (int y = 0; y < 5; y++) {
            for (int x = size.X - bWidth; x < size.X; x++) {
                var c = canvas.Data[x, y];
                Assert.True(c.Style.BColor.IsRgb, $"Dummy 'B' cell [{x}, {y}] with char '{c.Char}' does not have RGB background color");
            }
        }
    }
}
