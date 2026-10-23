// NeoKolors.Test
// Copyright (c) 2026 KryKom

using SkiaSharp;

namespace NeoKolors.Tui.Tests.Core;

public class RenderContextTests {

    #region Constructor & Basic Properties

    [Fact]
    public void Constructor_NullCanvas_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>("canvas", () => new RenderContext(null!));
    }

    [Fact]
    public void Constructor_ValidCanvas_ExposesCanvasAndDimensions() {
        using var canvas  = new NKCharCanvas(30, 20);
        var       context = new RenderContext(canvas);

        Assert.Same(canvas, context.Canvas);
        Assert.Equal(30,                       context.CanvasWidth);
        Assert.Equal(20,                       context.CanvasHeight);
        Assert.Equal(new Size2D(30, 20),       context.CanvasSize);
        Assert.Equal(new Size2D(30, 20),       context.Size);
        Assert.Equal(new Rect2D(0, 0, 30, 20), context.CanvasBounds);
        Assert.Null(context.Data);
    }

    [Fact]
    public void Constructor_InitialState_MatchesDefaultRenderState() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        Assert.Equal(0,            context.StateDepth);
        Assert.Equal(0,            context.SaveCount);
        Assert.Equal(Point2D.Zero, context.Origin);
        Assert.Null(context.Clip);
        Assert.Equal(0,               context.ZIndex);
        Assert.Equal(NKStyle.Default, context.CurrentStyle);
        Assert.False(context.AllowOverflow);
        Assert.Null(context.ContextArea);

        var state = context.CurrentState;
        Assert.Equal(Point2D.Zero, state.Origin);
        Assert.Null(state.Clip);
        Assert.Equal(0,               state.ZIndex);
        Assert.Equal(NKStyle.Default, state.Style);
        Assert.False(state.AllowOverflow);
        Assert.Null(state.ContextArea);
    }

    #endregion

    #region Property Setters

    [Fact]
    public void Origin_Set_UpdatesPropertyAndCurrentState() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        var newOrigin = new Point2D(4, 7);
        context.Origin = newOrigin;

        Assert.Equal(newOrigin, context.Origin);
        Assert.Equal(newOrigin, context.CurrentState.Origin);
    }

    [Fact]
    public void Clip_Set_UpdatesPropertyAndCurrentState() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        var clip = new Rect2D(1, 2, 8, 9);
        context.Clip = clip;

        Assert.Equal(clip, context.Clip);
        Assert.Equal(clip, context.CurrentState.Clip);

        context.Clip = null;
        Assert.Null(context.Clip);
        Assert.Null(context.CurrentState.Clip);
    }

    [Fact]
    public void ZIndex_Set_UpdatesPropertyAndCurrentState() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            ZIndex = 42
        };

        Assert.Equal(42, context.ZIndex);
        Assert.Equal(42, context.CurrentState.ZIndex);
    }

    [Fact]
    public void CurrentStyle_Set_UpdatesPropertyAndCurrentState() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        var style = new NKStyle(ConsoleColor.Cyan);
        context.CurrentStyle = style;

        Assert.Equal(style, context.CurrentStyle);
        Assert.Equal(style, context.CurrentState.Style);
    }

    [Fact]
    public void AllowOverflow_Set_UpdatesPropertyAndCurrentState() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            AllowOverflow = true
        };

        Assert.True(context.AllowOverflow);
        Assert.True(context.CurrentState.AllowOverflow);
    }

    [Fact]
    public void ContextArea_Set_UpdatesPropertyAndCurrentState() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        var area = new Rect2D(2, 3, 10, 12);
        context.ContextArea = area;

        Assert.Equal(area, context.ContextArea);
        Assert.Equal(area, context.CurrentState.ContextArea);
    }

    #endregion

    #region State Stack Management (Save, Restore, RestoreToDepth, Reset)

    [Fact]
    public void Save_IncrementsStateDepthAndSaveCount() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        Assert.Equal(0, context.StateDepth);
        Assert.Equal(0, context.SaveCount);

        var s1 = context.Save();
        Assert.Equal(1, context.StateDepth);
        Assert.Equal(1, context.SaveCount);

        var s2 = context.Save();
        Assert.Equal(2, context.StateDepth);
        Assert.Equal(2, context.SaveCount);
    }

    [Fact]
    public void Restore_OnEmptyStack_ThrowsInvalidOperationException() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        var ex = Assert.Throws<InvalidOperationException>(() => context.Restore());
        Assert.Contains("empty", ex.Message);
    }

    [Fact]
    public void Restore_RestoresPreviousStateAndDecrementsDepth() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            Origin        = new Point2D(2, 3),
            Clip          = new Rect2D(1, 1, 5, 5),
            ZIndex        = 5,
            CurrentStyle  = new NKStyle(ConsoleColor.Yellow),
            AllowOverflow = true,
            ContextArea   = new Rect2D(0, 0, 8, 8)
        };

        context.Save();

        // Mutate everything
        context.Origin        = new Point2D(20, 30);
        context.Clip          = new Rect2D(0, 0, 2, 2);
        context.ZIndex        = 99;
        context.CurrentStyle  = new NKStyle(ConsoleColor.Red);
        context.AllowOverflow = false;
        context.ContextArea   = new Rect2D(1, 1, 3, 3);

        context.Restore();

        Assert.Equal(0,                                context.StateDepth);
        Assert.Equal(new Point2D(2, 3),                context.Origin);
        Assert.Equal(new Rect2D(1, 1, 5, 5),           context.Clip);
        Assert.Equal(5,                                context.ZIndex);
        Assert.Equal(new NKStyle(ConsoleColor.Yellow), context.CurrentStyle);
        Assert.True(context.AllowOverflow);
        Assert.Equal(new Rect2D(0, 0, 8, 8), context.ContextArea);
    }

    [Fact]
    public void RestoreToDepth_RestoresToTargetLevel() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        for (int i = 1; i <= 5; i++) {
            context.Save();
            context.Origin = new Point2D(i, i);
        }

        Assert.Equal(5,                 context.StateDepth);
        Assert.Equal(new Point2D(5, 5), context.Origin);

        context.RestoreToDepth(2);

        Assert.Equal(2,                 context.StateDepth);
        Assert.Equal(new Point2D(2, 2), context.Origin);
    }

    [Fact]
    public void RestoreToDepth_GreaterOrEqualToCurrentDepth_DoesNothing() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        context.Save();
        context.Origin = new Point2D(3, 3);

        Assert.Equal(1, context.StateDepth);
        context.RestoreToDepth(1);
        Assert.Equal(1,                 context.StateDepth);
        Assert.Equal(new Point2D(3, 3), context.Origin);

        context.RestoreToDepth(5);
        Assert.Equal(1,                 context.StateDepth);
        Assert.Equal(new Point2D(3, 3), context.Origin);
    }

    [Fact]
    public void Reset_ClearsStackAndRestoresDefaultState() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        context.Save();
        context.Origin        = new Point2D(10, 20);
        context.Clip          = new Rect2D(1, 2, 3, 4);
        context.ZIndex        = 7;
        context.CurrentStyle  = new NKStyle(ConsoleColor.Green);
        context.AllowOverflow = true;
        context.ContextArea   = new Rect2D(0, 0, 5, 5);
        context.Save();

        Assert.Equal(2, context.StateDepth);

        context.Reset();

        Assert.Equal(0,            context.StateDepth);
        Assert.Equal(0,            context.SaveCount);
        Assert.Equal(Point2D.Zero, context.Origin);
        Assert.Null(context.Clip);
        Assert.Equal(0,               context.ZIndex);
        Assert.Equal(NKStyle.Default, context.CurrentStyle);
        Assert.False(context.AllowOverflow);
        Assert.Null(context.ContextArea);
    }

    #endregion

    #region RenderScope & Lifetime

    [Fact]
    public void RenderScope_Dispose_RestoresState() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(1, 1)
        };

        using (context.Save()) {
            context.Origin = new Point2D(10, 10);
            Assert.Equal(new Point2D(10, 10), context.Origin);
            Assert.Equal(1,                   context.StateDepth);
        }

        Assert.Equal(0,                 context.StateDepth);
        Assert.Equal(new Point2D(1, 1), context.Origin);
    }

    [Fact]
    public void RenderScope_NestedScopes_RestoreInOrder() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        using (context.Save()) {
            context.Origin = new Point2D(5, 5);

            using (context.Save()) {
                context.Origin = new Point2D(10, 10);
                Assert.Equal(new Point2D(10, 10), context.Origin);
                Assert.Equal(2,                   context.StateDepth);
            }

            Assert.Equal(new Point2D(5, 5), context.Origin);
            Assert.Equal(1,                 context.StateDepth);
        }

        Assert.Equal(Point2D.Zero, context.Origin);
        Assert.Equal(0,            context.StateDepth);
    }

    [Fact]
    public void RenderScope_MultipleDispose_IsIdempotent() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(2, 2)
        };

        var scope = context.Save();
        context.Origin = new Point2D(8, 8);

        scope.Dispose();
        Assert.Equal(new Point2D(2, 2), context.Origin);
        Assert.Equal(0,                 context.StateDepth);

        // Second dispose should not throw or alter state
        scope.Dispose();
        Assert.Equal(new Point2D(2, 2), context.Origin);
        Assert.Equal(0,                 context.StateDepth);
    }

    [Fact]
    public void RenderScope_DefaultStruct_DisposeDoesNotThrow() {
        RenderScope defaultScope = default;
        var         ex           = Record.Exception(() => defaultScope.Dispose());
        Assert.Null(ex);
    }

    #endregion

    #region Scoped Helpers: Push / With

    [Fact]
    public void PushOffset_Point2D_TranslatesOriginRelatively() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(3, 4)
        };

        using (context.PushOffset(new Point2D(2, 5))) {
            Assert.Equal(new Point2D(5, 9), context.Origin);
        }

        Assert.Equal(new Point2D(3, 4), context.Origin);
    }

    [Fact]
    public void PushOffset_Ints_TranslatesOriginRelatively() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(3, 4)
        };

        using (context.PushOffset(2, 5)) {
            Assert.Equal(new Point2D(5, 9), context.Origin);
        }

        Assert.Equal(new Point2D(3, 4), context.Origin);
    }

    [Fact]
    public void WithOffset_BehavesIdenticallyToPushOffset() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(1, 1)
        };

        using (context.WithOffset(new Point2D(2, 3))) {
            Assert.Equal(new Point2D(3, 4), context.Origin);
        }

        Assert.Equal(new Point2D(1, 1), context.Origin);

        using (context.WithOffset(4, 5)) {
            Assert.Equal(new Point2D(5, 6), context.Origin);
        }

        Assert.Equal(new Point2D(1, 1), context.Origin);
    }

    [Fact]
    public void PushOrigin_And_WithOrigin_SetsAbsoluteOrigin() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(10, 20)
        };

        using (context.PushOrigin(new Point2D(3, 7))) {
            Assert.Equal(new Point2D(3, 7), context.Origin);
        }

        Assert.Equal(new Point2D(10, 20), context.Origin);

        using (context.WithOrigin(new Point2D(5, 8))) {
            Assert.Equal(new Point2D(5, 8), context.Origin);
        }

        Assert.Equal(new Point2D(10, 20), context.Origin);
    }

    [Fact]
    public void PushClip_WithoutExistingClip_SetsGlobalClipOffsetByOrigin() {
        using var canvas = new NKCharCanvas(20, 20);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(5, 5)
        };

        using (context.PushClip(new Rect2D(1, 2, 10, 12))) {
            Assert.Equal(new Rect2D(6, 7, 15, 17), context.Clip);
        }

        Assert.Null(context.Clip);
    }

    [Fact]
    public void PushClip_WithExistingClip_IntersectTrue_Intersects() {
        using var canvas = new NKCharCanvas(20, 20);

        var context = new RenderContext(canvas) {
            Clip   = new Rect2D(0, 0, 10, 10),
            Origin = new Point2D(2, 2)
        };

        // local (0, 0, 12, 5) + origin (2, 2) = global (2, 2, 14, 7)
        // intersection of (0, 0, 10, 10) and (2, 2, 14, 7) = (2, 2, 10, 7)
        using (context.PushClip(new Rect2D(0, 0, 12, 5), intersect: true)) {
            Assert.Equal(new Rect2D(2, 2, 10, 7), context.Clip);
        }

        Assert.Equal(new Rect2D(0, 0, 10, 10), context.Clip);
    }

    [Fact]
    public void PushClip_WithExistingClip_IntersectFalse_ReplacesClip() {
        using var canvas = new NKCharCanvas(20, 20);

        var context = new RenderContext(canvas) {
            Clip   = new Rect2D(0, 0, 10, 10),
            Origin = new Point2D(2, 2)
        };

        using (context.PushClip(new Rect2D(0, 0, 12, 5), intersect: false)) {
            Assert.Equal(new Rect2D(2, 2, 14, 7), context.Clip);
        }

        Assert.Equal(new Rect2D(0, 0, 10, 10), context.Clip);
    }

    [Fact]
    public void WithClip_BehavesIdenticallyToPushClip() {
        using var canvas = new NKCharCanvas(20, 20);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(3, 3)
        };

        using (context.WithClip(new Rect2D(1, 1, 5, 5))) {
            Assert.Equal(new Rect2D(4, 4, 8, 8), context.Clip);
        }

        Assert.Null(context.Clip);
    }

    [Fact]
    public void PushGlobalClip_And_WithGlobalClip_SetsAbsoluteClipWithoutOriginOffset() {
        using var canvas = new NKCharCanvas(20, 20);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(10, 10)
        };

        var globalClip = new Rect2D(2, 3, 12, 15);

        using (context.PushGlobalClip(globalClip)) {
            Assert.Equal(globalClip, context.Clip);
        }

        Assert.Null(context.Clip);

        context.Clip = new Rect2D(0, 0, 8, 8);

        using (context.WithGlobalClip(new Rect2D(4, 4, 12, 12), intersect: true)) {
            Assert.Equal(new Rect2D(4, 4, 8, 8), context.Clip);
        }

        Assert.Equal(new Rect2D(0, 0, 8, 8), context.Clip);

        using (context.WithGlobalClip(new Rect2D(4, 4, 12, 12), intersect: false)) {
            Assert.Equal(new Rect2D(4, 4, 12, 12), context.Clip);
        }
    }

    [Fact]
    public void PushUnclipped_And_WithUnclipped_EnablesOverflow() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);
        Assert.False(context.AllowOverflow);

        using (context.PushUnclipped()) {
            Assert.True(context.AllowOverflow);
        }

        Assert.False(context.AllowOverflow);

        using (context.WithUnclipped()) {
            Assert.True(context.AllowOverflow);
        }

        Assert.False(context.AllowOverflow);
    }

    [Fact]
    public void AllowOverflowScope_And_WithOverflow_TogglesOverflow() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        using (context.AllowOverflowScope()) {
            Assert.True(context.AllowOverflow);

            using (context.AllowOverflowScope(false)) {
                Assert.False(context.AllowOverflow);
            }

            Assert.True(context.AllowOverflow);
        }

        Assert.False(context.AllowOverflow);

        using (context.WithOverflow()) {
            Assert.True(context.AllowOverflow);
        }
    }

    [Fact]
    public void PushZIndex_And_WithZIndex_SetsZIndex() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            ZIndex = 5
        };

        using (context.PushZIndex(15)) {
            Assert.Equal(15, context.ZIndex);
        }

        Assert.Equal(5, context.ZIndex);

        using (context.WithZIndex(25)) {
            Assert.Equal(25, context.ZIndex);
        }

        Assert.Equal(5, context.ZIndex);
    }

    [Fact]
    public void PushZIndexDelta_AddsDeltaToCurrentZIndex() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            ZIndex = 10
        };

        using (context.PushZIndexDelta(5)) {
            Assert.Equal(15, context.ZIndex);

            using (context.PushZIndexDelta(-8)) {
                Assert.Equal(7, context.ZIndex);
            }

            Assert.Equal(15, context.ZIndex);
        }

        Assert.Equal(10, context.ZIndex);
    }

    [Fact]
    public void PushStyle_OverrideCurrentTrue_MergesStyles() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            CurrentStyle = new NKStyle(textColor: ConsoleColor.Red)
        };

        var bgStyle = new NKStyle(textColor: NKColor.Inherit, backgroundColor: ConsoleColor.Blue);

        using (context.PushStyle(bgStyle, overrideCurrent: true)) {
            Assert.Equal(new NKColor(ConsoleColor.Red),  context.CurrentStyle.FColor);
            Assert.Equal(new NKColor(ConsoleColor.Blue), context.CurrentStyle.BColor);
        }

        Assert.Equal(new NKStyle(textColor: ConsoleColor.Red), context.CurrentStyle);
    }

    [Fact]
    public void PushStyle_OverrideCurrentFalse_ReplacesStyle() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            CurrentStyle = new NKStyle(textColor: ConsoleColor.Red)
        };

        var bgStyle = new NKStyle(backgroundColor: ConsoleColor.Blue);

        using (context.PushStyle(bgStyle, overrideCurrent: false)) {
            Assert.Equal(bgStyle, context.CurrentStyle);
        }

        Assert.Equal(new NKStyle(textColor: ConsoleColor.Red), context.CurrentStyle);
    }

    [Fact]
    public void WithStyle_MergesStyle() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            CurrentStyle = new NKStyle(textColor: ConsoleColor.Yellow)
        };

        using (context.WithStyle(new NKStyle(textColor: NKColor.Inherit, backgroundColor: ConsoleColor.DarkGray))) {
            Assert.Equal(new NKColor(ConsoleColor.Yellow),   context.CurrentStyle.FColor);
            Assert.Equal(new NKColor(ConsoleColor.DarkGray), context.CurrentStyle.BColor);
        }

        Assert.Equal(new NKStyle(textColor: ConsoleColor.Yellow), context.CurrentStyle);
    }

    [Fact]
    public void PushForeground_And_WithForeground_UpdatesTextColor() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        using (context.PushForeground(NKColor.Red)) {
            Assert.Equal(NKColor.Red, context.CurrentStyle.FColor);
        }

        Assert.Equal(NKColor.Default, context.CurrentStyle.FColor);

        using (context.WithForeground(NKColor.Green)) {
            Assert.Equal(NKColor.Green, context.CurrentStyle.FColor);
        }

        Assert.Equal(NKColor.Default, context.CurrentStyle.FColor);
    }

    [Fact]
    public void PushBackground_And_WithBackground_UpdatesBackgroundColor() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        using (context.PushBackground(NKColor.Blue)) {
            Assert.Equal(NKColor.Blue, context.CurrentStyle.BColor);
        }

        Assert.Equal(NKColor.Default, context.CurrentStyle.BColor);

        using (context.WithBackground(NKColor.Cyan)) {
            Assert.Equal(NKColor.Cyan, context.CurrentStyle.BColor);
        }

        Assert.Equal(NKColor.Default, context.CurrentStyle.BColor);
    }

    [Fact]
    public void PushTextStyle_And_WithTextStyle_UpdatesTextStyleFlags() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        using (context.PushTextStyle(NKTextStyles.BOLD)) {
            Assert.Equal(NKTextStyles.BOLD, context.CurrentStyle.Styles);
        }

        Assert.Equal(NKTextStyles.NONE, context.CurrentStyle.Styles);

        using (context.WithTextStyle(NKTextStyles.ITALIC)) {
            Assert.Equal(NKTextStyles.ITALIC, context.CurrentStyle.Styles);
        }

        Assert.Equal(NKTextStyles.NONE, context.CurrentStyle.Styles);
    }

    [Fact]
    public void PushGlobal_And_WithGlobal_ResetsOriginAndClipAndEnablesOverflow() {
        using var canvas = new NKCharCanvas(10, 10);

        var context = new RenderContext(canvas) {
            Origin        = new Point2D(5, 5),
            Clip          = new Rect2D(1, 1, 8, 8),
            AllowOverflow = false
        };

        using (context.PushGlobal()) {
            Assert.Equal(Point2D.Zero, context.Origin);
            Assert.Null(context.Clip);
            Assert.True(context.AllowOverflow);
        }

        Assert.Equal(new Point2D(5, 5),      context.Origin);
        Assert.Equal(new Rect2D(1, 1, 8, 8), context.Clip);
        Assert.False(context.AllowOverflow);

        using (context.WithGlobal()) {
            Assert.Equal(Point2D.Zero, context.Origin);
            Assert.Null(context.Clip);
            Assert.True(context.AllowOverflow);
        }
    }

    [Fact]
    public void PushElementContext_ConfiguresElementScope() {
        using var canvas  = new NKCharCanvas(50, 50);
        var       context = new RenderContext(canvas);

        var elemBounds  = new Rect2D(10, 15, 40, 45);
        var contentArea = new Rect2D(2,  3,  20, 20);

        // Without clipToContent
        using (context.PushElementContext(elemBounds, contentArea, zIndex: 4, clipToContent: false, allowOverflow: false)) {
            Assert.Equal(new Point2D(12, 18), context.Origin); // (10+2, 15+3)
            Assert.Null(context.Clip);
            Assert.Equal(4, context.ZIndex);
            Assert.False(context.AllowOverflow);
            Assert.Equal(new Rect2D(12, 18, 30, 35), context.ContextArea); // (2+10, 3+15, 20+10, 20+15)
        }

        // With clipToContent and allowOverflow
        using (context.PushElementContext(elemBounds, contentArea, zIndex: 6, clipToContent: true, allowOverflow: true)) {
            Assert.Equal(new Point2D(12, 18),        context.Origin);
            Assert.Equal(new Rect2D(12, 18, 30, 35), context.Clip);
            Assert.Equal(6,                          context.ZIndex);
            Assert.True(context.AllowOverflow);
            Assert.Equal(new Rect2D(12, 18, 30, 35), context.ContextArea);
        }
    }

    [Fact]
    public void PushScope_AllParameters_AppliesOverridesCorrectly() {
        using var canvas = new NKCharCanvas(30, 30);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(4, 4)
        };

        var offset      = new Point2D(2, 3);
        var localClip   = new Rect2D(1, 1, 10, 10);
        var style       = new NKStyle(ConsoleColor.Magenta);
        var contextArea = new Rect2D(0, 0, 15, 15);

        using (context.PushScope(
                offset: offset,
                clip: localClip,
                zIndex: 9,
                style: style,
                allowOverflow: true,
                contextArea: contextArea,
                intersectClip: false
            )) {
            Assert.Equal(new Point2D(6, 7),                 context.Origin); // 4+2, 4+3
            Assert.Equal(new Rect2D(7, 8, 16, 17),          context.Clip);   // 1+6, 1+7, 10+6, 10+7
            Assert.Equal(9,                                 context.ZIndex);
            Assert.Equal(new NKColor(ConsoleColor.Magenta), context.CurrentStyle.FColor);
            Assert.True(context.AllowOverflow);
            Assert.Equal(new Rect2D(6, 7, 21, 22), context.ContextArea); // 0+6, 0+7, 15+6, 15+7
        }

        Assert.Equal(new Point2D(4, 4), context.Origin);
        Assert.Null(context.Clip);
        Assert.Equal(0, context.ZIndex);
        Assert.False(context.AllowOverflow);
        Assert.Null(context.ContextArea);
    }

    #endregion

    #region Coordinate Transformations

    [Fact]
    public void ToCanvas_Point2D_And_Ints_TranslatesByOrigin() {
        using var canvas = new NKCharCanvas(20, 20);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(7, 11)
        };

        var point = new Point2D(3,   4);
        Assert.Equal(new Point2D(10, 15), context.ToCanvas(point));
        Assert.Equal(new Point2D(10, 15), context.ToCanvas(3, 4));
    }

    [Fact]
    public void ToLocal_Point2D_And_Ints_SubtractsOrigin() {
        using var canvas = new NKCharCanvas(20, 20);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(7, 11)
        };

        var canvasPoint = new Point2D(10, 15);
        Assert.Equal(new Point2D(3,       4), context.ToLocal(canvasPoint));
        Assert.Equal(new Point2D(3,       4), context.ToLocal(10, 15));
    }

    [Fact]
    public void ToCanvas_ToLocal_Roundtrip_PreservesPoint() {
        using var canvas = new NKCharCanvas(50, 50);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(13, -5)
        };

        var original = new Point2D(-8, 22);
        Assert.Equal(original, context.ToLocal(context.ToCanvas(original)));
        Assert.Equal(original, context.ToCanvas(context.ToLocal(original)));
    }

    [Fact]
    public void ToCanvas_Rect2D_TranslatesByOrigin() {
        using var canvas = new NKCharCanvas(30, 30);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(5, 10)
        };

        var localRect  = new Rect2D(2, 3, 12, 18);
        var canvasRect = context.ToCanvas(localRect);

        Assert.Equal(new Rect2D(7, 13, 17, 28), canvasRect);
    }

    [Fact]
    public void ToLocal_Rect2D_SubtractsOrigin() {
        using var canvas = new NKCharCanvas(30, 30);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(5, 10)
        };

        var canvasRect = new Rect2D(7, 13, 17, 28);
        var localRect  = context.ToLocal(canvasRect);

        Assert.Equal(new Rect2D(2, 3, 12, 18), localRect);
    }

    [Fact]
    public void ToCanvas_ToLocal_Rect2D_Roundtrip_PreservesRect() {
        using var canvas = new NKCharCanvas(30, 30);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(8, -4)
        };

        var original = new Rect2D(3, 5, 20, 25);
        Assert.Equal(original, context.ToLocal(context.ToCanvas(original)));
    }

    #endregion

    #region Visibility and Effective Clip

    [Fact]
    public void IsCanvasPointVisible_PhysicalCanvasBounds_ReturnsExpected() {
        using var canvas  = new NKCharCanvas(20, 10);
        var       context = new RenderContext(canvas);

        // Outside canvas
        Assert.False(context.IsCanvasPointVisible(-1, 5));
        Assert.False(context.IsCanvasPointVisible(20, 5));
        Assert.False(context.IsCanvasPointVisible(5,  -1));
        Assert.False(context.IsCanvasPointVisible(5,  10));

        // Inside canvas
        Assert.True(context.IsCanvasPointVisible(0,  0));
        Assert.True(context.IsCanvasPointVisible(19, 9));
        Assert.True(context.IsCanvasPointVisible(new Point2D(10, 5)));
    }

    [Fact]
    public void IsCanvasPointVisible_WithClip_WithoutOverflow_RespectsClip() {
        using var canvas = new NKCharCanvas(20, 10);

        var context = new RenderContext(canvas) {
            Clip = new Rect2D(5, 2, 15, 8)
        };

        // Inside clip
        Assert.True(context.IsCanvasPointVisible(5,  2)); // Lower bounds are inclusive
        Assert.True(context.IsCanvasPointVisible(14, 7)); // Within bounds
        Assert.True(context.IsCanvasPointVisible(new Point2D(10, 5)));

        // Outside clip but inside canvas
        Assert.False(context.IsCanvasPointVisible(4,  5)); // Left of clip
        Assert.False(context.IsCanvasPointVisible(15, 5)); // HigherX is exclusive
        Assert.False(context.IsCanvasPointVisible(10, 1)); // Above clip
        Assert.False(context.IsCanvasPointVisible(10, 8)); // HigherY is exclusive
    }

    [Fact]
    public void IsCanvasPointVisible_WithClip_WithAllowOverflow_IgnoresClip() {
        using var canvas = new NKCharCanvas(20, 10);

        var context = new RenderContext(canvas) {
            Clip          = new Rect2D(5, 2, 15, 8),
            AllowOverflow = true
        };

        // Points outside clip but within canvas return true
        Assert.True(context.IsCanvasPointVisible(2,  1));
        Assert.True(context.IsCanvasPointVisible(18, 9));
        Assert.True(context.IsCanvasPointVisible(new Point2D(4, 5)));

        // Points outside canvas still return false
        Assert.False(context.IsCanvasPointVisible(-1, 5));
        Assert.False(context.IsCanvasPointVisible(20, 5));
    }

    [Fact]
    public void IsPointVisible_TransformsLocalToCanvasSpace() {
        using var canvas = new NKCharCanvas(20, 20);

        var context = new RenderContext(canvas) {
            Origin = new Point2D(5, 5),
            Clip   = new Rect2D(5, 5, 15, 15)
        };

        // Local (0, 0) -> Canvas (5, 5), inside clip
        Assert.True(context.IsPointVisible(0, 0));
        Assert.True(context.IsPointVisible(new Point2D(0, 0)));

        // Local (10, 10) -> Canvas (15, 15), on exclusive higher boundary
        Assert.False(context.IsPointVisible(10, 10));

        // Local (-6, 0) -> Canvas (-1, 5), outside canvas bounds
        Assert.False(context.IsPointVisible(-6, 0));
    }

    [Fact]
    public void GetEffectiveClip_Scenarios() {
        using var canvas  = new NKCharCanvas(20, 10);
        var       context = new RenderContext(canvas);

        // 1. No clip: returns CanvasBounds
        Assert.Equal(context.CanvasBounds, context.GetEffectiveClip());

        // 2. AllowOverflow is true: returns CanvasBounds regardless of Clip
        context.Clip          = new Rect2D(2, 2, 6, 6);
        context.AllowOverflow = true;
        Assert.Equal(context.CanvasBounds, context.GetEffectiveClip());

        // 3. Clip within canvas and AllowOverflow is false: returns Clip
        context.AllowOverflow = false;
        Assert.Equal(new Rect2D(2, 2, 6, 6), context.GetEffectiveClip());

        // 4. Clip extends beyond canvas: returns intersection
        context.Clip = new Rect2D(-5, -5, 30, 20);
        Assert.Equal(new Rect2D(0, 0, 20, 10), context.GetEffectiveClip());
    }

    [Fact]
    public void IntersectAreas_CalculatesCorrectIntersection() {
        // Overlapping
        var a = new Rect2D(0,      0, 10, 10);
        var b = new Rect2D(5,      5, 15, 15);
        Assert.Equal(new Rect2D(5, 5, 10, 10), RenderContext.IntersectAreas(a, b));

        // Contained
        var c = new Rect2D(2, 2, 8, 8);
        Assert.Equal(c, RenderContext.IntersectAreas(a, c));

        // Disjoint horizontally
        var d1     = new Rect2D(0,  0, 5,  5);
        var d2     = new Rect2D(10, 0, 15, 5);
        var emptyH = RenderContext.IntersectAreas(d1, d2);
        Assert.True(emptyH.HigherX <= emptyH.LowerX || emptyH.HigherY <= emptyH.LowerY);

        // Disjoint vertically
        var v1     = new Rect2D(0, 0,  5, 5);
        var v2     = new Rect2D(0, 10, 5, 15);
        var emptyV = RenderContext.IntersectAreas(v1, v2);
        Assert.True(emptyV.HigherX <= emptyV.LowerX || emptyV.HigherY <= emptyV.LowerY);

        // Touching at boundary
        var t1         = new Rect2D(0, 0, 5,  5);
        var t2         = new Rect2D(5, 0, 10, 5);
        var emptyTouch = RenderContext.IntersectAreas(t1, t2);
        Assert.True(emptyTouch.HigherX <= emptyTouch.LowerX);
    }

    #endregion

    #region Drawing Delegation & Functional Drawing

    [Fact]
    public void DrawingMethods_DelegateToUnderlyingCanvas() {
        var spy     = new SpyCharCanvas();
        var context = new RenderContext(spy);

        // Draw(ICharCanvas, int, int, int)
        using var sourceCanvas = new NKCharCanvas(5, 5);
        context.Draw(sourceCanvas, 2, 3, 1);
        Assert.NotNull(spy.LastDrawCanvas);
        Assert.Same(sourceCanvas, spy.LastDrawCanvas.Value.Canvas);
        Assert.Equal(2, spy.LastDrawCanvas.Value.X);
        Assert.Equal(3, spy.LastDrawCanvas.Value.Y);
        Assert.Equal(1, spy.LastDrawCanvas.Value.Z);

        // Draw(AnsiChar, int, int, int)
        var ch = new AnsiChar('X', new NKStyle(ConsoleColor.Red));
        context.Draw(ch, 4, 5, 2);
        Assert.NotNull(spy.LastDrawChar);
        Assert.Equal(ch, spy.LastDrawChar.Value.C);
        Assert.Equal(4,  spy.LastDrawChar.Value.X);
        Assert.Equal(5,  spy.LastDrawChar.Value.Y);
        Assert.Equal(2,  spy.LastDrawChar.Value.Z);

        // Draw(AnsiString, int, int, int)
        var str = new AnsiString("Hello");
        context.Draw(str, 1, 2, 3);
        Assert.NotNull(spy.LastDrawString);
        Assert.Equal(str, spy.LastDrawString.Value.Str);
        Assert.Equal(1,   spy.LastDrawString.Value.X);
        Assert.Equal(2,   spy.LastDrawString.Value.Y);
        Assert.Equal(3,   spy.LastDrawString.Value.Z);

        // Draw(SKImage, int, int, int, int, int, SKSamplingOptions)
        using var surface = SKSurface.Create(new SKImageInfo(2, 2));
        using var img     = surface.Snapshot();
        context.Draw(img, 10, 11, 2, 2, 4);
        Assert.NotNull(spy.LastDrawImage);
        Assert.Same(img, spy.LastDrawImage.Value.Image);
        Assert.Equal(10, spy.LastDrawImage.Value.X);
        Assert.Equal(11, spy.LastDrawImage.Value.Y);
        Assert.Equal(2,  spy.LastDrawImage.Value.W);
        Assert.Equal(2,  spy.LastDrawImage.Value.H);
        Assert.Equal(4,  spy.LastDrawImage.Value.Z);

        // DrawVerticalLine
        context.DrawVerticalLine(ch, 3, 1, 5, 2);
        Assert.NotNull(spy.LastDrawVerticalLine);
        Assert.Equal(ch, spy.LastDrawVerticalLine.Value.C);
        Assert.Equal(3,  spy.LastDrawVerticalLine.Value.X);
        Assert.Equal(1,  spy.LastDrawVerticalLine.Value.Y);
        Assert.Equal(5,  spy.LastDrawVerticalLine.Value.Length);
        Assert.Equal(2,  spy.LastDrawVerticalLine.Value.Z);

        // DrawHorizontalLine
        context.DrawHorizontalLine(ch, 2, 4, 6, 2);
        Assert.NotNull(spy.LastDrawHorizontalLine);
        Assert.Equal(ch, spy.LastDrawHorizontalLine.Value.C);
        Assert.Equal(2,  spy.LastDrawHorizontalLine.Value.X);
        Assert.Equal(4,  spy.LastDrawHorizontalLine.Value.Y);
        Assert.Equal(6,  spy.LastDrawHorizontalLine.Value.Length);
        Assert.Equal(2,  spy.LastDrawHorizontalLine.Value.Z);

        // DrawRect
        context.DrawRect(ch, 1, 1, 4, 3, 2);
        Assert.NotNull(spy.LastDrawRect);
        Assert.Equal(ch, spy.LastDrawRect.Value.C);
        Assert.Equal(1,  spy.LastDrawRect.Value.X);
        Assert.Equal(1,  spy.LastDrawRect.Value.Y);
        Assert.Equal(4,  spy.LastDrawRect.Value.Width);
        Assert.Equal(3,  spy.LastDrawRect.Value.Height);
        Assert.Equal(2,  spy.LastDrawRect.Value.Z);

        // DrawBorder
        var border = NKBorderOld.GetAscii();
        context.DrawBorder(border, 0, 0, 10, 5, 1);
        Assert.NotNull(spy.LastDrawBorder);
        Assert.Equal(border, spy.LastDrawBorder.Value.Border);
        Assert.Equal(0,      spy.LastDrawBorder.Value.X);
        Assert.Equal(0,      spy.LastDrawBorder.Value.Y);
        Assert.Equal(10,     spy.LastDrawBorder.Value.Width);
        Assert.Equal(5,      spy.LastDrawBorder.Value.Height);
        Assert.Equal(1,      spy.LastDrawBorder.Value.Z);

        // Clean
        Assert.False(spy.CleanCalled);
        context.Clean();
        Assert.True(spy.CleanCalled);
    }

    [Fact]
    public void Drawing_WithRealCanvas_RendersAndCleansProperly() {
        using var canvas  = new NKCharCanvas(10, 10);
        var       context = new RenderContext(canvas);

        var ch = new AnsiChar('A', new NKStyle(ConsoleColor.Green));
        context.Draw(ch, 2, 3, 0);

        Assert.Equal('A', canvas.Data[2, 3].Char);

        context.Clean();
        Assert.Equal(NKCell.Default.Char, canvas.Data[2, 3].Char);
    }

    #endregion

    #region RenderState & Scoped Types

    [Fact]
    public void RenderState_ConstructorsAndEquality() {
        var defaultState = new RenderState();
        Assert.Equal(Point2D.Zero, defaultState.Origin);
        Assert.Null(defaultState.Clip);
        Assert.Equal(0,               defaultState.ZIndex);
        Assert.Equal(NKStyle.Default, defaultState.Style);
        Assert.False(defaultState.AllowOverflow);
        Assert.Null(defaultState.ContextArea);

        var customState = new RenderState(
            origin: new Point2D(3, 4),
            clip: new Rect2D(1, 1, 5, 5),
            zIndex: 10,
            style: new NKStyle(ConsoleColor.Blue),
            allowOverflow: true,
            contextArea: new Rect2D(0, 0, 10, 10)
        );

        Assert.Equal(new Point2D(3, 4),              customState.Origin);
        Assert.Equal(new Rect2D(1, 1, 5, 5),         customState.Clip);
        Assert.Equal(10,                             customState.ZIndex);
        Assert.Equal(new NKStyle(ConsoleColor.Blue), customState.Style);
        Assert.True(customState.AllowOverflow);
        Assert.Equal(new Rect2D(0, 0, 10, 10), customState.ContextArea);

        var modified = customState with { ZIndex = 20 };
        Assert.Equal(20,                 modified.ZIndex);
        Assert.Equal(customState.Origin, modified.Origin);
        Assert.NotEqual(customState, modified);
    }

    #endregion

    #region Spy / Mock Helper

    private sealed class SpyCharCanvas : ICharCanvas {
        public Size2D                  Size   { get; set; } = new(20, 10);
        public int                     Width  => Size.X;
        public int                     Height => Size.Y;
        public IReadOnlyList2D<NKCell> Data   => null!;

        public bool                                                                            CleanCalled            { get; private set; }
        public (ICharCanvas Canvas, int X, int Y, int Z)?                                      LastDrawCanvas         { get; private set; }
        public (AnsiChar C, int X, int Y, int Z)?                                              LastDrawChar           { get; private set; }
        public (AnsiString Str, int X, int Y, int Z)?                                          LastDrawString         { get; private set; }
        public (SKImage Image, int X, int Y, int W, int H, int Z, SKSamplingOptions Sampling)? LastDrawImage          { get; private set; }
        public (AnsiChar C, int X, int Y, int Length, int Z)?                                  LastDrawVerticalLine   { get; private set; }
        public (AnsiChar C, int X, int Y, int Length, int Z)?                                  LastDrawHorizontalLine { get; private set; }
        public (AnsiChar C, int X, int Y, int Width, int Height, int Z)?                       LastDrawRect           { get; private set; }
        public (NKBorderOld Border, int X, int Y, int Width, int Height, int Z)?               LastDrawBorder         { get; private set; }

        public void Resize(Size2D size) => Size = size;
        public void ResetZ()            { }
        public void Dispose()           { }

        public void Draw(ICharCanvas canvas, int xOffset, int yOffset, int z) => LastDrawCanvas = (canvas, xOffset, yOffset, z);

        public void Draw(AnsiChar c, int xOffset, int yOffset, int z) => LastDrawChar = (c, xOffset, yOffset, z);

        public void Draw(AnsiString str, int xOffset, int yOffset, int z) => LastDrawString = (str, xOffset, yOffset, z);

        public void Draw(SKImage image, int xOffset, int yOffset, int xSize, int ySize, int zLayer, SKSamplingOptions samplingOptions = default) => LastDrawImage = (image, xOffset, yOffset, xSize, ySize, zLayer, samplingOptions);

        public void DrawVerticalLine(AnsiChar c, int x, int y, int length, int z) => LastDrawVerticalLine = (c, x, y, length, z);

        public void DrawHorizontalLine(AnsiChar c, int x, int y, int length, int z) => LastDrawHorizontalLine = (c, x, y, length, z);

        public void DrawRect(AnsiChar c, int x, int y, int width, int height, int z) => LastDrawRect = (c, x, y, width, height, z);

        public void DrawBorder(NKBorderOld border, int x, int y, int width, int height, int z) => LastDrawBorder = (border, x, y, width, height, z);

        public void Clean() => CleanCalled = true;
    }

    #endregion

}