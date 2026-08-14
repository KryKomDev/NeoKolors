// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Console;

/// <summary>
/// A log writer that delegates log record writes to multiple underlying <see cref="ILogWriter"/> instances.
/// </summary>
public sealed class CompositeLogWriter : ILogWriter, IAsyncDisposable {

    private readonly List<ILogWriter> _writers = new();
    private          bool             _disposed;

    /// <summary>
    /// Gets the list of underlying log writers.
    /// </summary>
    public IReadOnlyList<ILogWriter> Writers => _writers;

    /// <summary>
    /// Initializes a new instance of <see cref="CompositeLogWriter"/> with the specified log writers.
    /// </summary>
    public CompositeLogWriter(params ILogWriter[] writers) {
        if (writers != null) {
            _writers.AddRange(writers);
        }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CompositeLogWriter"/> with the specified log writers.
    /// </summary>
    public CompositeLogWriter(IEnumerable<ILogWriter> writers) {
        if (writers != null) {
            _writers.AddRange(writers);
        }
    }

    /// <summary>
    /// Adds a log writer to the composite writer.
    /// </summary>
    public CompositeLogWriter AddWriter(ILogWriter writer) {
        if (writer == null)
            throw new ArgumentNullException(nameof(writer));

        _writers.Add(writer);

        return this;
    }

    /// <summary>
    /// Writes a log record to all registered log writers.
    /// </summary>
    public void Write(NKLogRecord record) {
        if (_disposed)
            return;

        foreach (var writer in _writers) {
            try {
                writer.Write(record);
            }
            catch {
                // Prevent single failing writer from breaking the logger chain
            }
        }
    }

    /// <summary>
    /// Disposes all registered child log writers.
    /// </summary>
    public void Dispose() {
        if (_disposed)
            return;

        _disposed = true;

        foreach (var writer in _writers) {
            writer.Dispose();
        }

        _writers.Clear();
    }

    /// <summary>
    /// Asynchronously disposes all registered child log writers.
    /// </summary>
    public async ValueTask DisposeAsync() {
        if (_disposed)
            return;

        _disposed = true;

        foreach (var writer in _writers) {
            if (writer is IAsyncDisposable asyncDisposable) {
                await asyncDisposable.DisposeAsync();
            }
            else {
                writer.Dispose();
            }
        }

        _writers.Clear();
    }
}