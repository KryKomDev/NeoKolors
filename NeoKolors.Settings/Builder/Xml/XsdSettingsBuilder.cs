// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Common.Util;
using NeoKolors.Settings.Builder.Info;
using OneOf;

namespace NeoKolors.Settings.Builder.Xml;

/// <summary>
/// The XsdSettingsBuilder class is responsible for generating XML Schema Definitions (XSD)
/// from settings data structures. It provides the ability to create XSD representations
/// based on either a settings builder instance or specific node information.
/// </summary>
public class XsdSettingsBuilder {
    
    private const string XSD_NAMESPACE = "http://www.w3.org/2001/XMLSchema";
    
    public string Namespace { get; }
    public OneOf<ISettingsBuilder, SettingsNodeInfo> Source { get; }
    
    public XsdSettingsBuilder(string ns, OneOf<ISettingsBuilder, SettingsNodeInfo> source) {
        Namespace = ns;
        Source = source;
    }
    
    public XsdSettingsBuilder(string ns, ISettingsNode source, string root) {
        Namespace = ns;
        Source = new SettingsNodeInfo(root, source);
    }

    /// <summary>
    /// Generates an XML Schema Definition (XSD) string based on the provided settings structure
    /// or settings node information.
    /// </summary>
    /// <param name="file">The file path where the generated XSD content will be saved.</param>
    public void GetXsd(string file) =>
        File.WriteAllText(file, GetXsd());

    /// <summary>
    /// Generates an XML Schema Definition (XSD) string based on the provided settings structure
    /// or settings node information.
    /// </summary>
    /// <returns>
    /// A string containing the generated XSD content.
    /// </returns>  
    public string GetXsd() => 
        GetRoot(Source.Match(
            b => b.ToXsd(),
            n => n.ToXsd())
        );

    private string GetRoot(string content) =>
        $"""
         <xsd:schema xmlns:xsd="{XSD_NAMESPACE}" targetNamespace="{Namespace}" elementFormDefault="qualified">
             {content.PadLinesLeft(4)}
         </xsd:schema>
         """;
}