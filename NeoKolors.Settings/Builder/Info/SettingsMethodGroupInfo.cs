// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Common.Util;

namespace NeoKolors.Settings.Builder.Info;

public readonly struct SettingsMethodGroupInfo : IEquatable<SettingsMethodGroupInfo> {
    public string Name { get; }
    public SettingsMethodGroup Group { get; }
    public string? Description { get; }
    
    public SettingsMethodGroupInfo(string name, SettingsMethodGroup group, string? description = null) {
        Name = name;
        Group = group;
        Description = description;
    }

    public override int GetHashCode() => Name.GetHashCode();

    public bool Equals(SettingsMethodGroupInfo other) =>
        Name == other.Name && Group.Equals(other.Group) && Description == other.Description;
    
    public override bool Equals(object? obj) => 
        obj is SettingsMethodGroupInfo other && Equals(other);

    public static bool operator ==(SettingsMethodGroupInfo left, SettingsMethodGroupInfo right) => left.Equals(right);
    public static bool operator !=(SettingsMethodGroupInfo left, SettingsMethodGroupInfo right) => !left.Equals(right);
    
    public string ToXsd() =>
        $"""
         <xsd:element name="{Name}">
             <xsd:annotation>
                 <xsd:documentation>{Description ?? ""}</xsd:documentation>
             </xsd:annotation>
             <xsd:complexType>
                 <xsd:choice>
                     {Group.Options.Select(o => o.ToXsd()).Join("\n").PadLinesLeft(12)}
                 </xsd:choice>
             </xsd:complexType>
         </xsd:element>
         """;
}