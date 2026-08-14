// NeoKolors
// Copyright (c) KryKom 2026

using System.Diagnostics.CodeAnalysis;
using Implyzer;

namespace NeoKolors.Extensions;

public delegate bool TryParseDelegate<T>([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out T result);

public delegate T ParseDelegate<out T>([NotNullWhen(true)] string? s);

/// <summary>
/// An interface that mimics the behavior of <see cref="System.IParsable{TSelf}"/>
/// but is compatible with other dotnet versions.
/// </summary>
[StaticAbstract("TryParse", typeof(TryParseDelegate<>), "TSelf", "T")]
[StaticAbstract("Parse",    typeof(ParseDelegate<>),    "TSelf", "T")]
[SuppressMessage("ReSharper", "InvalidXmlDocComment")]
public partial interface INKParsable<TSelf> where TSelf : INKParsable<TSelf>?;