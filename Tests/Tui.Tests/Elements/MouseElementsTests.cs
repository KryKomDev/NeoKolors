// NeoKolors.Test
// Copyright (c) 2026 KryKom

using NeoKolors.Console;
using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Tests.Elements;

using Element = IElement;
using ElementButton = Button;

public class MouseElementsTests {

    [Fact]
    public void HitMap_HitTest_OpaqueOcclusion_StopsAtTopmost() {
        var hitMap = new HitMap();

        var lowerTarget = new HitTarget {
            Element = "Lower",
            GlobalBounds = new Rect2D(0, 0, 10, 10),
            ZIndex = 0,
            PaintOrder = hitMap.NextPaintOrder(), // 1
            Behavior = HitTestBehavior.OPAQUE
        };
        var upperTarget = new HitTarget {
            Element = "Upper",
            GlobalBounds = new Rect2D(0, 0, 10, 10),
            ZIndex = 0,
            PaintOrder = hitMap.NextPaintOrder(), // 2
            Behavior = HitTestBehavior.OPAQUE
        };

        hitMap.Register(lowerTarget);
        hitMap.Register(upperTarget);

        var hits = hitMap.HitTest(new Point2D(5, 5));

        Assert.Single(hits);
        Assert.Same(upperTarget, hits[0]);
    }

    [Fact]
    public void HitMap_HitTest_Translucent_AllowsBubblingToUnderlyingTargets() {
        var hitMap = new HitMap();

        var lowerTarget = new HitTarget {
            Element = "Lower",
            GlobalBounds = new Rect2D(0, 0, 10, 10),
            ZIndex = 0,
            PaintOrder = hitMap.NextPaintOrder(), // 1
            Behavior = HitTestBehavior.OPAQUE
        };
        var upperTarget = new HitTarget {
            Element = "Upper",
            GlobalBounds = new Rect2D(0, 0, 10, 10),
            ZIndex = 0,
            PaintOrder = hitMap.NextPaintOrder(), // 2
            Behavior = HitTestBehavior.TRANSLUCENT
        };

        hitMap.Register(lowerTarget);
        hitMap.Register(upperTarget);

        var hits = hitMap.HitTest(new Point2D(5, 5));

        Assert.Equal(2, hits.Count);
        Assert.Same(upperTarget, hits[0]); // Topmost first
        Assert.Same(lowerTarget, hits[1]); // Lower behind it
    }

    [Fact]
    public void HitMap_HitTest_ZIndex_TakesPrecedenceOverPaintOrder() {
        var hitMap = new HitMap();

        // Rendered first (PaintOrder = 1), but with higher ZIndex (10)
        var dialogTarget = new HitTarget {
            Element = "Dialog",
            GlobalBounds = new Rect2D(0, 0, 10, 10),
            ZIndex = 10,
            PaintOrder = hitMap.NextPaintOrder(),
            Behavior = HitTestBehavior.OPAQUE
        };

        // Rendered later (PaintOrder = 2), but with lower ZIndex (0)
        var backgroundTarget = new HitTarget {
            Element = "Background",
            GlobalBounds = new Rect2D(0, 0, 10, 10),
            ZIndex = 0,
            PaintOrder = hitMap.NextPaintOrder(),
            Behavior = HitTestBehavior.OPAQUE
        };

        hitMap.Register(dialogTarget);
        hitMap.Register(backgroundTarget);

        var hits = hitMap.HitTest(new Point2D(5, 5));

        Assert.Single(hits);
        Assert.Same(dialogTarget, hits[0]);
    }

    [Fact]
    public void HitMap_HitTest_BehaviorNone_IsIgnored() {
        var hitMap = new HitMap();

        var bottomTarget = new HitTarget {
            Element = "Bottom",
            GlobalBounds = new Rect2D(0, 0, 10, 10),
            ZIndex = 0,
            PaintOrder = hitMap.NextPaintOrder(),
            Behavior = HitTestBehavior.OPAQUE
        };
        var overlayNone = new HitTarget {
            Element = "OverlayNone",
            GlobalBounds = new Rect2D(0, 0, 10, 10),
            ZIndex = 0,
            PaintOrder = hitMap.NextPaintOrder(),
            Behavior = HitTestBehavior.NONE
        };

        hitMap.Register(bottomTarget);
        hitMap.Register(overlayNone);

        var hits = hitMap.HitTest(new Point2D(5, 5));

        Assert.Single(hits);
        Assert.Same(bottomTarget, hits[0]);
    }

    [Fact]
    public void Dispatcher_FullClick_TriggersOnClick_WithCorrectPositions() {
        var hitMap = new HitMap();
        using var canvas = new NKCharCanvas(20, 20);
        var context = new RenderContext(canvas, hitMap);

        ElementMouseEventArgs? receivedClick = null;
        var region = new MouseRegion(
            child: new Text(new AnsiString("Hello")),
            onClick: e => receivedClick = e
        );

        // Render region at origin (5, 5) with size (10, 2)
        using (context.PushOrigin(new Point2D(5, 5))) {
            region.Render(context, new Size2D(10, 2));
        }

        var dispatcher = new ElementsMouseDispatcher();

        // 1. Mouse down at (7, 6) -> local pos should be (7 - 5, 6 - 5) = (2, 1)
        var down = new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(7, 6), released: false, moved: false);
        dispatcher.Dispatch(hitMap, down);

        Assert.Null(receivedClick);
        Assert.NotNull(dispatcher.PressedTarget);

        // 2. Mouse up at (7, 6) -> triggers click
        var up = new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(7, 6), released: true, moved: false);
        dispatcher.Dispatch(hitMap, up);

        Assert.NotNull(receivedClick);
        Assert.True(receivedClick.IsClick);
        Assert.Equal(MouseButton.LEFT, receivedClick.Button);
        Assert.Equal(new Point2D(7, 6), receivedClick.GlobalPosition);
        Assert.Equal(new Point2D(2, 1), receivedClick.LocalPosition);
        Assert.Same(region, receivedClick.Target);
    }

    [Fact]
    public void Dispatcher_PressAndReleaseOutside_DoesNotTriggerClick() {
        var hitMap = new HitMap();
        using var canvas = new NKCharCanvas(20, 20);
        var context = new RenderContext(canvas, hitMap);

        bool clicked = false;
        var region = new MouseRegion(
            child: new Text(new AnsiString("Hello")),
            onClick: _ => clicked = true
        );

        region.Render(context, new Size2D(10, 5));

        var dispatcher = new ElementsMouseDispatcher();

        // Down inside at (3, 3)
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(3, 3), released: false, moved: false));
        Assert.True(region.IsPressed.AsT);

        // Up outside at (15, 15)
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(15, 15), released: true, moved: false));

        Assert.False(clicked);
        Assert.False(region.IsPressed.AsT);
        Assert.Null(dispatcher.PressedTarget);
    }

    [Fact]
    public void Dispatcher_HoverTransitions_TogglesIsHoveredAndCallsCallbacks() {
        var hitMap = new HitMap();
        using var canvas = new NKCharCanvas(20, 20);
        var context = new RenderContext(canvas, hitMap);

        bool entered = false;
        bool left = false;
        int moveCount = 0;

        var region = new MouseRegion(
            child: new Text(new AnsiString("Hello")),
            onMouseEnter: _ => entered = true,
            onMouseLeave: _ => left = true,
            onMouseMove: _ => moveCount++
        );

        region.Render(context, new Size2D(10, 5));

        var dispatcher = new ElementsMouseDispatcher();

        // 1. Move outside (15, 15)
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(15, 15), released: false, moved: true));
        Assert.False(entered);
        Assert.False(region.IsHovered.AsT);

        // 2. Move inside (3, 3)
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(3, 3), released: false, moved: true));
        Assert.True(entered);
        Assert.True(region.IsHovered.AsT);
        Assert.Equal(1, moveCount);

        // 3. Move inside again (4, 4)
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(4, 4), released: false, moved: true));
        Assert.Equal(2, moveCount);
        Assert.False(left);

        // 4. Move outside (18, 18)
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(18, 18), released: false, moved: true));
        Assert.True(left);
        Assert.False(region.IsHovered.AsT);
    }

    [Fact]
    public void Dispatcher_WheelScroll_TriggersOnScroll() {
        var hitMap = new HitMap();
        using var canvas = new NKCharCanvas(20, 20);
        var context = new RenderContext(canvas, hitMap);

        ElementMouseEventArgs? receivedScroll = null;
        var region = new MouseRegion(
            child: new Text(new AnsiString("Hello")),
            onScroll: e => receivedScroll = e
        );

        region.Render(context, new Size2D(10, 5));

        var dispatcher = new ElementsMouseDispatcher();

        // Scroll Up
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.WHEEL_UP, KeyModifiers.NONE, new Point2D(2, 2), released: false, moved: false));
        Assert.NotNull(receivedScroll);
        Assert.True(receivedScroll.IsWheelUp);
        Assert.False(receivedScroll.IsWheelDown);

        // Scroll Down
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.WHEEL_DOWN, KeyModifiers.NONE, new Point2D(2, 2), released: false, moved: false));
        Assert.NotNull(receivedScroll);
        Assert.False(receivedScroll.IsWheelUp);
        Assert.True(receivedScroll.IsWheelDown);
    }

    [Fact]
    public void Dispatcher_Handled_StopsBubbling() {
        var hitMap = new HitMap();
        using var canvas = new NKCharCanvas(20, 20);
        var context = new RenderContext(canvas, hitMap);

        bool childClicked = false;
        bool parentClicked = false;

        var child = new MouseRegion(
            child: new Text(new AnsiString("Child")),
            behavior: HitTestBehavior.TRANSLUCENT,
            onClick: e => {
                childClicked = true;
                e.Handled = true; // Stop bubbling
            }
        );

        var parent = new MouseRegion(
            child: child,
            behavior: HitTestBehavior.TRANSLUCENT,
            onClick: _ => parentClicked = true
        );

        parent.Render(context, new Size2D(10, 5));

        var dispatcher = new ElementsMouseDispatcher();

        // Press and release at (2, 2)
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(2, 2), released: false, moved: false));
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(2, 2), released: true, moved: false));

        Assert.True(childClicked);
        Assert.False(parentClicked); // Stopped due to e.Handled = true
    }

    [Fact]
    public void Button_ClickAndHover_UpdatesVisualStatesAndFiresClick() {
        var hitMap = new HitMap();
        using var canvas = new NKCharCanvas(30, 10);
        var context = new RenderContext(canvas, hitMap);

        bool clicked = false;
        var button = ElementButton.FromText(
            "Submit",
            onClick: _ => clicked = true
        );

        button.Render(context, new Size2D(15, 3));

        Assert.False(button.IsHovered.AsT);
        Assert.False(button.IsPressed.AsT);

        var dispatcher = new ElementsMouseDispatcher();

        // 1. Move over button (5, 1)
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(5, 1), released: false, moved: true));
        Assert.True(button.IsHovered.AsT);
        Assert.False(button.IsPressed.AsT);

        // 2. Press button
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(5, 1), released: false, moved: false));
        Assert.True(button.IsPressed.AsT);

        // 3. Release button
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(5, 1), released: true, moved: false));
        Assert.False(button.IsPressed.AsT);
        Assert.True(clicked);
    }

    [Fact]
    public void ElementMouseExtensions_FluentHelpers_WrapCorrectly() {
        bool clicked = false;
        bool hovered = false;
        bool scrolled = false;

        Element element = new Text(new AnsiString("Test"))
            .OnClick(_ => clicked = true)
            .OnHover(_ => hovered = true)
            .OnScroll(_ => scrolled = true)
            .WithHitBehavior(HitTestBehavior.TRANSLUCENT);

        Assert.IsType<MouseRegion>(element);
        var region = (MouseRegion)element;
        Assert.Equal(HitTestBehavior.TRANSLUCENT, region.Behavior.AsT);
        Assert.NotNull(region.OnClick.AsT);
        Assert.NotNull(region.OnMouseEnter.AsT);
        Assert.NotNull(region.OnScroll.AsT);

        var hitMap = new HitMap();
        using var canvas = new NKCharCanvas(20, 20);
        var context = new RenderContext(canvas, hitMap);
        region.Render(context, new Size2D(10, 5));

        var dispatcher = new ElementsMouseDispatcher();
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(2, 2), released: false, moved: true));
        Assert.True(hovered);

        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.WHEEL_UP, KeyModifiers.NONE, new Point2D(2, 2), released: false, moved: false));
        Assert.True(scrolled);

        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(2, 2), released: false, moved: false));
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(2, 2), released: true, moved: false));
        Assert.True(clicked);
    }

    [Fact]
    public void Dispatcher_ClickAcrossReRenderPasses_TriggersOnClick() {
        var hitMap = new HitMap();
        using var canvas = new NKCharCanvas(20, 20);
        var context = new RenderContext(canvas, hitMap);

        bool clicked = false;
        var region = new MouseRegion(
            child: new Text(new AnsiString("Click Me")),
            onClick: _ => clicked = true
        );

        region.Render(context, new Size2D(10, 5));

        var dispatcher = new ElementsMouseDispatcher();

        // 1. Mouse down at (3, 3)
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(3, 3), released: false, moved: false));
        Assert.NotNull(dispatcher.PressedTarget);

        // 2. Re-render pass occurs between down and up (clearing hit map and registering new target at same location)
        hitMap.Clear();
        region.Render(context, new Size2D(10, 5));
        hitMap.Commit();

        // 3. Mouse up at (3, 3) - simulates Windows console event where Button is RELEASE
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(3, 3), released: true, moved: false));

        Assert.True(clicked, "Click should trigger even if UI was re-rendered between mouse-down and mouse-up!");
        Assert.Null(dispatcher.PressedTarget);
    }

    [Fact]
    public void Dispatcher_ClickWithWindowsReleaseButton_PreservesPressedButtonInEventArgs() {
        var hitMap = new HitMap();
        using var canvas = new NKCharCanvas(20, 20);
        var context = new RenderContext(canvas, hitMap);

        ElementMouseEventArgs? receivedClick = null;
        var region = new MouseRegion(
            child: new Text(new AnsiString("Click Me")),
            onClick: e => receivedClick = e
        );

        region.Render(context, new Size2D(10, 5));

        var dispatcher = new ElementsMouseDispatcher();

        // Down with LEFT button
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(3, 3), released: false, moved: false));

        // Up with Windows console RELEASE button code
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(3, 3), released: true, moved: false));

        Assert.NotNull(receivedClick);
        Assert.Equal(MouseButton.LEFT, receivedClick.Button);
    }

    [Fact]
    public void Dispatcher_HoverAcrossReRenderPasses_MaintainsHoverStateWithoutDuplicateEnter() {
        var hitMap = new HitMap();
        using var canvas = new NKCharCanvas(20, 20);
        var context = new RenderContext(canvas, hitMap);

        int enterCount = 0;
        int leaveCount = 0;
        var region = new MouseRegion(
            child: new Text(new AnsiString("Hover Me")),
            onMouseEnter: _ => enterCount++,
            onMouseLeave: _ => leaveCount++
        );

        region.Render(context, new Size2D(10, 5));

        var dispatcher = new ElementsMouseDispatcher();

        // 1. Hover into target
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(3, 3), released: false, moved: true));
        Assert.Equal(1, enterCount);
        Assert.Equal(0, leaveCount);

        // 2. Re-render pass occurs
        hitMap.Clear();
        region.Render(context, new Size2D(10, 5));
        hitMap.Commit();

        // 3. Mouse moves slightly within same target
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(4, 3), released: false, moved: true));
        Assert.Equal(1, enterCount); // Should NOT re-trigger enter
        Assert.Equal(0, leaveCount); // Should NOT trigger leave

        // 4. Mouse moves completely outside target
        dispatcher.Dispatch(hitMap, new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(15, 15), released: false, moved: true));
        Assert.Equal(1, enterCount);
        Assert.Equal(1, leaveCount);
    }
}
