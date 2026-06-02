// NeoKolors
// Copyright (c) krystof 2026

using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Extensions;
using NeoKolors.Tui.Fonts.Exceptions;
using NeoKolors.Tui.Fonts.Serialization.Xml;
using NeoKolors.Tui.Fonts.Serialization.Xml.V3;
using OneOf;
using OneOf.Types;
using XmlDefTuple = (NeoKolors.Tui.Fonts.Serialization.Xml.V3.XmlGlyphDef Def, string[]? Lines);

namespace NeoKolors.Tui.Fonts.Serialization;

public static partial class NKFontSerializer {

    internal static readonly NKLogger LOGGER = NKDebug.GetLogger("NKFontSerializer");
    
    private static readonly HttpClient HTTP = new();
    
    private const string MANIFEST_FILE_NAME = "manifest.nkfont";
    private const string CONFIG_FILE_NAME   = "conf.nkfc";
    private const string MAP_FILE_NAME      = "map.nkfm";
    
    /// <summary>
    /// Reads an embedded font resource from the specified assembly and deserializes it into an NKFont.
    /// Supports both binary (NKFB) and legacy XML ZIP archive formats.
    /// </summary>
    public static NKFont? ReadEmbedded(string path, Assembly assembly) {
        using var stream = assembly.GetManifestResourceStream(path);
        if (stream == null) {
            LOGGER.Error("Could not find embedded font '{0}'.", path);
            return null;
        }

        if (stream.CanSeek) {
            var startPos = stream.Position;
            if (stream.Length >= 4) {
                var buffer = new byte[4];
                int read = stream.Read(buffer, 0, 4);
                stream.Position = startPos;
                if (read == 4) {
                    uint magic = (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
                    if (magic == BINARY_MAGIC) {
                        return DeserializeBinary(stream);
                    }
                }
            }
        }

        return DeserializeXmlArchive(stream);
    }

    /// <summary>
    /// Reads an embedded font resource from the specified assembly and deserializes it into an NKFont.
    /// </summary>
    public static NKFont? ReadEmbedded<TAssemblyType>(string path) =>
        ReadEmbedded(path, typeof(TAssemblyType).Assembly);

    internal static NKFont? ReadInternal(string name) {
        try {
            var assembly = Assembly.Load("NeoKolors.Tui.Fonts.Assets");
            return ReadEmbedded($"NeoKolors.Tui.Fonts.Assets.{name}.nkf", assembly);
        } catch (Exception ex) {
            LOGGER.Error($"Failed to load built-in font assembly 'NeoKolors.Tui.Fonts.Assets': {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Deserializes an NKFont object from the specified XML resource path. The path can reference
    /// a local file or a remote resource accessible via HTTP/HTTPS.
    /// </summary>
    /// <param name="path">The path to the XML resource containing font data. It can be a file path or a URI.</param>
    /// <returns>An <see cref="NKFont"/> object deserialized from the XML resource, or null if the deserialization
    /// fails.</returns>
    public static NKFont? DeserializeXml(string path) => DeserializeXmlAsync(path).Result;

    /// <summary>
    /// Deserializes an NKFont object from the specified path asynchronously.
    /// The path can reference a file, a directory, or a URI.
    /// </summary>
    /// <param name="path">The path or URI to the font resource. Can be a local path or a remote URL.</param>
    /// <returns>A task representing the asynchronous operation. The task result is an <see cref="NKFont"/> object
    /// deserialized from the specified path or null if deserialization fails.</returns>
    public static async Task<NKFont?> DeserializeXmlAsync(string path) {
        if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var uri)) {
            LOGGER.Error($"Cannot resolve path format '{path}'.");
            return null;
        }

        if (uri.IsFile) {
            string localPath = uri.LocalPath;

            if (Directory.Exists(localPath)) {
                LOGGER.Info($"Deserializing unpackaged XML-based font from '{localPath}'.");
                return await DeserializeXmlDir(localPath);
            }

            if (File.Exists(localPath)) {
                LOGGER.Info($"Deserializing archived XML-based font from '{localPath}'.");
                await using Stream fileStream = File.OpenRead(localPath);
                return await DeserializeXmlArchiveAsync(fileStream);
            }
        }
        else if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) {
            LOGGER.Info($"Deserializing packaged XML-based font from web URI '{uri}'.");
            var webStream = await HTTP.GetStreamAsync(uri);
            return await DeserializeXmlArchiveAsync(webStream);
        }

        LOGGER.Error($"Cannot resolve path '{path}'.");
        return null;
    }

    /// <summary>
    /// Deserializes an NKFont object from the specified XML archive stream.
    /// The archive is expected to contain the necessary font configuration, glyph definitions, and related resources.
    /// </summary>
    /// <param name="archive">A stream representing the XML archive containing font data and metadata.</param>
    /// <returns>An <see cref="NKFont"/> object constructed from the contents of the archive, or <c>null</c> if
    /// deserialization fails.</returns>
    public static NKFont? DeserializeXmlArchive(Stream archive) => DeserializeXmlArchiveAsync(archive).Result;

    /// <summary>
    /// Deserializes an NKFont object asynchronously from the provided stream containing a font archive in XML format.
    /// </summary>
    /// <param name="archive">The stream containing the XML-based font archive to deserialize.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="NKFont"/>
    /// object if deserialization succeeds; otherwise, null if the required entries in the archive are missing or an
    /// error occurs.</returns>
    public static async Task<NKFont?> DeserializeXmlArchiveAsync(Stream archive) {
        ZipArchiveEntry? configEntry;
        ZipArchiveEntry? mapEntry; 
        
        var zip = new ZipArchive(archive);

        var manifestEntry = zip.GetEntry(MANIFEST_FILE_NAME);
        
        if (manifestEntry != null) {
            var manifestStream = manifestEntry.Open();

            var manifest = await DeserializeManifestAsync(manifestStream);

            configEntry = zip.GetEntry(manifest.Config);
            mapEntry    = zip.GetEntry(manifest.Map);
        }
        else {
            configEntry = zip.GetEntry(CONFIG_FILE_NAME);
            mapEntry    = zip.GetEntry(   MAP_FILE_NAME);
        }
        
        if (configEntry == null) {
            LOGGER.Error("A font config entry does not exist.");
            return null;
        }
        
        if (mapEntry == null) {
            LOGGER.Error("A font map entry does not exist.");
            return null;
        }

        var configTask = DeserializeConfigAsync(configEntry.Open());
        var mapTask    = DeserializeMapAsync   (mapEntry   .Open());
        
        var map = await mapTask;

        var glyphTasks = new List<Task<(XmlGlyphDef Def, string[]? Lines)>>();
        var failed     = false;
        
        for (var i = 0; i < map.Glyphs.Length; i++) {
            var glyph = map.Glyphs[i];

            var glyphLoadingTask = LoadGlyph(glyph);

            if (glyphLoadingTask.IsFaulted) {
                failed = true;
                continue;
            }
            
            glyphTasks.Add(glyphLoadingTask);
        }

        if (failed) {
            LOGGER.Error("One or more glyphs could not be loaded.");
            return null;
        }

        var glyphs = await Task.WhenAll(glyphTasks);
        var config = await configTask;
        
        return AssembleFont(config, glyphs);

        async Task<(XmlGlyphDef, string[]?)> LoadGlyph(XmlGlyphDef glyph) {
            var filePath = glyph switch {
                XmlComponentGlyphDef     c => c.File,
                XmlLigatureGlyphDef      l => l.File,
                XmlUnderlineGlyphDef     u => u.File,
                XmlStrikethroughGlyphDef s => s.File,
                _                          => null
            };

            if (filePath == null)
                return (glyph, null);

            var normalizedPath = filePath.Replace('\\', '/');
            var entry = zip.GetEntry(normalizedPath) ?? zip.GetEntry(filePath);
            if (entry == null) {
                foreach (var e in zip.Entries) {
                    if (e.FullName.Replace('\\', '/').Equals(normalizedPath, StringComparison.OrdinalIgnoreCase)) {
                        entry = e;
                        break;
                    }
                }
            }

            if (entry != null) {
                using var stream = entry.Open();
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var text = await reader.ReadToEndAsync();
                var lines = text.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
                return (glyph, lines);
            }

            LOGGER.Error($"The glyph file entry path '{filePath}' cannot be resolved inside the archive.");
            return await Task.FromException<(XmlGlyphDef, string[])>(new FileNotFoundException(filePath));
        }
    }

    /// <summary>
    /// Deserializes an NKFont object from a directory containing XML font resources.
    /// The directory must include a manifest file or the default config and map files.
    /// </summary>
    /// <param name="path">The path to the directory containing the font resources.</param>
    /// <returns>An <see cref="NKFont"/> object deserialized from the directory, or null if deserialization fails.</returns>
    private static async Task<NKFont?> DeserializeXmlDir(string path) {
        var manifestPath = Path.Combine(path, MANIFEST_FILE_NAME);

        string configPath;
        string mapPath;
        
        if (File.Exists(manifestPath)) {
            var manifestStream = File.Open(manifestPath, FileMode.Open);

            var manifest = await DeserializeManifestAsync(manifestStream);

            configPath = Path.IsPathRooted(manifest.Config) ? manifest.Config : Path.Combine(path, manifest.Config);
            mapPath    = Path.IsPathRooted(manifest.Map)    ? manifest.Map    : Path.Combine(path, manifest.Map);
        }
        else {
            configPath = Path.Combine(path, CONFIG_FILE_NAME);
            mapPath    = Path.Combine(path,    MAP_FILE_NAME);
        }
        
        if (!File.Exists(configPath)) {
            LOGGER.Error($"The font config file path '{configPath}' does not exist.");
            return null;
        }
        
        if (!File.Exists(mapPath)) {
            LOGGER.Error($"The font map file path '{configPath}' does not exist.");
            return null;
        }

        var configTask = DeserializeConfigAsync(File.Open(configPath, FileMode.Open));
        var mapTask    = DeserializeMapAsync   (File.Open(mapPath,    FileMode.Open));
        
        var map = await mapTask;

        var glyphTasks = new List<Task<(XmlGlyphDef Def, string[]? Lines)>>();
        var failed     = false;
        
        for (var i = 0; i < map.Glyphs.Length; i++) {
            var glyph = map.Glyphs[i];

            var glyphLoadingTask = LoadGlyph(glyph);

            if (glyphLoadingTask.IsFaulted) {
                failed = true;
                continue;
            }
            
            glyphTasks.Add(glyphLoadingTask);
        }

        if (failed) {
            LOGGER.Error("One or more glyphs could not be loaded.");
            return null;
        }

        var glyphs = await Task.WhenAll(glyphTasks);
        var config = await configTask;
        
        return AssembleFont(config, glyphs);

        async Task<(XmlGlyphDef, string[]?)> LoadGlyph(XmlGlyphDef glyph) {
            var filePath = glyph switch {
                XmlComponentGlyphDef     c => c.File,
                XmlLigatureGlyphDef      l => l.File,
                XmlUnderlineGlyphDef     u => u.File,
                XmlStrikethroughGlyphDef s => s.File,
                _                          => null
            };

            if (filePath == null)
                return (glyph, null);
            
            var glyphPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(path, filePath);

            if (File.Exists(glyphPath)) 
                return (glyph, await File.ReadAllLinesAsync(glyphPath));

            LOGGER.Error($"The glyph file path '{filePath}' cannot be resolved.");
            return await Task.FromException<(XmlGlyphDef, string[])>(new FileNotFoundException());
        }
    }

    private static readonly EventId GLYPH_ASSEMBLY_EVENT_ID = new(
        HashCode.Combine("NKGlyphAssembly", ""),
        "NKGlyphAssembly"
    );
    
    /// <summary>
    /// Combines the specified font configuration and glyph definitions into a single NKFont object.
    /// </summary>
    /// <param name="config">The configuration for the font, specifying metadata and settings.</param>
    /// <param name="glyphDefs">An array of tuples containing glyph definitions and their corresponding line data.</param>
    /// <returns>An <see cref="NKFont"/> object constructed from the provided configuration and glyphs, or null if the
    /// assembly fails.</returns>
    private static NKFont? AssembleFont(XmlFontConfig config, XmlDefTuple[] glyphDefs) {
        // see Docs/Eng/Fonts/Xml/Xml-Font-Compilation.md
        // here starts stage 2
        
        // stage 2 step 1
        Dictionary<NKGlyphSymbol, GlyphDefNode> symbolTable = new();
        Dictionary<string,        GlyphDefNode> idTable     = new();

        NKGlyph? underlineGlyph     = null;
        NKGlyph? strikethroughGlyph = null;
        bool     underlineAbove     = false;
        bool     strikethroughAbove = false;
        
        // stage 2 steps 2 and 3
        for (int i = 0; i < glyphDefs.Length; i++) {
            var g = glyphDefs[i];

            switch (g.Def) {
                case XmlComponentGlyphDef component: {
                    var node = new GlyphDefNode(component, g.Lines);

                    if (component.IsIdDefined) {
                        if (!idTable.TryAdd(component.Id, node)) {
                            LOGGER.Error($"A glyph with the same id ('{component.Id}') already exists.");

                            return null;
                        }
                    }

                    var symbol = NKGlyphSymbol.Simple(component.Symbol, (NKStyle)component.Styles);

                    if (!symbolTable.TryAdd(symbol, node)) {
                        LOGGER.Error($"A glyph with the same symbol ('{symbol.ToString()}') already exists.");
                        
                        return null;
                    }

                    break;  
                }
                case XmlCompoundGlyphDef compound: {
                    var node = new GlyphDefNode(compound, g.Lines);

                    if (compound.IsIdDefined) {
                        if (!idTable.TryAdd(compound.Id, node)) {
                            LOGGER.Error($"A glyph with the same id ('{compound.Id}') already exists.");

                            return null;
                        }
                    }

                    var symbol = NKGlyphSymbol.Simple(compound.Symbol, (NKStyle)compound.Styles);

                    if (!symbolTable.TryAdd(symbol, node)) {
                        LOGGER.Error($"A glyph with the same symbol ('{symbol.ToString()}') already exists.");
                        
                        return null;
                    }

                    break;
                }
                case XmlLigatureGlyphDef ligature: {
                    var node = new GlyphDefNode(ligature, g.Lines);

                    if (ligature.IsIdDefined) {
                        if (!idTable.TryAdd(ligature.Id, node)) {
                            LOGGER.Error($"A glyph with the same id ('{ligature.Id}') already exists.");

                            return null;
                        }
                    }

                    var symbol = NKGlyphSymbol.Ligature(ligature.Symbol, (NKStyle)ligature.Styles);

                    if (!symbolTable.TryAdd(symbol, node)) {
                        LOGGER.Error($"A glyph with the same symbol ('{symbol.ToString()}') already exists.");
                        
                        return null;
                    }

                    break;
                }
                case XmlAutoCompoundGlyphDef autoCompound: {
                    var baseSymbol = autoCompound.Symbol;
                    
                    foreach (var applicable in autoCompound.Applicable) 
                    foreach (var c in applicable.ApplicableChars) {
                        var symbolString = (autoCompound.SecondaryAfterBase 
                            ? $"{char.ToCombining(baseSymbol)}{c}" 
                            : $"{c}{char.ToCombining(baseSymbol)}")
                            .Normalize(NormalizationForm.FormC);

                        if (symbolString.Length != 1) {
                            LOGGER.Error($"The composed symbol '{symbolString}' is not valid for auto-compounds.");
                            
                            return null;
                        }

                        var symbolChar = symbolString[0];

                        var baseGlyph  = autoCompound.IsBaseMain ? autoCompound.Base : c.ToString();
                        var addedGlyph = autoCompound.IsBaseMain ? c.ToString()      : autoCompound.Base;
                        
                        var compound = new XmlCompoundGlyphDef(
                            symbolChar,
                            autoCompound.Align,
                            autoCompound.Styles,
                            baseGlyph,
                            addedGlyph
                        );

                        var symbol = NKGlyphSymbol.Simple(symbolChar);

                        var node = new GlyphDefNode(compound, g.Lines);
                        
                        if (!symbolTable.TryAdd(symbol, node)) {
                            LOGGER.Error($"A glyph with the same symbol ('{symbol.ToString()}') already exists.");
                        
                            return null;
                        }
                    }

                    break;
                }
                case XmlUnderlineGlyphDef underline: {
                    var r = CompileEffectGlyph(
                        g.Lines,
                        underline.Baseline,
                        underline.Mask ?? config.Defaults.Mask ?? new XmlGlyphMask()
                    );

                    if (r.IsT1) {
                        LOGGER.Error($"When compiling the underline glyph the following occured: {r.AsT1.Value}");
                        
                        return null;
                    }

                    underlineGlyph = r.AsT0.Value;
                    underlineAbove = underline.AboveLetters;
                    
                    break;
                }
                case XmlStrikethroughGlyphDef strikethrough: {
                    var r = CompileEffectGlyph(
                        g.Lines,
                        strikethrough.Baseline,
                        strikethrough.Mask ?? config.Defaults.Mask ?? new XmlGlyphMask()
                    );

                    if (r.IsT1) {
                        LOGGER.Error($"When compiling the strikethrough glyph the following occured: {r.AsT1.Value}");
                        
                        return null;
                    }

                    strikethroughGlyph = r.AsT0.Value;
                    strikethroughAbove = strikethrough.AboveLetters;

                    break;
                }
            }
        }
        
        // stage 2 step 4
        foreach (var g in symbolTable) {
            var node = g.Value;

            if (node.State == GlyphDefNodeState.PROCESSED)
                continue;

            var res = ResolveGlyph(config, node, symbolTable, idTable);

            if (res.IsT1) {
                LOGGER.Error(res.AsT1.Value);
                
                return null;
            }

            node.Glyph = res.AsT0.Value;
            node.State = GlyphDefNodeState.PROCESSED;
        }

        var glyphs = new Dictionary<NKGlyphSymbol, NKGlyph>();

        foreach (var g in symbolTable) {
            var gl = g.Value.Glyph;

            if (gl is null) {
                LOGGER.Crit($"Uncompiled glyph '{g.Key}'!");
                
                return null;
            }
            
            glyphs.Add(g.Key, gl);
        }
        
        return new NKFont(
            info:                            CreateFontInfo(config),
            glyphs:                          glyphs,
            unknownSymbolGlyph:              null,
            strikethroughGlyph:              strikethroughGlyph, 
            renderStrikethroughAboveLetters: strikethroughAbove, 
            underlineGlyph:                  underlineGlyph,
            renderUnderlineAboveLetters:     underlineAbove
        );
    }

    private static OneOf<Success<NKGlyph>, Error<string>> ResolveGlyph(
        XmlFontConfig                           config, 
        GlyphDefNode                            node, 
        Dictionary<NKGlyphSymbol, GlyphDefNode> symbolTable, 
        Dictionary<string, GlyphDefNode>        idTable) 
    {
        if (node.State == GlyphDefNodeState.VISITING)
            return new Error<string>("Could not link glyphs. Reference loop detected.");
        
        if (node.State == GlyphDefNodeState.PROCESSED)
            return new Success<NKGlyph>(node.Glyph!);

        node.State = GlyphDefNodeState.VISITING;
        
        switch (node.Def) {
            case XmlComponentGlyphDef component: {
                var r = CompileGlyph(
                    config,
                    node.Lines, 
                    component.Mask, 
                    component.AlignPoints,
                    component.AlignPointMarkers ?? config.Defaults.AlignPointMarkers, 
                    component.AlignPointReplace, 
                    component.Baseline
                );

                if (r.IsT1) {
                    return new Error<string>(
                        $"Could not assemble component glyph with ID '{component.Id}'. {r.AsT1.Value}"
                    );
                }

                foreach (var w in r.AsT0.Value.Warnings) {
                    LOGGER.Warn($"While compiling component glyph '{component.Id}', the following occured: {w}");
                }
                    
                node.Glyph = r.AsT0.Value.Glyph;

                break;
            }
            case XmlLigatureGlyphDef ligature: {
                var r = CompileGlyph(
                    config,
                    node.Lines, 
                    ligature.Mask, 
                    ligature.AlignPoints,
                    ligature.AlignPointMarkers ?? config.Defaults.AlignPointMarkers, 
                    ligature.AlignPointReplace, 
                    ligature.Baseline
                );

                if (r.IsT1) {
                    return new Error<string>(
                        $"Could not assemble component glyph with ID '{ligature.Id}'. {r.AsT1.Value}"
                    );
                }

                foreach (var w in r.AsT0.Value.Warnings) {
                    LOGGER.Warn($"While compiling ligature glyph '{ligature.Id}', the following occured: {w}");
                }
                    
                node.Glyph = r.AsT0.Value.Glyph;

                break;
            }
            case XmlCompoundGlyphDef compound: {
                var mr = ResolveReference(compound.Main,      compound.Styles, symbolTable, idTable);

                if (mr.IsT1)
                    return new Error<string>(mr.AsT1.Value);
                    
                var sr = ResolveReference(compound.Secondary, compound.Styles, symbolTable, idTable);
                    
                if (sr.IsT1)
                    return new Error<string>(sr.AsT1.Value);

                var mainNode      = mr.AsT0.Value;
                var secondaryNode = sr.AsT0.Value;

                var main = ResolveGlyph(config, mainNode, symbolTable, idTable);

                if (main.IsT1)
                    return new Error<string>(main.AsT1.Value);
                
                var secondary = ResolveGlyph(config, secondaryNode, symbolTable, idTable);
                
                if (secondary.IsT1)
                    return new Error<string>(secondary.AsT1.Value);
                
                var res = CompileCompoundGlyph(
                    main.AsT0.Value,
                    secondary.AsT0.Value,
                    compound.Align ?? config.Defaults.Align ?? new XmlGlyphAlign(XmlGlyphAlignDirection.TOP)
                );
                
                if (res.IsT1) {
                    return new Error<string>(
                        $"While compiling the compound glyph '{compound.Id}', the following occured: {res.AsT1.Value}"
                    );
                }

                node.Glyph = res.AsT0.Value;

                break;
            }
            default: {
                return new Error<string>("Weird definition.");
            }
        }

        node.State = GlyphDefNodeState.PROCESSED;
        
        return new Success<NKGlyph>(node.Glyph!);
    }

    private static OneOf<Success<(NKGlyph Glyph, string[] Warnings)>, Error<string>> CompileGlyph(
        XmlFontConfig              config,
        string[]?                  lines,
        XmlGlyphMask?              mask,
        XmlGlyphAlignPoint[]       alignPoints,
        char[]                     alignPointMarkers,
        XmlGlyphAlignPointReplace? alignPointReplace,
        int                        baseline) 
    {
        if (lines == null)
            return new Error<string>("The glyph has no content.");

        List<string> warns = [];
        
        var glyph = NKFontSerializerHelper.CreateFromLines(lines);

        var apr = alignPointReplace ?? config.Defaults.AlignPointReplace ?? new XmlGlyphAlignPointReplace();
        
        var detectedAlignPoints = NKFontSerializerHelper.DetectAlignPoints(
            glyph,
            alignPointMarkers.ToHashSet(),
            apr.Default,
            apr.CustomForg,
            apr.CustomBckg,
            apr.CustomPairs
        );

        var apc = new AlignPointCollection();

        foreach (var ap in alignPoints) 
            apc.Add(new AlignPoint(ap.Id, ap.Position));
        
        foreach (var ap in detectedAlignPoints.AlignPoints) {
            if (!apc.Add(ap)) {
                warns.Add(
                    $"Couldn't add align-point at {ap.Position}. " +
                    $"An align-point with the same ID ('{ap.Character}') already exists."
                );
            }
        }

        foreach (var ap in detectedAlignPoints.Failed) {
            warns.Add(
                $"Couldn't add align-point at {ap.Position}. " +
                $"An align-point with the same ID ('{ap.Character}') already exists."
            );
        }
            
        var m = mask ?? config.Defaults.Mask ?? new XmlGlyphMask();
        
        NKFontSerializerHelper.Mask(
            glyph, 
            m.SpaceConf,
            m.MapForg ?? [],
            m.MapBckg ?? []
        );

        var rr = NKFontSerializerHelper.Reduce(glyph);
        
        if (rr.IsT1)
            return rr.AsT1;

        (glyph, var offset) = rr.AsT0.Value;

        foreach (var ap in apc) {
            ap.SetPosition(ap.Position - offset);
        }

        var bottomTrim = lines.Length - offset.Y - glyph.GetLength(1);
        var newBaseline = baseline - bottomTrim;
        
        return new Success<(NKGlyph Glyph, string[] Warnings)>((
            new NKGlyph(glyph, newBaseline, apc),
            warns.ToArray()
        ));
    }
    
    private static OneOf<Success<NKGlyph>, Error<string>> CompileEffectGlyph(
        string[]?    lines, 
        int          baseline, 
        XmlGlyphMask mask) 
    {
        if (lines == null)
            return new Error<string>("Glyph contains no content.");
        
        var glyph = NKFontSerializerHelper.CreateFromLines(lines);
        
        NKFontSerializerHelper.Mask(glyph, mask.SpaceConf, mask.MapForg, mask.MapBckg);
        
        var result = NKFontSerializerHelper.Reduce(glyph);

        if (result.IsT1) 
            return result.AsT1;

        var success = result.AsT0.Value;
        var bottomTrim = lines.Length - success.Offset.Y - success.Glyph.GetLength(1);
        var newBaseline = baseline - bottomTrim;

        return new Success<NKGlyph>(new NKGlyph(success.Glyph, newBaseline));
    }

    private static OneOf<Success<GlyphDefNode>, Error<string>> ResolveReference(
        string                                  @ref,
        TextStyles                              styles,
        Dictionary<NKGlyphSymbol, GlyphDefNode> symbolTable,
        Dictionary<string,        GlyphDefNode> idTable) 
    {
        // see Docs/Engineering/Fonts/Xml/Xml-Font-Compilation.md
        // section 2.1 Reference resolution
        
        if (idTable.TryGetValue(@ref, out var n))
            return new Success<GlyphDefNode>(n);

        if (@ref.Length == 1) {
            var refChar = @ref[0];

            if (symbolTable.TryGetValue(NKGlyphSymbol.Simple(refChar, (NKStyle)styles), out var s))
                return new Success<GlyphDefNode>(s);

            var matches = symbolTable
                .Where(kvp => kvp.Key.IsSimple && kvp.Key.SimpleSymbol == refChar)
                .ToList();

            return GetBestMatch(matches, styles);
        }
        else {
            if (symbolTable.TryGetValue(NKGlyphSymbol.Ligature(@ref, (NKStyle)styles), out var s)) 
                return new Success<GlyphDefNode>(s);
            
            var matches = symbolTable
                .Where(kvp => kvp.Key.IsLigature && kvp.Key.LigatureSymbol == @ref)
                .ToList();

            return GetBestMatch(matches, styles);
        }
    }

    private static OneOf<Success<GlyphDefNode>, Error<string>> GetBestMatch(
        List<KeyValuePair<NKGlyphSymbol, GlyphDefNode>> matches, 
        TextStyles                                      styles) 
    {
        var           maxMatch = 0;
        GlyphDefNode? match    = null;

        foreach (var l in matches) {
            var mask = ~(l.Key.Styles.Styles ^ styles);
            var score = Numeric.PopCount((byte)mask);

            if (score <= maxMatch) continue;

            maxMatch = score;
            match = l.Value;
        }

        if (maxMatch == 0 && matches.Count > 0)
            return new Success<GlyphDefNode>(matches[0].Value);

        if (match is null)
            return new Error<string>("Could not resolve glyph reference.");
            
        return new Success<GlyphDefNode>(match);
    }

    private static OneOf<Success<NKGlyph>, Error<string>> CompileCompoundGlyph(
        NKGlyph        main,
        NKGlyph        secondary,
        XmlGlyphAlign? align) 
    {
        if (align is not { } a) {
            return new Error<string>("Could not resolve glyph alignment.");
        }
                    
        var glyph = a.IsDirection 
            ? NKGlyph.Combine(main, secondary, a.Direction)
            : NKGlyph.Combine(main, secondary, a.Char);

        if (glyph is null) {
            return new Error<string>("Could not combine glyphs.");
        }
        
        return new Success<NKGlyph>(glyph);
    }

    private static NKFontInfo CreateFontInfo(XmlFontConfig config) {
        return config.FontSpacing switch {
            XmlMonospacedFontConfig mono => new NKFontInfo(
                config.Name, 
                config.Ligatures, 
                config.Leading, 
                config.LetterSpacing, 
                config.LetterSpacing,
                new NKFontMonospacedConfig(mono.LetterWidth, mono.LetterHeight, mono.AlignToGrid)
            ),
            XmlVariableFontConfig variable => new NKFontInfo(
                config.Name, 
                config.Ligatures, 
                config.Leading, 
                config.LetterSpacing, 
                variable.WordSpacing, 
                new NKFontVariableConfig(variable.Kerning)
            ),
            _ => throw FontSerializerException.InvalidSpacingInfo()
        };
    }


    // ------------------------------------------------------------------------ //
    //                           XML DESERIALIZATION                            //
    // ------------------------------------------------------------------------ //
    
    
    private static XmlFontManifest DeserializeManifest(Stream content) => 
        Deserialize<XmlFontManifest>(content, FontSerializerException.ManifestDeserializationFailed);

    private static XmlFontConfig DeserializeConfig(Stream content) => 
        Deserialize<XmlFontConfig>(content, FontSerializerException.ConfDeserializationFailed);

    private static XmlFontMap DeserializeMap(Stream content) => 
        Deserialize<XmlFontMap>(content, FontSerializerException.MapDeserializationFailed);
    
    private static async Task<XmlFontManifest> DeserializeManifestAsync(Stream content) => 
        await DeserializeAsync<XmlFontManifest>(content, FontSerializerException.ManifestDeserializationFailed);

    private static async Task<XmlFontConfig> DeserializeConfigAsync(Stream content) => 
        await DeserializeAsync<XmlFontConfig>(content, FontSerializerException.ConfDeserializationFailed);

    private static async Task<XmlFontMap> DeserializeMapAsync(Stream content) => 
        await DeserializeAsync<XmlFontMap>(content, FontSerializerException.MapDeserializationFailed);


    // --------------------------------------------------------------------------- //
    //                         HELPER METHODS AND CLASSES                          //
    // --------------------------------------------------------------------------- //


    /// <summary>
    /// Deserializes an object of the specified type <typeparamref name="TResult"/> from a given XML stream.
    /// </summary>
    /// <typeparam name="TResult">The type of the object to be deserialized.</typeparam>
    /// <param name="stream">The input stream containing the XML content to deserialize.</param>
    /// <param name="fail">Creates an exception in case of deserialization failure.</param>
    /// <returns>An instance of <typeparamref name="TResult"/> deserialized from the provided stream.</returns>
    /// <exception cref="XmlException">Thrown when deserialization fails due to malformed XML or an error in the
    /// XML content.</exception>
    private static TResult Deserialize<TResult>(Stream stream, Func<Exception> fail) {
        var serializer = new XmlSerializer(typeof(TResult));

        using var reader = new XmlIgnoreNamespaceReader(stream);

        return (TResult)(serializer.Deserialize(reader) ?? throw fail());
    }
    
    /// <summary>
    /// Deserializes an object of the specified type <typeparamref name="TResult"/> from a given XML stream.
    /// </summary>
    /// <typeparam name="TResult">The type of the object to be deserialized.</typeparam>
    /// <param name="stream">The input stream containing the XML content to deserialize.</param>
    /// <param name="fail">Creates an exception in case of deserialization failure.</param>
    /// <returns>An instance of <typeparamref name="TResult"/> deserialized from the provided stream.</returns>
    /// <exception cref="XmlException">Thrown when deserialization fails due to malformed XML or an error in the
    /// XML content.</exception>
    private static ValueTask<TResult> DeserializeAsync<TResult>(Stream stream, Func<Exception> fail) {
        var serializer = new XmlSerializer(typeof(TResult));

        using var reader = new XmlIgnoreNamespaceReader(stream);

        return new ValueTask<TResult>((TResult)(serializer.Deserialize(reader) ?? throw fail()));
    }
    
    private class XmlIgnoreNamespaceReader : XmlTextReader {
        public XmlIgnoreNamespaceReader(Stream input) : base(input) { }

        public override string NamespaceURI => string.Empty;
    }
    
    private class GlyphDefNode {
        public GlyphDefNodeState State { get; set; }
        public XmlGlyphDef       Def   { get; }
        public string[]?         Lines { get; }
        public NKGlyph?          Glyph { get; set; }

        public GlyphDefNode(XmlGlyphDef def, string[]? lines) {
            State = GlyphDefNodeState.UNVISITED;
            Def   = def;
            Lines = lines;
            Glyph = null;
        }
    }

    private enum GlyphDefNodeState {
        UNVISITED,
        VISITING,
        PROCESSED
    }
}