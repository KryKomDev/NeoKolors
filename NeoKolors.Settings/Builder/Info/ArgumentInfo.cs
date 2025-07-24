// NeoKolors
// Copyright (c) 2025 KryKom

using System.Runtime.CompilerServices;
using NeoKolors.Common.Util;
using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Builder.Xml.Exception;

namespace NeoKolors.Settings.Builder.Info;

public readonly struct ArgumentInfo : ISettingsElementInfo<IArgument>, IEquatable<ArgumentInfo> {
    
    public string Name { get; }
    public IArgument Value { get; }
    object? ISettingsElementInfo.Value => Value;
    public string? Description { get; }

    public ArgumentInfo(string name, IArgument value, string? description = null) {
        Name = name;
        Value = value;
        Description = description;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Name.GetHashCode();

    public bool Equals(ArgumentInfo other) => Name == other.Name && Value.Equals(other.Value) && Description == other.Description;
    public override bool Equals(object? obj) => obj is ArgumentInfo other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ArgumentInfo left, ArgumentInfo right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ArgumentInfo left, ArgumentInfo right) => !left.Equals(right);

    public string ToXsd() =>
        Value is IXsdArgument x
            ? $"""
               <xsd:element name="{Name}">
                   <xsd:annotation>
                       <xsd:documentation>{Description ?? ""}</xsd:documentation>
                   </xsd:annotation>
                   {x.ToXsd().PadLinesLeft(4)}
               </xsd:element>
               """
            : throw XsdBuilderException.ArgumentXsdIncompatible(Name);
}