// NeoKolors
// Copyright (c) 2025 KryKom

#define NKFONT_XSD_LOCAL

using System.IO.Compression;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NeoKolors.Tui.Fonts.Serialization.Xml;
using NeoKolors.Tui.Fonts.Exceptions;

namespace NeoKolors.Tui.Fonts.Serialization;

public static class NKFontSerializer {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(NKFontSerializer));

    // Do not ask me why I did this. I know it is not necessary,
    // but WHO DA FUCK ACTUALLY CARES?
    // IT'S MANAGED AT COMPILE TIME ANYWAY!!! 
    private const string CONF_SCHEMA_URL = NKFontSchema.SCHEMA_NAMESPACE;
    private const string MAP_SCHEMA_URL = NKFontSchema.SCHEMA_NAMESPACE;
    
    private static XmlSchemaSet? CONFIG_SCHEMA;
    private static XmlSchemaSet? MAP_SCHEMA;
    
    static NKFontSerializer() => (CONFIG_SCHEMA, MAP_SCHEMA) = InitSchemas();

    /// <summary>
    /// Reloads the configuration and mapping schemas used for font serialization.
    /// Allows specifying optional paths to local schema files for debugging or testing purposes.
    /// </summary>
    /// <param name="confPath">The optional path to a local configuration schema file. If null,
    /// the schema is retrieved asynchronously.</param>
    /// <param name="mapPath">The optional path to a local mapping schema file. If null, the schema
    /// is retrieved asynchronously.</param>
    public static void Reload(string? confPath = null, string? mapPath = null) {
        CONFIG_SCHEMA = confPath is not null ? GetConfigSchemaLocal(confPath) : GetConfigSchema();
        MAP_SCHEMA    = mapPath  is not null ? GetMapSchemaLocal(mapPath)     : GetMapSchema();
    }
    
    private static readonly Lazy<Task<(XmlSchemaSet ConfigSchema, XmlSchemaSet MapSchema)>> SCHEMA_INITIALIZATION_LAZY 
        = new(InitializeSchemasAsync);

    private static readonly Lazy<(XmlSchemaSet ConfigSchema, XmlSchemaSet MapSchema)> SCHEMA_INIT
        = new(InitSchemas);

    private static (XmlSchemaSet ConfigSchema, XmlSchemaSet MapSchema) InitSchemas() {
        var assembly = Assembly.GetExecutingAssembly();
        const string configResource = "NeoKolors.Tui...Schemas.Fonts.V2.Font.Config.xsd";
        const string mapResource    = "NeoKolors.Tui...Schemas.Fonts.V2.Font.Map.xsd";
        
        LOGGER.Info("Loading font XSDs...");
        
        LOGGER.Trace("Reading font config XSD...");
        
        using var cStream = assembly.GetManifestResourceStream(configResource);
        if (cStream == null)
            throw FontSerializerException.CouldNotLoadConfigXsd();
            
        XmlSchemaSet configSchema = new();
        
        LOGGER.Trace("Decompiling font config XSD...");
        
        using (var xsdReader = XmlReader.Create(cStream)) {
            configSchema.Add(null, xsdReader);
        }
        
        LOGGER.Trace("Reading font map XSD...");
        
        using var mStream = assembly.GetManifestResourceStream(mapResource);
        if (mStream == null)
            throw FontSerializerException.CouldNotLoadMapXsd();
         
        LOGGER.Trace("Decompiling font map XSD...");
        
        XmlSchemaSet mapSchema = new();
        
        using (var xsdReader = XmlReader.Create(mStream)) {
            mapSchema.Add(null, xsdReader);
        }
        
        LOGGER.Info("Finished loading font XSDs.");
        
        return (configSchema, mapSchema);
    }
    
    private static async Task<(XmlSchemaSet ConfigSchema, XmlSchemaSet MapSchema)> InitializeSchemasAsync() {
        LOGGER.Info("Downloading font XSD's...");
        
        using var http = new HttpClient();
        
        var cfgTask = http.GetStringAsync(CONF_SCHEMA_URL);
        var mapTask = http.GetStringAsync(MAP_SCHEMA_URL);

        await Task.WhenAll(cfgTask, mapTask);
        
        var cfg = await cfgTask;
        var map = await mapTask;
        
        LOGGER.Trace("Finished downloading font XSD's.");
        
        var configSchema = new XmlSchemaSet();
        var mapSchema = new XmlSchemaSet();
        
        LOGGER.Info("Compiling font config XSD...");
        using (var stringReader = new StringReader(cfg))
        using (var xsdReader = XmlReader.Create(stringReader)) {
            configSchema.Add(null, xsdReader);
        }
        
        LOGGER.Info("Compiling font mapPath XSD...");
        using (var stringReader = new StringReader(map))
        using (var xsdReader = XmlReader.Create(stringReader)) {
            mapSchema.Add(null, xsdReader);
        }
        
        LOGGER.Info("Finished compiling font XSD's.");
        
        return (configSchema, mapSchema);
    }

    private static async Task<XmlSchemaSet> GetConfigSchemaAsync() {
        var schemas = await SCHEMA_INITIALIZATION_LAZY.Value;
        return schemas.ConfigSchema;
    }

    private static async Task<XmlSchemaSet> GetMapSchemaAsync() {
        var schemas = await SCHEMA_INITIALIZATION_LAZY.Value;
        return schemas.MapSchema;
    }
    
    private static XmlSchemaSet GetConfigSchema() => SCHEMA_INIT.Value.ConfigSchema;
    private static XmlSchemaSet GetMapSchema() => SCHEMA_INIT.Value.MapSchema;
    
    //
    // Methods for local XSD loading.
    // This shall not be used in release builds.
    // ONLY FOR DEBUGGING AND TESTING PURPOSES!!!
    //
    
    private static XmlSchemaSet GetConfigSchemaLocal(string path) {
        LOGGER.Info("Reading font config XSD...");
        
        string raw = File.ReadAllText(path);
        
        LOGGER.Info("Compiling font config XSD...");
        
        var configSchema = new XmlSchemaSet();
        
        using (var stringReader = new StringReader(raw))
        using (var xsdReader = XmlReader.Create(stringReader)) {
            configSchema.Add(null, xsdReader);
        }
        
        LOGGER.Info("Finished reading font config XSD.");
        
        return configSchema;
    }
    
    private static XmlSchemaSet GetMapSchemaLocal(string path) {
        LOGGER.Info("Reading font map XSD...");
        
        string raw = File.ReadAllText(path);
        
        LOGGER.Info("Compiling font map XSD...");
        
        var mapSchema = new XmlSchemaSet();
        
        using (var stringReader = new StringReader(raw))
        using (var xsdReader = XmlReader.Create(stringReader)) {
            mapSchema.Add(null, xsdReader);
        }
        
        LOGGER.Info("Finished reading font config XSD.");
        
        return mapSchema;
    }
    

    // ----------------------------------------------------------- //
    //                        FONT LOADING                         //
    // ----------------------------------------------------------- //

    private const string CONFIG_FILE_NAME = "conf.nkfc";
    private const string MAP_FILE_NAME = "map.nkfm";
    private const string MANIFEST_FILE_NAME = "manifest.nkfont";
    
    
    // --------------------- PUBLIC METHODS --------------------- //
    
    /// <summary>
    /// Reads and deserializes an NKFont object from the specified path.
    /// The path can reference either a file or a directory.
    /// </summary>
    /// <param name="path">The path to the font resource, which can be a file or directory.</param>
    /// <returns>An <see cref="NKFont"/> object deserialized from the specified path.</returns>
    public static NKFont Read(string path) {
        if (path.StartsWith('.'))
            return ReadLocal(path);
        
        Uri uri;

        try {
            uri = new Uri(path);
        }
        catch (UriFormatException e) {
            throw FontSerializerException.CannotResolveUri(e);
        }
        
        return uri.HostNameType == UriHostNameType.Basic 
            ? ReadLocal(path) 
            : ReadExternal(path);
    }

    /// <summary>
    /// Reads and deserializes an NKFont object from the specified .nkf file path.
    /// </summary>
    /// <param name="path">The path to the font file.</param>
    /// <returns>An <see cref="NKFont"/> object deserialized from the specified file.</returns>
    /// <exception cref="FontSerializerException">Thrown if the specified file path is invalid
    /// or the font file cannot be loaded.</exception>
    public static NKFont ReadFile(string path) {
        if (!File.Exists(path))
            throw FontSerializerException.InvalidPath(path);

        using var stream = File.OpenRead(path);
        return ExtractZip(stream);
    }
    
    /// <summary>
    /// Reads and deserializes an NKFont object from a directory by processing its configuration
    /// and mapping files. The directory must contain a valid configuration file ("conf.nkfc")
    /// and a mapping file ("Map.xml").
    /// </summary>
    /// <param name="path">The path to the directory containing the font files.</param>
    /// <returns>An <see cref="NKFont"/> object constructed from the provided directory structure.</returns>
    /// <exception cref="FontSerializerException">Thrown when required files ("conf.nkfc" or "Map.xml") are missing
    /// in the specified directory, or if file deserialization fails.</exception>
    /// <exception cref="NotImplementedException">Thrown when functionality for reading font files
    /// has not yet been implemented.</exception>
    public static NKFont ReadDir(string path) {
        string manifestPath = Path.Combine(path, MANIFEST_FILE_NAME);

        FontConfig config;
        FontMap map;
        
        if (File.Exists(manifestPath)) {
            var manifest = DeserializeManifestFromFile(manifestPath);
            
            string configPath = Path.Combine(path, manifest.Config);
            if (!File.Exists(configPath))
                throw FontSerializerException.ConfigNotFound(path);

            string mapPath = Path.Combine(path, manifest.Map);
            if (!File.Exists(mapPath))
                throw FontSerializerException.MapNotFound(path);

            config = DeserializeConfFromFile(configPath);
            map = DeserializeMapFromFile(mapPath);
        }
        else {
            string configPath = Path.Combine(path, CONFIG_FILE_NAME);
            if (!File.Exists(configPath))
                throw FontSerializerException.ConfigNotFound(path);

            string mapPath = Path.Combine(path, MAP_FILE_NAME);
            if (!File.Exists(mapPath))
                throw FontSerializerException.MapNotFound(path);

            config = DeserializeConfFromFile(configPath);
            map = DeserializeMapFromFile(mapPath);
        }

        map.SetDefaults(config);
        
        var glyphs = LoadGlyphsDir(map, path);

        return CreateNKFont(config, glyphs);
    }

    /// <summary>
    /// Downloads and deserializes an NKFont object from an external source specified by the provided path.
    /// This method synchronously waits for the asynchronous operation to complete.
    /// </summary>
    /// <param name="path">The URL from which the font resource will be downloaded.
    /// It should point to a valid location for the font.</param>
    /// <returns>An <see cref="NKFont"/> object deserialized from the downloaded resource.</returns>
    public static NKFont ReadExternal(string path) => ReadExternalAsync(path).Result;

    /// <summary>
    /// Downloads and deserializes an NKFont object from an external source specified by the provided path.
    /// The path should point to a valid URL for downloading the font resource.
    /// </summary>
    /// <param name="path">The URL from which the font resource will be downloaded.</param>
    /// <returns>A <see cref="NKFont"/> object deserialized from the downloaded resource.</returns>
    public static async Task<NKFont> ReadExternalAsync(string path) {
        LOGGER.Info("Downloading font from '{0}'...", path);
        
        Stream stream;

        using (var http = new HttpClient())
            stream = await http.GetStreamAsync(path);

        return ExtractZip(stream);
    }

    /// <summary>
    /// Reads an embedded font resource from the assembly's manifest and extracts it.
    /// Intended for internal use to load resources bundled within the application.
    /// </summary>
    /// <param name="path">The name of the embedded resource.</param>
    /// <param name="assembly">The assembly the resource is taken from.</param>
    /// <returns>An <see cref="NKFont"/> instance containing the deserialized font data if the resource is found, or null if not.</returns>
    public static NKFont? ReadEmbedded(string path, Assembly assembly) {
        using var stream = assembly.GetManifestResourceStream(path);
        if (stream == null) {
            LOGGER.Error("Could not find embedded font '{0}'.", path);
            return null;
        }

        return ExtractZip(stream);
    }

    /// <summary>
    /// Reads an embedded font resource from the specified assembly.
    /// The resource must be accessible as an embedded resource within the assembly.
    /// </summary>
    /// <param name="path">The path to the embedded font resource within the assembly.</param>
    /// <typeparam name="TAssemblyType">A type contained in the assembly the resource is taken from.</typeparam>
    /// <returns>An <see cref="NKFont"/> object representing the font, or null if the resource is not found.</returns>
    public static NKFont? ReadEmbedded<TAssemblyType>(string path) =>
        ReadEmbedded(path, typeof(TAssemblyType).Assembly);

    /// <summary>
    /// Reads an embedded NKFont resource from the specified name.
    /// This method resolves the embedded resource path based on the provided font name.
    /// </summary>
    /// <param name="name">The name of the embedded font resource to read.
    /// This should correspond to a valid built-in font resource within the assembly.</param>
    /// <returns>The deserialized <see cref="NKFont"/> object if the embedded resource is found and successfully resolved;
    /// otherwise, null if the resource cannot be located.</returns>
    internal static NKFont? ReadInternal(string name)
        => ReadEmbedded<NKFont>($"{typeof(NKFont).Assembly.GetName().Name}...Fonts.Builtin.{name}.nkf");

    // --------------------- PRIVATE METHODS --------------------- //
    
    /// <summary>
    /// Reads a local font from the specified path.
    /// The path can point to a single font file or a directory containing the font resources.
    /// </summary>
    /// <param name="path">The path to the local font resource, which can be a file or directory.</param>
    /// <returns>An <see cref="NKFont"/> object constructed from the specified local resources.</returns>
    private static NKFont ReadLocal(string path) {
        LOGGER.Info("Reading local font from '{0}'...", path);
        return Path.HasExtension(path) 
            ? ReadFile(path) 
            : ReadDir(path);
    }

    /// <summary>
    /// Loads glyph data based on the specified font configuration and map.
    /// Builds and returns an IGlyph instance containing the glyph representation.
    /// </summary>
    /// <param name="map">The font mapping structure that defines the glyph composition and their properties.</param>
    /// <param name="basePath">The base path of the font.</param>
    /// <returns>An <see cref="GlyphInfo"/> object array containing the loaded glyph data.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
    private static GlyphInfo[] LoadGlyphsDir(FontMap map, string basePath) {
        Dictionary<string, GlyphInfo> glyphs = [];
        
        // load ligatures and component glyphs
        foreach (var glyphInfo in map.GetItems()) {
            if (glyphInfo.IsComponent) {
                if (glyphs.ContainsKey(glyphInfo.Id))
                    LOGGER.Error("Duplicate glyph ID '{0}' found in font map.", glyphInfo.Id);
                else
                    glyphs.Add(glyphInfo.Id, NKGlyphReader.ReadComponent(basePath, glyphInfo.AsComponent));
            }
            else if (glyphInfo.IsLigature) {
                if (glyphs.ContainsKey(glyphInfo.Id))
                    LOGGER.Error("Duplicate glyph ID '{0}' found in font map.", glyphInfo.Id);
                else
                    glyphs.Add(glyphInfo.Id, NKGlyphReader.ReadLigature(basePath, glyphInfo.AsLigature));
            }
        }

        LinkCompounds(map, ref glyphs);
        
        return glyphs.Values.ToArray();
    }
    
    
    /// <summary>
    /// Extracts and deserializes an <see cref="NKFont"/> object from a ZIP archive stream.
    /// The ZIP archive must contain the required configuration and mapping files.
    /// </summary>
    /// <param name="stream">The stream representing the ZIP archive to be extracted.</param>
    /// <returns>An <see cref="NKFont"/> object deserialized from the extracted content.</returns>
    /// <exception cref="FontSerializerException">Thrown if the required configuration or map files are missing
    /// in the ZIP archive.</exception>
    private static NKFont ExtractZip(Stream stream) {
        LOGGER.Info("Unpacking font...");

        ZipArchive zip = new(stream, ZipArchiveMode.Read);

        // try to load manifest
        var manifestEntry = zip.GetEntry(MANIFEST_FILE_NAME);
        
        FontConfig config;
        FontMap map;

        if (manifestEntry is not null) {
            var manifest = DeserializeManifestFromStream(manifestEntry.Open());
            
            var configStream = zip.GetEntry(manifest.Config);
            if (configStream is null) throw FontSerializerException.ConfigNotFound();
            config = DeserializeConfFromStream(configStream.Open());

            var mapStream = zip.GetEntry(manifest.Map);
            if (mapStream is null) throw FontSerializerException.MapNotFound();
            map = DeserializeMapFromStream(mapStream.Open());
        } else {
            // load config
            var configStream = zip.GetEntry(CONFIG_FILE_NAME);

            if (configStream is null)
                throw FontSerializerException.ConfigNotFound();

            config = DeserializeConfFromStream(configStream.Open());
        
            // load map
            var mapStream = zip.GetEntry(MAP_FILE_NAME);
        
            if (mapStream is null)
                throw FontSerializerException.MapNotFound();
        
            map = DeserializeMapFromStream(mapStream.Open());
        }
        
        map.SetDefaults(config);
        
        var glyphs = LoadGlyphsZip(map, zip);
        
        return CreateNKFont(config, glyphs);
    }


    /// <summary>
    /// Loads glyphs defined in the provided font map from the specified zip archive.
    /// Processes both ligatures and component glyphs, linking compound glyphs as needed.
    /// </summary>
    /// <param name="map">The font map containing the glyph definitions to be processed.</param>
    /// <param name="zip">The zip archive from which the glyph data is loaded.</param>
    /// <returns>An array of <see cref="GlyphInfo"/> objects representing the loaded glyphs.</returns>
    private static GlyphInfo[] LoadGlyphsZip(FontMap map, ZipArchive zip) {
        var glyphs = new Dictionary<string, GlyphInfo>();

        // load ligatures and component glyphs
        foreach (var glyphInfo in map.GetItems()) {
            if (glyphInfo.TryAsComponent(out var g)) {
                if (glyphs.ContainsKey(g.Id)) {
                    LOGGER.Error("Duplicate glyph ID '{0}' found in font map.", g.Id);
                    continue;
                }
                
                glyphs.Add(g.Id, NKGlyphReader.ReadComponent(zip, g));
            }
            else if (glyphInfo.TryAsLigature(out var l)) {
                if (glyphs.ContainsKey(l.Id)) {
                    LOGGER.Error("Duplicate glyph ID '{0}' found in font map.", l.Id);
                    continue;
                }

                glyphs.Add(l.Id, NKGlyphReader.ReadLigature(zip, l));
            }
        }
        
        LinkCompounds(map, ref glyphs);
        
        return glyphs.Values.ToArray();
    }


    /// <summary>
    /// Links compound glyphs by processing the font map and combining main and secondary glyphs.
    /// Updates the provided dictionary of glyphs with compound and auto-compound glyphs.
    /// </summary>
    /// <param name="map">The font map containing glyph information to be processed.</param>
    /// <param name="glyphs">A reference to the dictionary of glyphs that will be updated with newly created
    /// compound glyphs.</param>
    private static void LinkCompounds(FontMap map, ref readonly Dictionary<string, GlyphInfo> glyphs) {
        
        // load compound glyphs
        foreach (var glyphInfo in map.GetItems()) {
            if (!glyphInfo.IsCompound) continue;
            
            var g = glyphInfo.AsCompound;
            
            if (glyphs.ContainsKey(g.Id)) {
                LOGGER.Error("Duplicate compound glyph ID '{0}' found in font map.", g.Id);
                continue;
            }

            if (!glyphs.ContainsKey(g.Main)) {
                LOGGER.Error("Main glyph ID '{0}' not found in font map for compound glyph ID '{1}'.", 
                    g.Main, g.Id);
                continue;
            }

            if (!glyphs.ContainsKey(g.Secondary)) {
                LOGGER.Error("Secondary glyph ID '{0}' not found in font map for compound glyph ID '{1}'.", 
                    g.Secondary, g.Id);
                continue;
            }
            
            var main = glyphs[g.Main];
            var nd = glyphs[g.Secondary];
            
            glyphs.Add(
                glyphInfo.Id, 
                new SimpleGlyphInfo(
                    new NKCompoundGlyph(main.Glyph, nd.Glyph, glyphInfo.AsCompound.Align.Convert()), 
                    NKGlyphReader.ParseSymbol(glyphInfo.Symbol)
                )
            );
        }
        
        // load auto-compound glyphs
        foreach (var glyphInfo in map.GetItems()) {
            if (!glyphInfo.IsAutoCompound) continue;

            var g = glyphInfo.AsAutoCompound;

            if (glyphs.ContainsKey(g.Id))
                LOGGER.Error("Duplicate auto-compound glyph ID '{0}' found in font map.", g.Id);

            if (!glyphs.TryGetValue(g.BaseId, out var main)) {
                LOGGER.Error("Base glyph ID '{0}' not found in font map for auto-compound glyph ID '{1}'.", 
                    g.BaseId, g.Id);
                continue;
            }

            if (glyphInfo.IsAutoCompound) {
                glyphs.Add(glyphInfo.Id, new AutoCompoundGlyphInfo(
                    main.Glyph,
                    NKGlyphReader.ParseSymbol(g.Symbol),
                    g.Align.Convert(),
                    g.Applicable,
                    g.MainFirst
                ));
            }
        }
    }
    

    /// <summary>
    /// Creates an <see cref="NKFont"/> instance using the provided font configuration and glyph information.
    /// </summary>
    /// <param name="config">The font configuration object containing metadata such as name, spacing, and other
    /// font properties.</param>
    /// <param name="glyphs">An array of <see cref="GlyphInfo"/> representing the set of glyphs for the font.</param>
    /// <returns>A new <see cref="NKFont"/> object constructed with the given configuration and glyph data.</returns>
    /// <exception cref="FontSerializerException">
    /// Thrown when invalid or missing properties are detected in the font configuration, such as a null or
    /// invalid font name.
    /// </exception>
    private static NKFont CreateNKFont(FontConfig config, GlyphInfo[] glyphs) {
        return new NKFont(
            new NKFontInfo(
                (Equals(config.Name, null)
                    ? throw FontSerializerException.InvalidFontName("<null>") 
                    : config.Name)!,
                config.Item switch {
                    null => throw FontSerializerException.InvalidSpacingInfo(),
                    Monospace m => new MonospaceInfo(m.LetterWidth, m.LetterHeight, m.AlignToGrid),
                    Variable v => new VariableInfo(v.Kerning, v.LineKerning, v.WordSpacing, v.EmptyLineHeight),
                    _ => throw FontSerializerException.InvalidSpacingInfo()
                },
                config.Ligatures,
                config.LetterSpacing,
                config.LineSpacing
            ),
            glyphs
        );
    }
    
    
    // ----------------------------------------------------------- //
    //                    XML DESERIALIZATION                      //
    // ----------------------------------------------------------- //
    
    /// <summary>
    /// Deserializes a font configuration file into a <see cref="FontConfig"/> object from the specified file path.
    /// </summary>
    /// <param name="filePath">The path of the font configuration file to be deserialized.</param>
    /// <returns>A deserialized <see cref="FontConfig"/> object containing the font configuration data.</returns>
    /// <exception cref="FontSerializerException">Thrown if deserialization of the font configuration file fails.</exception>
    public static FontConfig DeserializeConfFromFile(string filePath) {
        try {
            var serializer = new XmlSerializer(typeof(FontConfig));

            using var fileStream = new FileStream(filePath, FileMode.Open);
            return (FontConfig)(serializer.Deserialize(fileStream)
                                ?? throw FontSerializerException.ConfDeserializationFailed());
        }
        catch (Exception x) {
            throw FontSerializerException.DeserializationError(filePath, x);
        }
    }

    
    /// <summary>
    /// Deserializes a font configuration from the specified XML string into a <see cref="FontConfig"/> object.
    /// </summary>
    /// <param name="xmlContent">The XML string containing the font configuration data to be deserialized.</param>
    /// <returns>A deserialized <see cref="FontConfig"/> object containing the font configuration data.</returns>
    /// <exception cref="FontSerializerException">Thrown if deserialization of the font configuration fails.</exception>
    public static FontConfig DeserializeConfFromString(string xmlContent) {
        var serializer = new XmlSerializer(typeof(FontConfig));

        using var stringReader = new StringReader(xmlContent);
        return (FontConfig)(serializer.Deserialize(stringReader) 
                            ?? throw FontSerializerException.ConfDeserializationFailed());
    }
    
    
    /// <summary>
    /// Deserializes a font configuration from the specified XML stream into a <see cref="FontConfig"/> object.
    /// </summary>
    /// <param name="xmlContent">The XML stream containing the font configuration data to be deserialized.</param>
    /// <returns>A deserialized <see cref="FontConfig"/> object containing the font configuration data.</returns>
    /// <exception cref="FontSerializerException">Thrown if deserialization of the font configuration fails.</exception>
    public static FontConfig DeserializeConfFromStream(Stream xmlContent) {
        var serializer = new XmlSerializer(typeof(FontConfig));
        
        return (FontConfig)(serializer.Deserialize(xmlContent) 
                            ?? throw FontSerializerException.ConfDeserializationFailed());
    }

    
    /// <summary>
    /// Deserializes a font mapping file into a <see cref="FontMap"/> object from the specified file path.
    /// </summary>
    /// <param name="filePath">The path of the font mapping file to be deserialized.</param>
    /// <returns>A deserialized <see cref="FontMap"/> object containing the font mappings data.</returns>
    /// <exception cref="FontSerializerException">Thrown if deserialization of the font mapping file fails.</exception>
    public static FontMap DeserializeMapFromFile(string filePath) {
        try {
            var serializer = new XmlSerializer(typeof(FontMap));

            using var fileStream = new FileStream(filePath, FileMode.Open);
            return (FontMap)(serializer.Deserialize(fileStream)
                             ?? throw FontSerializerException.MapDeserializationFailed());
        }
        catch (Exception x) {
            throw FontSerializerException.DeserializationError(filePath, x);
        }
    }

    
    /// <summary>
    /// Deserializes a font map from an XML string representation into a <see cref="FontMap"/> object.
    /// </summary>
    /// <param name="xmlContent">The XML string content representing the font map to be deserialized.</param>
    /// <returns>A deserialized <see cref="FontMap"/> object containing font mapping data.</returns>
    /// <exception cref="FontSerializerException">Thrown if deserialization of the font map fails.</exception>
    public static FontMap DeserializeMapFromString(string xmlContent) {
        var serializer = new XmlSerializer(typeof(FontMap));

        using var stringReader = new StringReader(xmlContent);
        return (FontMap)(serializer.Deserialize(stringReader) 
                         ?? throw FontSerializerException.MapDeserializationFailed());
    }

    
    /// <summary>
    /// Deserializes a font map from the specified XML stream into a <see cref="FontMap"/> object.
    /// </summary>
    /// <param name="xmlContent">The XML stream containing the font map data to be deserialized.</param>
    /// <returns>A deserialized <see cref="FontMap"/> object containing the font mapping data.</returns>
    /// <exception cref="FontSerializerException">Thrown if deserialization of the font map stream fails.</exception>
    public static FontMap DeserializeMapFromStream(Stream xmlContent) {
        var serializer = new XmlSerializer(typeof(FontMap));

        return (FontMap)(serializer.Deserialize(xmlContent) 
                         ?? throw FontSerializerException.MapDeserializationFailed());
    }

    /// <summary>
    /// Deserializes a font manifest file into a <see cref="FontManifest"/> object from the specified file path.
    /// </summary>
    /// <param name="filePath">The path of the font manifest file to be deserialized.</param>
    /// <returns>A deserialized <see cref="FontManifest"/> object containing the font manifest data.</returns>
    /// <exception cref="FontSerializerException">Thrown if deserialization of the font manifest file fails.</exception>
    public static FontManifest DeserializeManifestFromFile(string filePath) {
        try {
            var serializer = new XmlSerializer(typeof(FontManifest));

            using var fileStream = new FileStream(filePath, FileMode.Open);
            return (FontManifest)(serializer.Deserialize(fileStream)
                                  ?? throw FontSerializerException.ManifestDeserializationFailed());
        }
        catch (Exception x) {
            throw FontSerializerException.DeserializationError(filePath, x);
        }
    }
    
    /// <summary>
    /// Deserializes a font manifest from the specified XML stream into a <see cref="FontManifest"/> object.
    /// </summary>
    /// <param name="xmlContent">The XML stream containing the font manifest data to be deserialized.</param>
    /// <returns>A deserialized <see cref="FontManifest"/> object containing the font manifest data.</returns>
    /// <exception cref="FontSerializerException">Thrown if deserialization of the font manifest stream fails.</exception>
    public static FontManifest DeserializeManifestFromStream(Stream xmlContent) {
        var serializer = new XmlSerializer(typeof(FontManifest));

        return (FontManifest)(serializer.Deserialize(xmlContent) 
                              ?? throw FontSerializerException.ManifestDeserializationFailed());
    }
    
    
    // ----------------------------------------------------------- //
    //                      FONT ARCHIVATION                       //
    // ----------------------------------------------------------- //

    /// <summary>
    /// Compresses the specified directory into a ZIP archive at the given output path.
    /// </summary>
    /// <param name="path">The path of the directory to be compressed. Must be an existing directory.</param>
    /// <param name="outputPath">The path where the compressed ZIP archive will be saved.</param>
    /// <exception cref="FontSerializerException">Thrown when the specified path is not a directory.</exception>
    public static void CreateArchive(string path, string outputPath) {
        if (!Directory.Exists(path))
            throw FontSerializerException.ZipInPathMustBeDir(path);
        
        if (File.Exists(outputPath))
            File.Delete(outputPath);
        
        ZipFile.CreateFromDirectory(path, outputPath, CompressionLevel.Optimal, false);
    }
}