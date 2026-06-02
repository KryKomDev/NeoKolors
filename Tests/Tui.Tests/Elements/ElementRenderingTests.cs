// NeoKolors.Test
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Values;
using SkiaSharp;

namespace NeoKolors.Tui.Tests.Elements;

public class ElementRenderingTests {
    private NKCharCanvas CreateCanvas(int width = 80, int height = 25) {
        return new NKCharCanvas(width, height);
    }

    [Fact]
    public void TextBlock_ShouldRenderWithoutException() {
        var element = new TextBlock("Hello World");
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void TextBox_ShouldRenderWithoutException() {
        var element = new TextBox { Text = "Editable Text" };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void PasswordBox_ShouldRenderWithoutException() {
        var element = new PasswordBox { Password = "Password123" };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void RichEditBox_ShouldRenderWithoutException() {
        var element = new RichEditBox { Text = "Line 1\nLine 2" };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void Button_ShouldRenderWithoutException() {
        var element = new Button("Click Me");
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void CheckBox_ShouldRenderWithoutException() {
        var element = new CheckBox("Option 1");
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void RadioButton_ShouldRenderWithoutException() {
        var element = new RadioButton("Option A");
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void ToggleSwitch_ShouldRenderWithoutException() {
        var element = new ToggleSwitch("Toggle state");
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void Slider_ShouldRenderWithoutException() {
        var element = new Slider { Minimum = 0, Maximum = 10, Value = 5 };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void ProgressBar_ShouldRenderWithoutException() {
        var element = new ProgressBar { Minimum = 0, Maximum = 100, Value = 45 };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void ScrollViewer_ShouldRenderWithoutException() {
        var element = new ScrollViewer { Content = new TextBlock("Scroll content") };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void ListView_ShouldRenderWithoutException() {
        var element = new ListView { ItemsSource = new List<string> { "Apple", "Banana", "Cherry" } };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void GridView_ShouldRenderWithoutException() {
        var element = new GridView { ItemsSource = new List<string> { "One", "Two", "Three" } };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void ComboBox_ShouldRenderWithoutException() {
        var element = new ComboBox { ItemsSource = new List<string> { "Blue", "Red", "Yellow" } };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void TreeView_ShouldRenderWithoutException() {
        var element = new TreeView();
        var node = new TreeViewNode("System");
        node.Children.Add(new TreeViewNode("Disk"));
        element.RootNodes.Add(node);
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void MenuBar_ShouldRenderWithoutException() {
        var element = new MenuBar();
        element.AddChild(new Button("File"));
        element.AddChild(new Button("Edit"));
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void Canvas_ShouldRenderWithoutException() {
        var element = new Canvas();
        var child = new TextBlock("On Canvas");
        child.Style.Position = new Position(Dimension.Chars(2), Dimension.Chars(3));
        element.AddChild(child);
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.Single(element.Children);
    }

    [Fact]
    public void Grid_ShouldRenderWithoutException() {
        var element = new Grid();
        element.AddChild(new TextBlock("Col 1"), 0, 0);
        element.AddChild(new TextBlock("Col 2"), 0, 1);
        var canvas = CreateCanvas();
        element.Render(canvas);
    }

    [Fact]
    public void StackPanel_ShouldRenderWithoutException() {
        var element = new StackPanel();
        element.AddChild(new TextBlock("Item 1"));
        element.AddChild(new TextBlock("Item 2"));
        var canvas = CreateCanvas();
        element.Render(canvas);
    }

    [Fact]
    public void RelativePanel_ShouldRenderWithoutException() {
        var element = new RelativePanel();
        var block1 = new TextBlock("Reference");
        var block2 = new TextBlock("Relative");
        element.AddChild(block1);
        element.AddChild(block2);
        element.SetBelow(block2, block1);
        var canvas = CreateCanvas();
        element.Render(canvas);
    }

    [Fact]
    public void GroupBox_ShouldRenderWithoutException() {
        var element = new GroupBox("Group Box", new TextBlock("Child"));
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void Expander_ShouldRenderWithoutException() {
        var element = new Expander("Title", new TextBlock("Content"));
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void ToolTip_ShouldRenderWithoutException() {
        var element = new ToolTip("Tip of the day") { IsOpen = true };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void Page_ShouldRenderWithoutException() {
        var element = new Page();
        element.AddChild(new TextBlock("Page content"));
        var canvas = CreateCanvas();
        element.Render(canvas);
    }

    [Fact]
    public void Frame_ShouldRenderWithoutException() {
        var element = new Frame();
        var page = new Page();
        page.AddChild(new TextBlock("Framed content"));
        element.Navigate(page);
        var canvas = CreateCanvas();
        element.Render(canvas);
    }

    [Fact]
    public void AsciiImage_ShouldRenderWithoutException() {
        var element = new AsciiImage("  ______  \n /      \\ \n|  o  o  |\n \\______/ ");
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void SixelImage_ShouldRenderWithoutException() {
        var info = new SKImageInfo(100, 100);
        using var surface = SKSurface.Create(info);
        using var image = surface.Snapshot();
        var element = new SixelImage(image);
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.Width > 0);
    }

    [Fact]
    public void TextBlocks_In_StackPanel_ShouldRenderOnDifferentLines() {
        var panel = new StackPanel(Orientation.VERTICAL);
        var text1 = new TextBlock("Hello");
        var text2 = new TextBlock("World");
        panel.AddChild(text1);
        panel.AddChild(text2);

        var canvas = new NKCharCanvas(80, 25);
        panel.Render(canvas);

        // Extract lines from canvas
        var lines = new List<string>();
        for (int y = 0; y < 10; y++) {
            var line = "";
            for (int x = 0; x < 10; x++) {
                var c = canvas[x, y].Char;
                line += c.HasValue ? c.Value : ' ';
            }
            lines.Add(line.Trim());
        }

        // Verify that "Hello" and "World" render on separate lines
        Assert.Contains("Hello", lines[0]);
        Assert.Contains("World", lines[1]);
    }
}
