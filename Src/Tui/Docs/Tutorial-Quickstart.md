# Tutorial: TUI Quick-Start Guide

This tutorial walks you through building your first interactive Text User Interface (TUI) application using **NeoKolors.Tui**. We will create an interactive counter dashboard with button clicks, mouse handling, and responsive resizing.

---

## 1. Prerequisites & Installation

Create a new .NET console application targeting .NET 8 or higher:

```bash
dotnet new console -n QuickstartApp
cd QuickstartApp
```

Add the required NeoKolors packages:

```bash
dotnet add package NeoKolors.Common
dotnet add package NeoKolors.Console
dotnet add package NeoKolors.Tui.Core
dotnet add package NeoKolors.Tui.Elements
dotnet add package NeoKolors.Tui.Lifecycle.Apps
```

---

## 2. Setting Up the Main Application (`Program.cs`)

Replace the contents of `Program.cs` with the following code:

```csharp
using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Lifecycle.Apps;

// 1. Ensure UTF-8 output encoding for proper border characters
Console.OutputEncoding = Encoding.UTF8;

// 2. Setup low-level debugging logs (optional)
NKDebug.Logger.Level = LoggerLevel.CRIT | LoggerLevel.ERROR | LoggerLevel.WARN | LoggerLevel.INFO;
NKDebug.Logger.FileConfig = LogFileConfig.Replace("./app.log");

// 3. State variable
int counter = 0;

// 4. Initialize screen, hit map, and lifecycle host
var initialSize = NKConsole.WindowSize;
var screen = new NKCharScreen(initialSize.X > 0 ? initialSize : new Size2D(80, 24));
var hitMap = new HitMap();
var mouseDispatcher = new ElementsMouseDispatcher();
var host = new AppLifecycleHost();
var loopStrategy = new ThrottledAppLoopStrategy {
    ActiveFps = 60,
    BackgroundFps = 10,
    RenderOnlyWhenDirty = true
};

// 5. Declare the UI builder function
IElement BuildUi() {
    var flex = new Flex {
        Direction = FlexDirection.Column,
        Justify = FlexJustify.Center,
        Align = FlexAlign.Center,
        Width = screen.Size.X,
        Height = screen.Size.Y,
        Gap = 1
    };

    var title = new Text("Welcome to NeoKolors TUI!") {
        Style = new NKStyle(NKColor.FromRgb(0, 220, 180), NKColor.Inherit, TextStyles.Bold)
    };

    var counterText = new Text($"Clicks: {counter}") {
        Style = new NKStyle(NKColor.FromRgb(255, 215, 0), NKColor.Inherit)
    };

    var btn = new Button {
        Text = " [ Click Me ] ",
        NormalStyle = new NKStyle(NKColor.FromRgb(255, 255, 255), NKColor.FromRgb(40, 80, 160), TextStyles.Bold),
        HoverStyle = new NKStyle(NKColor.FromRgb(255, 255, 255), NKColor.FromRgb(60, 120, 240), TextStyles.Bold)
    };

    // Button click handler
    btn.OnClick += () => {
        counter++;
        view.RootElement = BuildUi();
        view.Invalidate();
        loopStrategy.SignalWakeup();
    };

    var box = new Border(btn) {
        Title = " Interactive Button ",
        Padding = new Spacing(2, 1),
        Style = NKBorderStyle.GetRounded(NKColor.FromRgb(0, 220, 180))
    };

    flex.Add(title);
    flex.Add(counterText);
    flex.Add(box);

    return flex;
}

// 6. Instantiate the view presenter
var view = new NKAppView(BuildUi(), screen, hitMap);

// 7. Responsive window resize handling
view.BeforeRender += () => {
    var winSize = NKConsole.WindowSize;
    if (winSize != screen.Size && winSize is { X: > 10, Y: > 10 }) {
        screen.Resize(winSize);
        host.NotifyWindowResized(winSize.X, winSize.Y);
        view.RootElement = BuildUi();
    }
};

// 8. Wire up mouse and keyboard inputs
NKConsole.Mouse += m => {
    mouseDispatcher.Dispatch(hitMap, m);
    view.Invalidate();
    loopStrategy.SignalWakeup();
};

NKConsole.Key += k => {
    if (k.Down && (k.Key == KeyCode.ESCAPE || k.Char is 'q' or 'Q')) {
        host.RequestClose();
    }
};

// 9. Run the application
try {
    NKConsole.HideCursor();
    NKConsole.EnableAltBuffer();
    host.Start();

    while (host.State != LifecycleState.STOPPED) {
        loopStrategy.Tick(host, view);
    }
} finally {
    NKConsole.DisableAltBuffer();
    NKConsole.ShowCursor();
}
```

---

## 3. Running the Application

Execute your new TUI application from your terminal:

```bash
dotnet run
```

You can now click the button with your mouse, watch the counter increment, resize your terminal window to see responsive reflowing, and press `Escape` or `q` to cleanly exit!
