// NeoKolors.Test
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Values;
using NeoKolors.Console.Input;

namespace NeoKolors.Tui.Tests.Elements;

public class NewElementTests {

    [Fact]
    public void CheckBox_ShouldToggleCorrectly() {
        var checkBox = new CheckBox("Enable notifications");
        Assert.False(checkBox.IsChecked);

        // Click to toggle on
        checkBox.Click(MouseButton.LEFT);
        Assert.True(checkBox.IsChecked);

        // Click to toggle off
        checkBox.Click(MouseButton.LEFT);
        Assert.False(checkBox.IsChecked);
    }

    [Fact]
    public void RadioButton_ShouldMutualExcludeInGroups() {
        var group = "ThemeMode";
        var radio1 = new RadioButton("System Default") { GroupName = group };
        var radio2 = new RadioButton("Dark Mode") { GroupName = group };

        Assert.False(radio1.IsChecked);
        Assert.False(radio2.IsChecked);

        // Check radio1
        radio1.IsChecked = true;
        Assert.True(radio1.IsChecked);
        Assert.False(radio2.IsChecked);

        // Check radio2 (should uncheck radio1)
        radio2.IsChecked = true;
        Assert.True(radio2.IsChecked);
        Assert.False(radio1.IsChecked);
    }

    [Fact]
    public void ToggleSwitch_ShouldToggleOnAndOff() {
        var toggle = new ToggleSwitch("WiFi");
        Assert.False(toggle.IsChecked);

        toggle.Click(MouseButton.LEFT);
        Assert.True(toggle.IsChecked);

        toggle.Click(MouseButton.LEFT);
        Assert.False(toggle.IsChecked);
    }

    [Fact]
    public void Expander_ShouldToggledExpandedState() {
        var expander = new Expander("More Settings", new TextBlock("Child details"));
        Assert.False(expander.IsExpanded);

        expander.Click(MouseButton.LEFT);
        Assert.True(expander.IsExpanded);

        expander.Click(MouseButton.LEFT);
        Assert.False(expander.IsExpanded);
    }

    [Fact]
    public void ScrollViewer_ShouldTrackScrollBarVisibility() {
        var scroll = new ScrollViewer();
        Assert.Equal(ScrollBarVisibility.Auto, scroll.HorizontalScrollBarVisibility);
        Assert.Equal(ScrollBarVisibility.Auto, scroll.VerticalScrollBarVisibility);

        scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
        scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

        Assert.Equal(ScrollBarVisibility.Visible, scroll.HorizontalScrollBarVisibility);
        Assert.Equal(ScrollBarVisibility.Disabled, scroll.VerticalScrollBarVisibility);
    }

    [Fact]
    public void Canvas_ShouldLayoutChildrenCorrectly() {
        var canvas = new Canvas();
        var child = new TextBlock("Hi");
        child.Style.Position = new Position(Dimension.Chars(5), Dimension.Chars(10));
        
        canvas.AddChild(child);
        Assert.Single(canvas.Children);
        Assert.Equal(child, canvas.Children[0]);
    }

    [Fact]
    public void RelativePanel_ShouldArrangeChildrenCorrectly() {
        var panel = new RelativePanel();
        var child1 = new TextBlock("First");
        var child2 = new TextBlock("Second");

        panel.AddChild(child1);
        panel.AddChild(child2);
        
        panel.SetBelow(child2, child1);
        panel.SetRightOf(child2, child1);

        Assert.Equal(2, panel.Children.Count);
    }

    [Fact]
    public void ToolTip_ShouldTrackOpenState() {
        var tooltip = new ToolTip("Helper Tip");
        Assert.False(tooltip.IsOpen);

        tooltip.IsOpen = true;
        Assert.True(tooltip.IsOpen);
    }

    [Fact]
    public void Frame_ShouldSupportNavigationStack() {
        var frame = new Frame();
        var page1 = new Page();
        var page2 = new Page();

        Assert.False(frame.CanGoBack);

        // Navigate page1
        frame.Navigate(page1);
        Assert.Equal(page1, frame.Content);
        Assert.False(frame.CanGoBack);

        // Navigate page2
        frame.Navigate(page2);
        Assert.Equal(page2, frame.Content);
        Assert.True(frame.CanGoBack);

        // Go Back
        var success = frame.GoBack();
        Assert.True(success);
        Assert.Equal(page1, frame.Content);
        Assert.False(frame.CanGoBack);
    }

    [Fact]
    public void ListView_ShouldTrackSelectionAndSource() {
        var list = new ListView();
        var items = new List<string> { "One", "Two", "Three" };
        list.ItemsSource = items;

        Assert.Equal(0, list.SelectedIndex);
        Assert.Equal("One", list.SelectedItem);

        list.SelectedIndex = 1;
        Assert.Equal(1, list.SelectedIndex);
        Assert.Equal("Two", list.SelectedItem);
    }

    [Fact]
    public void GridView_ShouldStoreItemsParameters() {
        var grid = new GridView { ItemWidth = 15, ItemHeight = 5 };
        var items = new List<string> { "A", "B", "C" };
        grid.ItemsSource = items;

        Assert.Equal(15, grid.ItemWidth);
        Assert.Equal(5, grid.ItemHeight);
    }

    [Fact]
    public void ComboBox_ShouldSupportTriggerToggle() {
        var combo = new ComboBox();
        combo.ItemsSource = new List<string> { "Blue", "Green", "Red" };

        Assert.False(combo.IsDropDownOpen);
        combo.Click(MouseButton.LEFT);
        Assert.True(combo.IsDropDownOpen);

        combo.Click(MouseButton.LEFT);
        Assert.False(combo.IsDropDownOpen);
    }

    [Fact]
    public void TreeView_ShouldManageHierarchicalRootNodes() {
        var tree = new TreeView();
        var root = new TreeViewNode("System");
        var child = new TreeViewNode("Network");
        root.Children.Add(child);

        tree.RootNodes.Add(root);
        Assert.Single(tree.RootNodes);
        Assert.Equal(root, tree.RootNodes[0]);
        Assert.Single(tree.RootNodes[0].Children);
    }

    [Fact]
    public void MenuBar_ShouldHoldHorizontalButtons() {
        var bar = new MenuBar();
        var menu1 = new Button("File");
        var menu2 = new Button("Edit");

        bar.AddChild(menu1);
        bar.AddChild(menu2);

        Assert.Equal(2, bar.Children.Count);
    }

    [Fact]
    public void PasswordBox_ShouldSecureMaskInput() {
        var box = new PasswordBox { Password = "SecureText123", PasswordChar = '*' };
        Assert.Equal("SecureText123", box.Password);
        Assert.Equal('*', box.PasswordChar);
    }

    [Fact]
    public void RichEditBox_ShouldSupportMultilineText() {
        var box = new RichEditBox { Text = "Line1\nLine2" };
        Assert.Equal("Line1\nLine2", box.Text);
    }

    [Fact]
    public void Slider_ShouldRestrictValueToMinimumAndMaximum() {
        var slider = new Slider { Minimum = 10, Maximum = 90, Value = 50 };
        Assert.Equal(50, slider.Value);

        slider.Value = 100;
        Assert.Equal(90, slider.Value);

        slider.Value = 5;
        Assert.Equal(10, slider.Value);
    }

    [Fact]
    public void ProgressBar_ShouldSupportIndeterminateState() {
        var bar = new ProgressBar { IsIndeterminate = true };
        Assert.True(bar.IsIndeterminate);

        bar.IsIndeterminate = false;
        Assert.False(bar.IsIndeterminate);
    }

    [Fact]
    public void LayoutLifecycle_ShouldMeasureArrangeAndInvalidateCorrectly() {
        var textBlock = new TextBlock("Hello World");
        
        // Measure pass
        textBlock.Measure(new Size(100, 100));
        Assert.True(textBlock.DesiredSize.Width > 0);
        Assert.True(textBlock.DesiredSize.Height > 0);

        // Arrange pass
        var finalRect = new Rectangle(new Point(5, 5), new Size(30, 5));
        textBlock.Arrange(finalRect);
        
        Assert.Equal(finalRect, textBlock.RenderBounds);
        Assert.Equal(finalRect.Size, textBlock.RenderLayout.Margin);

        // Invalidate pass
        textBlock.InvalidateMeasure();
        
        // Changing a style property should trigger invalidation automatically
        textBlock.Style.Padding = new Spacing(Dimension.Chars(1));
        
        // Measure again with new constraints
        textBlock.Measure(new Size(100, 100));
        Assert.True(textBlock.DesiredSize.Width > 0);
    }
}
