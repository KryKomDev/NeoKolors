// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts.Exceptions;

public sealed class FontSerializerException : Exception {
    
    private const string H_LINK = "https://krykomdev.github.io/NeoKolors/tui/Fonts/";
    
    private FontSerializerException(string message, string link = "", Exception? inner = null) : base(message, inner) {
        if (link != "") HelpLink = link;
    }
    
    public static FontSerializerException InvalidWordSpacing(string raw) 
        => new($"Invalid word spacing. Expected int or 'space-char' got '{raw}'.");
    
    public static FontSerializerException InvalidMask(string raw)
        => new($"Invalid mask '{raw}'.", H_LINK + ""); // TODO: link

    public static FontSerializerException InvalidGlyphAlign(string raw)
        => new($"Invalid align '{raw}'.", H_LINK + "");
    
    public static FontSerializerException InvalidAlignPointReplace(string raw) 
        => new($"Invalid align point replace mode '{raw}'.", H_LINK + "");
    
    public static FontSerializerException DuplicateGlyphId(string id) 
        => new($"Duplicate glyph id '{id}'.", H_LINK + "");
    
    public static FontSerializerException InvalidSymbol(string symbol) 
        => new($"Invalid symbol '{symbol}'.", H_LINK + "");
    
    public static FontSerializerException ConfDeserializationFailed()
        => new("Deserialization of font configuration file failed.");
    
    public static FontSerializerException MapDeserializationFailed()
        => new("Deserialization of font mappings file failed.");

    public static FontSerializerException ManifestDeserializationFailed()
        => new("Deserialization of font manifest file failed.");
    
    public static FontSerializerException InvalidFontName(string name) 
        => new($"Invalid font name '{name}'.");
    
    public static FontSerializerException ConfigNotFound(string? path = null)
        => new($"Font configuration file not found{(path is null ? "" : " in directory '" + path + "'")}.");
    
    public static FontSerializerException MapNotFound(string? path = null)
        => new($"Font mappings file not found{(path is null ? "" : " in directory '" + path + "'")}.");

    public static FontSerializerException ManifestNotFound(string? path = null)
        => new($"Font manifest file not found{(path is null ? "" : " in directory '" + path + "'")}.");
        
    public static FontSerializerException CannotResolveUri(UriFormatException inner)
        => new("Cannot resolve font URI.", inner: inner);
    
    public static FontSerializerException InvalidPath(string path)
        => new($"Cannot load font. Path '{path}' is invalid.");
    
    public static FontSerializerException ZipInPathMustBeDir(string path)
        => new($"The input path of font serializer must be a directory. '{path}' is not a directory.");
    
    public static FontSerializerException InvalidMapItem(Type type) 
        => new($"Invalid font map item: type '{type.Name}'.");

    public static FontSerializerException UnreadableStream() 
        => new("Cannot read font stream.");
    
    public static FontSerializerException InvalidSymbolFormat(string s) 
        => new($"Invalid symbol format: '{s}'. " +
               $"Symbol definition must be a single character or hexadecimal character definition.");
    
    public static FontSerializerException InvalidSpacingInfo() 
        => new("Invalid spacing info.");
    
    public static FontSerializerException CouldNotLoadConfigXsd() 
        => new("Could not load font configuration XSD.");
    
    public static FontSerializerException CouldNotLoadMapXsd() 
        => new("Could not load font mappings XSD.");
    
    public static FontSerializerException DeserializationError(string path, Exception inner) 
        => new($"An error occurred while reading font file {path}.", H_LINK, inner);
}