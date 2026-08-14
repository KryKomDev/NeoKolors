// NeoKolors
// Copyright (c) krystof 2026

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace NeoKolors.Console;

/// <summary>
/// Provider for <see cref="NKLogger"/> instances for Microsoft.Extensions.Logging framework.
/// </summary>
[ProviderAlias("NeoKolors")]
public sealed class NKLoggerProvider : ILoggerProvider, ISupportExternalScope {

    private readonly ConcurrentDictionary<string, NKLogger> _loggers = new(StringComparer.Ordinal);
    private readonly NKLoggerOptions _options;
    private IExternalScopeProvider? _scopeProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="NKLoggerProvider"/> with default options.
    /// </summary>
    public NKLoggerProvider() : this(new NKLoggerOptions()) { }

    /// <summary>
    /// Initializes a new instance of <see cref="NKLoggerProvider"/> with custom options.
    /// </summary>
    public NKLoggerProvider(NKLoggerOptions options) {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Creates a new <see cref="ILogger"/> instance for the specified category name.
    /// </summary>
    public ILogger CreateLogger(string categoryName) {
        return _loggers.GetOrAdd(categoryName, name => new NKLogger(_options.Writer, name, _options.Level, _options.Enabled) {
            ScopeProvider = _scopeProvider
        });
    }

    /// <summary>
    /// Sets the external scope provider for loggers created by this provider.
    /// </summary>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider) {
        _scopeProvider = scopeProvider;
        foreach (var logger in _loggers.Values) {
            logger.ScopeProvider = _scopeProvider;
        }
    }

    /// <summary>
    /// Disposes the provider and underlying log writer if disposable.
    /// </summary>
    public void Dispose() {
        if (_disposed) return;
        _disposed = true;

        _loggers.Clear();
        _options.Writer.Dispose();
    }
}
