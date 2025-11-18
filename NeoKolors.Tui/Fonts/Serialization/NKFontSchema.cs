// NeoKolors
// Copyright (c) 2025 KryKom

// This controls what schema location is used for the font definitions.
// Valid values are: NKFONT_XML_RELEASE, NKFONT_XML_SNAPSHOT, NKFONT_XML_WIP.
#define NKFONT_XML_WIP

namespace NeoKolors.Tui.Fonts.Serialization;

public static class NKFontSchema {
    
    /// <summary>
    /// Specifies the current version of the NeoKolors font schema.
    /// </summary>
    public const string VERSION = "2";
    
    private const string RELEASE_SCHEMA_NAMESPACE = $"https://raw.githubusercontent.com/KryKomDev/NeoKolors/refs/heads/main/NeoKolors.Tui/Schemas/Fonts/V{VERSION}";
    
    // The WIP branch is used for development and testing new features and changes.
    private const string WIP_SCHEMA_NAMESPACE = $"https://raw.githubusercontent.com/KryKomDev/NeoKolors/refs/heads/wip/NeoKolors.Tui/Schemas/Fonts/V{VERSION}";
    
    // The snapshot branch is used for testing and development.
    private const string SNAPSHOT_SCHEMA_NAMESPACE = $"https://raw.githubusercontent.com/KryKomDev/NeoKolors/refs/heads/snapshot/NeoKolors.Tui/Schemas/Fonts/V{VERSION}";
    
    // Pozn. autora: už mi z tech padělanejch xsd jebe.
    // Asi se z toho zabiju už fakt.
    
    /// <summary>
    /// The base URL for the schema location of NeoKolors font definitions. This constant provides
    /// the foundation for constructing fully qualified URLs to various XML schema files used
    /// for font configuration and mapping in NeoKolors.
    /// </summary>
    public const string SCHEMA_NAMESPACE = RELEASE_SCHEMA_NAMESPACE;
    
    /// <summary>
    /// Defines the base URL for the XML schema location of NeoKolors font definitions,
    /// dynamically switching between release and work-in-progress namespaces depending on the build configuration.
    /// This constant serves as the root for constructing URLs to specific schema files.
    /// </summary>
    public const string SCHEMA_LOCATION = 
        #if NKFONT_XML_RELEASE || RELEASE  // allow the RELEASE configuration to override any of the other settings
            RELEASE_SCHEMA_NAMESPACE;
        #elif NKFONT_XML_SNAPSHOT
            SNAPSHOT_SCHEMA_NAMESPACE;
        #elif NKFONT_XML_WIP
            WIP_SCHEMA_NAMESPACE; 
        #else
            RELEASE_SCHEMA_NAMESPACE;
        #endif
        
    public const string COMMON_SCHEMA_LOCATION = SCHEMA_LOCATION + "/Font.Common.xsd";
    public const string MAP_SCHEMA_LOCATION    = SCHEMA_LOCATION + "/Font.Map.xsd";
    public const string CONFIG_SCHEMA_LOCATION = SCHEMA_LOCATION + "/Font.Config.xsd";
}