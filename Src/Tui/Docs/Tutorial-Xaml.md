# Tutorial: Designing TUIs with XAML

NeoKolors features a desktop-grade, source-generator-backed XAML layout engine. This allows you to define your TUI structure and design aesthetics in declarative XML/XAML markup, while keeping user interaction logic in C# partial classes.

---

## 1. How the XAML Engine Works

At build time, the NeoKolors incremental source generator (`XamlSourceGenerator`) searches for files ending with `.xaml` or `.nkxaml`. 

For each layout file, the generator produces a companion partial class containing:
1. **Property Accessors**: Automatically generated C# fields and property accessors for elements that have a `Name`, `Id`, or `x:Name` attribute.
2. **`InitializeComponent()` Method**: A method that instantiates the elements, configures their style properties, binds event callbacks to C# handler methods, builds the layout tree hierarchy, and maps the components.

---

## 2. Declaring a XAML View

To create a new XAML-based view, you declare a markup file (e.g., `MainView.xaml`) and a code-behind partial class file (e.g., `MainView.cs`).

### A. The XAML Markup (`MainView.xaml`)
```xml
<Page x:Class="MyDashboard.MainView" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <Grid x:Name="LayoutGrid">
        <!-- Top Navigation -->
        <StackPanel Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2"
                    BackgroundColor="dark-grey"
                    Orientation="horizontal">
            <TextBlock x:Name="Title" TextColor="red" Content="Dashboard Page" Margin="1ch 1ch 0ch 0ch" />
            <TextBlock x:Name="TimerText" Content="Time: --:--" Margin="2ch 1ch 0ch 0ch" />
        </StackPanel>

        <!-- Sidebar -->
        <StackPanel Grid.Row="1" Grid.Column="0" BackgroundColor="black">
            <Button x:Name="HomeBtn" Content="[ Home ]" OnClick="OnHomeClicked">
                <Button.Triggers>
                    <Trigger Property="IsHovered" Value="True">
                        <Trigger.Setters>
                            <Setter Property="BackgroundColor" Value="blue" />
                        </Trigger.Setters>
                    </Trigger>
                    <Trigger Property="IsHovered" Value="False">
                        <Trigger.Setters>
                            <Setter Property="BackgroundColor" Value="default" />
                        </Trigger.Setters>
                    </Trigger>
                </Button.Triggers>
            </Button>
        </StackPanel>
    </Grid>
</Page>
```

### B. The C# Code-Behind (`MainView.cs`)
```csharp
using System;
using NeoKolors.Console;
using NeoKolors.Tui.Elements;

namespace MyDashboard;

public partial class MainView 
{
    public MainView() 
    {
        // 1. Instantiates and configures the layout tree generated from XAML
        InitializeComponent();

        // 2. Configure grid dimensions dynamically in C#
        LayoutGrid.RowDefinitions.Add(GridLength.Auto);
        LayoutGrid.RowDefinitions.Add(GridLength.Star());
        LayoutGrid.ColumnDefinitions.Add(GridLength.Chars(15));
        LayoutGrid.ColumnDefinitions.Add(GridLength.Star());

        // 3. Register C# frame renders
        TimerText.OnRender += () => {
            TimerText.Content = "Time: " + DateTime.Now.ToString("HH:mm:ss");
        };
    }

    // 4. Declare event handler method mapped in XAML OnClick attribute
    public void OnHomeClicked() 
    {
        NKDebug.Info("Home button clicked!");
    }
}
```

---

## 3. XAML Syntax and Features

### 3.1 Named Elements
Adding `x:Name`, `Name`, or `Id` to any XAML element generates a public property in the partial class:
```xml
<TextBlock x:Name="Title" Content="Active Users" />
```
Generates in `MainView.g.cs`:
```csharp
public global::NeoKolors.Tui.Elements.TextBlock Title { get; private set; }
```

### 3.2 Setting Properties and Conversions
Attributes on XAML tags are parsed and passed to the property engine. Custom types are converted automatically:
- **`NKColor`**: Specified by name (`red`, `blue`, `dark-grey`, `default`) or HSL/RGB hex strings.
- **`Thickness` (Margins/Paddings)**: Evaluated as space-separated units (e.g. `Margin="1ch 2ch 1ch 2ch"` or `Padding="1"`).
- **`Dimension`**: E.g. `Width="50ch"` or `Height="star"`.

### 3.3 Event Handlers
Events are wired directly to C# class methods by matching event name attributes to method signatures in the code-behind:
```xml
<Button OnClick="OnHomeClicked" />
```
*Supported events include: `OnClick`, `OnRelease`, `OnHover`, `OnHoverOut`, `GotFocus`, `LostFocus`, `ValueChanged`, `OnRender`, and more.*

### 3.4 Grid Row and Column Attachments
Elements placed inside a `<Grid>` declare their relative coordinate positions using attached properties:
```xml
<TextBlock Grid.Row="1" Grid.Column="2" Grid.RowSpan="2" Grid.ColumnSpan="1" />
```

### 3.5 RelativePanel Sibling Alignment
Elements inside a `<RelativePanel>` can position themselves relative to sibling controls via named alignments:
```xml
<TextBlock x:Name="Label1" Content="Username:" />
<TextBox x:Name="Input1" RelativePanel.RightOf="Label1" />
<Button x:Name="SubmitBtn" RelativePanel.Below="Input1" />
```

### 3.6 Interactive Style Triggers
Dynamic styles, such as cursor hover highlights, can be defined inside elements using property triggers:
```xml
<Button Content="Submit">
    <Button.Triggers>
        <Trigger Property="IsHovered" Value="True">
            <Trigger.Setters>
                <Setter Property="BackgroundColor" Value="magenta" />
                <Setter Property="TextColor" Value="white" />
            </Trigger.Setters>
        </Trigger>
    </Button.Triggers>
</Button>
```
