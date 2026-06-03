# NeoKolors.Tui

[![Build Status](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/build.yml?style=for-the-badge&labelColor=%236c7086&color=%2389b4fa&label=build)](https://github.com/KryKomDev/NeoKolors/actions/workflows/build.yml) [![Test Status](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/build.yml?style=for-the-badge&labelColor=%236c7086&color=%23f9e2af&label=tests)](https://github.com/KryKomDev/NeoKolors/actions/workflows/build.yml) [![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui?style=for-the-badge&labelColor=%236c7086&color=%23f5c2e7)](https://www.nuget.org/packages/NeoKolors.Tui) [![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui?style=for-the-badge&labelColor=%236c7086&color=%23a6e3a1)](https://www.nuget.org/packages/NeoKolors.Tui) [![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%236c7086&color=%23fab387)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)

`NeoKolors.Tui` is a modern, web-inspired, and desktop-grade Text User Interface (TUI) framework for C#. Modelled with influence from UWP/WinUI, it features a Document Object Model (DOM), visual tree layout, CSS-like styling, reactive events, mouse support, and a high-performance rendering system for terminal screens.

---

## Core Features

- **XAML Layout Declarations**: Define complex console screens cleanly using XML-based XAML layouts, parsed and compiled via the `Portable.Xaml` engine.
- **Incremental Source Generation**: A custom build-time [XamlSourceGenerator](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Generators/XamlSourceGenerator.cs) automatically matches `.xaml`/`.nkxaml` layout files to C# code-behind classes, generating typed element bindings and the visual initializer logic (`InitializeComponent`).
- **Comprehensive Control Suite**: Includes container grids, panels, interactive input controls, scrollable areas, and support for terminal media:
  - **Layout & Panels**: [Grid](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/Grid.cs), [StackPanel](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/StackPanel.cs), `RelativePanel`, `ScrollViewer`, `Canvas`, `Panel`, `Page`.
  - **Inputs & Action Elements**: `Button`, `TextBox`, `PasswordBox`, `CheckBox`, `RadioButton`, `ComboBox`, `Slider`, `ToggleSwitch`.
  - **Output & Media**: `TextBlock`, `ProgressBar`, `ToolTip`, `AsciiImage`, `SixelImage` (renders high-quality image graphics using terminal Sixel protocols).
- **CSS-Inspired Styling Engine**: Elements support style collections, triggers, and precise positioning (margins, padding, viewport sizing like `vh`/`vw`, or characters like `ch`).
- **Interactive Event Pipeline**: Full mouse support (clicks, hovering, scrolling, dragging) with event bubbling/cascades, and a standardized keyboard shortcut and resize notification bus.
- **Optimized Rendering Loops**: Supports multiple rendering execution patterns:
  - *Lazy Mode*: Draws only when keyboard/mouse inputs occur to preserve CPU resources.
  - *Limited Mode*: Locks application redraws to a specified target frame rate (e.g. 60 or 144 FPS).
  - *Unlimited Mode*: Executes redrawing continuously at maximum possible speeds, tracking throughput performance.

---

## Core Architecture

The framework revolves around three main parts:
1. **[IApplication](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/IApplication.cs) & [NKApplication](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/NKApplication.cs)**: Drives the lifecycle of the TUI application, controlling the main render loops, intercepting hardware console inputs (via the NeoKolors Driver), and translating them into UI events.
2. **[IDom](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Dom/IDom.cs) & [NKDom](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Dom/NKDom.cs)**: Stores the visual tree layout representation, and provides element queries (getting components by ID, Class, or Type) to dynamically update the view hierarchy.
3. **[IElement](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/IElement.cs) & [AbstractElement](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/AbstractElement.cs)**: The structural base classes representing visual components, managing dimensions, styling configurations, focus status, visibility, and drawing behavior.

---

## Getting Started

Building a NeoKolors TUI application follows a familiar XAML & Code-Behind pattern:

### 1. Declare the View (`MainView.xaml`)
Create an XML file representing your visual structure. Specify the class name using the `x:Class` attribute:

```xml
<Page x:Class="MyAwesomeApp.MainView" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StackPanel Id="MainContainer" Padding="2ch" Alignment="Center">
        <TextBlock 
            Id="Header" 
            Content="Welcome to NeoKolors!" 
            TextColor="cyan" 
            TextStyle="Bold, Underline"/>
        
        <Button Id="StartBtn" Content="Click Me!" MarginTop="2ch"/>
    </StackPanel>
</Page>
```

### 2. Create the Code-Behind (`MainView.xaml.cs`)
Implement the partial C# class matching the XAML file declaration:

```csharp
using NeoKolors.Tui.Elements;

namespace MyAwesomeApp;

public partial class MainView {
    public MainView() {
        // Automatically generated by the XamlSourceGenerator 
        // to load the layout and bind elements (e.g. Header, StartBtn).
        InitializeComponent();
        
        // Bind events programmatically
        StartBtn.OnClick += () => {
            Header.Content = "Application Started!";
            Header.TextColor = "green";
        };
    }
}
```

### 3. Initialize and Start (`Program.cs`)
Configure the console settings, initialize your visual DOM, and start the application life-cycle:

```csharp
using System.Text;
using NeoKolors.Tui;
using NeoKolors.Tui.Dom;

namespace MyAwesomeApp;

internal static class Program {
    public static void Main(string[] args) {
        // Ensure UTF-8 output encoding is configured for unicode characters
        Console.OutputEncoding = Encoding.UTF8;

        // Instantiate the XAML visual view root
        var mainView = new MainView();
        var dom = new NKDom(mainView);

        // Configure the application rendering and inputs
        var config = new NKAppConfig(
            rendering: RenderingConfig.Limited(60) // Lock to 60 FPS
        );

        var app = new NKApplication(config, mainView);

        // Run the TUI
        app.Start();
    }
}
```