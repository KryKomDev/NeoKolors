#!/usr/bin/env dotnet

#:property TargetFramework=net11.0
#:property LangVersion=15

#:project ../Src/Common/NeoKolors.Common.csproj
#:project ../Src/Console/NeoKolors.Console.csproj
#:project ../Src/Tui/Core/NeoKolors.Tui.Core.csproj
#:project ../Src/Tui/Elements/NeoKolors.Tui.Elements.csproj
#:project ../Src/Tui/Fonts/NeoKolors.Tui.Fonts.csproj
#:project ../Src/Tui/Lifecycle/Core/NeoKolors.Tui.Lifecycle.Core.csproj
#:project ../Src/Tui/Lifecycle/Apps/NeoKolors.Tui.Lifecycle.Apps.csproj

using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Lifecycle.Apps;

var logStream = new FileStream("./logs/example_app.blog", FileMode.Create, FileAccess.Write);

NKDebug.Logger.Writer = new CompositeLogWriter(
    new BinaryLogWriter(logStream),
    TextLogWriter.CreateFromFile("./logs/example_app.log", append: false)
);

Console.OutputEncoding = Encoding.UTF8;

// 1. Reactive state signals
var counter     = new Signal<int>(0);
var scrollValue = new Signal<int>(0);
var mousePos    = new Signal<Point2D>(Point2D.Zero);
var lastAction  = new Signal<string>("Application started");
var activeTab   = new Signal<int>(0); // 0: Overview, 1: Table (List2D), 2: List Points

// 2. Setup screen, hit map, and lifecycle host
var initialSize     = NKConsole.WindowSize;
var screen          = new NKCharScreen(initialSize.X > 0 ? initialSize : new Size2D(100, 30));
var hitMap          = new HitMap();
var mouseDispatcher = new ElementsMouseDispatcher();
var host            = new AppLifecycleHost();

var loopStrategy = new ThrottledAppLoopStrategy {
    ActiveFps           = 60,
    BackgroundFps       = 10,
    RenderOnlyWhenDirty = false
};

// 4. Create NKAppView using the App Lifecycle Framework
var view = new NKAppView(BuildUi(), screen, hitMap);

// Responsive resize
view.BeforeRender += () => {
    var winSize = NKConsole.WindowSize;

    if (winSize != screen.Size && winSize is { X: > 10, Y: > 10 }) {
        screen.Resize(winSize);
        host.NotifyWindowResized(winSize.X, winSize.Y);
        view.RootElement = BuildUi();
    }
};

activeTab.Changed += () => {
    view.RootElement = BuildUi();
    view.Invalidate();
    loopStrategy.SignalWakeup();
};

// 5. Wire up mouse and keyboard input
NKConsole.Mouse += m => {
    mousePos.Value = m.Position;
    mouseDispatcher.Dispatch(hitMap, m);
    view.Invalidate();
    loopStrategy.SignalWakeup();
};

NKConsole.Key += k => {
    if (k.Down) {
        if (k.Key == KeyCode.ESCAPE || k.Char is 'q' or 'Q' || (k.Key == KeyCode.C && k.Modifiers.HasFlag(KeyModifiers.LEFT_CTRL))) {
            host.RequestClose();
        }

        view.Invalidate();
        loopStrategy.SignalWakeup();
    }
};

NKConsole.FocusIn  += () => host.NotifyFocusChanged(true);
NKConsole.FocusOut += () => host.NotifyFocusChanged(false);

Console.CancelKeyPress += (_, e) => {
    e.Cancel = true;
    host.RequestClose();
};

// 6. Terminal Initialization and Application Loop
try {
    NKConsole.HideCursor();
    NKConsole.EnableAltBuffer();
    NKConsole.ResetModes(EscapeCodes.DecMode.AUTOWRAP);
    NKConsole.InputDriver.Config.MouseReportLevel = ReportedMouseEvents.ALL;
    NKConsole.StartInputInterception();

    host.Start();
    loopStrategy.Run(host, view);
}
finally {
    host.Stop();
    NKConsole.InputDriver.Config.MouseReportLevel = ReportedMouseEvents.NONE;
    NKConsole.StopInputInterception();
    NKConsole.SetModes(EscapeCodes.DecMode.AUTOWRAP);
    NKConsole.DisableAltBuffer();
    NKConsole.ShowCursor();
}

return;

// 3. Declarative UI builder
IElement BuildUi() {
    var currentPos = mousePos.Value;
    var clicks     = counter.Value;
    var scroll     = scrollValue.Value;
    var action     = lastAction.Value;
    var tab        = activeTab.Value;

    // Header & Status Bar
    var titleText = new Text(new AnsiString(" NeoKolors TUI Showcase ", new NKStyle(textColor: NKColor.Cyan, styles: NKTextStyles.BOLD)));

    var statusText = new Text(
        (Func<AnsiString>)(() => new AnsiString(
            $"Clicks: {counter.Value} | Scroll: {scrollValue.Value} | Mouse: ({mousePos.Value.X}, {mousePos.Value.Y}) | Tab: {tab}",
            new NKStyle(textColor: NKColor.Gray)
        ))
    );

    var quitNotice = new Text(new AnsiString("[Q / Esc to Quit]", new NKStyle(textColor: NKColor.Red, styles: NKTextStyles.BOLD)));

    var topBar = new Border(
        element: new Flex(
            direction: FlexDirection.ROW,
            justifyContent: JustifyContent.SPACE_BETWEEN,
            alignItems: FlexAlign.CENTER,
            children: new IElement[] { titleText, statusText, quitNotice },
            sizing: new Sizing(Dimension.Auto, Dimension.Chars(1))
        ),
        style: NKBorderStyle.GetRounded()
    );

    // Tab Navigation Buttons
    var tabButtons = new Flex(
        direction: FlexDirection.ROW,
        gap: 1,
        children: new IElement[] {
            Button.FromText(
                tab == 0 ? " [● Overview] " : " [○ Overview] ",
                onClick: _ => {
                    activeTab.Value  = 0;
                    lastAction.Value = "Switched to Overview tab";
                },
                normalStyle: tab == 0
                    ? new NKStyle(textColor: NKColor.Black, backgroundColor: NKColor.Cyan, styles: NKTextStyles.BOLD)
                    : new NKStyle(textColor: NKColor.Cyan)
            ),
            Button.FromText(
                tab == 1 ? " [● Data Table] " : " [○ Data Table] ",
                onClick: _ => {
                    activeTab.Value  = 1;
                    lastAction.Value = "Switched to Data Table tab";
                },
                normalStyle: tab == 1
                    ? new NKStyle(textColor: NKColor.Black, backgroundColor: NKColor.Magenta, styles: NKTextStyles.BOLD)
                    : new NKStyle(textColor: NKColor.Magenta)
            ),
            Button.FromText(
                tab == 2 ? " [● List Points] " : " [○ List Points] ",
                onClick: _ => {
                    activeTab.Value  = 2;
                    lastAction.Value = "Switched to List Points tab";
                },
                normalStyle: tab == 2
                    ? new NKStyle(textColor: NKColor.Black, backgroundColor: NKColor.Yellow, styles: NKTextStyles.BOLD)
                    : new NKStyle(textColor: NKColor.Yellow)
            )
        }
    );

    // Left Sidebar: Interactive controls & scroll testing zone
    var incBtn = Button.FromText(
        " [ + ] Increment ",
        onClick: _ => {
            counter.Value++;
            lastAction.Value = $"Counter incremented -> {counter.Value}";
        },
        hoverStyle: new NKStyle(textColor: NKColor.Black, backgroundColor: NKColor.Green, styles: NKTextStyles.BOLD)
    );

    var decBtn = Button.FromText(
        " [ - ] Decrement ",
        onClick: _ => {
            counter.Value--;
            lastAction.Value = $"Counter decremented -> {counter.Value}";
        },
        hoverStyle: new NKStyle(textColor: NKColor.Black, backgroundColor: NKColor.Yellow, styles: NKTextStyles.BOLD)
    );

    var resetBtn = Button.FromText(
        " [ 0 ] Reset All ",
        onClick: _ => {
            counter.Value     = 0;
            scrollValue.Value = 0;
            lastAction.Value  = "Reset all counters";
        },
        hoverStyle: new NKStyle(textColor: NKColor.Black, backgroundColor: NKColor.Red, styles: NKTextStyles.BOLD)
    );

    var exitBtn = Button.FromText(
        " [ X ] Exit App  ",
        onClick: _ => host.RequestClose(),
        hoverStyle: new NKStyle(textColor: NKColor.White, backgroundColor: NKColor.DarkRed, styles: NKTextStyles.BOLD)
    );

    // Mouse wheel scrollable target region
    var scrollBox = new Border(
            element: new Text(
                (Func<AnsiString>)(() => new AnsiString(
                    $"Scroll Wheel Target\nScroll Value: {scrollValue.Value}",
                    new NKStyle(textColor: NKColor.Cyan)
                ))
            ),
            style: NKBorderStyle.GetRounded(),
            padding: new Spacing(Dimension.Chars(0), Dimension.Chars(1))
        )
        .OnScroll(
            e => {
                if (e.IsWheelUp) {
                    scrollValue.Value++;
                    lastAction.Value = $"Wheel Up -> {scrollValue.Value}";
                }
                else if (e.IsWheelDown) {
                    scrollValue.Value--;
                    lastAction.Value = $"Wheel Down -> {scrollValue.Value}";
                }
            }
        )
        .OnHover(
            onMouseEnter: _ => lastAction.Value = "Hovering over scroll zone",
            onMouseLeave: _ => lastAction.Value = "Left scroll zone"
        );

    var sidebar = new Border(
        element: new Flex(
            direction: FlexDirection.COLUMN,
            gap: 1,
            children: new IElement[] {
                new Text(new AnsiString(" Sidebar Controls ", new NKStyle(textColor: NKColor.Green, styles: NKTextStyles.BOLD))),
                incBtn,
                decBtn,
                resetBtn,
                scrollBox,
                exitBtn
            }
        ),
        style: NKBorderStyle.GetRounded()
    );

    // Right Content Area based on current tab
    IElement contentPanel;

    if (tab == 1) {
        // Table Showcase using Metriks List2D
        var tableHeaders = new[] { "Module", "Component", "Role", "Reactive" };

        var tableRows = new List<IReadOnlyList<string>> {
            new[] { "Core", "HitMap", "Spatial 2D Raycast", "Yes" },
            new[] { "Core", "RenderContext", "ZIndex Stack & Clipping", "Yes" },
            new[] { "Elements", "Flex", "Direction, Gap, Align", "Yes" },
            new[] { "Elements", "Table", "Metriks List2D Data", "Yes" },
            new[] { "Elements", "Button", "Hover & Press Visuals", "Yes" },
            new[] { "Elements", "MouseRegion", "Event Routing & Bubbling", "Yes" },
            new[] { "Lifecycle", "AppLifecycleHost", "Activation & Loop State", "Yes" },
            new[] { "Lifecycle", "NKAppView", "Render Pipeline & Screen", "Yes" }
        };

        var table = Table.FromText(tableHeaders, tableRows, NeoKolors.Tui.Core.NKTableStyle.GetRounded());

        contentPanel = new Border(
            element: new Flex(
                direction: FlexDirection.COLUMN,
                gap: 1,
                children: new IElement[] {
                    new Text(new AnsiString(" Tabular Data (Powered by Metriks List2D) ", new NKStyle(textColor: NKColor.Magenta, styles: NKTextStyles.BOLD))),
                    table
                }
            ),
            style: NKBorderStyle.GetRounded()
        );
    }
    else if (tab == 2) {
        // List Points Showcase
        var bulletList = new List(
            items: new IElement[] {
                new Text(new AnsiString("Bullet: Declarative elements measured on layout")),
                new Text(new AnsiString("Bullet: Immediate-mode render stack execution")),
                new Text(new AnsiString("Bullet: Z-Index sorting with painter's algorithm"))
            },
            pointGenerator: ListPoints.Bullet,
            pointGap: 1
        );

        var numberedList = new List(
            items: new IElement[] {
                new Text(new AnsiString("Start AppLifecycleHost")),
                new Text(new AnsiString("Attach Mouse & Keyboard listeners")),
                new Text(new AnsiString("Execute render pass via NKAppView")),
                new Text(new AnsiString("Dispatch mouse hover/clicks via ElementsMouseDispatcher"))
            },
            pointGenerator: ListPoints.NumberDot,
            pointGap: 1
        );

        var arrowList = new List(
            items: new IElement[] {
                new Text(new AnsiString("Fast multi-target .NET compilation")),
                new Text(new AnsiString("Cross-platform ANSI terminal driver")),
                new Text(new AnsiString("Zero external UI runtime dependencies"))
            },
            pointGenerator: ListPoints.Arrow,
            pointGap: 1
        );

        contentPanel = new Border(
            element: new Flex(
                direction: FlexDirection.COLUMN,
                gap: 1,
                children: new IElement[] {
                    new Text(new AnsiString(" Custom List Marker Generators ", new NKStyle(textColor: NKColor.Yellow, styles: NKTextStyles.BOLD))),
                    new Text(new AnsiString("Unordered Bullet List:",          new NKStyle(textColor: NKColor.Cyan))),
                    bulletList,
                    new Text(new AnsiString("Ordered Numbered List:", new NKStyle(textColor: NKColor.Green))),
                    numberedList,
                    new Text(new AnsiString("Arrow Marker List:", new NKStyle(textColor: NKColor.Magenta))),
                    arrowList
                }
            ),
            style: NKBorderStyle.GetRounded()
        );
    }
    else {
        // Overview Tab
        var welcomeCard = new Border(
            element: new Flex(
                direction: FlexDirection.COLUMN,
                gap: 1,
                children: new IElement[] {
                    new Text(new AnsiString(" NeoKolors Declarative Elements & Mouse Framework ", new NKStyle(textColor: NKColor.Cyan, styles: NKTextStyles.BOLD))),
                    new Text(new AnsiString("Welcome! This interactive testing app demonstrates the new elements system:")),
                    new Text(new AnsiString("  • Real-time mouse clicks, hover transitions, and mouse wheel scrolling")),
                    new Text(new AnsiString("  • Flexible layouts using FlexBox (Row/Column, JustifyContent, Gap)")),
                    new Text(new AnsiString("  • Tables backed by Metriks IReadOnlyList2D")),
                    new Text(new AnsiString("  • Customizable lists with ordered and unordered point generators")),
                    new Text(new AnsiString("  • Coordinated by the new AppLifecycle framework (Host, AppView, Throttled Loop)"))
                }
            ),
            style: NKBorderStyle.GetRounded()
        );

        var diagCard = new Border(
            element: new Flex(
                direction: FlexDirection.COLUMN,
                gap: 1,
                children: new IElement[] {
                    new Text(new AnsiString(" Live Diagnostics ", new NKStyle(textColor: NKColor.Green, styles: NKTextStyles.BOLD))),
                    new Text((Func<AnsiString>)(() => new AnsiString($"Last Action     : {lastAction.Value}",                                 new NKStyle(textColor: NKColor.White)))),
                    new Text((Func<AnsiString>)(() => new AnsiString($"Cursor Screen   : X={mousePos.Value.X}, Y={mousePos.Value.Y}",         new NKStyle(textColor: NKColor.White)))),
                    new Text((Func<AnsiString>)(() => new AnsiString($"Hovered Targets : {mouseDispatcher.HoveredTargets.Count}",             new NKStyle(textColor: NKColor.White)))),
                    new Text((Func<AnsiString>)(() => new AnsiString($"Lifecycle State : {host.ActivationState} (Running: {host.IsRunning})", new NKStyle(textColor: NKColor.White)))),
                    new Text((Func<AnsiString>)(() => new AnsiString($"Target FrameRate: {loopStrategy.ActiveFps} FPS",                       new NKStyle(textColor: NKColor.White))))
                }
            ),
            style: NKBorderStyle.GetRounded()
        );

        contentPanel = new Flex(
            direction: FlexDirection.COLUMN,
            gap: 1,
            children: new IElement[] {
                welcomeCard,
                diagCard
            }
        );
    }

    // Main Body: Sidebar on left, Content Panel on right
    var body = new Flex(
        direction: FlexDirection.ROW,
        gap: 1,
        children: new[] {
            sidebar,
            contentPanel
        }
    );

    // Full Screen Column
    return new Flex(
        direction: FlexDirection.COLUMN,
        gap: 1,
        padding: new Spacing(Dimension.Chars(1)),
        children: new IElement[] {
            topBar,
            tabButtons,
            body
        }
    );
}