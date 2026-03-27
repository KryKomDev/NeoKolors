// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Settings.Argument;

/// <summary>
/// Represents a schema argument within the system settings.
/// Provides functionality for converting the argument into a standard format.
/// </summary>
public interface IXsdArgument : IParsableArgument {
    
    /// <summary>
    /// Converts the schema argument to its corresponding XSD (XML Schema Definition) type representation.
    /// </summary>
    /// <returns>
    /// A string representing the XSD type (should be xsd:simpleType or xsd:complexType) of the argument.
    /// </returns>
    public string ToXsd();
}