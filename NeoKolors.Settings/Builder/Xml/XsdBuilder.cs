// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Common.Util;
using NeoKolors.Settings.Builder.Info;
using OneOf;

namespace NeoKolors.Settings.Builder.Xml;

public class XsdBuilder {
    
    public const string XSD_NAMESPACE = "http://www.w3.org/2001/XMLSchema";
    
    public string Namespace { get; }
    public OneOf<ISettingsBuilder, SettingsNodeInfo> Source { get; }
    
    public XsdBuilder(string ns, OneOf<ISettingsBuilder, SettingsNodeInfo> source) {
        Namespace = ns;
        Source = source;
    }
    
    public XsdBuilder(string ns, ISettingsNode source, string root) {
        Namespace = ns;
        Source = new SettingsNodeInfo(root, source);
    }

    public string GetXsd() => 
        GetRoot(Source.Match(
            b => b.ToXsd(),
            n => n.ToXsd())
        );

    private string GetRoot(string content) =>
        $"""
         <xsd:schema xmlns:xsd="{XSD_NAMESPACE}" targetNamespace="{Namespace}">
             {content.PadLinesLeft(4)}
         </xsd:schema>
         """;
}