// NeoKolors
// Copyright (c) 2026 KryKom

using System.CommandLine;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Extensions;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Fonts.Serialization;

using NeoKolors.Tui.Fonts.Assets;

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

        NKDebug.Logger.FileConfig = LogFileConfig.NewDatetime($"{logsDir}/{{0}}.log");

        NKDebug.Logger.SimpleMessages = true;
    }
    
    static int Main(string[] args) {
        
        // Support UTF-8 encoding for smooth unicode characters
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;

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

        compileCommand.SetAction(parseResult => {
            string xmlPath = parseResult.GetValue(xmlPathArg) ?? string.Empty;
            string? outputNkf = parseResult.GetValue(outputNkfArg);
            HandleCompile(xmlPath, outputNkf);
        });

        // 2. display command
        var fontArg = new Argument<string>("font") {
            Description = "A built-in font name, compiled .nkf file, or raw XML font folder/zip."
        };
        
        var textArg = new Argument<string[]>("text") {
            Description = "The text to display."
        };
        
        var displayCommand = new Command("display", "Display/render the given string using a specified font.");
        displayCommand.Arguments.Add(fontArg);
        displayCommand.Arguments.Add(textArg);

        displayCommand.Aliases.Add("show");
        displayCommand.Aliases.Add("render");

        displayCommand.SetAction(parseResult => {
            string font = parseResult.GetValue(fontArg) ?? string.Empty;
            string[] textParts = parseResult.GetValue(textArg) ?? Array.Empty<string>();
            string text = string.Join(" ", textParts);
            HandleDisplay(font, text);
        });

        // 3. create command
        var outputDirArg = new Argument<string>("output-dir") {
            Description = "The output directory where the new XML definition will be created."
        };

        var fontNameArg = new Argument<string>("font-name") {
            Description = "The internal name of the font. If omitted, defaults to the name of the folder.",
            Arity = ArgumentArity.ZeroOrOne
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

        createCommand.SetAction(parseResult => {
            string outputDir = parseResult.GetValue(outputDirArg) ?? string.Empty;
            string? fontName = parseResult.GetValue(fontNameArg);
            bool allAscii = parseResult.GetValue(allAsciiOption);
            HandleCreate(outputDir, fontName, allAscii);
        });

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
        
        glyphsCommand.SetAction(parseResult => {
            string font = parseResult.GetValue(glyphsFontArg) ?? string.Empty;
            HandleGlyphs(font);
        });

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
        
        inspectCommand.SetAction(parseResult => {
            string font = parseResult.GetValue(inspectFontArg) ?? string.Empty;
            string glyph = parseResult.GetValue(inspectGlyphArg) ?? string.Empty;
            HandleInspect(font, glyph);
        });

        // 7. add command
        var addPathArg = new Argument<string>("font-path") {
            Description = "The path to the .flf or .nkf font file to add."
        };
        var addCommand = new Command("add", "Add a font file to the tools internal storage.");
        addCommand.Arguments.Add(addPathArg);
        addCommand.Aliases.Add("import");
        addCommand.Aliases.Add("install");
        addCommand.SetAction(parseResult => {
            string fontPath = parseResult.GetValue(addPathArg) ?? string.Empty;
            HandleAdd(fontPath);
        });

        // 8. remove command
        var removeNameArg = new Argument<string>("font-name") {
            Description = "The name of the font to remove from internal storage."
        };
        var removeCommand = new Command("remove", "Remove a font from the tools internal storage.");
        removeCommand.Arguments.Add(removeNameArg);
        removeCommand.Aliases.Add("delete");
        removeCommand.Aliases.Add("uninstall");
        removeCommand.SetAction(parseResult => {
            string fontName = parseResult.GetValue(removeNameArg) ?? string.Empty;
            HandleRemove(fontName);
        });

        // 9. clean-logs command
        var cleanLogsCommand = new Command("clean-logs", "Clean all log files generated by the nkfont tool.");
        cleanLogsCommand.Aliases.Add("clear-logs");
        cleanLogsCommand.Aliases.Add("flush-logs");
        cleanLogsCommand.SetAction(_ => {
            HandleCleanLogs();
        });

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

        PrintInfo($"Reading and parsing XML font definition from '{fullXmlPath}'...");
        Tui.Fonts.NKFont? font;

        try {
            font = NKFontSerializer.DeserializeXml(fullXmlPath);
        }
        catch (Exception ex) {
            PrintError($"Failed to deserialize XML font: {ex}");

            return;
        }

        if (font == null) {
            PrintError("Failed to load/compile XML font definition. Please check XML validation logs.");

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

        PrintInfo($"Serializing compiled font '{font.Name}' to '{outputPath}'...");

        try {
            NKFontSerializer.SerializeBinary(font, outputPath);
            PrintSuccess($"Successfully compiled font '{font.Name}' to '{outputPath}'!");
        }
        catch (Exception ex) {
            PrintError($"Failed to save compiled font: {ex.Message}");
        }
    }

    static void HandleDisplay(string fontKey, string text) {
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

        var size = font.GetSize(ansiString, maxWidth: NKConsole.WindowSize.X);

        if (size.X == 0 || size.Y == 0) {
            NKConsole.WriteLine("Warning: Rendered text has zero width or height.", ConsoleColor.Yellow);

            return;
        }

        var canvas = new NKCharCanvas(size.X, size.Y);
        font.PlaceString(ansiString, canvas, maxWidth: NKConsole.WindowSize.X);

        NKConsole.WriteLine();
        NKConsole.WriteLine($"Text size: {size}", ConsoleColor.Yellow);
        NKConsole.WriteLine();
        
        // Render cell by cell to console using NeoKolors styles
        for (int y = 0; y < canvas.Height; y++) {
            for (int x = 0; x < canvas.Width; x++) {
                var cell = canvas[x, y];
                NKConsole.Write(cell.Char ?? ' ', cell.Style);
            }

            System.Console.WriteLine();
        }
        
        NKConsole.WriteLine();
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

        PrintInfo($"Creating blank XML nkfont definition at '{fullOutputDir}' for font '{fontName}'...");

        try {
            Directory.CreateDirectory(fullOutputDir);

            string manifestContent = ReadTemplate("manifest.nkfont");
            string configContent = ReadTemplate("Config.xml").Replace("{FONT_NAME}", fontName);
            string mapContent;

            if (allAscii) {
                // Directories
                Directory.CreateDirectory(Path.Combine(fullOutputDir, "lowercase"));
                Directory.CreateDirectory(Path.Combine(fullOutputDir, "uppercase"));
                Directory.CreateDirectory(Path.Combine(fullOutputDir, "digits"));
                Directory.CreateDirectory(Path.Combine(fullOutputDir, "special"));

                // Special characters mapping dictionary
                var specialChars = new Dictionary<char, string> {
                    { '!', "exclamation" },
                    { '"', "quote" },
                    { '#', "hash" },
                    { '$', "dollar" },
                    { '%', "percent" },
                    { '&', "ampersand" },
                    { '\'', "apostrophe" },
                    { '(', "lparen" },
                    { ')', "rparen" },
                    { '*', "asterisk" },
                    { '+', "plus" },
                    { ',', "comma" },
                    { '-', "dash" },
                    { '.', "dot" },
                    { '/', "slash" },
                    { ':', "colon" },
                    { ';', "semicolon" },
                    { '<', "lt" },
                    { '=', "equal" },
                    { '>', "gt" },
                    { '?', "question" },
                    { '@', "at" },
                    { '[', "lbracket" },
                    { '\\', "backslash" },
                    { ']', "rbracket" },
                    { '^', "caret" },
                    { '_', "underscore" },
                    { '`', "backtick" },
                    { '{', "lbrace" },
                    { '|', "pipe" },
                    { '}', "rbrace" },
                    { '~', "tilde" }
                };

                var mapLines = new List<string> {
                    "<FontMap xmlns=\"https://krykomdev.github.io/NeoKolors/Schemas/Fonts/v3/\">"
                };

                // Helper to format XML Symbol
                string GetXmlSymbol(char c) {
                    return c switch {
                        '&' => "\\x26",
                        '<' => "\\x3c",
                        '>' => "\\x3e",
                        '"' => "\\x22",
                        '\\' => "\\\\",
                        _ => c.ToString()
                    };
                }

                // Lowercase letters (a-z)
                for (char c = 'a'; c <= 'z'; c++) {
                    File.WriteAllText(Path.Combine(fullOutputDir, "lowercase", $"{c}.nkg"), ".", System.Text.Encoding.UTF8);
                    mapLines.Add($"    <Component Symbol=\"{c}\" File=\"lowercase/{c}.nkg\"/>");
                }

                // Uppercase letters (A-Z)
                for (char c = 'A'; c <= 'Z'; c++) {
                    File.WriteAllText(Path.Combine(fullOutputDir, "uppercase", $"{c}.nkg"), ".", System.Text.Encoding.UTF8);
                    mapLines.Add($"    <Component Symbol=\"{c}\" File=\"uppercase/{c}.nkg\"/>");
                }

                // Digits (0-9)
                for (char c = '0'; c <= '9'; c++) {
                    File.WriteAllText(Path.Combine(fullOutputDir, "digits", $"{c}.nkg"), ".", System.Text.Encoding.UTF8);
                    mapLines.Add($"    <Component Symbol=\"{c}\" File=\"digits/{c}.nkg\"/>");
                }

                // Special characters
                foreach (var kvp in specialChars) {
                    char symbol = kvp.Key;
                    string name = kvp.Value;
                    File.WriteAllText(Path.Combine(fullOutputDir, "special", $"{name}.nkg"), ".", System.Text.Encoding.UTF8);
                    mapLines.Add($"    <Component Symbol=\"{GetXmlSymbol(symbol)}\" File=\"special/{name}.nkg\"/>");
                }

                mapLines.Add("</FontMap>");
                mapContent = string.Join(Environment.NewLine, mapLines);
            }
            else {
                Directory.CreateDirectory(Path.Combine(fullOutputDir, "lowercase"));
                mapContent = ReadTemplate("Map.xml");
                string aNkgContent = ReadTemplate("a.nkg");
                File.WriteAllText(Path.Combine(fullOutputDir, "lowercase", "a.nkg"), aNkgContent, System.Text.Encoding.UTF8);
            }

            // Write common files
            File.WriteAllText(Path.Combine(fullOutputDir, "manifest.nkfont"), manifestContent, System.Text.Encoding.UTF8);
            File.WriteAllText(Path.Combine(fullOutputDir, "Config.xml"), configContent, System.Text.Encoding.UTF8);
            File.WriteAllText(Path.Combine(fullOutputDir, "Map.xml"), mapContent, System.Text.Encoding.UTF8);

            PrintSuccess($"Successfully created blank XML font definition '{fontName}' at '{fullOutputDir}'!");
            PrintSuccess("You can compile it using: nkfont compile <path-to-folder> <output-file.nkf>");
        }
        catch (Exception ex) {
            PrintError($"Failed to create template: {ex.Message}");
        }
    }

    static string ReadTemplate(string name) {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        string resourceName = $"NeoKolors.Tools.NKFont.Templates.{name}";
        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null) {
            throw new FileNotFoundException($"Embedded template '{name}' not found. Resource attempted: {resourceName}");
        }

        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);

        return reader.ReadToEnd();
    }

    static void HandleList() {
        System.Console.WriteLine();
        NKConsole.WriteLine("Available Fonts:", new NKStyle(ConsoleColor.Yellow, s: TextStyles.BOLD));
        NKConsole.WriteLine("────────────────", ConsoleColor.DarkYellow);
        System.Console.WriteLine();

        var fontRows = new List<(string Name, string Author, string License, string Type, string Source)>();

        // Collect built-in fonts
        foreach (var value in FontAtlas.Values) {
            string author = "-";
            string license = "-";
            if (value is Tui.Fonts.NKFont nkFont) {
                author = nkFont.Info.Author ?? "-";
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
                string name = Path.GetFileNameWithoutExtension(file);
                string ext = Path.GetExtension(file).ToUpperInvariant();
                string author = "-";
                string license = "-";
                var font = LoadFromFile(file);
                if (font is Tui.Fonts.NKFont nkFont) {
                    author = nkFont.Info.Author ?? "-";
                    license = nkFont.Info.LicenseType ?? "-";
                }
                fontRows.Add((name, author, license, ext, "Stored"));
            }
        }

        // Dynamically calculate column widths (at least as wide as headers)
        int colName = Math.Max("FONT NAME".Length, fontRows.Count > 0 ? fontRows.Max(x => x.Name.Length) : 0) + 1;
        int colAuthor = Math.Max("AUTHOR".Length, fontRows.Count > 0 ? fontRows.Max(x => x.Author.Length) : 0) + 1;
        int colLicense = Math.Max("LICENSE".Length, fontRows.Count > 0 ? fontRows.Max(x => x.License.Length) : 0) + 1;
        int colType = Math.Max("TYPE/FORMAT".Length, fontRows.Count > 0 ? fontRows.Max(x => x.Type.Length) : 0) + 1;
        int colSource = Math.Max("SOURCE".Length, fontRows.Count > 0 ? fontRows.Max(x => x.Source.Length) : 0) + 1;

        // Print table headers
        NKConsole.Write("  " + "FONT NAME".PadRight(colName), new NKStyle(ConsoleColor.Cyan, s: TextStyles.BOLD));
        NKConsole.Write("│ " + "AUTHOR".PadRight(colAuthor), new NKStyle(ConsoleColor.Cyan, s: TextStyles.BOLD));
        NKConsole.Write("│ " + "LICENSE".PadRight(colLicense), new NKStyle(ConsoleColor.Cyan, s: TextStyles.BOLD));
        NKConsole.Write("│ " + "TYPE/FORMAT".PadRight(colType), new NKStyle(ConsoleColor.Cyan, s: TextStyles.BOLD));
        NKConsole.WriteLine("│ " + "SOURCE".PadRight(colSource), new NKStyle(ConsoleColor.Cyan, s: TextStyles.BOLD));

        // Separator row
        string sepLine = "  " + new string('─', colName) +
                         "┼─" + new string('─', colAuthor) +
                         "┼─" + new string('─', colLicense) +
                         "┼─" + new string('─', colType) +
                         "┼─" + new string('─', colSource);
        NKConsole.WriteLine(sepLine, ConsoleColor.DarkGray);

        // Render table rows
        foreach (var r in fontRows.OrderBy(x => x.Source).ThenBy(x => x.Name)) {
            NKConsole.Write("  " + r.Name.PadRight(colName), new NKStyle(ConsoleColor.Yellow, s: TextStyles.BOLD));
            NKConsole.Write("│ ", ConsoleColor.DarkGray);
            NKConsole.Write(r.Author.PadRight(colAuthor), ConsoleColor.White);
            NKConsole.Write("│ ", ConsoleColor.DarkGray);
            NKConsole.Write(r.License.PadRight(colLicense), ConsoleColor.Green);
            NKConsole.Write("│ ", ConsoleColor.DarkGray);
            NKConsole.Write(r.Type.PadRight(colType), ConsoleColor.Cyan);
            NKConsole.Write("│ ", ConsoleColor.DarkGray);
            NKConsole.WriteLine(r.Source.PadRight(colSource), r.Source == "Built-in" ? ConsoleColor.Magenta : ConsoleColor.Blue);
        }

        System.Console.WriteLine();
    }

    static void HandleGlyphs(string fontKey) {
        var font = LoadFont(fontKey);

        if (font == null) {
            PrintError($"Could not load font '{fontKey}'.");
            return;
        }

        if (font is not Tui.Fonts.NKFont nkFont) {
            PrintError($"Font '{fontKey}' of type '{font.GetType().Name}' does not support glyph listing.");
            return;
        }

        System.Console.WriteLine();
        
        // Print Font Details Header
        NKConsole.WriteLine("Font Details:", new NKStyle(ConsoleColor.Yellow, s: TextStyles.BOLD));
        NKConsole.WriteLine("─────────────", ConsoleColor.DarkYellow);
        
        NKConsole.Write("  Name:            ", ConsoleColor.Cyan);
        NKConsole.WriteLine(nkFont.Name, new NKStyle(ConsoleColor.White, s: TextStyles.BOLD));

        if (!string.IsNullOrEmpty(nkFont.Info.Author)) {
            NKConsole.Write("  Author:          ", ConsoleColor.Cyan);
            NKConsole.WriteLine(nkFont.Info.Author, ConsoleColor.White);
        }

        if (!string.IsNullOrEmpty(nkFont.Info.LicenseType)) {
            NKConsole.Write("  License Type:    ", ConsoleColor.Cyan);
            NKConsole.WriteLine(nkFont.Info.LicenseType, ConsoleColor.White);
        }

        if (!string.IsNullOrEmpty(nkFont.Info.LicenseFile)) {
            NKConsole.Write("  License File:    ", ConsoleColor.Cyan);
            NKConsole.WriteLine(nkFont.Info.LicenseFile, ConsoleColor.White);
        }
        
        NKConsole.Write("  Proportions:     ", ConsoleColor.Cyan);
        NKConsole.WriteLine(nkFont.Info.ProportionType.ToString(), ConsoleColor.White);
        
        NKConsole.Write("  Ligatures:       ", ConsoleColor.Cyan);
        NKConsole.WriteLine(nkFont.Info.Ligatures ? "Yes" : "No", ConsoleColor.White);
        
        NKConsole.Write("  Leading:         ", ConsoleColor.Cyan);
        NKConsole.WriteLine(nkFont.Info.Leading.ToString(), ConsoleColor.White);
        
        NKConsole.Write("  Letter Spacing:  ", ConsoleColor.Cyan);
        NKConsole.WriteLine(nkFont.Info.LetterSpacing.ToString(), ConsoleColor.White);
        
        NKConsole.Write("  Word Spacing:    ", ConsoleColor.Cyan);
        NKConsole.WriteLine(nkFont.Info.WordSpacing.ToString(), ConsoleColor.White);
        
        NKConsole.Write("  Total Glyphs:    ", ConsoleColor.Cyan);
        NKConsole.WriteLine(nkFont.Glyphs.Count.ToString(), new NKStyle(ConsoleColor.White, s: TextStyles.BOLD));
        System.Console.WriteLine();

        // Print Table Header
        NKConsole.WriteLine("Glyphs Defined in Font:", new NKStyle(ConsoleColor.Yellow, s: TextStyles.BOLD));
        NKConsole.WriteLine("───────────────────────", ConsoleColor.DarkYellow);
        System.Console.WriteLine();
        
        // Define Column Widths
        const int colSymbol = 14;
        const int colType = 12;
        const int colSize = 10;
        const int colBaseline = 12;

        // Print table header row
        NKConsole.Write("  " + "SYMBOL".PadRight(colSymbol), new NKStyle(ConsoleColor.Cyan, s: TextStyles.BOLD));
        NKConsole.Write("│ " + "TYPE".PadRight(colType), new NKStyle(ConsoleColor.Cyan, s: TextStyles.BOLD));
        NKConsole.Write("│ " + "SIZE".PadRight(colSize), new NKStyle(ConsoleColor.Cyan, s: TextStyles.BOLD));
        NKConsole.Write("│ " + "BASELINE".PadRight(colBaseline), new NKStyle(ConsoleColor.Cyan, s: TextStyles.BOLD));
        NKConsole.WriteLine("│ STYLES", new NKStyle(ConsoleColor.Cyan, s: TextStyles.BOLD));

        // Horizontal separator line
        string sepLine = "  " + new string('─', colSymbol) + "┼" + new string('─', colType + 1) + "┼" + new string('─', colSize + 1) + "┼" + new string('─', colBaseline + 1) + "┼" + new string('─', 20);
        NKConsole.WriteLine(sepLine, ConsoleColor.DarkGray);

        // Sort Glyphs: Simple first sorted by char, then Ligatures sorted alphabetically
        var sortedGlyphs = nkFont.Glyphs
            .OrderBy(g => g.Key.IsLigature)
            .ThenBy(g => g.Key.IsSimple ? ((int)g.Key.SimpleSymbol).ToString("D5") : g.Key.LigatureSymbol)
            .ToList();

        foreach (var (symbol, glyph) in sortedGlyphs) {
            string symStr = symbol.IsSimple ? FormatSymbol(symbol.SimpleSymbol) : FormatLigature(symbol.LigatureSymbol);
            string typeStr = symbol.IsSimple ? "Simple" : "Ligature";
            string sizeStr = $"{glyph.Width}x{glyph.Height}";
            string baseStr = glyph.BaselineOffset.ToString();
            string stylesStr = symbol.Styles.Styles == TextStyles.NONE ? "None" : symbol.Styles.Styles.ToString();

            // Print styled row
            NKConsole.Write("  " + symStr.PadRight(colSymbol), new NKStyle(ConsoleColor.Yellow, s: TextStyles.BOLD));
            
            NKConsole.Write("│ ", ConsoleColor.DarkGray);
            NKConsole.Write(typeStr.PadRight(colType), symbol.IsSimple ? ConsoleColor.White : ConsoleColor.Cyan);
            
            NKConsole.Write("│ ", ConsoleColor.DarkGray);
            NKConsole.Write(sizeStr.PadRight(colSize), ConsoleColor.Green);
            
            NKConsole.Write("│ ", ConsoleColor.DarkGray);
            NKConsole.Write(baseStr.PadRight(colBaseline), ConsoleColor.Magenta);
            
            NKConsole.Write("│ ", ConsoleColor.DarkGray);
            NKConsole.WriteLine(stylesStr, symbol.Styles.Styles == TextStyles.NONE ? ConsoleColor.DarkGray : ConsoleColor.Blue);
        }
        
        System.Console.WriteLine();
    }

    static void HandleInspect(string fontKey, string glyphInput) {
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
        NKGlyph? matchedGlyph = null;

        char searchChar = '\0';
        bool isSimpleSearch = false;
        string normalizedInput = glyphInput.Trim();

        if (normalizedInput.Equals("[space]", StringComparison.OrdinalIgnoreCase) || normalizedInput.Equals("space", StringComparison.OrdinalIgnoreCase)) {
            searchChar = ' ';
            isSimpleSearch = true;
        }
        else if (normalizedInput.Equals("[tab]", StringComparison.OrdinalIgnoreCase) || normalizedInput.Equals("tab", StringComparison.OrdinalIgnoreCase)) {
            searchChar = '\t';
            isSimpleSearch = true;
        }
        else if (normalizedInput.Equals("[newline]", StringComparison.OrdinalIgnoreCase) || normalizedInput.Equals("newline", StringComparison.OrdinalIgnoreCase)) {
            searchChar = '\n';
            isSimpleSearch = true;
        }
        else if (normalizedInput.Equals("[return]", StringComparison.OrdinalIgnoreCase) || 
                 normalizedInput.Equals("return", StringComparison.OrdinalIgnoreCase) || 
                 normalizedInput.Equals("cr", StringComparison.OrdinalIgnoreCase)) {
            searchChar = '\r';
            isSimpleSearch = true;
        }
        else if (glyphInput.Length == 1) {
            searchChar = glyphInput[0];
            isSimpleSearch = true;
        }

        foreach (var (sym, value) in nkFont.Glyphs) {
            if ((!isSimpleSearch || !sym.IsSimple || sym.SimpleSymbol != searchChar) &&
                (!sym.IsLigature || !sym.LigatureSymbol.Equals(glyphInput, StringComparison.Ordinal)))
            {
                continue;
            }

            matchedSymbol = sym;
            matchedGlyph = value;
            break;
        }

        // If not found yet and not exactly matched, try case-insensitive ligature search
        if (matchedGlyph == null) {
            foreach (var kvp in nkFont.Glyphs) {
                var sym = kvp.Key;

                if (!sym.IsLigature || !sym.LigatureSymbol.Equals(glyphInput, StringComparison.OrdinalIgnoreCase))
                    continue;

                matchedSymbol = sym;
                matchedGlyph = kvp.Value;
                break;
            }
        }

        if (matchedSymbol == null || matchedGlyph == null) {
            PrintError($"Glyph '{glyphInput}' not found in font '{font.Name}'.");
            return;
        }

        // Print Glyph Metadata
        NKConsole.WriteLine("Glyph Details:", new NKStyle(ConsoleColor.Yellow, s: TextStyles.BOLD));
        NKConsole.WriteLine("──────────────", ConsoleColor.DarkYellow);

        string symStr = matchedSymbol.Value.IsSimple 
            ? FormatSymbol(matchedSymbol.Value.SimpleSymbol) 
            : FormatLigature(matchedSymbol.Value.LigatureSymbol);

        NKConsole.Write("  Symbol:          ", ConsoleColor.Cyan);
        NKConsole.WriteLine(symStr, new NKStyle(ConsoleColor.Yellow, s: TextStyles.BOLD));

        NKConsole.Write("  Type:            ", ConsoleColor.Cyan);
        NKConsole.WriteLine(matchedSymbol.Value.IsSimple ? "Simple" : "Ligature", ConsoleColor.White);

        NKConsole.Write("  Dimensions:      ", ConsoleColor.Cyan);
        NKConsole.WriteLine($"{matchedGlyph.Width}x{matchedGlyph.Height}", ConsoleColor.Green);

        NKConsole.Write("  Baseline Offset: ", ConsoleColor.Cyan);
        NKConsole.WriteLine(matchedGlyph.BaselineOffset.ToString(), ConsoleColor.Magenta);

        NKConsole.Write("  Styles:          ", ConsoleColor.Cyan);
        string stylesStr = matchedSymbol.Value.Styles.Styles == TextStyles.NONE 
            ? "None" 
            : matchedSymbol.Value.Styles.Styles.ToString();
        NKConsole.WriteLine(stylesStr, matchedSymbol.Value.Styles.Styles == TextStyles.NONE ? ConsoleColor.DarkGray : ConsoleColor.Blue);

        if (matchedGlyph.AlignPoints.Any()) {
            NKConsole.Write("  Align Points:    ", ConsoleColor.Cyan);
            var points = matchedGlyph.AlignPoints.Select(p => $"'{p.Character}' at {p.Position.X},{p.Position.Y}");
            NKConsole.WriteLine(string.Join(", ", points), ConsoleColor.White);
        }
        else {
            NKConsole.Write("  Align Points:    ", ConsoleColor.Cyan);
            NKConsole.WriteLine("None", ConsoleColor.DarkGray);
        }

        System.Console.WriteLine();

        // Visual Representation
        NKConsole.WriteLine("Visual Representation:", new NKStyle(ConsoleColor.Yellow, s: TextStyles.BOLD));
        NKConsole.WriteLine("──────────────────────", ConsoleColor.DarkYellow);

        int baselineY = matchedGlyph.Height + matchedGlyph.BaselineOffset - 1;

        // Top Border
        NKConsole.Write("  ┌", ConsoleColor.DarkGray);
        NKConsole.Write(new string('─', matchedGlyph.Width), ConsoleColor.DarkGray);
        NKConsole.WriteLine("┐", ConsoleColor.DarkGray);

        // Rows
        for (int y = 0; y < matchedGlyph.Height; y++) {
            NKConsole.Write("  │", ConsoleColor.DarkGray);
            for (int x = 0; x < matchedGlyph.Width; x++) {
                var cell = matchedGlyph.Glyph[x, y];

                switch (cell.Type) {
                    case GlyphCellType.BACKGROUND: {
                        NKConsole.Write("·", ConsoleColor.DarkGray);

                        break;
                    }
                    case GlyphCellType.FOREGROUND: {
                        NKConsole.Write("·", ConsoleColor.Blue);

                        break;
                    }
                    case GlyphCellType.CHARACTER: {
                        NKConsole.Write(cell.Character.ToString(), new NKStyle(ConsoleColor.Yellow, s: TextStyles.BOLD));

                        break;
                    }
                    default: {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
            NKConsole.Write("│", ConsoleColor.DarkGray);

            // Tag next to baseline row
            if (y == baselineY) {
                NKConsole.Write("  <-- Baseline", new NKStyle(ConsoleColor.Magenta, s: TextStyles.BOLD));
            }
            
            System.Console.WriteLine();
        }

        // Bottom Border
        NKConsole.Write("  └", ConsoleColor.DarkGray);
        NKConsole.Write(new string('─', matchedGlyph.Width), ConsoleColor.DarkGray);
        NKConsole.WriteLine("┘", ConsoleColor.DarkGray);

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
            ' ' => "[Space]",
            '\t' => "[Tab]",
            '\n' => "[Newline]",
            '\r' => "[Carriage Return]",
            _ => $"'{c}'"
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
            if (font != null) return font;
        }
        else {
            string[] possibleExtensions = { ".nkf", ".flf" };
            foreach (var ext in possibleExtensions) {
                string pathWithExt = Path.Combine(FONTS_STORAGE_DIR, fontKey + ext);
                if (File.Exists(pathWithExt)) {
                    var font = LoadFromFile(pathWithExt);
                    if (font != null) return font;
                }
            }
        }

        // 3. Try loading from binary file path
        if (File.Exists(fontKey)) {
            var font = LoadFromFile(Path.GetFullPath(fontKey));
            if (font != null) return font;
        }

        // 4. Try loading as general XML path (directory or zip)
        if (Directory.Exists(fontKey) || File.Exists(fontKey)) {
            string fullPath = Path.GetFullPath(fontKey);

            try {
                var font = NKFontSerializer.DeserializeXml(fullPath);

                if (font != null) return font;
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
                int read = fs.Read(buffer, 0, 5);

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

        PrintInfo($"Validating font file '{Path.GetFileName(fullPath)}'...");
        var font = LoadFromFile(fullPath);
        if (font == null) {
            PrintError("Failed to load font. The file is not a valid FIGlet (.flf) or NeoKolors binary (.nkf) font.");
            return;
        }

        string destFileName = Path.GetFileName(fullPath);
        string destPath = Path.Combine(FONTS_STORAGE_DIR, destFileName);

        try {
            if (!Directory.Exists(FONTS_STORAGE_DIR)) {
                Directory.CreateDirectory(FONTS_STORAGE_DIR);
            }
            File.Copy(fullPath, destPath, overwrite: true);
            PrintSuccess($"Successfully added font '{font.Name}' to internal storage at '{destPath}'!");
        }
        catch (Exception ex) {
            PrintError($"Failed to save font to internal storage: {ex.Message}");
        }
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
        var logsDir = Path.Combine(installDir, "logs");

        if (!Directory.Exists(logsDir)) {
            PrintSuccess("No log directory found. Nothing to clean.");
            return;
        }

        try {
            var files = Directory.GetFiles(logsDir, "*.log");
            int deletedCount = 0;
            int failedCount = 0;

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
                PrintSuccess($"Successfully cleaned {deletedCount} log file(s).");
            }
            if (failedCount > 0) {
                PrintInfo($"{failedCount} active log file(s) are currently in use by the running tool and were not deleted.");
            }
            if (deletedCount == 0 && failedCount == 0) {
                PrintInfo("No log files to clean.");
            }
        }
        catch (Exception ex) {
            PrintError($"Failed to clean logs: {ex.Message}");
        }
    }

    static void PrintError(string msg) {
        NKConsole.Write("[ERROR] ", new NKStyle(ConsoleColor.Red, s: TextStyles.BOLD));
        NKConsole.WriteLine(msg, ConsoleColor.Red);
    }

    static void PrintSuccess(string msg) {
        NKConsole.Write("[SUCCESS] ", new NKStyle(ConsoleColor.Green, s: TextStyles.BOLD));
        NKConsole.WriteLine(msg, ConsoleColor.Green);
    }

    static void PrintInfo(string msg) {
        NKConsole.Write("[INFO] ", new NKStyle(ConsoleColor.Cyan, s: TextStyles.BOLD));
        NKConsole.WriteLine(msg, ConsoleColor.Cyan);
    }
}