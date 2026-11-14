#!/usr/bin/env dotnet

#:property TargetFramework=net11.0
#:property LangVersion=15

#:project ..\Src\Common\NeoKolors.Common.csproj
#:project ..\Src\Console\NeoKolors.Console.csproj
#:project ..\Src\Tui\Elements\NeoKolors.Tui.Elements.csproj
#:project ..\Src\Tui\Widgets\NeoKolors.Tui.Widgets.csproj
#:project ..\Src\Tui\Core\NeoKolors.Tui.Core.csproj

using System.Globalization;
using System.Text;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Console.Ansi;
using NeoKolors.Console.Exceptions;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Widgets;
using NKTableStyle = NeoKolors.Tui.Core.NKTableStyle;

// =============================================================================
//  NeoKolors Terminal Palette Feature Detector
//  Demonstrating:
//   • Querying the terminal's 16 ANSI palette colors using OSC 4 escape sequences
//   • Batched multi-argument OSC 4 query: \e]4;0;?;1;?;...;15;?\a
//   • Single-color on-demand query: \e]4;{index};?\a
//   • Parsing terminal responses (X11 RGB color specs: rgb:rrrr/gggg/bbbb)
//   • Visual presentation of swatches, contrast, and color values
// =============================================================================

NKConsole.WriteLine();
NKConsole.WriteLine("================================================================================".AddColor(0x38bdf8));
NKConsole.WriteLine("                NeoKolors Terminal Palette Feature Detector                     ".AddStyle(NKTextStyles.BOLD).AddColor(0x38bdf8));
NKConsole.WriteLine("================================================================================".AddColor(0x38bdf8));
NKConsole.WriteLine();

// 1. Detect Terminal Environment
var termInfo = NKConsole.GetVTInfo();
NKConsole.WriteLine($"• Detected Terminal : {termInfo.Platform.ToString().AddStyle(NKTextStyles.BOLD)}{(termInfo.AdditionalInfo != null ? $" ({termInfo.AdditionalInfo})" : "")}");
NKConsole.WriteLine($"• Input Driver      : {NKConsole.InputDriver.GetType().Name}");
NKConsole.WriteLine($"• Output Driver     : {NKConsole.OutputDriver.GetType().Name}");
NKConsole.WriteLine($"• Input Redirected  : {Console.IsInputRedirected}");
NKConsole.WriteLine();

// 2. Querying 16 Basic Colors via Batched OSC 4 Query
NKConsole.WriteLine("Sending batched OSC 4 query for colors 0-15...".AddStyle(NKTextStyles.ITALIC));
var queryIndices = Enumerable.Range(0, 16).ToArray();
var query        = VTQuery.RequestRedefineColors(queryIndices);
NKConsole.WriteLine($"Sequence: {query.GetEscSeq().Replace("\e", "\\e").Replace("\a", "\\a")}".AddColor(0x94a3b8));
NKConsole.WriteLine();

IReadOnlyDictionary<int, NKColor>? queriedColors  = null;
var                                querySupported = false;

if (Console.IsInputRedirected) {
    NKConsole.WriteLine(
        "[!] Input is redirected. Interactive OSC terminal queries require a connected TTY."
            .AddColor(0xf59e0b)
    );
}
else {
    try {
        queriedColors  = await NKConsole.GetColorsAsync(queryIndices, timeout: TimeSpan.FromSeconds(2.5));
        querySupported = true;
        NKConsole.WriteLine(
            $"[✓] Success: Terminal responded with {queriedColors.Count} palette color definitions!"
                .AddColor(0x22c55e)
                .AddStyle(NKTextStyles.BOLD)
        );
    }
    catch (VTQueryTimeoutException ex) {
        NKConsole.WriteLine(
            $"[⚠] Query timed out ({ex.Timeout.TotalSeconds:0.#}s): Terminal emulator did not respond to OSC 4 query."
                .AddColor(0xf59e0b)
        );
    }
    catch (Exception ex) {
        NKConsole.WriteLine(
            $"[✕] Error querying palette: {ex.Message}".AddColor(0xef4444)
        );
    }
}

// 3. Fallback table if query not supported or incomplete
var defaultColors = new Dictionary<int, NKColor> {
    [0]  = NKColor.FromRgb(12,  12,  12),  // Black
    [1]  = NKColor.FromRgb(197, 15,  31),  // Dark Red
    [2]  = NKColor.FromRgb(19,  161, 14),  // Dark Green
    [3]  = NKColor.FromRgb(193, 156, 0),   // Dark Yellow
    [4]  = NKColor.FromRgb(0,   55,  218), // Dark Blue
    [5]  = NKColor.FromRgb(136, 23,  152), // Dark Magenta
    [6]  = NKColor.FromRgb(58,  150, 221), // Dark Cyan
    [7]  = NKColor.FromRgb(204, 204, 204), // Gray
    [8]  = NKColor.FromRgb(118, 118, 118), // Dark Gray
    [9]  = NKColor.FromRgb(231, 72,  86),  // Red
    [10] = NKColor.FromRgb(22,  198, 12),  // Green
    [11] = NKColor.FromRgb(249, 241, 165), // Yellow
    [12] = NKColor.FromRgb(59,  120, 255), // Blue
    [13] = NKColor.FromRgb(180, 0,   158), // Magenta
    [14] = NKColor.FromRgb(97,  214, 214), // Cyan
    [15] = NKColor.FromRgb(242, 242, 242)  // White
};

NKConsole.WriteLine();
NKConsole.WriteLine($"Palette Table ({(querySupported ? "Queried from Terminal" : "Default Fallback Preview")}):".AddStyle(NKTextStyles.BOLD));
NKConsole.WriteLine();

// Prepare table data
var header = new Text[] { "#", "Color Name", "Hex", "RGB Channels", "Color Swatch", "Contrast Sample" };
var rows   = new Text[16][];

for (var i = 0; i < 16; i++) {
    var color = queriedColors != null && queriedColors.TryGetValue(i, out var qCol)
        ? qCol
        : defaultColors[i];

    var colorName  = ((NKConsoleColor)i).ToString();
    var hex        = color.IsRgb ? $"#{color.AsRgb:X6}" : color.ToString("#p", CultureInfo.InvariantCulture);
    var rgb        = color.IsRgb ? $"R:{color.AsRgb.R,3} G:{color.AsRgb.G,3} B:{color.AsRgb.B,3}" : "N/A";
    var swatch     = new AnsiString("        ",    new NKStyle(backgroundColor: color));
    var textSample = new AnsiString("Sample Text", new NKStyle(textColor: color));

    rows[i] = [
        $"{i:D2}",
        colorName,
        hex,
        rgb,
        swatch,
        textSample
    ];
}

NKWidgets.Render(new Table(header, rows, NKTableStyle.Minimal));

// 4. Visual Palette Ribbons
NKConsole.WriteLine();
NKConsole.WriteLine("Standard Palette Ribbon (00 - 07):".AddStyle(NKTextStyles.BOLD));
var ribbonStandard = new StringBuilder();

for (var i = 0; i < 8; i++) {
    var c  = queriedColors != null && queriedColors.TryGetValue(i, out var qCol) ? qCol : defaultColors[i];
    var fg = i is 0 or 1 or 4 or 5 ? "\e[38;2;255;255;255m" : "\e[38;2;0;0;0m";
    ribbonStandard.Append($"{c.Bckg}{fg}  {i:D2}  {EscapeCodes.FORMATTING_RESET}");
}

NKConsole.WriteLine(ribbonStandard.ToString());

NKConsole.WriteLine();
NKConsole.WriteLine("High-Intensity / Bright Ribbon (08 - 15):".AddStyle(NKTextStyles.BOLD));
var ribbonBright = new StringBuilder();

for (var i = 8; i < 16; i++) {
    var c  = queriedColors != null && queriedColors.TryGetValue(i, out var qCol) ? qCol : defaultColors[i];
    var fg = i is 8 or 12 or 13 ? "\e[38;2;255;255;255m" : "\e[38;2;0;0;0m";
    ribbonBright.Append($"{c.Bckg}{fg}  {i:D2}  {EscapeCodes.FORMATTING_RESET}");
}

NKConsole.WriteLine(ribbonBright.ToString());

NKConsole.ReadKey();