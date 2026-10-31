// NeoKolors
// Copyright (c) 2025 KryKom

using System.Collections;
using String = NeoKolors.Extensions.String;

namespace NeoKolors.Settings.Builder;

public readonly struct SettingsMethodOptionInfo {
    public string Name { get; }
    public SettingsMethodOption Option { get; }
    public string? Description { get; }
    
    public SettingsMethodOptionInfo(string name, SettingsMethodOption option, string? description = null) {
        Name = name;
        Option = option;
        Description = description;
    }

    public string ToXsd() =>
        $"""
         <xsd:element name="{Name}">
             <xsd:annotation>
                 <xsd:documentation>{Description ?? ""}</xsd:documentation>
             </xsd:annotation>
             <xsd:complexType>
                 <xsd:sequence>
                     {String.Join((IEnumerable)Option.Arguments.Select(a => a.ToXsd()), "\n").PadLinesLeft(12)}
                 </xsd:sequence>
             </xsd:complexType>
         </xsd:element>
         """;
}