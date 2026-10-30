#!/usr/bin/env dotnet

#:property TargetFramework=net11.0
#:property LangVersion=15

#:project ../Src/Common/NeoKolors.Common.csproj
#:project ../Src/Console/NeoKolors.Console.csproj
#:project ../Src/Tui/Core/NeoKolors.Tui.Core.csproj
#:project ../Src/Tui/Elements/NeoKolors.Tui.Elements.csproj
#:project ../Src/Tui/Lifecycle/Core/NeoKolors.Tui.Lifecycle.Core.csproj
#:project ../Src/Tui/Widgets/NeoKolors.Tui.Widgets.csproj

using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Widgets.Elements;
using NeoKolors.Tui.Widgets.Live;

// =============================================================================
//  NeoKolors.Tui.Widgets & Inline Lifecycle Showcase
//  Demonstrating:
//   • Non-blocking inline execution without full-screen alternate buffers
//   • Animated spinners with customizable styles and state transitions
//   • Smooth progress bars with fractional unicode sub-cell blocks
//   • Composing widgets with standard declarative elements (Flex, Border, Text)
//   • Reactive updates via Signal<T>
// =============================================================================

Console.OutputEncoding = Encoding.UTF8;

// Print a styled header banner
NKConsole.WriteLine();
NKConsole.WriteLine(" ╔══════════════════════════════════════════════════════════════════════════════╗", NKColor.FromRgb(0, 180, 240));
NKConsole.WriteLine(" ║              NeoKolors.Tui.Widgets - Inline Lifecycle Showcase               ║", NKColor.FromRgb(0, 220, 180));
NKConsole.WriteLine(" ╚══════════════════════════════════════════════════════════════════════════════╝", NKColor.FromRgb(0, 180, 240));
NKConsole.WriteLine();

// -----------------------------------------------------------------------------
// Stage 1: Fast Status Spinners with Success/Error Transitions
// -----------------------------------------------------------------------------
NKConsole.WriteLine("Step 1: Running background tasks with animated status spinners...", NKColor.FromRgb(220, 220, 100));
NKConsole.WriteLine();

await NKWidgets.StatusAsync(
    "Resolving project package graph...",
    async ctx => {
        await Task.Delay(600);
        ctx.Status = "Fetching remote package index from NuGet feed...";
        await Task.Delay(800);
        ctx.Status = "Validating SHA-512 signatures and integrity...";
        await Task.Delay(500);
        ctx.Success("Dependency resolution complete (4 dependencies verified)");
    }
);

await NKWidgets.StatusAsync(
    "Verifying native POSIX / Win32 console drivers...",
    async ctx => {
        await Task.Delay(700);
        ctx.Success("Native ANSI terminal drivers verified");
    }
);

NKConsole.WriteLine();

// -----------------------------------------------------------------------------
// Stage 2: Smooth Progress Bar with Sub-cell Fractional Unicode Blocks
// -----------------------------------------------------------------------------
NKConsole.WriteLine("Step 2: Progress tracking with fractional block smoothing...", NKColor.FromRgb(220, 220, 100));
NKConsole.WriteLine();

await NKWidgets.ProgressAsync(
    "Compiling native bytecode assets...",
    async ctx => {
        for (int i = 0; i <= 100; i += 2) {
            ctx.Report(i / 100.0, $"Compiling native bytecode assets ({i}% completed)...");
            await Task.Delay(25);
        }
    }
);

NKConsole.WriteLine();

// -----------------------------------------------------------------------------
// Stage 3: Spinner Presets Showcase
// -----------------------------------------------------------------------------
NKConsole.WriteLine("Step 3: Comparing multiple spinner animation styles running live...", NKColor.FromRgb(220, 220, 100));
NKConsole.WriteLine();

var spinnerGrid = new Flex(
    direction: FlexDirection.COLUMN,
    gap: 0,
    children: new IElement[] {
        CreateSpinnerRow("Dots (Braille) :", SpinnerStyle.Dots,   NKColor.FromRgb(0,   220, 255)),
        CreateSpinnerRow("Line (ASCII)   :", SpinnerStyle.Line,   NKColor.FromRgb(255, 180, 50)),
        CreateSpinnerRow("Circle (Arc)   :", SpinnerStyle.Circle, NKColor.FromRgb(180, 100, 255)),
        CreateSpinnerRow("Pulse (Bar)    :", SpinnerStyle.Pulse,  NKColor.FromRgb(255, 80,  140)),
        CreateSpinnerRow("Arc (Quarter)  :", SpinnerStyle.Arc,    NKColor.FromRgb(80,  255, 120)),
        CreateSpinnerRow("Arrow (Compass):", SpinnerStyle.Arrow,  NKColor.FromRgb(255, 230, 80)),
    }
);

await NKWidgets.LiveAsync(
    spinnerGrid,
    async ctx => {
        await Task.Delay(2500);
    },
    fps: 20,
    persistOnExit: true
);

NKConsole.WriteLine();

// -----------------------------------------------------------------------------
// Stage 4: Composite Live UI Composed of Normal Elements
// -----------------------------------------------------------------------------
NKConsole.WriteLine("Step 4: Composite live UI composed with standard Flex, Border, and Text...", NKColor.FromRgb(220, 220, 100));
NKConsole.WriteLine();

var buildProgress = new Signal<double>(0.0);
var buildStatus   = new Signal<string>("Preparing build pipeline...");
var currentTask   = new Signal<string>("Init");

var compositeCard = new Border(
    element: new Flex(
        direction: FlexDirection.COLUMN,
        gap: 0,
        padding: new Spacing(1, 0, 1, 0),
        children: new IElement[] {
            // Header Row: Spinner + Title
            new Flex(
                direction: FlexDirection.ROW,
                gap: 1,
                children: new IElement[] {
                    new SpinnerElement(SpinnerStyle.Dots) {
                        FrameStyle = NKStyle.Default.WithFColor(NKColor.FromRgb(0, 255, 200))
                    },
                    new Text(new AnsiString(
                        "Pipeline Worker active", 
                        new NKStyle(textColor: NKColor.Cyan, styles: NKTextStyles.BOLD)
                    ))
                }
            ),

            // Current Task Text
            new Text(
                (Func<AnsiString>)(() => new AnsiString(
                    $"  Target : {currentTask.Value}",
                    new NKStyle(textColor: NKColor.White)
                ))
            ),

            // Progress Bar
            new Flex(
                direction: FlexDirection.ROW,
                gap: 1,
                children: new IElement[] {
                    new ProgressBarElement {
                        Value           = buildProgress,
                        UseSmoothBlocks = true,
                        Width           = 36,
                        FilledStyle     = NKStyle.Default.WithFColor(NKColor.FromRgb(0,  220, 180)),
                        EmptyStyle      = NKStyle.Default.WithFColor(NKColor.FromRgb(50, 60,  70))
                    }
                }
            ),

            // Status Subtitle
            new Text(
                (Func<AnsiString>)(() => new AnsiString(
                    $"  Status : {buildStatus.Value}",
                    new NKStyle(textColor: NKColor.Gray)
                ))
            )
        }
    ),
    style: NKBorderStyle.GetRounded()
);

await NKWidgets.LiveAsync(
    compositeCard,
    async ctx => {
        string[] tasks = [
            "Analyzing project references",
            "Compiling cross-platform IL",
            "Optimizing bit-packed styling structs",
            "Generating documentation bundles",
            "Packaging NuGet archives (.snupkg)"
        ];

        for (int t = 0; t < tasks.Length; t++) {
            currentTask.Value = tasks[t];

            for (int p = 0; p <= 20; p++) {
                double totalPct = (t * 20 + p) / 100.0;
                buildProgress.Value = totalPct;
                buildStatus.Value   = $"{tasks[t]} ({p * 5}%)...";
                ctx.Invalidate();
                await Task.Delay(25);
            }
        }

        currentTask.Value = "All tasks completed successfully!";
        buildStatus.Value = "Artifacts ready in output folder.";
        ctx.Invalidate();
        await Task.Delay(300);
    },
    fps: 25,
    persistOnExit: true
);

NKConsole.WriteLine();
NKConsole.WriteLine("✔ All showcase stages completed successfully!", NKColor.FromRgb(0, 255, 120));
NKConsole.WriteLine();

// Helper method to construct rows for the spinner style comparison
static Flex CreateSpinnerRow(string label, SpinnerStyle style, NKColor color) {
    return new Flex(
        direction: FlexDirection.ROW,
        gap: 1,
        children: new IElement[] {
            new Text(new AnsiString(label, new NKStyle(textColor: NKColor.Gray))),
            new SpinnerElement(style) {
                FrameStyle = NKStyle.Default.WithFColor(color)
            }
        }
    );
}