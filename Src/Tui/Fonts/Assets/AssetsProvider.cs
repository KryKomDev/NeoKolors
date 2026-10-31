// NeoKolors
// Copyright (c) KryKom 2026

using System.Reflection;
using NeoKolors.Console;

#if NK_FONTS_ENABLE_AUTO_REGISTRATION
using System.Runtime.CompilerServices;
#endif

namespace NeoKolors.Tui.Fonts.Assets;

/// <summary>
/// Provides access to the pre-compiled built-in font assets.
/// </summary>
public static class AssetsProvider {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger("Fonts.AssetsProvider");
    
    private static readonly Assembly ASSEMBLY = typeof(AssetsProvider).Assembly;

    private static NKFont? BYTESIZED;
    private static NKFont? FUTURE;
    private static NKFont? DUMMY;
    private static NKFont? STANDARD;
    private static NKFont? SLANT;
    private static NKFont? SHADOW;
    private static NKFont? BLOCK;

    /// <summary>
    /// Gets the pre-compiled Bytesized font.
    /// </summary>
    public static NKFont? Bytesized => BYTESIZED ??= LoadFont("Bytesized");

    /// <summary>
    /// Gets the pre-compiled Future font.
    /// </summary>
    public static NKFont? Future => FUTURE ??= LoadFont("Future");

    /// <summary>
    /// Gets the pre-compiled Dummy font.
    /// </summary>
    public static NKFont? Dummy => DUMMY ??= LoadFont("Dummy");

    /// <summary>
    /// Gets the built-in Standard FIGlet font.
    /// </summary>
    public static NKFont? Standard => STANDARD ??= LoadFigletFont("Standard");

    /// <summary>
    /// Gets the built-in Slant FIGlet font.
    /// </summary>
    public static NKFont? Slant => SLANT ??= LoadFigletFont("Slant");

    /// <summary>
    /// Gets the built-in Shadow FIGlet font.
    /// </summary>
    public static NKFont? Shadow => SHADOW ??= LoadFigletFont("Shadow");

    /// <summary>
    /// Gets the built-in Block FIGlet font.
    /// </summary>
    public static NKFont? Block => BLOCK ??= LoadFigletFont("Block");

    /// <summary>
    /// Resolves and returns a Stream for the specified built-in font resource.
    /// </summary>
    /// <param name="name">The name of the font (e.g., "Bytesized", "Future", "Dummy").</param>
    /// <returns>A Stream for the embedded font resource, or null if not found.</returns>
    public static Stream? GetFontStream(string name) {
        var resourceName = $"NeoKolors.Tui.Fonts.Assets.{name}.nkf";
        var stream = ASSEMBLY.GetManifestResourceStream(resourceName);
        if (stream != null) {
            return stream;
        }

        // Fallback for subfolders (e.g. nkf/ or compiled/) when embedded without LogicalName
        var suffix = $".{name}.nkf";
        var matched = ASSEMBLY.GetManifestResourceNames()
            .FirstOrDefault(r => r.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) ||
                                 r.Equals($"{name}.nkf", StringComparison.OrdinalIgnoreCase));

        return matched != null ? ASSEMBLY.GetManifestResourceStream(matched) : null;
    }

    /// <summary>
    /// Registers the built-in font resources into the font atlas for later usage in the application.
    /// </summary>
    #if NK_FONTS_ENABLE_AUTO_REGISTRATION
    #pragma warning disable CA2255
    [ModuleInitializer]
    #pragma warning restore CA2255
    #endif
    public static void RegisterFonts() {
        RegisterFont("Bytesized", Bytesized);
        RegisterFont("Future",    Future);
        RegisterFont("Dummy",     Dummy);
        RegisterFont("Standard",  Standard);
        RegisterFont("Slant",     Slant);
        RegisterFont("Shadow",    Shadow);
        RegisterFont("Block",     Block);
    }

    private static void RegisterFont(string name, NKFont? font) {
        if (font is not null)
            FontAtlas.TryAdd(name, font);     
        else 
            LOGGER.Error($"The '{name}' font not found.");
    }

    private static NKFont? LoadFont(string name) {
        using var stream = GetFontStream(name);
        
        return stream == null ? null : NKFontSerializer.DeserializeBinary(stream);
    }

    private static NKFont? LoadFigletFont(string name) {
        var lowerName = name.ToLowerInvariant();
        var resourceName = $"NeoKolors.Tui.Fonts.Assets.Figlet.{lowerName}.flf";
        using var stream = ASSEMBLY.GetManifestResourceStream(resourceName);
        if (stream == null) {
            return null;
        }

        var effectiveMeta = new FigletMetadata();

        // 1. Look for embedded directory manifest
        string[] manifestNames = [
            "NeoKolors.Tui.Fonts.Assets.Figlet.manifest.xml",
            "NeoKolors.Tui.Fonts.Assets.Figlet.figlet.manifest.xml"
        ];

        foreach (var mfName in manifestNames) {
            using var mfStream = ASSEMBLY.GetManifestResourceStream(mfName);
            if (mfStream != null) {
                var dict = FigletMetadata.FromManifestXml(mfStream);
                if (dict.TryGetValue(name, out var mfMeta)) {
                    effectiveMeta.MergeWith(mfMeta);
                    break;
                }
            }
        }

        // 2. Look for embedded font sidecar
        string[] sidecarNames = [
            $"NeoKolors.Tui.Fonts.Assets.Figlet.{lowerName}.meta.xml",
            $"NeoKolors.Tui.Fonts.Assets.Figlet.{lowerName}.xml"
        ];

        foreach (var scName in sidecarNames) {
            using var scStream = ASSEMBLY.GetManifestResourceStream(scName);
            if (scStream != null) {
                var scMeta = FigletMetadata.FromXml(scStream);
                effectiveMeta.MergeWith(scMeta);
                break;
            }
        }

        return NKFontSerializer.DeserializeFiglet(stream, name, metadata: effectiveMeta);
    }
}