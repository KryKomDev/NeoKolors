// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Lifecycle.Apps;

namespace NeoKolors.Tui.Lifecycle.Tests;

public class NKAppViewTests {
    private sealed class MockElement : IElement {
        public int MeasureCount { get; private set; }
        public int RenderCount { get; private set; }
        public Size2D LastParentSize { get; private set; }

        public Size2D Measure(LayoutContext context) {
            MeasureCount++;
            return context.Parent;
        }

        public void Render(RenderContext context, Size2D parent) {
            RenderCount++;
            LastParentSize = parent;
        }
    }

    private sealed class MockScreen : ICharScreen {
        public int CleanCount { get; private set; }
        public int RenderCount { get; private set; }
        public int Width => 80;
        public int Height => 25;
        public Size2D Size => new(80, 25);
        public IReadOnlyList2D<NKCell> Data => new List2D<NKCell>(80, 25);

        public void Clean() => CleanCount++;
        public void Render() => RenderCount++;
        public void Resize(Size2D newSize) { }
        public void ResetZ() { }
        public void Dispose() { }

        public void Draw(ICharCanvas canvas, int x, int y, int z) { }
        public void Draw(AnsiChar c, int x, int y, int z) { }
        public void Draw(AnsiString str, int x, int y, int z) { }
        public void Draw(SkiaSharp.SKImage image, int x, int y, int w, int h, int z, SkiaSharp.SKSamplingOptions sampling = default) { }
        public void DrawVerticalLine(AnsiChar c, int x, int y, int len, int z) { }
        public void DrawHorizontalLine(AnsiChar c, int x, int y, int len, int z) { }
        public void DrawRect(AnsiChar c, int x, int y, int w, int h, int z) { }
        public void DrawBorder(NKBorderOld border, int x, int y, int w, int h, int z) { }
    }

    [Fact]
    public void NKAppView_InitialState_IsDirty() {
        var element = new MockElement();
        var screen = new MockScreen();
        var view = new NKAppView(element, screen);

        Assert.True(view.IsDirty);
        Assert.Same(element, view.RootElement);
        Assert.Same(screen, view.Screen);
    }

    [Fact]
    public void NKAppView_Render_ExecutesPipelineAndFlushes() {
        var element = new MockElement();
        var screen = new MockScreen();
        var view = new NKAppView(element, screen);

        bool beforeFired = false;
        bool afterFired = false;
        view.BeforeRender += () => beforeFired = true;
        view.AfterRender += () => afterFired = true;

        view.Render();

        Assert.True(beforeFired);
        Assert.True(afterFired);
        Assert.Equal(1, screen.CleanCount);
        Assert.Equal(1, element.MeasureCount);
        Assert.Equal(1, element.RenderCount);
        Assert.Equal(new Size2D(80, 25), element.LastParentSize);
        Assert.Equal(1, screen.RenderCount);
        Assert.False(view.IsDirty);
    }

    [Fact]
    public void NKAppView_SetRootElement_InvalidatesView() {
        var element1 = new MockElement();
        var element2 = new MockElement();
        var screen = new MockScreen();
        var view = new NKAppView(element1, screen);

        view.Render();
        Assert.False(view.IsDirty);

        bool invalidated = false;
        view.Invalidated += () => invalidated = true;

        view.RootElement = element2;
        Assert.True(view.IsDirty);
        Assert.True(invalidated);
        Assert.Same(element2, view.RootElement);
    }
}
