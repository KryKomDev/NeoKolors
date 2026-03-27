// 
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Console;

/// <summary>
/// a wrapper exception for fancy formatting
/// </summary>
[Obsolete("Automatic unhandled exception interruption is available. Set 'NKDebug.ExceptionFormatting' to true to enable automatic interruption.")]
public sealed class FancyException : Exception {
    public Exception OriginalException { get; }
    public override string ToString() => NKDebug.Formatter.Format(OriginalException);
    public FancyException(Exception e) => OriginalException = e;

    /// <summary>
    /// creates a new fancy exception from an exception
    /// </summary>
    internal static FancyException Create(Exception e) => new(e);
}