//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.CodeAnalysis;
using NeoKolors.Settings.Argument.Exception;
using NeoKolors.Settings.Attributes;
using static NeoKolors.Settings.Argument.AllowedPathType;

namespace NeoKolors.Settings.Argument;

/// <summary>
/// argument for a path to a file or directory
/// </summary>
[ExcludeFromCodeCoverage]
[DisplayType("path")]
public class PathArgument : IArgument<string>, IXsdArgument {
    public string Value { get; private set; }
    public string DefaultValue { get; }
    public bool MustExist => !AllowedPathType.HasFlag(NOT_EXISTING);
    public bool AllowFilePath => AllowedPathType.HasFlag(FILE);
    public bool AllowDirectoryPath => AllowedPathType.HasFlag(DIRECTORY);
    public AllowedPathType AllowedPathType { get; }
    
    public PathArgument(string defaultValue = ".", AllowedPathType allowedPathType = FILE | DIRECTORY | NOT_EXISTING) {
        DefaultValue = defaultValue;
        Value = DefaultValue;
        AllowedPathType = allowedPathType;
    }

    public string ToXsd() =>
        """
        <xsd:simpleType>
            <xsd:restriction base="xsd:anyURI"/>
        </xsd:simpleType>
        """;
    
    void IArgument.Set(object? value) => Set(value);
    public string Get() => Value;
    public string GetDefault() => DefaultValue;
    object IArgument.GetDefault() => GetDefault();
    public void Reset() => Value = DefaultValue;

    public IArgument<string> Clone() => (IArgument<string>)MemberwiseClone();

    public void Set(object? value) {
        if (value is string s) {
            Set(s);
        }
        else if (value is PathArgument p) {
            Set(p.Value);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(string), value?.GetType());
        }
    }

    public void Set(string path) {
        if (MustExist && !Directory.Exists(path) && !File.Exists(path)) 
            throw new InvalidArgumentInputException("Path does not exist.");

        bool isFile = Path.HasExtension(path);
        if (!AllowFilePath && isFile) throw new InvalidArgumentInputException("Path must point to a file.");
        if (!AllowDirectoryPath && !isFile) throw new InvalidArgumentInputException("Path must point to a directory.");
            
        Value = path;
    }

    object IArgument.Get() => Get();
    void IArgument.Reset() => Reset();
    IArgument IArgument.Clone() => Clone();
    public bool Equals(IArgument? other) {
        return other is PathArgument p &&
               Value == p.Value &&
               DefaultValue == p.DefaultValue &&
               AllowedPathType == p.AllowedPathType;
    }

    object ICloneable.Clone() => Clone();
}

/// <summary>
/// determines the allowed path types
/// </summary>
[Flags]
public enum AllowedPathType : byte {
    NOT_EXISTING = 1,
    DIRECTORY = 2,
    FILE = 4
}