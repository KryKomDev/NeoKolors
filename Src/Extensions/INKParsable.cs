// NeoKolors
// Copyright (c) KryKom 2026

/*
using System.Diagnostics.CodeAnalysis;
using Implyzer;

// ReSharper disable once InvalidXmlDocComment
// ReSharper disable once RedundantNullableFlowAttribute

namespace NeoKolors.Extensions;

public delegate bool TryParseDelegate<T>([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out T? result);

public delegate T ParseDelegate<out T>([NotNullWhen(true)] string? s);

/// <summary>
/// An interface that mimics the behavior of <see cref="System.IParsable{TSelf}"/>
/// but is compatible with other dotnet versions.
/// </summary>
[StaticAbstract("TryParse", typeof(TryParseDelegate<>), "TSelf", "T")]
[StaticVirtual("Parse", typeof(ParseDelegate<>), "TSelf", "T", DefaultType = typeof(INKParsableDefaultImplementations))]
public partial interface INKParsable<TSelf> where TSelf : INKParsable<TSelf>?;

public static class INKParsableDefaultImplementations {
    public static T? Parse<T>([NotNullWhen(true)] string? s) where T : INKParsable<T> {
        return INKParsable.TryParse<T>(s, out var result)
            ? result
            : throw new FormatException($"Invalid {typeof(T).Name} format: '{s}'");
    }
}
*/