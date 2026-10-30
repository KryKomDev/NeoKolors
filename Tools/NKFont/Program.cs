// NeoKolors
// Copyright (c) 2026 KryKom

using System.CommandLine;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Extensions;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Fonts.Assets;
using NeoKolors.Tui.Widgets.Live;
using static System.Text.Encoding;
using NKTableStyle = NeoKolors.Tui.Core.NKTableStyle;

namespace NeoKolors.Tools.NKFont;

internal static class Program {

    private static readonly string FONTS_STORAGE_DIR = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "NeoKolors",
        "NKFont",
        "Fonts"
    );

    static Program() {
        var installDir = AppContext.BaseDirectory;

        if (!Directory.Exists(installDir))
            Directory.CreateDirectory(installDir);

        var logsDir = Path.Combine(installDir, "logs");

        if (!Directory.Exists(logsDir))
            Directory.CreateDirectory(logsDir);

        if (!Directory.Exists(FONTS_STORAGE_DIR))
            Directory.CreateDirectory(FONTS_STORAGE_DIR);

        NKDebug.SetFileLogging(LogFileConfig.NewDatetime($"{logsDir}/{{0}}.log"));
    }

    static int Main(string[] args) {
        // Support UTF-8 encoding for smooth unicode characters
        System.Console.OutputEncoding = UTF8;

        // Force-load and register built-in fonts
        AssetsProvider.RegisterFonts();

        // 1. compile command
        var xmlPathArg = new Argument<string>("xml-path") {
            Description = "The path to the XML font folder or zip file."
        };

        var outputNkfArg = new Argument<string>("output-nkf") {
            Description =
                "The path to save the compiled binary font (.nkf). If omitted," +
                " saves using the font's internal name in the current directory.",
            Arity = ArgumentArity.ZeroOrOne
        };

        var compileCommand = new Command(
            "compile",
            "Compile an XML font definition (directory or zip) into a compiled binary .nkf font."
        );

        compileCommand.Arguments.Add(xmlPathArg);
        compileCommand.Arguments.Add(outputNkfArg);

        compileCommand.SetAction(
            parseResult => {
                string  xmlPath   = parseResult.GetValue(xmlPathArg) ?? string.Empty;
                string? outputNkf = parseResult.GetValue(outputNkfArg);
                HandleCompile(xmlPath, outputNkf);
            }
        );

        // 2. display command
        var fontArg = new Argument<string>("font") {
            Description = "A built-in font name, compiled .nkf file, or raw XML font folder/zip."
        };

        var textArg = new Argument<string[]>("text") {
            Description = "The text to display."
        };

        var halignOption = new Option<HorizontalAlign>("--horizontal-align", "--halign", "-a") {
            Description         = "Horizontal alignment of rendered text (Left, Center, Right).",
            DefaultValueFactory = _ => HorizontalAlign.LEFT
        };

        var valignOption = new Option<VerticalAlign>("--vertical-align", "--valign") {
            Description         = "Vertical alignment of rendered text (Top, Center, Bottom).",
            DefaultValueFactory = _ => VerticalAlign.TOP
        };

        var overflowOption = new Option<bool>("--overflow") {
            Description         = "Allow text to overflow outside bounds instead of clipping.",
            DefaultValueFactory = _ => false
        };

        var justifyOption = new Option<bool>("--justify", "--justify-text", "-j") {
            Description         = "Justify text within the available width.",
            DefaultValueFactory = _ => false
        };

        var wordWrapOption = new Option<WordWrapType>("--word-wrap", "--wrap") {
            Description         = "Word wrap behavior (None, Hard, Word).",
            DefaultValueFactory = _ => WordWrapType.WORD
        };

        var widthOption = new Option<int?>("--width", "-w") {
            Description = "Explicit bounding box width in characters."
        };

        var heightOption = new Option<int?>("--height", "-H") {
            Description = "Explicit bounding box height in rows."
        };

        var colorOption = new Option<string?>("--color", "--coloring", "-c") {
            Description = "Foreground color for rendering (e.g. Red, #FF0000)."
        };

        var hgradOption = new Option<string?>("--hgrad") {
            Description = "Horizontal gradient color stops, comma-separated (e.g. Red,Blue or #FF0000,#00FF00)."
        };

        var vgradOption = new Option<string?>("--vgrad") {
            Description = "Vertical gradient color stops, comma-separated (e.g. Red,Blue or #FF0000,#00FF00)."
        };

        var letterSpacingOption = new Option<int?>("--letter-spacing") {
            Description = "Letter spacing adjustment in characters."
        };

        var wordSpacingOption = new Option<int?>("--word-spacing") {
            Description = "Word spacing adjustment in characters."
        };

        var leadingOption = new Option<int?>("--leading", "--line-spacing") {
            Description = "Line spacing / leading in rows."
        };

        var ligaturesOption = new Option<bool?>("--ligatures") {
            Description = "Enable or disable font ligatures."
        };

        var proportionsOption = new Option<string?>("--proportions") {
            Description = "Font proportions mode: monospaced (mono), variable (var), or kerning."
        };

        var displayCommand = new Command("display", "Display/render the given string using a specified font.");
        displayCommand.Arguments.Add(fontArg);
        displayCommand.Arguments.Add(textArg);
        displayCommand.Options.Add(halignOption);
        displayCommand.Options.Add(valignOption);
        displayCommand.Options.Add(overflowOption);
        displayCommand.Options.Add(justifyOption);
        displayCommand.Options.Add(wordWrapOption);
        displayCommand.Options.Add(widthOption);
        displayCommand.Options.Add(heightOption);
        displayCommand.Options.Add(colorOption);
        displayCommand.Options.Add(hgradOption);
        displayCommand.Options.Add(vgradOption);
        displayCommand.Options.Add(letterSpacingOption);
        displayCommand.Options.Add(wordSpacingOption);
        displayCommand.Options.Add(leadingOption);
        displayCommand.Options.Add(ligaturesOption);
        displayCommand.Options.Add(proportionsOption);

        displayCommand.Aliases.Add("show");
        displayCommand.Aliases.Add("render");

        displayCommand.SetAction(
            parseResult => {
                string   font      = parseResult.GetValue(fontArg) ?? string.Empty;
                string[] textParts = parseResult.GetValue(textArg) ?? Array.Empty<string>();
                string   text      = string.Join(" ", textParts);

                var displayOpts = new DisplayOptions(
                    HorizontalAlign: parseResult.GetValue(halignOption),
                    VerticalAlign: parseResult.GetValue(valignOption),
                    Overflow: parseResult.GetValue(overflowOption),
                    Justify: parseResult.GetValue(justifyOption),
                    WordWrap: parseResult.GetValue(wordWrapOption),
                    Width: parseResult.GetValue(widthOption),
                    Height: parseResult.GetValue(heightOption),
                    Color: parseResult.GetValue(colorOption),
                    HGrad: parseResult.GetValue(hgradOption),
                    VGrad: parseResult.GetValue(vgradOption),
                    LetterSpacing: parseResult.GetValue(letterSpacingOption),
                    WordSpacing: parseResult.GetValue(wordSpacingOption),
                    Leading: parseResult.GetValue(leadingOption),
                    Ligatures: parseResult.GetValue(ligaturesOption),
                    Proportions: parseResult.GetValue(proportionsOption)
                );

                HandleDisplay(font, text, displayOpts);
            }
        );

        // 3. create command
        var outputDirArg = new Argument<string>("output-dir") {
            Description = "The output directory where the new XML definition will be created."
        };

        var fontNameArg = new Argument<string>("font-name") {
            Description = "The internal name of the font. If omitted, defaults to the name of the folder.",
            Arity       = ArgumentArity.ZeroOrOne
        };

        var allAsciiOption = new Option<bool>("--all-ascii") {
            Description = "Add all printable ASCII characters to the blank font structure."
        };

        allAsciiOption.Aliases.Add("-a");

        var createCommand = new Command("create", "Create a new blank XML nkfont definition folder structure.");
        createCommand.Arguments.Add(outputDirArg);
        createCommand.Arguments.Add(fontNameArg);
        createCommand.Options.Add(allAsciiOption);

        createCommand.Aliases.Add("new");

        createCommand.SetAction(
            parseResult => {
                string  outputDir = parseResult.GetValue(outputDirArg) ?? string.Empty;
                string? fontName  = parseResult.GetValue(fontNameArg);
                bool    allAscii  = parseResult.GetValue(allAsciiOption);
                HandleCreate(outputDir, fontName, allAscii);
            }
        );

        // 4. list command
        var listCommand = new Command("list", "List all available built-in fonts.");
        listCommand.SetAction(_ => { HandleList(); });

        // 5. glyphs command
        var glyphsFontArg = new Argument<string>("font") {
            Description = "A built-in font name, compiled .nkf file, or raw XML font folder/zip."
        };

        var glyphsCommand = new Command("font", "List all glyphs contained inside a specified font.");
        glyphsCommand.Arguments.Add(glyphsFontArg);

        glyphsCommand.Aliases.Add("inspect-font");

        glyphsCommand.SetAction(
            parseResult => {
                string font = parseResult.GetValue(glyphsFontArg) ?? string.Empty;
                HandleInspectFont(font);
            }
        );

        // 6. inspect command
        var inspectFontArg = new Argument<string>("font") {
            Description = "A built-in font name, compiled .nkf file, or raw XML font folder/zip."
        };

        var inspectGlyphArg = new Argument<string>("glyph") {
            Description = "The character or string ligature representation of the glyph to inspect."
        };

        var inspectCommand = new Command("glyph", "Inspect a single glyph within a font, showing its properties and rendering it visually.");
        inspectCommand.Arguments.Add(inspectFontArg);
        inspectCommand.Arguments.Add(inspectGlyphArg);

        inspectCommand.Aliases.Add("inspect-glyph");

        inspectCommand.SetAction(
            parseResult => {
                string font  = parseResult.GetValue(inspectFontArg)  ?? string.Empty;
                string glyph = parseResult.GetValue(inspectGlyphArg) ?? string.Empty;
                HandleInspectGlyph(font, glyph);
            }
        );

        // 7. add command
        var addPathArg = new Argument<string>("font-path") {
            Description = "The path to the .flf or .nkf font file to add."
        };

        var addCommand = new Command("add", "Add a font file to the tools internal storage.");
        addCommand.Arguments.Add(addPathArg);
        addCommand.Aliases.Add("import");
        addCommand.Aliases.Add("install");

        addCommand.SetAction(
            parseResult => {
                string fontPath = parseResult.GetValue(addPathArg) ?? string.Empty;
                HandleAdd(fontPath);
            }
        );

        // 8. remove command
        var removeNameArg = new Argument<string>("font-name") {
            Description = "The name of the font to remove from internal storage."
        };

        var removeCommand = new Command("remove", "Remove a font from the tools internal storage.");
        removeCommand.Arguments.Add(removeNameArg);
        removeCommand.Aliases.Add("delete");
        removeCommand.Aliases.Add("uninstall");

        removeCommand.SetAction(
            parseResult => {
                string fontName = parseResult.GetValue(removeNameArg) ?? string.Empty;
                HandleRemove(fontName);
            }
        );

        // 9. clean-logs command
        var cleanLogsCommand = new Command("clean-logs", "Clean all log files generated by the nkfont tool.");
        cleanLogsCommand.Aliases.Add("clear-logs");
        cleanLogsCommand.Aliases.Add("flush-logs");

        cleanLogsCommand.SetAction(
            _ => {
                HandleCleanLogs();
            }
        );

        // Add commands to root
        var rootCommand = new RootCommand("NeoKolors NKFont CLI Tool - Manage, compile, display, and create TUI fonts.");
        rootCommand.Subcommands.Add(compileCommand);
        rootCommand.Subcommands.Add(displayCommand);
        rootCommand.Subcommands.Add(createCommand);
        rootCommand.Subcommands.Add(listCommand);
        rootCommand.Subcommands.Add(glyphsCommand);
        rootCommand.Subcommands.Add(inspectCommand);
        rootCommand.Subcommands.Add(addCommand);
        rootCommand.Subcommands.Add(removeCommand);
        rootCommand.Subcommands.Add(cleanLogsCommand);

        // Invoke parsing and return the exit code
        return rootCommand.Parse(args).Invoke();
    }

    static void HandleCompile(string xmlPath, string? outputNkf) {
        string fullXmlPath = Path.GetFullPath(xmlPath);

        if (!Directory.Exists(fullXmlPath) && !File.Exists(fullXmlPath)) {
            PrintError($"Source path '{fullXmlPath}' does not exist.");

            return;
        }

        NKWidgets.Status(
            $"Reading and parsing XML font definition from '{Path.GetFileName(fullXmlPath)}'...",
            ctx => {
                Tui.Fonts.NKFont? font;

                try {
                    font = NKFontSerializer.DeserializeXml(fullXmlPath);
                }
                catch (Exception ex) {
                    ctx.Fail($"Failed to deserialize XML font: {ex.Message}");

                    return;
                }

                if (font == null) {
                    ctx.Fail("Failed to load/compile XML font definition. Please check XML validation logs.");

                    return;
                }

                string outputPath;

                if (!string.IsNullOrEmpty(outputNkf)) {
                    outputPath = Path.GetFullPath(outputNkf);
                }
                else {
                    string safeName = string.Join("_", font.Name.Split(Path.GetInvalidFileNameChars()));
                    outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"{safeName}.nkf");
                }

                ctx.Status = $"Serializing compiled font '{font.Name}' to '{Path.GetFileName(outputPath)}'...";

                try {
                    NKFontSerializer.SerializeBinary(font, outputPath);
                    ctx.Success($"Successfully compiled font '{font.Name}' ({font.Glyphs.Count} glyphs) to '{outputPath}'!");
                }
                catch (Exception ex) {
                    ctx.Fail($"Failed to save compiled font: {ex.Message}");
                }
            }
        );
    }

    private record DisplayOptions(
        HorizontalAlign HorizontalAlign,
        VerticalAlign   VerticalAlign,
        bool            Overflow,
        bool            Justify,
        WordWrapType    WordWrap,
        int?            Width,
        int?            Height,
        string?         Color,
        string?         HGrad,
        string?         VGrad,
        int?            LetterSpacing,
        int?            WordSpacing,
        int?            Leading,
        bool?           Ligatures,
        string?         Proportions
    );

    static void HandleDisplay(string fontKey, string text, DisplayOptions options) {
        var font = LoadFont(fontKey);

        if (font == null) {
            PrintError($"Could not load font '{fontKey}'.");

            return;
        }

        text = text.Unescape();

        if (!AnsiString.TryParse(text, out var ansiString)) {
            PrintError($"Could not parse text '{text}'.");

            return;
        }

        IColoringStrategy? coloringStrategy = null;

        if (!string.IsNullOrEmpty(options.HGrad)) {
            var colors = ParseColorList(options.HGrad);

            if (colors == null || colors.Length == 0) {
                return;
            }

            coloringStrategy = ColoringStrategies.HGrad(colors);
        }
        else if (!string.IsNullOrEmpty(options.VGrad)) {
            var colors = ParseColorList(options.VGrad);

            if (colors == null || colors.Length == 0) {
                return;
            }

            coloringStrategy = ColoringStrategies.VGrad(colors);
        }
        else if (!string.IsNullOrEmpty(options.Color)) {
            if (NKColor.TryParse(options.Color, out var col)) {
                coloringStrategy = ColoringStrategies.Solid(col);
            }
            else {
                PrintError($"Could not parse color '{options.Color}'.");

                return;
            }
        }

        FontProportionsInfo? proportionsInfo = null;

        if (!string.IsNullOrEmpty(options.Proportions)) {
            var propLower = options.Proportions.Trim().ToLowerInvariant();

            switch (propLower) {
                case "mono" or "monospaced":
                    if (font is Tui.Fonts.NKFont nkf && nkf.Info.ProportionType.GetIsMonospaced()) {
                        proportionsInfo = nkf.Info.FontPropoInfo;
                    }
                    else {
                        proportionsInfo = new FontProportionsInfo(new NKFontMonospacedConfig(1, 1, false));
                    }

                    break;
                case "var" or "variable": proportionsInfo = new FontProportionsInfo(new NKFontVariableConfig(kerning: false)); break;
                case "kerning":           proportionsInfo = new FontProportionsInfo(new NKFontVariableConfig(kerning: true)); break;
                default:
                    PrintError($"Unknown proportions '{options.Proportions}'. Supported values: monospaced (mono), variable (var), kerning.");

                    return;
            }
        }

        FontSpacingInfo? spacingInfo = null;

        if (options.LetterSpacing != null || options.WordSpacing != null || options.Leading != null || options.Ligatures != null) {
            var baseSpacing = font is Tui.Fonts.NKFont nkf
                ? new FontSpacingInfo(
                    ligatures: nkf.Info.Ligatures,
                    leading: nkf.Info.Leading,
                    letterSpacing: nkf.Info.LetterSpacing,
                    wordSpacing: nkf.Info.WordSpacing
                )
                : new FontSpacingInfo();

            spacingInfo = new FontSpacingInfo(
                ligatures: options.Ligatures         ?? baseSpacing.Ligatures,
                leading: options.Leading             ?? baseSpacing.Leading,
                letterSpacing: options.LetterSpacing ?? baseSpacing.LetterSpacing,
                wordSpacing: options.WordSpacing     ?? baseSpacing.WordSpacing
            );
        }

        var renderingOptions = new TextRenderingOptions(
            horizontalAlign: options.HorizontalAlign,
            verticalAlign: options.VerticalAlign,
            overflow: options.Overflow,
            justifyText: options.Justify,
            wordWrap: options.WordWrap,
            coloring: coloringStrategy,
            proportions: proportionsInfo,
            spacing: spacingInfo
        );

        int maxWindowWidth     = Math.Max(20, NKConsole.WindowSize.X - 1);
        int effectiveWrapWidth = options.Width ?? maxWindowWidth;

        var measuredSize = options.WordWrap == WordWrapType.NONE
            ? font.GetSize(ansiString, renderingOptions)
            : font.GetSize(ansiString, renderingOptions, maxWidth: effectiveWrapWidth);

        if (measuredSize.X == 0 || measuredSize.Y == 0) {
            NKConsole.WriteLine("Warning: Rendered text has zero width or height.", ConsoleColor.Yellow);

            return;
        }

        int boundsWidth;
        int canvasWidth;

        if (options.Width != null) {
            boundsWidth = options.Width.Value;
            canvasWidth = options.Overflow ? Math.Max(boundsWidth, measuredSize.X) : boundsWidth;
        }
        else if (options.HorizontalAlign != HorizontalAlign.LEFT || options.Justify) {
            boundsWidth = maxWindowWidth;
            canvasWidth = maxWindowWidth;
        }
        else {
            boundsWidth = measuredSize.X;
            canvasWidth = measuredSize.X;
        }

        int boundsHeight;
        int canvasHeight;

        if (options.Height != null) {
            boundsHeight = options.Height.Value;
            canvasHeight = options.Overflow ? Math.Max(boundsHeight, measuredSize.Y) : boundsHeight;
        }
        else {
            boundsHeight = measuredSize.Y;
            canvasHeight = measuredSize.Y;
        }

        boundsWidth  = Math.Max(1, boundsWidth);
        boundsHeight = Math.Max(1, boundsHeight);
        canvasWidth  = Math.Max(1, canvasWidth);
        canvasHeight = Math.Max(1, canvasHeight);

        var bounds = new Rect2D(0, 0, boundsWidth, boundsHeight);

        using var canvas = new NKCharCanvas(canvasWidth, canvasHeight);
        font.DrawString(ansiString, canvas, bounds, renderingOptions);

        NKConsole.WriteLine();
        NKConsole.WriteLine($"Text size: {measuredSize} | Font: {font.Name}", ConsoleColor.Yellow);
        NKConsole.WriteLine();

        // var displayCard = new Border(
        //     element: new CanvasElement(canvas),
        //     style: NKBorderStyle.GetRounded()
        // );

        NKWidgets.Render(new CanvasElement(canvas));
        NKConsole.WriteLine();
    }

    private sealed class CanvasElement(ICharCanvas canvas) : IElement {
        public Size2D Measure(LayoutContext context) => canvas.Size;

        public void Render(RenderContext context, Size2D parent) {
            context.Canvas.Draw(canvas, context.Origin.X, context.Origin.Y, context.ZIndex);
        }
    }

    static NKColor[]? ParseColorList(string input) {
        var parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 0) {
            PrintError("Gradient color list cannot be empty.");

            return null;
        }

        var colors = new List<NKColor>(parts.Length);

        foreach (var part in parts) {
            if (NKColor.TryParse(part, out var c)) {
                colors.Add(c);
            }
            else {
                PrintError($"Could not parse color '{part}'.");

                return null;
            }
        }

        return colors.ToArray();
    }

    static void HandleCreate(string outputDir, string? fontName, bool allAscii = false) {
        string fullOutputDir = Path.GetFullPath(outputDir);

        if (string.IsNullOrWhiteSpace(fontName)) {
            fontName = Path.GetFileName(fullOutputDir.TrimEnd('/', '\\'));
        }

        if (string.IsNullOrWhiteSpace(fontName)) {
            fontName = "NewFont";
        }

        if (Directory.Exists(fullOutputDir) && Directory.EnumerateFileSystemEntries(fullOutputDir).Any()) {
            PrintError($"Directory '{fullOutputDir}' is not empty. Please specify a new or empty directory.");

            return;
        }

        try {
            if (allAscii) {
                NKWidgets.Progress(
                    $"Scaffolding full ASCII font template for '{fontName}'...",
                    pctx => {
                        Directory.CreateDirectory(fullOutputDir);

                        string manifestContent = ReadTemplate("manifest.nkfont");
                        string configContent   = ReadTemplate("Config.xml").Replace("{FONT_NAME}", fontName);

                        Directory.CreateDirectory(Path.Combine(fullOutputDir, "lowercase"));
                        Directory.CreateDirectory(Path.Combine(fullOutputDir, "uppercase"));
                        Directory.CreateDirectory(Path.Combine(fullOutputDir, "digits"));
                        Directory.CreateDirectory(Path.Combine(fullOutputDir, "special"));

                        var specialChars = new Dictionary<char, string> {
                            { '!', "exclamation" }, { '"', "quote" }, { '#', "hash" }, { '$', "dollar" },
                            { '%', "percent" }, { '&', "ampersand" }, { '\'', "apostrophe" }, { '(', "lparen" },
                            { ')', "rparen" }, { '*', "asterisk" }, { '+', "plus" }, { ',', "comma" },
                            { '-', "dash" }, { '.', "dot" }, { '/', "slash" }, { ':', "colon" },
                            { ';', "semicolon" }, { '<', "lt" }, { '=', "equal" }, { '>', "gt" },
                            { '?', "question" }, { '@', "at" }, { '[', "lbracket" }, { '\\', "backslash" },
                            { ']', "rbracket" }, { '^', "caret" }, { '_', "underscore" }, { '`', "backtick" },
                            { '{', "lbrace" }, { '|', "pipe" }, { '}', "rbrace" }, { '~', "tilde" }
                        };

                        var mapLines = new List<string> {
                            "<FontMap xmlns=\"https://krykomdev.github.io/NeoKolors/Schemas/Fonts/v3/\">"
                        };

                        string GetXmlSymbol(char c) =>
                            c switch {
                                '&'  => "\\x26",
                                '<'  => "\\x3c",
                                '>'  => "\\x3e",
                                '"'  => "\\x22",
                                '\\' => "\\\\",
                                _    => c.ToString()
                            };

                        int totalCount = 26 + 26 + 10 + specialChars.Count;
                        int current    = 0;

                        for (char c = 'a'; c <= 'z'; c++) {
                            File.WriteAllText(Path.Combine(fullOutputDir, "lowercase", $"{c}.nkg"), ".", UTF8);
                            mapLines.Add($"    <Component Symbol=\"{c}\" File=\"lowercase/{c}.nkg\"/>");
                            current++;
                            pctx.Report(current / (double)totalCount, $"Generating lowercase/{c}.nkg...");
                        }

                        for (char c = 'A'; c <= 'Z'; c++) {
                            File.WriteAllText(Path.Combine(fullOutputDir, "uppercase", $"{c}.nkg"), ".", UTF8);
                            mapLines.Add($"    <Component Symbol=\"{c}\" File=\"uppercase/{c}.nkg\"/>");
                            current++;
                            pctx.Report(current / (double)totalCount, $"Generating uppercase/{c}.nkg...");
                        }

                        for (char c = '0'; c <= '9'; c++) {
                            File.WriteAllText(Path.Combine(fullOutputDir, "digits", $"{c}.nkg"), ".", UTF8);
                            mapLines.Add($"    <Component Symbol=\"{c}\" File=\"digits/{c}.nkg\"/>");
                            current++;
                            pctx.Report(current / (double)totalCount, $"Generating digits/{c}.nkg...");
                        }

                        foreach (var kvp in specialChars) {
                            char   symbol = kvp.Key;
                            string name   = kvp.Value;
                            File.WriteAllText(Path.Combine(fullOutputDir, "special", $"{name}.nkg"), ".", UTF8);
                            mapLines.Add($"    <Component Symbol=\"{GetXmlSymbol(symbol)}\" File=\"special/{name}.nkg\"/>");
                            current++;
                            pctx.Report(current / (double)totalCount, $"Generating special/{name}.nkg...");
                        }

                        mapLines.Add("</FontMap>");
                        string mapContent = string.Join(Environment.NewLine, mapLines);

                        File.WriteAllText(Path.Combine(fullOutputDir, "manifest.nkfont"), manifestContent, UTF8);
                        File.WriteAllText(Path.Combine(fullOutputDir, "Config.xml"),      configContent,   UTF8);
                        File.WriteAllText(Path.Combine(fullOutputDir, "Map.xml"),         mapContent,      UTF8);
                    }
                );

                PrintSuccess($"Successfully created full ASCII XML font definition '{fontName}' at '{fullOutputDir}'!");
                PrintSuccess("You can compile it using: nkfont compile <path-to-folder> <output-file.nkf>");
            }
            else {
                NKWidgets.Status(
                    $"Creating blank XML font definition for '{fontName}'...",
                    ctx => {
                        Directory.CreateDirectory(fullOutputDir);

                        string manifestContent = ReadTemplate("manifest.nkfont");
                        string configContent   = ReadTemplate("Config.xml").Replace("{FONT_NAME}", fontName);

                        Directory.CreateDirectory(Path.Combine(fullOutputDir, "lowercase"));
                        string mapContent  = ReadTemplate("Map.xml");
                        string aNkgContent = ReadTemplate("a.nkg");
                        File.WriteAllText(Path.Combine(fullOutputDir, "lowercase", "a.nkg"), aNkgContent, UTF8);

                        File.WriteAllText(Path.Combine(fullOutputDir, "manifest.nkfont"), manifestContent, UTF8);
                        File.WriteAllText(Path.Combine(fullOutputDir, "Config.xml"),      configContent,   UTF8);
                        File.WriteAllText(Path.Combine(fullOutputDir, "Map.xml"),         mapContent,      UTF8);

                        ctx.Success($"Successfully created blank XML font definition '{fontName}' at '{fullOutputDir}'!");
                    }
                );

                PrintSuccess("You can compile it using: nkfont compile <path-to-folder> <output-file.nkf>");
            }
        }
        catch (Exception ex) {
            PrintError($"Failed to create template: {ex.Message}");
        }
    }

    static string ReadTemplate(string name) {
        var       assembly     = System.Reflection.Assembly.GetExecutingAssembly();
        string    resourceName = $"NeoKolors.Tools.NKFont.Templates.{name}";
        using var stream       = assembly.GetManifestResourceStream(resourceName);

        if (stream == null) {
            throw new FileNotFoundException($"Embedded template '{name}' not found. Resource attempted: {resourceName}");
        }

        using var reader = new StreamReader(stream, UTF8);

        return reader.ReadToEnd();
    }

    static void HandleList() {
        System.Console.WriteLine();
        NKConsole.WriteLine("Available Fonts:", new NKStyle(ConsoleColor.Yellow, styles: NKTextStyles.BOLD));
        NKConsole.WriteLine("────────────────", ConsoleColor.DarkYellow);
        System.Console.WriteLine();

        var fontRows = new List<(string Name, string Author, string License, string Type, string Source)>();

        // Collect built-in fonts
        foreach (var value in FontAtlas.Values) {
            string author  = "-";
            string license = "-";

            if (value is Tui.Fonts.NKFont nkFont) {
                author  = nkFont.Info.Author      ?? "-";
                license = nkFont.Info.LicenseType ?? "-";
            }

            fontRows.Add((value.Name, author, license, value.GetType().Name, "Built-in"));
        }

        // Collect stored fonts
        if (Directory.Exists(FONTS_STORAGE_DIR)) {
            var files = Directory.EnumerateFiles(FONTS_STORAGE_DIR)
                .Where(f => f.EndsWith(".flf", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".nkf", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var file in files) {
                string name    = Path.GetFileNameWithoutExtension(file);
                string ext     = Path.GetExtension(file).ToUpperInvariant();
                string author  = "-";
                string license = "-";
                var    font    = LoadFromFile(file);

                if (font is Tui.Fonts.NKFont nkFont) {
                    author  = nkFont.Info.Author      ?? "-";
                    license = nkFont.Info.LicenseType ?? "-";
                }

                fontRows.Add((name, author, license, ext, "Stored"));
            }
        }

        if (fontRows.Count == 0) {
            NKConsole.WriteLine("  No fonts available.", ConsoleColor.DarkGray);
            System.Console.WriteLine();

            return;
        }

        var headers = new IElement[] {
            new Text(new AnsiString("FONT NAME",   new NKStyle(ConsoleColor.Cyan, styles: NKTextStyles.BOLD))),
            new Text(new AnsiString("AUTHOR",      new NKStyle(ConsoleColor.Cyan, styles: NKTextStyles.BOLD))),
            new Text(new AnsiString("LICENSE",     new NKStyle(ConsoleColor.Cyan, styles: NKTextStyles.BOLD))),
            new Text(new AnsiString("TYPE/FORMAT", new NKStyle(ConsoleColor.Cyan, styles: NKTextStyles.BOLD))),
            new Text(new AnsiString("SOURCE",      new NKStyle(ConsoleColor.Cyan, styles: NKTextStyles.BOLD)))
        };

        var sorted = fontRows.OrderBy(x => x.Source).ThenBy(x => x.Name).ToList();
        var rows   = new IReadOnlyList<IElement>[sorted.Count];

        for (int i = 0; i < sorted.Count; i++) {
            var r = sorted[i];

            rows[i] = [
                new Text(new AnsiString(r.Name,    new NKStyle(ConsoleColor.Yellow, styles: NKTextStyles.BOLD))),
                new Text(new AnsiString(r.Author,  new NKStyle(ConsoleColor.White))),
                new Text(new AnsiString(r.License, new NKStyle(ConsoleColor.Green))),
                new Text(new AnsiString(r.Type,    new NKStyle(ConsoleColor.Cyan))),
                new Text(new AnsiString(r.Source,  new NKStyle(r.Source == "Built-in" ? ConsoleColor.Magenta : ConsoleColor.Blue)))
            ];
        }

        var table = new Table(headers, rows, NKTableStyle.GetMinimal());
        NKWidgets.Render(table);
        System.Console.WriteLine();
    }

    static void HandleInspectFont(string fontKey) {
        var font = LoadFont(fontKey);

        if (font == null) {
            PrintError($"Could not load font '{fontKey}'.");

            return;
        }

        if (font is not Tui.Fonts.NKFont nkFont) {
            PrintError($"Font '{fontKey}' of flags '{font.GetType().Name}' does not support glyph listing.");

            return;
        }

        System.Console.WriteLine();

        // Print Font Details Header
        NKConsole.WriteLine("Font Details:", new NKStyle(ConsoleColor.Yellow, styles: NKTextStyles.BOLD));
        NKConsole.WriteLine("─────────────", ConsoleColor.DarkYellow);
        System.Console.WriteLine();

        var detailsRows = new List<IReadOnlyList<IElement>> {
            new IElement[] { new Text(new AnsiString("Name", new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(nkFont.Name, new NKStyle(ConsoleColor.White, styles: NKTextStyles.BOLD))) }
        };

        if (!string.IsNullOrEmpty(nkFont.Info.Author)) {
            detailsRows.Add([new Text(new AnsiString("Author", new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(nkFont.Info.Author, new NKStyle(ConsoleColor.White)))]);
        }

        if (!string.IsNullOrEmpty(nkFont.Info.LicenseType)) {
            detailsRows.Add([new Text(new AnsiString("License Flags", new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(nkFont.Info.LicenseType, new NKStyle(ConsoleColor.White)))]);
        }

        if (!string.IsNullOrEmpty(nkFont.Info.LicenseFile)) {
            detailsRows.Add([new Text(new AnsiString("License File", new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(nkFont.Info.LicenseFile, new NKStyle(ConsoleColor.White)))]);
        }

        detailsRows.Add([new Text(new AnsiString("Proportions",    new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(nkFont.Info.ProportionType.ToString(), new NKStyle(ConsoleColor.White)))]);
        detailsRows.Add([new Text(new AnsiString("Ligatures",      new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(nkFont.Info.Ligatures ? "Yes" : "No",  new NKStyle(ConsoleColor.White)))]);
        detailsRows.Add([new Text(new AnsiString("Leading",        new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(nkFont.Info.Leading.ToString(),        new NKStyle(ConsoleColor.White)))]);
        detailsRows.Add([new Text(new AnsiString("Letter Spacing", new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(nkFont.Info.LetterSpacing.ToString(),  new NKStyle(ConsoleColor.White)))]);
        detailsRows.Add([new Text(new AnsiString("Word Spacing",   new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(nkFont.Info.WordSpacing.ToString(),    new NKStyle(ConsoleColor.White)))]);
        detailsRows.Add([new Text(new AnsiString("Total Glyphs",   new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(nkFont.Glyphs.Count.ToString(),        new NKStyle(ConsoleColor.Green, styles: NKTextStyles.BOLD)))]);

        var detailsTable = new Table(null, detailsRows, NKTableStyle.GetNormal(features: NKTableStyle.Features.VERTICAL_ELEMENT_SEPARATOR));
        NKWidgets.Render(detailsTable);
        System.Console.WriteLine();

        // Print Table Header
        NKConsole.WriteLine("Glyphs Defined in Font:", new NKStyle(ConsoleColor.Yellow, styles: NKTextStyles.BOLD));
        NKConsole.WriteLine("───────────────────────", ConsoleColor.DarkYellow);
        System.Console.WriteLine();

        var glyphHeaders = new IElement[] {
            new Text(new AnsiString("SYMBOL",   new NKStyle(ConsoleColor.Cyan, styles: NKTextStyles.BOLD))),
            new Text(new AnsiString("TYPE",     new NKStyle(ConsoleColor.Cyan, styles: NKTextStyles.BOLD))),
            new Text(new AnsiString("SIZE",     new NKStyle(ConsoleColor.Cyan, styles: NKTextStyles.BOLD))),
            new Text(new AnsiString("BASELINE", new NKStyle(ConsoleColor.Cyan, styles: NKTextStyles.BOLD))),
            new Text(new AnsiString("STYLES",   new NKStyle(ConsoleColor.Cyan, styles: NKTextStyles.BOLD)))
        };

        var sortedGlyphs = nkFont.Glyphs
            .OrderBy(g => g.Key.IsLigature)
            .ThenBy(g => g.Key.IsSimple ? ((int)g.Key.SimpleSymbol).ToString("D5") : g.Key.LigatureSymbol)
            .ToList();

        var glyphRows = new IReadOnlyList<IElement>[sortedGlyphs.Count];

        for (int i = 0; i < sortedGlyphs.Count; i++) {
            var (symbol, glyph) = sortedGlyphs[i];
            string symStr    = symbol.IsSimple ? FormatSymbol(symbol.SimpleSymbol) : FormatLigature(symbol.LigatureSymbol);
            string typeStr   = symbol.IsSimple ? "Simple" : "Ligature";
            string sizeStr   = $"{glyph.Width}x{glyph.Height}";
            string baseStr   = glyph.BaselineOffset.ToString();
            string stylesStr = symbol.Styles.Styles == NKTextStyles.NONE ? "None" : symbol.Styles.Styles.ToString();

            glyphRows[i] = [
                new Text(new AnsiString(symStr,    new NKStyle(ConsoleColor.Yellow, styles: NKTextStyles.BOLD))),
                new Text(new AnsiString(typeStr,   new NKStyle(symbol.IsSimple ? ConsoleColor.White : ConsoleColor.Cyan))),
                new Text(new AnsiString(sizeStr,   new NKStyle(ConsoleColor.Green))),
                new Text(new AnsiString(baseStr,   new NKStyle(ConsoleColor.Magenta))),
                new Text(new AnsiString(stylesStr, new NKStyle(symbol.Styles.Styles == NKTextStyles.NONE ? ConsoleColor.DarkGray : ConsoleColor.Blue)))
            ];
        }

        var glyphsTable = new Table(
            glyphHeaders,
            glyphRows,
            NKTableStyle.GetMinimal()
        );

        NKWidgets.Render(glyphsTable);
        System.Console.WriteLine();
    }

    static void HandleInspectGlyph(string fontKey, string glyphInput) {
        var font = LoadFont(fontKey);

        if (font == null) {
            PrintError($"Could not load font '{fontKey}'.");

            return;
        }

        if (font is not Tui.Fonts.NKFont nkFont) {
            PrintError($"Font '{fontKey}' of type '{font.GetType().Name}' does not support glyph inspection.");

            return;
        }

        System.Console.WriteLine();

        // Search for the glyph in the font
        NKGlyphSymbol? matchedSymbol = null;
        NKGlyph?       matchedGlyph  = null;

        char   searchChar      = '\0';
        bool   isSimpleSearch  = false;
        string normalizedInput = glyphInput.Trim();

        if (normalizedInput.Equals("[space]", StringComparison.OrdinalIgnoreCase) || normalizedInput.Equals("space", StringComparison.OrdinalIgnoreCase)) {
            searchChar     = ' ';
            isSimpleSearch = true;
        }
        else if (normalizedInput.Equals("[tab]", StringComparison.OrdinalIgnoreCase) ||
            normalizedInput.Equals("tab",        StringComparison.OrdinalIgnoreCase)) {
            searchChar     = '\t';
            isSimpleSearch = true;
        }
        else if (normalizedInput.Equals("[newline]", StringComparison.OrdinalIgnoreCase) ||
            normalizedInput.Equals("newline",        StringComparison.OrdinalIgnoreCase)) {
            searchChar     = '\n';
            isSimpleSearch = true;
        }
        else if (normalizedInput.Equals("[return]", StringComparison.OrdinalIgnoreCase) ||
            normalizedInput.Equals("return",        StringComparison.OrdinalIgnoreCase) ||
            normalizedInput.Equals("cr",            StringComparison.OrdinalIgnoreCase)) {
            searchChar     = '\r';
            isSimpleSearch = true;
        }
        else if (glyphInput.Length == 1) {
            searchChar     = glyphInput[0];
            isSimpleSearch = true;
        }

        foreach (var (sym, value) in nkFont.Glyphs) {
            if ((!isSimpleSearch || !sym.IsSimple || sym.SimpleSymbol != searchChar) &&
                (!sym.IsLigature || !sym.LigatureSymbol.Equals(glyphInput, StringComparison.Ordinal))) {
                continue;
            }

            matchedSymbol = sym;
            matchedGlyph  = value;

            break;
        }

        // If not found yet and not exactly matched, try case-insensitive ligature search
        if (matchedGlyph == null) {
            foreach (var kvp in nkFont.Glyphs) {
                var sym = kvp.Key;

                if (!sym.IsLigature || !sym.LigatureSymbol.Equals(glyphInput, StringComparison.OrdinalIgnoreCase))
                    continue;

                matchedSymbol = sym;
                matchedGlyph  = kvp.Value;

                break;
            }
        }

        if (matchedSymbol == null || matchedGlyph == null) {
            PrintError($"Glyph '{glyphInput}' not found in font '{font.Name}'.");

            return;
        }

        // Print Glyph Metadata
        NKConsole.WriteLine("Glyph Details:", new NKStyle(ConsoleColor.Yellow, styles: NKTextStyles.BOLD));
        NKConsole.WriteLine("──────────────", ConsoleColor.DarkYellow);
        System.Console.WriteLine();

        string symStr = matchedSymbol.Value.IsSimple
            ? FormatSymbol(matchedSymbol.Value.SimpleSymbol)
            : FormatLigature(matchedSymbol.Value.LigatureSymbol);

        string stylesStr = matchedSymbol.Value.Styles.Styles == NKTextStyles.NONE
            ? "None"
            : matchedSymbol.Value.Styles.Styles.ToString();

        string alignPointsStr = matchedGlyph.AlignPoints.Any()
            ? string.Join(", ", matchedGlyph.AlignPoints.Select(p => $"'{p.Character}' at {p.Position.X},{p.Position.Y}"))
            : "None";

        var metaRows = new List<IReadOnlyList<IElement>> {
            new IElement[] { new Text(new AnsiString("Symbol",          new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(symStr,                                               new NKStyle(ConsoleColor.Yellow, styles: NKTextStyles.BOLD))) },
            new IElement[] { new Text(new AnsiString("Type",            new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(matchedSymbol.Value.IsSimple ? "Simple" : "Ligature", new NKStyle(ConsoleColor.White))) },
            new IElement[] { new Text(new AnsiString("Dimensions",      new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString($"{matchedGlyph.Width}x{matchedGlyph.Height}",        new NKStyle(ConsoleColor.Green))) },
            new IElement[] { new Text(new AnsiString("Baseline Offset", new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(matchedGlyph.BaselineOffset.ToString(),               new NKStyle(ConsoleColor.Magenta))) },
            new IElement[] { new Text(new AnsiString("Styles",          new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(stylesStr,                                            new NKStyle(matchedSymbol.Value.Styles.Styles == NKTextStyles.NONE ? ConsoleColor.DarkGray : ConsoleColor.Blue))) },
            new IElement[] { new Text(new AnsiString("Align Points",    new NKStyle(ConsoleColor.Cyan))), new Text(new AnsiString(alignPointsStr,                                       new NKStyle(matchedGlyph.AlignPoints.Any() ? ConsoleColor.White : ConsoleColor.DarkGray))) }
        };

        var metaTable = new Table(null, metaRows, NKTableStyle.GetBorderless());
        NKWidgets.Render(metaTable);
        System.Console.WriteLine();

        // Visual Representation
        NKConsole.WriteLine("Visual Representation:", new NKStyle(ConsoleColor.Yellow, styles: NKTextStyles.BOLD));
        NKConsole.WriteLine("──────────────────────", ConsoleColor.DarkYellow);
        System.Console.WriteLine();

        int baselineY   = matchedGlyph.Height + matchedGlyph.BaselineOffset - 1;
        var rowElements = new List<IElement>();

        for (int y = 0; y < matchedGlyph.Height; y++) {
            var cells = new List<AnsiChar>();

            for (int x = 0; x < matchedGlyph.Width; x++) {
                var cell = matchedGlyph.Glyph[x, y];

                switch (cell.Type) {
                    case GlyphCellType.BACKGROUND: cells.Add(new AnsiChar('·',            new NKStyle(ConsoleColor.DarkGray))); break;
                    case GlyphCellType.FOREGROUND: cells.Add(new AnsiChar('·',            new NKStyle(ConsoleColor.Blue))); break;
                    case GlyphCellType.CHARACTER:  cells.Add(new AnsiChar(cell.Character, new NKStyle(ConsoleColor.Yellow, styles: NKTextStyles.BOLD))); break;
                }
            }

            if (y == baselineY) {
                cells.Add(new AnsiChar(' ', NKStyle.Default));
                cells.Add(new AnsiChar(' ', NKStyle.Default));

                foreach (char ch in "<-- Baseline") {
                    cells.Add(new AnsiChar(ch, new NKStyle(ConsoleColor.Magenta, styles: NKTextStyles.BOLD)));
                }
            }

            rowElements.Add(new Text(new AnsiString(cells)));
        }

        // var visualBorder = new Border(
        //     element: new Flex(direction: FlexDirection.COLUMN, children: rowElements.ToArray()),
        //     style: NKBorderStyle.GetRounded(),
        //     padding: new Spacing(horizontal: Dimension.Chars(1), vertical: Dimension.Chars(0))
        // );

        NKWidgets.Render(new Flex(direction: FlexDirection.COLUMN, children: rowElements.ToArray()));

        // Baseline notes if outside grid
        if (baselineY >= matchedGlyph.Height) {
            int rowsBelow = baselineY - matchedGlyph.Height + 1;
            NKConsole.Write("  Note: ", ConsoleColor.Cyan);
            NKConsole.WriteLine($"Baseline is {rowsBelow} row(s) below the grid.", ConsoleColor.DarkGray);
        }
        else if (baselineY < 0) {
            int rowsAbove = -baselineY;
            NKConsole.Write("  Note: ", ConsoleColor.Cyan);
            NKConsole.WriteLine($"Baseline is {rowsAbove} row(s) above the grid.", ConsoleColor.DarkGray);
        }

        System.Console.WriteLine();
    }

    static string FormatSymbol(char c) {
        return c switch {
            ' '  => "[Space]",
            '\t' => "[Tab]",
            '\n' => "[Newline]",
            '\r' => "[Carriage Return]",
            _    => $"'{c}'"
        };
    }

    static string FormatLigature(string s) {
        return $"\"{s}\"";
    }

    static IAsciiFont? LoadFont(string fontKey) {
        if (FontAtlas.TryGet(fontKey, out var asciiFont)) {
            return asciiFont;
        }

        // 2. Try loading from internal storage
        string internalPath = Path.Combine(FONTS_STORAGE_DIR, fontKey);

        if (File.Exists(internalPath)) {
            var font = LoadFromFile(internalPath);

            if (font != null)
                return font;
        }
        else {
            string[] possibleExtensions = { ".nkf", ".flf" };

            foreach (var ext in possibleExtensions) {
                string pathWithExt = Path.Combine(FONTS_STORAGE_DIR, fontKey + ext);

                if (File.Exists(pathWithExt)) {
                    var font = LoadFromFile(pathWithExt);

                    if (font != null)
                        return font;
                }
            }
        }

        // 3. Try loading from binary file path
        if (File.Exists(fontKey)) {
            var font = LoadFromFile(Path.GetFullPath(fontKey));

            if (font != null)
                return font;
        }

        // 4. Try loading as general XML path (directory or zip)
        if (Directory.Exists(fontKey) || File.Exists(fontKey)) {
            string fullPath = Path.GetFullPath(fontKey);

            try {
                var font = NKFontSerializer.DeserializeXml(fullPath);

                if (font != null)
                    return font;
            }
            catch {
                // Ignore and fall through
            }
        }

        return null;
    }

    static IAsciiFont? LoadFromFile(string fullPath) {
        try {
            using var fs = File.OpenRead(fullPath);

            if (fs.Length >= 5) {
                var buffer = new byte[5];
                int read   = fs.Read(buffer, 0, 5);

                if (read == 5) {
                    uint magic = (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));

                    if (magic == 0x4E4B4642) {
                        // BINARY_MAGIC
                        fs.Position = 0;

                        return NKFontSerializer.DeserializeBinary(fs);
                    }

                    if (buffer[0] == 'f' && buffer[1] == 'l' && buffer[2] == 'f' && buffer[3] == '2' && buffer[4] == 'a') {
                        // FIGLET
                        fs.Position = 0;

                        return NKFontSerializer.DeserializeFiglet(fs, Path.GetFileNameWithoutExtension(fullPath));
                    }
                }
            }
        }
        catch {
            // Ignore
        }

        return null;
    }

    static void HandleAdd(string fontPath) {
        string fullPath = Path.GetFullPath(fontPath);

        if (!File.Exists(fullPath)) {
            PrintError($"Font file '{fullPath}' does not exist.");

            return;
        }

        NKWidgets.Status(
            $"Validating font file '{Path.GetFileName(fullPath)}'...",
            ctx => {
                var font = LoadFromFile(fullPath);

                if (font == null) {
                    ctx.Fail("Failed to load font. The file is not a valid FIGlet (.flf) or NeoKolors binary (.nkf) font.");

                    return;
                }

                string destFileName = Path.GetFileName(fullPath);
                string destPath     = Path.Combine(FONTS_STORAGE_DIR, destFileName);

                ctx.Status = $"Importing font '{font.Name}' into internal storage...";

                try {
                    if (!Directory.Exists(FONTS_STORAGE_DIR)) {
                        Directory.CreateDirectory(FONTS_STORAGE_DIR);
                    }

                    File.Copy(fullPath, destPath, overwrite: true);
                    ctx.Success($"Successfully added font '{font.Name}' to internal storage at '{destPath}'!");
                }
                catch (Exception ex) {
                    ctx.Fail($"Failed to save font to internal storage: {ex.Message}");
                }
            }
        );
    }

    static void HandleRemove(string fontName) {
        if (string.IsNullOrWhiteSpace(fontName)) {
            PrintError("Font name cannot be empty.");

            return;
        }

        string[] candidates = {
            fontName,
            fontName + ".nkf",
            fontName + ".flf"
        };

        bool deleted = false;

        foreach (var c in candidates) {
            string path = Path.Combine(FONTS_STORAGE_DIR, c);

            if (File.Exists(path)) {
                try {
                    File.Delete(path);
                    PrintSuccess($"Successfully removed '{c}' from internal storage.");
                    deleted = true;
                }
                catch (Exception ex) {
                    PrintError($"Failed to delete '{c}': {ex.Message}");

                    return;
                }
            }
        }

        if (!deleted) {
            PrintError($"Font '{fontName}' was not found in internal storage.");
        }
    }

    static void HandleCleanLogs() {
        var installDir = AppContext.BaseDirectory;
        var logsDir    = Path.Combine(installDir, "logs");

        if (!Directory.Exists(logsDir)) {
            PrintSuccess("No log directory found. Nothing to clean.");

            return;
        }

        NKWidgets.Status(
            "Cleaning application log files...",
            ctx => {
                try {
                    var files        = Directory.GetFiles(logsDir, "*.log");
                    int deletedCount = 0;
                    int failedCount  = 0;

                    foreach (var file in files) {
                        try {
                            File.Delete(file);
                            deletedCount++;
                        }
                        catch (IOException) {
                            // Active log file might be locked by current process
                            failedCount++;
                        }
                        catch (Exception) {
                            failedCount++;
                        }
                    }

                    if (deletedCount > 0) {
                        ctx.Success($"Successfully cleaned {deletedCount} log file(s)." + (failedCount > 0 ? $" ({failedCount} active files preserved)" : ""));
                    }
                    else if (failedCount > 0) {
                        ctx.Success($"{failedCount} active log file(s) are currently in use by the running tool and were preserved.");
                    }
                    else {
                        ctx.Success("No log files to clean.");
                    }
                }
                catch (Exception ex) {
                    ctx.Fail($"Failed to clean logs: {ex.Message}");
                }
            }
        );
    }

    static void PrintError(string msg) {
        NKConsole.Write("[ERROR] ", new NKStyle(ConsoleColor.Red, styles: NKTextStyles.BOLD));
        NKConsole.WriteLine(msg, ConsoleColor.Red);
    }

    static void PrintSuccess(string msg) {
        NKConsole.Write("[SUCCESS] ", new NKStyle(ConsoleColor.Green, styles: NKTextStyles.BOLD));
        NKConsole.WriteLine(msg, ConsoleColor.Green);
    }

    static void PrintInfo(string msg) {
        NKConsole.Write("[INFO] ", new NKStyle(ConsoleColor.Cyan, styles: NKTextStyles.BOLD));
        NKConsole.WriteLine(msg, ConsoleColor.Cyan);
    }

    static void PrintWarning(string msg) {
        NKConsole.Write("[WARNING] ", new NKStyle(ConsoleColor.Yellow, styles: NKTextStyles.BOLD));
        NKConsole.WriteLine(msg, ConsoleColor.Yellow);
    }
}