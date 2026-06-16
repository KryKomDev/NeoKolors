// NeoKolors.Test
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Values;
using SkiaSharp;
using NeoKolors.Console.Input;
using NeoKolors.Tui.Events;
using NeoKolors.Console;

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
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void TextBox_ShouldRenderWithoutException() {
        var element = new TextBox { Text = "Editable Text" };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void PasswordBox_ShouldRenderWithoutException() {
        var element = new PasswordBox { Password = "Password123" };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void TextBox_ShouldSizeToHeightThree_WithBorder() {
        var element = new TextBox { Text = "Test" };
        element.Style.Border = BorderStyle.GetNormal();
        element.Measure(new Size2D(20, 10));
        element.Arrange(new Area2D(0, 0, 20, 3));
        Assert.Equal(3, element.DesiredSize.Y);
        Assert.Equal(1, element.RenderLayout.Content.SizeY);
    }

    [Fact]
    public void PasswordBox_ShouldSizeToHeightThree_WithBorder() {
        var element = new PasswordBox { Password = "Test" };
        element.Style.Border = BorderStyle.GetNormal();
        element.Measure(new Size2D(20, 10));
        element.Arrange(new Area2D(0, 0, 20, 3));
        Assert.Equal(3, element.DesiredSize.Y);
        Assert.Equal(1, element.RenderLayout.Content.SizeY);
    }

    [Fact]
    public void TextBox_ShouldSizeToHeightOne_WithoutBorder() {
        var element = new TextBox { Text = "Test" };
        element.Style.Border = BorderStyle.Borderless;
        element.Measure(new Size2D(20, 10));
        element.Arrange(new Area2D(0, 0, 20, 1));
        Assert.Equal(1, element.DesiredSize.Y);
        Assert.Equal(1, element.RenderLayout.Content.SizeY);
    }

    [Fact]
    public void PasswordBox_ShouldSizeToHeightOne_WithoutBorder() {
        var element = new PasswordBox { Password = "Test" };
        element.Style.Border = BorderStyle.Borderless;
        element.Measure(new Size2D(20, 10));
        element.Arrange(new Area2D(0, 0, 20, 1));
        Assert.Equal(1, element.DesiredSize.Y);
        Assert.Equal(1, element.RenderLayout.Content.SizeY);
    }

    [Fact]
    public void RichEditBox_ShouldRenderWithoutException() {
        var element = new RichEditBox { Text = "Line 1\nLine 2" };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void Button_ShouldRenderWithoutException() {
        var element = new Button("Click Me");
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void Button_WithBorder_PlacementTest() {
        var button = new Button("Click me") {
            Style = new StyleCollection {
                Border = BorderStyle.GetNormal(),
            }
        };
        var canvas = CreateCanvas(20, 5);
        button.Measure(new Size2D(20, 5));
        button.Arrange(new Area2D(0, 0, 10, 3));
        button.Render(canvas);
        
        // Assert the size
        Assert.Equal(10, button.DesiredSize.X); // 8 text + 2 border
        Assert.Equal(3, button.DesiredSize.Y); // 1 text + 2 border
        
        // Assert content rectangle
        Assert.Equal(new Area2D(1, 1, 9, 2), button.RenderLayout.Content);
        
        // Assert that the text is written at the correct coordinates on the canvas
        // (column 1 to 8 on row 1 should contain "Click me")
        var row1 = "";
        for (int x = 1; x <= 8; x++) {
            row1 += canvas[x, 1].Char;
        }
        Assert.Equal("Click me", row1);
    }

    [Fact]
    public void Button_WithBorder_TextBlockContent_PlacementTest() {
        var textBlock = new TextBlock("Click me");
        var button = new Button(textBlock) {
            Style = new StyleCollection {
                Border = BorderStyle.GetNormal(),
            }
        };
        var canvas = CreateCanvas(20, 5);
        button.Measure(new Size2D(20, 5));
        button.Arrange(new Area2D(0, 0, 10, 3));
        button.Render(canvas);
        
        // Assert the size
        Assert.Equal(10, button.DesiredSize.X); // 8 text + 2 border
        Assert.Equal(3, button.DesiredSize.Y); // 1 text + 2 border
        
        // Assert child element's render bounds
        Assert.Equal(new Area2D(1, 1, 9, 2), textBlock.RenderBounds);
        
        // Assert that the text is written at the correct coordinates on the canvas
        var row1 = "";
        for (int x = 1; x <= 8; x++) {
            row1 += canvas[x, 1].Char;
        }
        Assert.Equal("Click me", row1);
    }

    [Fact]
    public void Button_WithBorder_FixedSize_PlacementTest() {
        var button = new Button("Click me") {
            Style = new StyleCollection {
                Border = BorderStyle.GetNormal(),
                Width = Dimension.Chars(12),
                Height = Dimension.Chars(3)
            }
        };
        var canvas = CreateCanvas(20, 5);
        button.Measure(new Size2D(20, 5));
        button.Arrange(new Area2D(0, 0, 12, 3)); // 12x3 in exclusive coords
        button.Render(canvas);
        
        // Assert the desired size is exactly what was specified in Style (12x3)
        Assert.Equal(12, button.DesiredSize.X);
        Assert.Equal(3, button.DesiredSize.Y);
        
        // Assert content rectangle has correct size (12 - 2 borders = 10 width, 3 - 2 borders = 1 height)
        Assert.Equal(new Area2D(1, 1, 11, 2), button.RenderLayout.Content);
        
        // Assert that the text is written at the correct coordinates on the canvas
        // "Click me" is 8 chars. Content box is 10 chars.
        // It should be centered in the content box. (10 - 8) / 2 = 1 char padding.
        // Content box starts at x = 1, so text starts at x = 2.
        var row1 = "";
        for (int x = 2; x <= 9; x++) {
            row1 += canvas[x, 1].Char;
        }
        Assert.Equal("Click me", row1);
    }

    [Fact]
    public void Button_WithBorder_MinMaxConstraints_PlacementTest() {
        var button = new Button("Click me") {
            Style = new StyleCollection {
                Border = BorderStyle.GetNormal(),
                MinWidth = Dimension.Chars(12),
                MaxWidth = Dimension.Chars(15),
                MinHeight = Dimension.Chars(3),
                MaxHeight = Dimension.Chars(4)
            }
        };
        var canvas = CreateCanvas(20, 10);
        button.Measure(new Size2D(20, 10));
        
        // Desired size should be 12x3 (since content 8+2=10 is boosted to MinWidth=12, and 1+2=3 matches MinHeight=3)
        Assert.Equal(12, button.DesiredSize.X);
        Assert.Equal(3, button.DesiredSize.Y);
        
        // Arrange to larger size (20x10)
        button.Arrange(new Area2D(0, 0, 20, 10));
        
        // RenderLayout.Border should be clamped to MaxWidth=15 and MaxHeight=4
        Assert.Equal(15, button.RenderLayout.Border.SizeX);
        Assert.Equal(4, button.RenderLayout.Border.SizeY);
    }

    [Fact]
    public void Programmatic_MainView_LayoutTest() {
        var grid = new Grid();
        grid.RowDefinitions.Add(GridLength.Auto);
        grid.RowDefinitions.Add(GridLength.Star());
        grid.ColumnDefinitions.Add(GridLength.Chars(15));
        grid.ColumnDefinitions.Add(GridLength.Star());

        var title = new TextBlock("NeoKolors TUI Dashboard");
        grid.AddChild(title, 0, 0, 1, 2);

        var sidebar = new StackPanel(Orientation.VERTICAL);
        grid.AddChild(sidebar, 1, 0);

        var buttonHome = new Button("Home") {
            Style = new StyleCollection {
                Border = BorderStyle.GetNormal()
            }
        };
        sidebar.AddChild(buttonHome);

        var textBlockGellow = new TextBlock("Gellow") {
            Style = new StyleCollection {
                Border = BorderStyle.GetNormal()
            }
        };
        sidebar.AddChild(textBlockGellow);

        var buttonSettings = new Button("Settings");
        sidebar.AddChild(buttonSettings);

        var contentArea = new ScrollViewer {
            Content = new TextBlock("Dashboard content goes here...")
        };
        grid.AddChild(contentArea, 1, 1);

        var canvas = CreateCanvas(80, 25);
        grid.Measure(new Size2D(80, 25));
        grid.Arrange(new Area2D(0, 0, 80, 25));
        grid.Render(canvas);

        Assert.Equal(new Area2D(0, 0, 80, 1), title.RenderBounds);
        Assert.Equal(new Area2D(0, 1, 15, 25), sidebar.RenderBounds);
        Assert.Equal(new Area2D(0, 1, 6, 4), buttonHome.RenderBounds);
    }

    [Fact]
    public void Xaml_MainView_LayoutTest() {
        var xamlPath = Path.Combine(AppContext.BaseDirectory, "../../../../../Examples/DashBoard/App.xaml");
        var xaml = File.ReadAllText(xamlPath);
        xaml = xaml.Replace("GridColumnSpan.=\"2\"", "Grid.ColumnSpan=\"2\"")
                   .Replace("x:name", "x:Name");
        
        var loader = new NeoKolors.Tui.Dom.XamlElementLoader(typeof(Button).Assembly);
        var dom = loader.Load(xaml);
        var page = dom.BaseElement as Page;
        Assert.NotNull(page);

        // Find elements and apply MainView code-behind customizations
        var childrenList = page.GetChildNode() as List<IElement>;
        Assert.NotNull(childrenList);
        var grid = childrenList[0] as Grid;
        Assert.NotNull(grid);

        grid.RowDefinitions.Add(GridLength.Auto);
        grid.RowDefinitions.Add(GridLength.Star());
        grid.ColumnDefinitions.Add(GridLength.Chars(15));
        grid.ColumnDefinitions.Add(GridLength.Star());

        var sidebar = grid.Children.OfType<StackPanel>().FirstOrDefault(s => s.Info.Id == "Sidebar");
        Assert.NotNull(sidebar);
        var buttonHome = sidebar.Children.OfType<Button>().FirstOrDefault(b => b.Info.Id == "B");
        Assert.NotNull(buttonHome);
        var textBlockGellow = sidebar.Children.OfType<TextBlock>().FirstOrDefault(t => t.Info.Id == "T");
        Assert.NotNull(textBlockGellow);

        buttonHome.Style.Border = BorderStyle.GetNormal();
        textBlockGellow.Style.Border = BorderStyle.GetNormal();

        var canvas = CreateCanvas(80, 25);
        page.Measure(new Size2D(80, 25));
        page.Arrange(new Area2D(0, 0, 80, 25));
        page.Render(canvas);

        Assert.Equal(new Area2D(0, 0, 80, 1), grid.Children[0].RenderBounds);
        Assert.Equal(new Area2D(0, 1, 15, 25), sidebar.RenderBounds);
        Assert.Equal(new Area2D(0, 1, 6, 4), buttonHome.RenderBounds);
        Assert.Equal(new Area2D(0, 4, 8, 7), textBlockGellow.RenderBounds);
    }

    [Fact]
    public void CheckBox_ShouldRenderWithoutException() {
        var element = new CheckBox("Option 1");
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void RadioButton_ShouldRenderWithoutException() {
        var element = new RadioButton("Option A");
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void ToggleSwitch_ShouldRenderWithoutException() {
        var element = new ToggleSwitch("Toggle state");
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void Slider_ShouldRenderWithoutException() {
        var element = new Slider { Minimum = 0, Maximum = 10, Value = 5 };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void Slider_WithCustomUnit_ShouldRenderCorrectLabel() {
        var slider = new Slider { Minimum = 0, Maximum = 100, Value = 50, Unit = "px" };
        slider.Measure(new Size2D(20, 1));
        slider.Arrange(new Area2D(0, 0, 20, 1));
        var canvas = CreateCanvas(20, 1);
        slider.Render(canvas);

        string result = "";
        for (int x = 0; x < 20; x++) {
            result += canvas[x, 0].Char ?? ' ';
        }

        Assert.Contains("50px", result);
    }

    [Fact]
    public void Slider_MouseInteraction_ShouldUpdateValue() {
        var app = new MockApplication();
        AppEventBus.SetSourceApplication(app);

        var slider = new Slider { Minimum = 0, Maximum = 100, Value = 0, Unit = "%" };
        slider.Measure(new Size2D(20, 1));
        slider.Arrange(new Area2D(0, 0, 20, 1));

        // Click the slider to select/activate it
        slider.Click(MouseButton.LEFT);

        // Start dragging by clicking near the middle (X = 10)
        app.TriggerMouseEvent(new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(10, 0), released: false, moved: false));

        Assert.True(slider.IsSelected);
        Assert.True(slider.Value > 0);
        
        // Drag to another position (X = 5)
        app.TriggerMouseEvent(new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(5, 0), released: false, moved: true));
        
        // Verify value has changed accordingly
        Assert.True(slider.Value > 0);

        // Release dragging
        app.TriggerMouseEvent(new MouseEventArgs(MouseButton.RELEASE, KeyModifiers.NONE, new Point2D(5, 0), released: true, moved: false));
    }

    [Fact]
    public void ProgressBar_ShouldRenderWithoutException() {
        var element = new ProgressBar { Minimum = 0, Maximum = 100, Value = 45 };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void ScrollViewer_ShouldRenderWithoutException() {
        var element = new ScrollViewer { Content = new TextBlock("Scroll content") };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void ListView_ShouldRenderWithoutException() {
        var element = new ListView { ItemsSource = new List<string> { "Apple", "Banana", "Cherry" } };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void GridView_ShouldRenderWithoutException() {
        var element = new GridView { ItemsSource = new List<string> { "One", "Two", "Three" } };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void ComboBox_ShouldRenderWithoutException() {
        var element = new ComboBox { ItemsSource = new List<string> { "Blue", "Red", "Yellow" } };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void TreeView_ShouldRenderWithoutException() {
        var element = new TreeView();
        var node = new TreeViewNode("System");
        node.Children.Add(new TreeViewNode("Disk"));
        element.RootNodes.Add(node);
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void MenuBar_ShouldRenderWithoutException() {
        var element = new MenuBar();
        element.AddChild(new Button("File"));
        element.AddChild(new Button("Edit"));
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
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
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void Expander_ShouldRenderWithoutException() {
        var element = new Expander("Title", new TextBlock("Content"));
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void ToolTip_ShouldRenderWithoutException() {
        var element = new ToolTip("Tip of the day") { IsOpen = true };
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
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
        Assert.True(element.DesiredSize.X > 0);
    }

    [Fact]
    public void SixelImage_ShouldRenderWithoutException() {
        var info = new SKImageInfo(100, 100);
        using var surface = SKSurface.Create(info);
        using var image = surface.Snapshot();
        var element = new SixelImage(image);
        var canvas = CreateCanvas();
        element.Render(canvas);
        Assert.True(element.DesiredSize.X > 0);
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

    [Fact]
    public void OverlappingElements_WithSolidBackground_ShouldOverwriteCharactersBeneath() {
        var canvas = new NKCharCanvas(10, 5);

        var rootCanvas = new Canvas();
        rootCanvas.Style.Width = Dimension.Chars(10);
        rootCanvas.Style.Height = Dimension.Chars(5);

        // First child: bottom text "XXXXX" at y = 1
        var bottomText = new TextBlock("XXXXX");
        bottomText.Style.Position = new Position(Dimension.Chars(0), Dimension.Chars(1));
        rootCanvas.AddChild(bottomText);

        // Second child: top element at (1, 1), size 3x1, with non-Inherit background
        var topBlock = new Canvas {
            Style = new StyleCollection {
                Position = new Position(Dimension.Chars(1), Dimension.Chars(1)),
                Width = Dimension.Chars(3),
                Height = Dimension.Chars(1),
                BackgroundColor = NKColor.Default // non-inherit background
            }
        };
        rootCanvas.AddChild(topBlock);

        rootCanvas.Render(canvas);

        // Since the top block has width 3 and is at x = 1, it covers x = 1, 2, 3.
        // The background color is not Inherit, so it should overwrite the 'X' characters under its content box with ' '.
        Assert.Equal('X', canvas[0, 1].Char); // x = 0 is not covered, should still be 'X'
        Assert.Equal(' ', canvas[1, 1].Char); // x = 1 is covered, should be overwritten with ' '
        Assert.Equal(' ', canvas[2, 1].Char); // x = 2 is covered, should be overwritten with ' '
        Assert.Equal(' ', canvas[3, 1].Char); // x = 3 is covered, should be overwritten with ' '
        Assert.Equal('X', canvas[4, 1].Char); // x = 4 is not covered, should still be 'X'
    }

    [Fact]
    public void OverlappingElements_WithInheritBackground_ShouldNotOverwriteCharactersBeneath() {
        var canvas = new NKCharCanvas(10, 5);

        var rootCanvas = new Canvas();
        rootCanvas.Style.Width = Dimension.Chars(10);
        rootCanvas.Style.Height = Dimension.Chars(5);

        // First child: bottom text "XXXXX" at y = 1
        var bottomText = new TextBlock("XXXXX");
        bottomText.Style.Position = new Position(Dimension.Chars(0), Dimension.Chars(1));
        rootCanvas.AddChild(bottomText);

        // Second child: top element at (1, 1), size 3x1, with Inherit background
        var topBlock = new Canvas {
            Style = new StyleCollection {
                Position = new Position(Dimension.Chars(1), Dimension.Chars(1)),
                Width = Dimension.Chars(3),
                Height = Dimension.Chars(1),
                BackgroundColor = NKColor.Inherit // inherit background
            }
        };
        rootCanvas.AddChild(topBlock);

        rootCanvas.Render(canvas);

        // Since the top block has Inherit background, it should NOT overwrite the characters.
        Assert.Equal('X', canvas[0, 1].Char);
        Assert.Equal('X', canvas[1, 1].Char);
        Assert.Equal('X', canvas[2, 1].Char);
        Assert.Equal('X', canvas[3, 1].Char);
        Assert.Equal('X', canvas[4, 1].Char);
    }

    [Fact]
    public void Place_MenuDropdownOverlay_ShouldPreserveChessBoardBackground() {
        var canvas = new NKCharCanvas(10, 10);

        var topLevelGrid = new Grid();
        topLevelGrid.Style.Width = Dimension.Chars(10);
        topLevelGrid.Style.Height = Dimension.Chars(10);
        topLevelGrid.RowDefinitions.Add(GridLength.Chars(1)); // Row 0: Menu Bar
        topLevelGrid.RowDefinitions.Add(GridLength.Chars(2)); // Row 1: Toolbar
        topLevelGrid.RowDefinitions.Add(GridLength.Star(1));   // Row 2: Body (Board)

        // Add a board button in row 2
        var boardBtn = new Button(" ");
        boardBtn.Style.Set(new Styles.Properties.BackgroundColorProperty(NKColor.FromRgb(255, 0, 0))); // Red board square
        topLevelGrid.AddChild(boardBtn, 2, 0, 1, 1);

        // Add the dropdown menu overlay in row 1, rowSpan 2
        var overlay = new Grid();
        var dismissPanel = new Button("");
        dismissPanel.Style.Set(new Styles.Properties.BackgroundColorProperty(NKColor.Inherit));
        dismissPanel.Style.Set(new Styles.Properties.BorderProperty(BorderStyle.GetBorderless()));
        overlay.AddChild(dismissPanel, 0, 0, 3, 1);

        topLevelGrid.AddChild(overlay, 1, 0, 2, 1);

        // Render the tree
        topLevelGrid.Render(canvas);

        // Check if the board button cell's background is still Red (not default/black)
        var cell = canvas[0, 3];
        Assert.Equal(NKColor.FromRgb(255, 0, 0), cell.Style.GetBColor());
    }

    [Fact]
    public void Render_ElementWithTextColor_ShouldStyleAllCellsForeground() {
        var canvas = new NKCharCanvas(5, 5);

        var btn = new Button("");
        btn.Style.Width = Dimension.Chars(5);
        btn.Style.Height = Dimension.Chars(5);
        btn.Style.Set(new Styles.Properties.TextColorProperty(NKColor.FromRgb(0, 0, 255))); // Blue text
        btn.Style.Set(new Styles.Properties.BackgroundColorProperty(NKColor.FromRgb(255, 0, 0))); // Red bg

        btn.Render(canvas);

        // Check if all cells have blue text color
        for (int x = 0; x < 5; x++) {
            for (int y = 0; y < 5; y++) {
                Assert.Equal(NKColor.FromRgb(0, 0, 255), canvas[x, y].Style.GetFColor());
            }
        }
    }

    [Fact]
    public void TextBox_ShouldScrollContent_WhenCursorMovesOut() {
        var textBox = new TextBox { Text = "1234567890" }; // Length 10
        textBox.Style.Width = Dimension.Chars(5);
        textBox.Measure(new Size2D(5, 1));
        textBox.Arrange(new Area2D(0, 0, 5, 1));

        var app = new MockApplication();
        AppEventBus.SetSourceApplication(app);

        textBox.Select();

        // Move cursor to the end (END key)
        app.TriggerKeyEvent(new KeyEventArgs(KeyCode.END, KeyModifiers.NONE, '\0', down: true));

        // Render and inspect the canvas.
        var canvas = CreateCanvas(5, 1);
        textBox.Render(canvas);

        string result = "";
        for (int x = 0; x < 5; x++) {
            result += canvas[x, 0].Char ?? ' ';
        }

        // maxScroll = 10 - 5 + 1 = 6.
        // With cursor at 10, scrollOffset = 6.
        // Visible characters should be from index 6 to 9: "7890" plus a space for the cursor
        Assert.Equal("7890 ", result);

        // Move cursor to the beginning (HOME key)
        app.TriggerKeyEvent(new KeyEventArgs(KeyCode.HOME, KeyModifiers.NONE, '\0', down: true));

        canvas = CreateCanvas(5, 1);
        textBox.Render(canvas);

        result = "";
        for (int x = 0; x < 5; x++) {
            result += canvas[x, 0].Char ?? ' ';
        }

        // With cursor at 0: scrollOffset = 0.
        // Visible characters: "12345"
        Assert.Equal("12345", result);
    }

    [Fact]
    public void PasswordBox_ShouldScrollContent_WhenCursorMovesOut() {
        var passwordBox = new PasswordBox { Password = "1234567890", PasswordChar = '*' }; // Length 10
        passwordBox.Style.Width = Dimension.Chars(5);
        passwordBox.Measure(new Size2D(5, 1));
        passwordBox.Arrange(new Area2D(0, 0, 5, 1));

        var app = new MockApplication();
        AppEventBus.SetSourceApplication(app);

        passwordBox.Select();

        // Move cursor to the end (END key)
        app.TriggerKeyEvent(new KeyEventArgs(KeyCode.END, KeyModifiers.NONE, '\0', down: true));

        // Render and inspect.
        var canvas = CreateCanvas(5, 1);
        passwordBox.Render(canvas);

        string result = "";
        for (int x = 0; x < 5; x++) {
            result += canvas[x, 0].Char ?? ' ';
        }

        // maxScroll = 10 - 5 + 1 = 6.
        // With cursor at 10, scrollOffset = 6.
        // Visible characters should be 4 password chars plus 1 empty cell for the cursor: "**** "
        Assert.Equal("**** ", result);

        // Move cursor to the beginning (HOME key)
        app.TriggerKeyEvent(new KeyEventArgs(KeyCode.HOME, KeyModifiers.NONE, '\0', down: true));

        canvas = CreateCanvas(5, 1);
        passwordBox.Render(canvas);

        result = "";
        for (int x = 0; x < 5; x++) {
            result += canvas[x, 0].Char ?? ' ';
        }

        // With cursor at 0: scrollOffset = 0.
        // Visible characters: 5 password chars: "*****"
        Assert.Equal("*****", result);
    }
}

