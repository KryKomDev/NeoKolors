// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts.V2.Serialization;

public static class NKFontSchema {
    
    /// <summary>
    /// Specifies the current version of the NeoKolors font schema.
    /// </summary>
    public const string VERSION = "2";
    
    private const string RELEASE_SCHEMA_NAMESPACE = $"https://raw.githubusercontent.com/KryKomDev/NeoKolors/refs/heads/main/NeoKolors.Tui/Schemas/Fonts/V{VERSION}/";
    
    // The WIP branch is used for development and testing new features and changes.
    private const string WIP_SCHEMA_NAMESPACE = $"https://raw.githubusercontent.com/KryKomDev/NeoKolors/refs/heads/wip/NeoKolors.Tui/Schemas/Fonts/V{VERSION}/";
    
    // The snapshot branch is used for testing and development.
    private const string SNAPSHOT_SCHEMA_NAMESPACE = $"https://raw.githubusercontent.com/KryKomDev/NeoKolors/refs/heads/snapshot/NeoKolors.Tui/Schemas/Fonts/V{VERSION}/";
    
    // Pozn. autora: už mi z tech padělanejch xsd jebe.
    // Asi se z toho zabiju už fakt.
    
    /// <summary>
    /// The base URL for the schema location of NeoKolors font definitions. This constant provides
    /// the foundation for constructing fully qualified URLs to various XML schema files used
    /// for font configuration and mapping in NeoKolors.
    /// </summary>
    public const string SCHEMA_NAMESPACE = RELEASE_SCHEMA_NAMESPACE;
        // #if RELEASE
        //     RELEASE_SCHEMA_NAMESPACE;
        // #elif WIP
        //     WIP_SCHEMA_NAMESPACE; 
        // #else
        //     SNAPSHOT_SCHEMA_NAMESPACE; 
        // #endif

    /// <summary>
    /// Defines the base URL for the XML schema location of NeoKolors font definitions,
    /// dynamically switching between release and work-in-progress namespaces depending on the build configuration.
    /// This constant serves as the root for constructing URLs to specific schema files.
    /// </summary>
    public const string SCHEMA_LOCATION = 
        #if RELEASE
            RELEASE_SCHEMA_NAMESPACE;
        #elif WIP
            WIP_SCHEMA_NAMESPACE; 
        #else
            SNAPSHOT_SCHEMA_NAMESPACE; 
        #endif
        
    public const string COMMON_SCHEMA_LOCATION = SCHEMA_LOCATION + "NeoKolors.Font.Common.xsd";
    public const string MAP_SCHEMA_LOCATION    = SCHEMA_LOCATION + "NeoKolors.Font.Map.xsd";
    public const string CONFIG_SCHEMA_LOCATION = SCHEMA_LOCATION + "NeoKolors.Font.Config.xsd";
}