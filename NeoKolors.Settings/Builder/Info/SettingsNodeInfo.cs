// NeoKolors
// Copyright (c) 2025 KryKom

using System.Runtime.CompilerServices;
using NeoKolors.Common.Util;

namespace NeoKolors.Settings.Builder.Info;

public readonly struct SettingsNodeInfo<TResult> : IEquatable<SettingsNodeInfo<TResult>> {
    
    public string Name { get; }
    public ISettingsNode<TResult> Node { get; }
    public string? Description { get; }

    public SettingsNodeInfo(string name, ISettingsNode<TResult> node, string? description = null) {
        Name = name;
        Node = node;
        Description = description;
    }

    public override int GetHashCode() => Name.GetHashCode();

    public bool Equals(SettingsNodeInfo<TResult> other) => Name == other.Name && Node.Equals(other.Node);
    public override bool Equals(object? obj) => obj is SettingsNodeInfo other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(SettingsNodeInfo<TResult> left, SettingsNodeInfo<TResult> right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(SettingsNodeInfo<TResult> left, SettingsNodeInfo<TResult> right) => !left.Equals(right);
    
    public string ToXsd() =>
        $"""
         <xsd:element name="{Name}">
             <xsd:annotation>
                 <xsd:documentation>{Description ?? ""}</xsd:documentation>
             </xsd:annotation>
             <xsd:complexType>
                 <xsd:sequence>
                     {Node.Elements.Select(e => e.ToXsd()).Join("\n").PadLinesLeft(12)}
                 </xsd:sequence>
             </xsd:complexType>
         </xsd:element>

         """;
    
    public static implicit operator SettingsNodeInfo(SettingsNodeInfo<TResult> node) => 
        new(node.Name, node.Node, node.Description);

    public static implicit operator SettingsNodeInfo<TResult>(SettingsNodeInfo node) =>
        node.Node is ISettingsNode<TResult> n
            ? new SettingsNodeInfo<TResult>(node.Name, n, node.Description)
            : throw new InvalidCastException();
}

public readonly struct SettingsNodeInfo : IEquatable<SettingsNodeInfo> {
    
    public string Name { get; }
    public ISettingsNode Node { get; }
    public string? Description { get; }

    public SettingsNodeInfo(string name, ISettingsNode node, string? description = null) {
        Name = name;
        Node = node;
        Description = description;
    }

    public override int GetHashCode() => Name.GetHashCode();

    public bool Equals(SettingsNodeInfo other) => Name == other.Name && Node.Equals(other.Node);
    public override bool Equals(object? obj) => obj is SettingsNodeInfo other && Equals(other);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(SettingsNodeInfo left, SettingsNodeInfo right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(SettingsNodeInfo left, SettingsNodeInfo right) => !left.Equals(right);

    public string ToXsd() =>
        $"""
         <xsd:element name="{Name}">
             <xsd:annotation>
                 <xsd:documentation>{Description ?? ""}</xsd:documentation>
             </xsd:annotation>
             <xsd:complexType>
                 <xsd:sequence>
                     {Node.Elements.Select(e => e.ToXsd()).Join("\n").PadLinesLeft(12)}
                 </xsd:sequence>
             </xsd:complexType>
         </xsd:element>
         """;
}