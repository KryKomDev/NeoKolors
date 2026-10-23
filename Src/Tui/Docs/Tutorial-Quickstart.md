# Tutorial: TUI Quick-Start Guide

This tutorial walks you through building your first interactive Text User Interface (TUI) application using **NeoKolors.Tui**. We will create a simple dashboard showing system statistics and an interactive button to trigger logging, all designed in C#.

---

## 1. Prerequisites & Installation

To follow this tutorial, you need a .NET project targeting .NET 6.0 or higher.

Add the required NeoKolors packages to your project using the .NET CLI:

```bash
dotnet add package NeoKolors.Common
dotnet add package NeoKolors.Console
dotnet add package NeoKolors.Tui
```

---

## 2. Bootstrapping the TUI Application

To start a TUI application, we configure the rendering frame rate, keyboard controls, and active screen buffers, and then pass them to `NKApplication`.

Create a file named `Program.cs` and add the following bootstrapping code:

```csharp
using System;
using System.Text;
using NeoKolors.Console;
using NeoKolors.Console.Ansi;
using NeoKolors.Tui;
using NeoKolors.Tui.Elements;

namespace QuickstartApp;

public static class Program 
{
    static void Main(string[] args) 
    {
        // 1. Ensure UTF-8 output encoding for proper border characters
        Console.OutputEncoding = Encoding.UTF8;

        // 2. Setup low-level debugging logs to help track TUI events
        NKDebug.Logger.Level = LoggerLevel.CRITICAL | LoggerLevel.ERROR | LoggerLevel.WARNING | LoggerLevel.INFORMATION;
        NKDebug.Logger.FileConfig = LogFileConfig.Replace("./app.log");

        // 3. Create the configuration for our app
        var config = new NKAppConfig(
            mouseReportProtocol: MouseReportProtocol.SGR,
            mouseReportLevel: MouseReportLevel.ALL,
            rendering: RenderingConfig.Limited(60), // Limit rendering to 60 FPS
            ctrlCForceQuits: false, // Override Ctrl+C so we can exit gracefully
            interruptCombination: new KeyEventArgs(KeyCode.Q, KeyModifiers.LEFT_CTRL, 'q') // Ctrl+Q exits the app
        );

        // 4. Instantiate our root page (defined in the next section)
        var mainPage = new MainPage();

        // 5. Initialize and run the application
        var app = new NKApplication(config, mainPage);
        app.Start();
    }
}
```

---

## 3. Designing the Layout (MainPage.cs)

Our interface will use a `Grid` layout containing a header stack panel and a main content area. Let's create `MainPage.cs` to set up this layout and add interactive elements.

Create a file named `MainPage.cs`:

```csharp
using System;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Styles.Properties;

namespace QuickstartApp;

public class MainPage : Page 
{
    private readonly TextBlock _timeText;
    private readonly Button _actionButton;
    private readonly TextBox _inputBox;

    public MainPage() 
    {
        // 1. Setup the Root Layout Grid
        var grid = new Grid();
        
        // Define rows: Top Navigation (Auto-height), and Main Area (Star/Stretch height)
        grid.RowDefinitions.Add(GridLength.Auto);
        grid.RowDefinitions.Add(GridLength.Star());

        // Define columns: Sidebar (15 characters), Main Panel (Star/Stretch width)
        grid.ColumnDefinitions.Add(GridLength.Chars(15));
        grid.ColumnDefinitions.Add(GridLength.Star());

        // Set grid background color
        grid.Style.Set(BackgroundColorProperty.Instance, NKColor.FromRgb(30, 30, 30));

        // 2. Top Navigation Bar (Span across both columns)
        var topNav = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        topNav.Style.Set(BackgroundColorProperty.Instance, NKColor.FromRgb(50, 50, 50));
        
        var title = new TextBlock
        {
            Content = "NeoKolors Dashboard",
            Margin = new Thickness(1, 1, 0, 0)
        };
        title.Style.Set(TextColorProperty.Instance, NKColor.FromRgb(255, 100, 100));

        _timeText = new TextBlock
        {
            Content = "Time: -",
            Margin = new Thickness(2, 1, 0, 0)
        };

        topNav.AddChild(title);
        topNav.AddChild(_timeText);

        // Bind update event to render loop for the live clock
        _timeText.OnRender += UpdateClock;

        // Add topNav to row 0, spanning columns 0 to 1
        grid.AddChild(topNav, row: 0, column: 0, rowSpan: 1, columnSpan: 2);

        // 3. Sidebar (Column 0, Row 1)
        var sidebar = new StackPanel();
        sidebar.Style.Set(BackgroundColorProperty.Instance, NKColor.FromRgb(40, 40, 40));

        _actionButton = new Button
        {
            Content = "[ Click Me ]",
            Margin = new Thickness(1, 1, 1, 1)
        };
        _actionButton.OnClick += HandleButtonClick;

        sidebar.AddChild(_actionButton);
        grid.AddChild(sidebar, row: 1, column: 0);

        // 4. Main Body Panel (Column 1, Row 1)
        var mainPanel = new StackPanel();
        mainPanel.Style.Set(MarginProperty.Instance, new Thickness(2, 1, 2, 1));

        var welcomeText = new TextBlock
        {
            Content = "Welcome to NeoKolors! Type below to see input mapping:"
        };

        _inputBox = new TextBox
        {
            Placeholder = "Type here...",
            Border = BorderStyle.Normal,
            Margin = new Thickness(0, 1, 0, 0)
        };

        mainPanel.AddChild(welcomeText);
        mainPanel.AddChild(_inputBox);
        grid.AddChild(mainPanel, row: 1, column: 1);

        // Set the Grid as the content of our page
        Content = grid;
    }

    private void UpdateClock() 
    {
        _timeText.Content = "Time: " + DateTime.Now.ToString("HH:mm:ss");
    }

    private void HandleButtonClick() 
    {
        NKDebug.Info("Action button was clicked!");
        _inputBox.Content = "Button Clicked!";
    }
}
```

---

## 4. Key Takeaways

1. **Terminal Configurations**: Always set output encoding to UTF-8 using `Console.OutputEncoding = Encoding.UTF8` to correctly support borders, symbols, and custom glyphs.
2. **Layout Sizing**: Use `GridLength.Auto` to size cells based on child nodes, `GridLength.Chars(N)` to enforce a fixed block width/height, and `GridLength.Star()` to share remaining empty space.
3. **OnRender Event Hooks**: Subscribe to `OnRender` on elements (like `_timeText` above) to run frame-based logic (e.g., updating timestamps or progress bars) before drawing to the terminal.
4. **Mouse support**: Set `mouseReportLevel` and `mouseReportProtocol` in `NKAppConfig` to enable pointer hover state computations and mouse clicking triggers.
