// NeoKolors
// Copyright (c) KryKom 2026

using Microsoft.Build.Framework;

namespace NeoKolors.Tui.Fonts.Build;

/// <summary>
/// A compiled MSBuild task that deserializes raw XML fonts and compiles them
/// into pre-validated binary Protobuf (.nkf) font assets.
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


        void Compile(string xmlSubDir, string outputFileName) {
            string xmlDir     = Path.Combine(baseDir, xmlSubDir);
            string outputPath = Path.Combine(baseDir, outputFileName);

            if (!Directory.Exists(xmlDir)) {
                Log.LogWarning("[CompileFont] Source XML directory not found: " + xmlDir);

                return;
            }

            Log.LogMessage(MessageImportance.High, $"[CompileFont] Compiling XML Font directory '{xmlDir}' -> '{outputPath}'...");

            var result = NKFontSerializer.TryDeserializeXml(xmlDir);

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
                throw new InvalidOperationException("Failed to deserialize font from directory: " + xmlDir);
            }

            NKFontSerializer.SerializeBinary(result.Font, outputPath);
            Log.LogMessage(MessageImportance.High, "[CompileFont] Successfully compiled font: " + outputPath);
        }

        try {
            Compile("Bytesized", "Bytesized.nkf");
            Compile("Future",    "Future.nkf");
            Compile("Dummy",     "Dummy.nkf");

            return true;
        }
        catch (Exception ex) {
            Log.LogError("Critical Error during font compilation: " + ex.Message + "\n" + ex.StackTrace);

            return false;
        }
    }
}