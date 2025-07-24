// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Console;

internal sealed class TextWriterReference : IDisposable {
    
    private TextWriter _textWriter;
    private int _referenceCount;
    private bool _disposed;
    private readonly object _lock = new();

    public TextWriterReference(TextWriter textWriter) {
        _textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
        _referenceCount = 1;
    }

    public TextWriter TextWriter {
        get {
            lock (_lock) {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(TextWriterReference));
                
                return _textWriter;
            }
        }
        set {
            lock (_lock) {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(TextWriterReference));
                
                _textWriter = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
    }

    public TextWriterReference AddReference() {
        lock (_lock) {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TextWriterReference));
            
            _referenceCount++;
            return this;
        }
    }

    public void Dispose() {
        lock (_lock) {
            if (_disposed) return;

            _referenceCount--;

            if (_referenceCount > 0) return;
            
            _textWriter.Close();
            _textWriter.Dispose();
            _disposed = true;
        }
    }
    
    public static implicit operator TextWriter(TextWriterReference reference) => reference.TextWriter;
}