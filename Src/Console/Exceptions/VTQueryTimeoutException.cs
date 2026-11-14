//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Console.Ansi;

namespace NeoKolors.Console.Exceptions;

/// <summary>
///     Exception thrown when a virtual terminal query (<see cref="VTQuery" />) times out
///     waiting for a response from the terminal emulator.
/// </summary>
public class VTQueryTimeoutException : TimeoutException {

    /// <summary>
    ///     Gets the VT query that timed out.
    /// </summary>
    public VTQuery Query { get; }

    /// <summary>
    ///     Gets the duration waited before timing out.
    /// </summary>
    public TimeSpan Timeout { get; }

    public VTQueryTimeoutException(in VTQuery query, TimeSpan timeout, string? message = null)
        : base(message ?? $"Terminal query '{query.Type}' (sequence: '{query.GetEscSeq().Replace("\e", "\\e").Replace("\a", "\\a")}') timed out after {timeout.TotalMilliseconds:F0}ms waiting for response.") {
        Query   = query;
        Timeout = timeout;
    }

    public VTQueryTimeoutException(string message) : base(message) {
        Query   = default;
        Timeout = TimeSpan.Zero;
    }

    public VTQueryTimeoutException(string message, Exception innerException) : base(message, innerException) {
        Query   = default;
        Timeout = TimeSpan.Zero;
    }
}