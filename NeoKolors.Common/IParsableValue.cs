// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics.CodeAnalysis;

namespace NeoKolors.Tui.Styles.Values;

public interface IParsableValue<TSelf> : IParsableValue where TSelf : struct, IParsableValue<TSelf> {
    
    /// <summary>Parses a string into a value.</summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="s" />.</param>
    /// <returns>The result of parsing <paramref name="s" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s" /> is <c>null</c>.</exception>
    /// <exception cref="FormatException"><paramref name="s" /> is not in the correct format.</exception>
    /// <exception cref="OverflowException"><paramref name="s" /> is not representable by <typeparamref name="TSelf" />.</exception>
    public new TSelf Parse(string s, IFormatProvider? provider);

    /// <summary>Tries to parse a string into a value.</summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="s" />.</param>
    /// <param name="result">On return, contains the result of successfully parsing <paramref name="s" /> or an undefined value on failure.</param>
    /// <returns><c>true</c> if <paramref name="s" /> was successfully parsed; otherwise, <c>false</c>.</returns>
    public bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out TSelf result);
    
    object IParsableValue.Parse(string s, IFormatProvider? provider) => Parse(s, provider);

    bool IParsableValue.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out object? result) {
        var res = TryParse(s, provider, out var d);
        result = d;
        return res;
    }
}

public interface IParsableValue {
    
    /// <summary>Parses a string into a value.</summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="s" />.</param>
    /// <returns>The result of parsing <paramref name="s" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s" /> is <c>null</c>.</exception>
    /// <exception cref="FormatException"><paramref name="s" /> is not in the correct format.</exception>
    /// <exception cref="OverflowException"><paramref name="s" /> is not representable by output type.</exception>
    public object? Parse(string s, IFormatProvider? provider);

    /// <summary>Tries to parse a string into a value.</summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="s" />.</param>
    /// <param name="result">On return, contains the result of successfully parsing <paramref name="s" /> or an undefined value on failure.</param>
    /// <returns><c>true</c> if <paramref name="s" /> was successfully parsed; otherwise, <c>false</c>.</returns>
    public bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out object? result);
}