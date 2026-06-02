// NeoKolors.Test
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Console.Events;
using NeoKolors.Console.Input;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Tests.Elements;

public class MouseCascadeTests {
    private class DummyApplication : IApplication {
        public IRenderable Base { get; set; } = null!;
        public void Start() { }
        public void Stop() { }
        public event KeyEventHandler KeyEvent = delegate { };
        public event ResizeEventHandler ResizeEvent = delegate { };
        public event AppStartEventHandler StartEvent = delegate { };
        public event AppStopEventHandler StopEvent = delegate { };
    }

    [Fact]
    public void HitTest_ShouldFindCorrectElementAndRespectVisibility() {
        // Arrange
        var root = new StackPanel();
        root.Arrange(new Rectangle(0, 0, 10, 10));

        var button = new Button { Content = "Click Me" };
        button.Arrange(new Rectangle(1, 1, 5, 3));

        root.SetChildNode([button]);

        // Act & Assert
        // Coordinate (2, 2) is inside the button
        var hit1 = MouseCascadeController.HitTest(root, 2, 2);
        Assert.Equal(button.GetChildNode(), hit1);

        // Coordinate (8, 8) is outside the button but inside root
        var hit2 = MouseCascadeController.HitTest(root, 8, 8);
        Assert.Equal(root, hit2);

        // Coordinate (15, 15) is outside both
        var hit3 = MouseCascadeController.HitTest(root, 15, 15);
        Assert.Null(hit3);

        // Make button invisible
        button.Style.Set(new VisibleProperty(false));
        var hit4 = MouseCascadeController.HitTest(root, 2, 2);
        Assert.Equal(root, hit4); // button is invisible, so it should hit root
    }

    [Fact]
    public void FindPath_ShouldBuildCorrectAncestorChain() {
        // Arrange
        var root = new StackPanel();
        var panel = new StackPanel();
        var button = new Button();

        root.SetChildNode([panel]);
        panel.SetChildNode([button]);

        // Act
        var path = new List<IElement>();
        bool found = MouseCascadeController.FindPath(root, button, path);

        // Assert
        Assert.True(found);
        Assert.Equal(3, path.Count);
        Assert.Equal(root, path[0]);
        Assert.Equal(panel, path[1]);
        Assert.Equal(button, path[2]);
    }

    [Fact]
    public void MouseCascade_ShouldDispatchClickAndHoverEvents() {
        // Arrange
        var root = new StackPanel();
        root.Arrange(new Rectangle(0, 0, 10, 10));

        var textBlock = new TextBlock("Hello");
        textBlock.Arrange(new Rectangle(1, 1, 5, 2));

        root.SetChildNode([textBlock]);

        var app = new DummyApplication { Base = root };
        var controller = new MouseCascadeController(app);

        bool textBlockClicked = false;
        MouseButton? clickedButton = null;
        textBlock.OnClick += btn => {
            textBlockClicked = true;
            clickedButton = btn;
        };

        bool textBlockHovered = false;
        textBlock.OnHover += () => textBlockHovered = true;

        bool textBlockHoverOut = false;
        textBlock.OnHoverOut += () => textBlockHoverOut = true;

        // Act - Simulate Hover
        var hoverEventArgs = new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(2, 2), false, true);
        controller.HandleMouseEvent(hoverEventArgs);
        Assert.True(textBlockHovered);
        Assert.False(textBlockHoverOut);

        // Act - Simulate Click
        var pressEventArgs = new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(2, 2), false, false);
        controller.HandleMouseEvent(pressEventArgs);
        Assert.True(textBlockClicked);
        Assert.Equal(MouseButton.LEFT, clickedButton);

        // Act - Simulate Hover Out
        var hoverOutEventArgs = new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(8, 8), false, true);
        controller.HandleMouseEvent(hoverOutEventArgs);
        Assert.True(textBlockHoverOut);
    }
}
