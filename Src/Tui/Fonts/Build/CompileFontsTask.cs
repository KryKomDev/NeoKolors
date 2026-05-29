// NeoKolors
// Copyright (c) krystof 2026

using Microsoft.Build.Framework;
using NeoKolors.Tui.Fonts.Serialization;

namespace NeoKolors.Tui.Fonts.Build;

/// <summary>
/// A compiled MSBuild task that deserializes raw XML fonts and compiles them
/// into pre-validated binary MessagePack (.nkf) font assets.
/// </summary>
public class CompileFontsTask : Microsoft.Build.Utilities.Task {
    /// <summary>
    /// The base directory containing the font source folders and compiled assets.
    /// </summary>
    [Required]
    public string SourceDir { get; set; } = string.Empty;

    public override bool Execute() {
        if (string.IsNullOrWhiteSpace(SourceDir)) {
            Log.LogError("SourceDir parameter is required and cannot be empty.");
            return false;
        }

        string baseDir = Path.GetFullPath(SourceDir);

        // Prevent parallel write conflicts during multi-targeting build
        var lockFile = Path.Combine(baseDir, "compile.lock");
        if (File.Exists(lockFile)) {
            try {
                var lastWrite = File.GetLastWriteTimeUtc(lockFile);
                if ((DateTime.UtcNow - lastWrite).TotalSeconds < 3) {
                    Log.LogMessage(MessageImportance.High, "[FontTask] Fonts compiled recently. Skipping parallel execution.");
                    return true;
                }
            }
            catch {
                // Ignore lock file read/write issues and proceed
            }
        }

        try {
            File.WriteAllText(lockFile, DateTime.UtcNow.ToString());
        }
        catch {
            // Ignore lock file write issues and proceed
        }

        void Compile(string xmlSubDir, string outputFileName) {
            string xmlDir = Path.Combine(baseDir, xmlSubDir);
            string outputPath = Path.Combine(baseDir, outputFileName);

            if (!Directory.Exists(xmlDir)) {
                Log.LogWarning("[FontTask] Source XML directory not found: " + xmlDir);
                return;
            }

            Log.LogMessage(MessageImportance.High, $"[FontTask] Compiling XML Font directory '{xmlDir}' -> '{outputPath}'...");
            
            var font = NKFontSerializer.DeserializeXml(xmlDir);
            if (font == null) {
                throw new InvalidOperationException("Failed to deserialize font from directory: " + xmlDir);
            }

            NKFontSerializer.SerializeBinary(font, outputPath);
            Log.LogMessage(MessageImportance.High, "[FontTask] Successfully compiled font: " + outputPath);
        }

        try {
            Compile("Bytesized", "Bytesized.nkf");
            Compile("Future", "Future.nkf");
            Compile("Dummy", "Dummy.nkf");
            return true;
        }
        catch (Exception ex) {
            Log.LogError("Critical Error during font compilation: " + ex.Message + "\n" + ex.StackTrace);
            return false;
        }
    }
}
