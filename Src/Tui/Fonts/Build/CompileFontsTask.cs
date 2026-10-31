// NeoKolors
// Copyright (c) KryKom 2026

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NeoKolors.Tui.Fonts.Build;

/// <summary>
/// A compiled MSBuild task that deserializes raw XML fonts and compiles them
/// into pre-validated binary Protobuf (.nkf) font assets.
/// </summary>
public class CompileFontsTask : Microsoft.Build.Utilities.Task {

    /// <summary>
    /// Base directory containing the font source folders and compiled assets.
    /// If not specified, defaults to the current working directory.
    /// </summary>
    public string? SourceDir { get; set; }

    /// <summary>
    /// Default output directory for compiled .nkf files.
    /// If relative, it is resolved against SourceDir.
    /// If not specified, defaults to SourceDir.
    /// </summary>
    public string? OutputDir { get; set; }

    /// <summary>
    /// The list of fonts to compile. Each item can be a font name or folder path.
    /// Supported metadata per item:
    /// - OutputPath / OutputFile / TargetFile: specific destination file path or directory.
    /// - OutputDir / OutputDirectory: destination directory override for this font.
    /// - FontName / Name: custom name for the font asset.
    /// If not specified or empty, falls back to discovering font directories in SourceDir,
    /// or default known fonts ("Bytesized", "Future", "Dummy").
    /// </summary>
    public ITaskItem[]? Fonts { get; set; }

    /// <summary>
    /// If true, forces recompilation of all fonts regardless of whether output files are up to date.
    /// Defaults to false.
    /// </summary>
    public bool ForceRebuild { get; set; }

    /// <summary>
    /// The list of compiled font files (.nkf) generated or validated by this task.
    /// </summary>
    [Output]
    public ITaskItem[] OutputFiles { get; set; } = [];

    private record FontEntry(
        string     SourceDir,
        string     FontName,
        string?    ExplicitOutputPath,
        string?    ExplicitOutputDir,
        ITaskItem? OriginalItem
    );

    public override bool Execute() {
        string baseDir = string.IsNullOrWhiteSpace(SourceDir)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(SourceDir);

        string resolvedOutputDir = string.IsNullOrWhiteSpace(OutputDir)
            ? baseDir
            : Path.IsPathRooted(OutputDir)
                ? OutputDir
                : Path.GetFullPath(Path.Combine(baseDir, OutputDir));

        var itemsToProcess = GetFontsToProcess(baseDir);

        if (itemsToProcess.Count == 0) {
            Log.LogWarning("[CompileFont] No fonts found to compile in: " + baseDir);
            OutputFiles = [];

            return true;
        }

        var outputList = new List<ITaskItem>();

        try {
            foreach (var fontItem in itemsToProcess) {
                if (!ProcessFont(fontItem, baseDir, resolvedOutputDir, outputList)) {
                    return false;
                }
            }

            OutputFiles = outputList.ToArray();

            return true;
        }
        catch (Exception ex) {
            Log.LogError(
                "[CompileFont] Critical Error during font compilation: " + ex.Message + "\n" +
                ex.StackTrace
            );

            return false;
        }
    }

    private List<FontEntry> GetFontsToProcess(string baseDir) {
        var result = new List<FontEntry>();

        if (Fonts is { Length: > 0 }) {
            foreach (var item in Fonts) {
                string spec = item.ItemSpec?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(spec)) {
                    continue;
                }

                string fullSourcePath = Path.IsPathRooted(spec)
                    ? Path.GetFullPath(spec)
                    : Path.GetFullPath(Path.Combine(baseDir, spec));

                string sourceDir = File.Exists(fullSourcePath) && !Directory.Exists(fullSourcePath)
                    ? Path.GetDirectoryName(fullSourcePath) ?? fullSourcePath
                    : fullSourcePath;

                string fontName = item.GetMetadata("FontName");

                if (string.IsNullOrWhiteSpace(fontName)) {
                    fontName = item.GetMetadata("Name");
                }

                if (string.IsNullOrWhiteSpace(fontName)) {
                    fontName = Path.GetFileName(
                        sourceDir.TrimEnd(
                            Path.DirectorySeparatorChar,
                            Path.AltDirectorySeparatorChar
                        )
                    );
                }

                string? outPath = item.GetMetadata("OutputPath");

                if (string.IsNullOrWhiteSpace(outPath))
                    outPath = item.GetMetadata("OutputFile");

                if (string.IsNullOrWhiteSpace(outPath))
                    outPath = item.GetMetadata("TargetFile");

                string? outDir = item.GetMetadata("OutputDir");

                if (string.IsNullOrWhiteSpace(outDir))
                    outDir = item.GetMetadata("OutputDirectory");

                result.Add(
                    new FontEntry(
                        SourceDir: sourceDir,
                        FontName: fontName,
                        ExplicitOutputPath: string.IsNullOrWhiteSpace(outPath) ? null : outPath,
                        ExplicitOutputDir: string.IsNullOrWhiteSpace(outDir) ? null : outDir,
                        OriginalItem: item
                    )
                );
            }
        }
        else {
            // Auto-discovery fallback in baseDir
            if (Directory.Exists(baseDir)) {
                var subDirs = Directory.GetDirectories(baseDir);

                foreach (var dir in subDirs) {
                    bool isFont = File.Exists(Path.Combine(dir, "manifest.nkfont")) ||
                        File.Exists(Path.Combine(dir,           "Config.xml"));

                    if (isFont) {
                        string name = Path.GetFileName(dir);

                        result.Add(
                            new FontEntry(
                                SourceDir: dir,
                                FontName: name,
                                ExplicitOutputPath: null,
                                ExplicitOutputDir: null,
                                OriginalItem: null
                            )
                        );
                    }
                }
            }

            // Fallback to default known fonts if nothing discovered
            if (result.Count == 0) {
                string[] defaultFonts = ["Bytesized", "Future", "Dummy"];

                foreach (var name in defaultFonts) {
                    string dir = Path.Combine(baseDir, name);

                    if (Directory.Exists(dir)) {
                        result.Add(
                            new FontEntry(
                                SourceDir: dir,
                                FontName: name,
                                ExplicitOutputPath: null,
                                ExplicitOutputDir: null,
                                OriginalItem: null
                            )
                        );
                    }
                }
            }
        }

        return result;
    }

    private bool ProcessFont(
        FontEntry       font,
        string          baseDir,
        string          defaultOutputDir,
        List<ITaskItem> outputList
    ) {
        if (!Directory.Exists(font.SourceDir)) {
            Log.LogError($"[CompileFont] Source XML directory not found: '{font.SourceDir}'");

            return false;
        }

        string outputPath;

        if (!string.IsNullOrEmpty(font.ExplicitOutputPath)) {
            outputPath = Path.IsPathRooted(font.ExplicitOutputPath)
                ? font.ExplicitOutputPath
                : Path.GetFullPath(Path.Combine(baseDir, font.ExplicitOutputPath));

            if (outputPath.EndsWith(Path.DirectorySeparatorChar.ToString())    ||
                outputPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()) ||
                Directory.Exists(outputPath)
            ) {
                outputPath = Path.Combine(outputPath, $"{font.FontName}.nkf");
            }
            else if (!Path.HasExtension(outputPath)) {
                outputPath += ".nkf";
            }
        }
        else if (!string.IsNullOrEmpty(font.ExplicitOutputDir)) {
            string customDir = Path.IsPathRooted(font.ExplicitOutputDir)
                ? font.ExplicitOutputDir
                : Path.GetFullPath(Path.Combine(baseDir, font.ExplicitOutputDir));

            outputPath = Path.Combine(customDir, $"{font.FontName}.nkf");
        }
        else {
            outputPath = Path.Combine(defaultOutputDir, $"{font.FontName}.nkf");
        }

        string? outDir = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir)) {
            Directory.CreateDirectory(outDir);
        }

        // Check if output is already up-to-date
        bool needsRebuild = ForceRebuild || !File.Exists(outputPath);

        if (!needsRebuild) {
            var outputWriteTime = File.GetLastWriteTimeUtc(outputPath);

            foreach (
                var file in Directory.EnumerateFiles(
                    font.SourceDir,
                    "*",
                    SearchOption.AllDirectories
                )
            ) {
                if (File.GetLastWriteTimeUtc(file) > outputWriteTime) {
                    needsRebuild = true;

                    break;
                }
            }
        }

        if (!needsRebuild) {
            Log.LogMessage(
                MessageImportance.Normal,
                $"[CompileFont] Font '{font.FontName}' is up-to-date: '{outputPath}'"
            );

            var upToDateItem = new TaskItem(outputPath);
            upToDateItem.SetMetadata("FontName",  font.FontName);
            upToDateItem.SetMetadata("SourceDir", font.SourceDir);

            upToDateItem.SetMetadata(
                "LogicalName",
                $"NeoKolors.Tui.Fonts.Assets.{font.FontName}.nkf"
            );

            outputList.Add(upToDateItem);

            return true;
        }

        Log.LogMessage(
            MessageImportance.High,
            $"[CompileFont] Compiling XML Font '{font.FontName}' ('{font.SourceDir}') -> '{outputPath}'..."
        );

        var result = NKFontSerializer.TryDeserializeXml(font.SourceDir);

        foreach (var info in result.Infos) {
            Log.LogMessage(MessageImportance.Normal, "[CompileFont] " + info);
        }

        foreach (var warning in result.Warnings) {
            Log.LogWarning("[CompileFont] " + warning);
        }

        foreach (var error in result.Errors) {
            Log.LogError("[CompileFont] " + error);
        }

        if (!result.Success || result.Font == null) {
            Log.LogError(
                $"[CompileFont] Failed to deserialize font from directory: '{font.SourceDir}'"
            );

            return false;
        }

        NKFontSerializer.SerializeBinary(result.Font, outputPath);

        Log.LogMessage(
            MessageImportance.High,
            $"[CompileFont] Successfully compiled font: '{outputPath}'"
        );

        var compiledItem = new TaskItem(outputPath);
        compiledItem.SetMetadata("FontName",    font.FontName);
        compiledItem.SetMetadata("SourceDir",   font.SourceDir);
        compiledItem.SetMetadata("LogicalName", $"NeoKolors.Tui.Fonts.Assets.{font.FontName}.nkf");
        outputList.Add(compiledItem);

        return true;
    }
}