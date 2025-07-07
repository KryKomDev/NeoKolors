// NeoKolors
// Copyright (c) 2025 KryKom

using System.Runtime.CompilerServices;
using NeoKolors.Common.Util;
using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Builder.Xml.Exception;

namespace NeoKolors.Settings.Builder.Info;

public readonly struct ArgumentInfo : IEquatable<ArgumentInfo> {
    
    public string Name { get; }
    public IArgument Argument { get; }
    public string? Description { get; }

    public ArgumentInfo(string name, IArgument argument, string? description = null) {
        Name = name;
        Argument = argument;
        Description = description;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Name.GetHashCode();

    public bool Equals(ArgumentInfo other) => Name == other.Name && Argument.Equals(other.Argument) && Description == other.Description;
    public override bool Equals(object? obj) => obj is ArgumentInfo other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ArgumentInfo left, ArgumentInfo right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ArgumentInfo left, ArgumentInfo right) => !left.Equals(right);

    public string ToXsd() {
        if (Argument is IXsdArgument x)
            return $"""
                    <xsd:element name="{Name}">
                        <xsd:annotation>
                            <xsd:documentation>{Description ?? ""}</xsd:documentation>
                        </xsd:annotation>
                        {x.ToXsd().PadLinesLeft(4)}
                    </xsd:element>
                    """;
        
        throw XsdBuilderException.ArgumentXsdIncompatible(Name);
    }
}