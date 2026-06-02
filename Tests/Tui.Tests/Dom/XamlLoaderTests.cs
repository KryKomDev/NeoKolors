// NeoKolors.Test
// Copyright (c) 2026 KryKom

using NeoKolors.Common;
using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Tests.Dom;

public class XamlLoaderTests {

    [Fact]
    public void Load_SimpleButton_CreatesButtonWithIdAndContent() {
        const string xaml = 
            """
            <Button xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" x:Name="MyButton" Content="Click Me" />
            """;

        var loader = new XamlElementLoader();
        var dom = loader.Load(xaml);

        Assert.NotNull(dom.BaseElement);
        Assert.IsType<Button>(dom.BaseElement);

        var button = (Button)dom.BaseElement;
        Assert.Equal("MyButton", button.Info.Id);
        Assert.Equal("Click Me", button.Content);
    }

    [Fact]
    public void Load_NameAndIdMapping_TranslatesCorrectly() {
        const string xaml = 
            """
            <Button Name="ButtonWithName" />
            """;

        var loader = new XamlElementLoader();
        var dom = loader.Load(xaml);

        var button = (Button)dom.BaseElement;
        Assert.Equal("ButtonWithName", button.Info.Id);
    }

    [Fact]
    public void Load_PropertyElementSyntax_SetsContentObject() {
        const string xaml = 
            """
            <Button xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" x:Name="MyButton">
                <Button.Content>
                    <TextBlock Content="Hello World" />
                </Button.Content>
            </Button>
            """;

        var loader = new XamlElementLoader();
        var dom = loader.Load(xaml);

        var button = (Button)dom.BaseElement;
        Assert.IsType<TextBlock>(button.Content);

        var textBlock = (TextBlock)button.Content;
        Assert.Equal("Hello World", textBlock.Content.Plain);
    }

    [Fact]
    public void Load_StandardChildUnderContentControl_SetsContent() {
        const string xaml = 
            """
            <Expander Header="Drawer">
                <TextBlock Content="Expander Content" />
            </Expander>
            """;

        var loader = new XamlElementLoader();
        var dom = loader.Load(xaml);

        var expander = (Expander)dom.BaseElement;
        Assert.Equal("Drawer", expander.Header);
        Assert.IsType<TextBlock>(expander.Content);

        var text = (TextBlock)expander.Content;
        Assert.Equal("Expander Content", text.Content.Plain);
    }

    [Fact]
    public void Load_GridAttachedProperties_RowAndColumnResolved() {
        const string xaml = 
            """
            <Grid xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <TextBlock x:Name="Cell" Grid.Row="2" Grid.Column="1" Grid.RowSpan="2" Grid.ColumnSpan="3" Content="In Cell" />
            </Grid>
            """;

        var loader = new XamlElementLoader();
        var dom = loader.Load(xaml);

        Assert.IsType<Grid>(dom.BaseElement);
        var grid = (Grid)dom.BaseElement;

        Assert.Single(grid.Children);
        var child = grid.Children[0];
        Assert.Equal("Cell", child.Info.Id);

        // Fetch GridPosition using reflection on grid's internal positions
        var positionsField = typeof(Grid).GetField("_positions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(positionsField);

        var positions = (System.Collections.IDictionary)positionsField.GetValue(grid)!;
        Assert.True(positions.Contains(child));

        var gridPosition = positions[child]!;
        var rowProp = gridPosition.GetType().GetProperty("Row")!;
        var colProp = gridPosition.GetType().GetProperty("Column")!;
        var rowSpanProp = gridPosition.GetType().GetProperty("RowSpan")!;
        var colSpanProp = gridPosition.GetType().GetProperty("ColumnSpan")!;

        Assert.Equal(2, rowProp.GetValue(gridPosition));
        Assert.Equal(1, colProp.GetValue(gridPosition));
        Assert.Equal(2, rowSpanProp.GetValue(gridPosition));
        Assert.Equal(3, colSpanProp.GetValue(gridPosition));
    }

    [Fact]
    public void Load_RelativePanelAttachedProperties_RelationshipsResolved() {
        const string xaml = 
            """
            <RelativePanel xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <TextBlock x:Name="Anchor" Content="Anchor" />
                <TextBlock x:Name="Target" RelativePanel.Below="Anchor" RelativePanel.RightOf="Anchor" Content="Target" />
            </RelativePanel>
            """;

        var loader = new XamlElementLoader();
        var dom = loader.Load(xaml);

        var panel = (RelativePanel)dom.BaseElement;
        Assert.Equal(2, panel.Children.Count);

        var anchor = panel.Children.First(c => c.Info.Id == "Anchor");
        var target = panel.Children.First(c => c.Info.Id == "Target");

        // Verify below relationship via reflection on panel's private dictionaries
        var belowField = typeof(RelativePanel).GetField("_below", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var rightOfField = typeof(RelativePanel).GetField("_rightOf", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.NotNull(belowField);
        Assert.NotNull(rightOfField);

        var below = (System.Collections.IDictionary)belowField.GetValue(panel)!;
        var rightOf = (System.Collections.IDictionary)rightOfField.GetValue(panel)!;

        Assert.True(below.Contains(target));
        Assert.Equal(anchor, below[target]);

        Assert.True(rightOf.Contains(target));
        Assert.Equal(anchor, rightOf[target]);
    }

    [Fact]
    public void Load_StyleProperties_AppliedCorrectly() {
        const string xaml = 
            """
            <StackPanel Width="120px" Height="50px" BackgroundColor="Red" />
            """;

        var loader = new XamlElementLoader();
        var dom = loader.Load(xaml);

        var div = (StackPanel)dom.BaseElement;
        Assert.Equal(Dimension.Pixels(120), div.Style.GetWidth());
        Assert.Equal(Dimension.Pixels(50), div.Style.GetHeight());
        Assert.Equal(new NKColor(ConsoleColor.Red), div.Style.GetBackgroundColor());
    }

    [Fact]
    public void Load_CaseInsensitiveAttributes_RecognizedCorrectly() {
        const string xaml = 
            """
            <StackPanel width="80px" backgroundcolor="Blue" />
            """;

        var loader = new XamlElementLoader();
        var dom = loader.Load(xaml);

        var div = (StackPanel)dom.BaseElement;
        Assert.Equal(Dimension.Pixels(80), div.Style.GetWidth());
        Assert.Equal(new NKColor(ConsoleColor.Blue), div.Style.GetBackgroundColor());
    }

    [Fact]
    public void SourceGenerator_CompilesAndWiresNamedElements() {
        var view = new MyTestView();
        
        Assert.NotNull(view);
        Assert.Equal(2, view.Children.Count);
        
        Assert.NotNull(view.SubmitButton);
        Assert.Equal("SubmitButton", view.SubmitButton.Info.Id);
        Assert.Equal("Submit", view.SubmitButton.Content);

        Assert.NotNull(view.StatusLabel);
        Assert.Equal("StatusLabel", view.StatusLabel.Info.Id);
        Assert.Equal("Ready", view.StatusLabel.Content.Plain);
    }
}
