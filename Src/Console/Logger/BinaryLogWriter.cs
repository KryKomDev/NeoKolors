// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Console;

public class BinaryLogWriter : ILogWriter, IAsyncDisposable {

    private Stream? _output;
    private bool    _disposed;

    public BinaryLogWriter(Stream? output = null) {
        _output = output;
    }

    /// <summary>
    /// Creates a <see cref="BinaryLogWriter"/> configured with a log file configuration.
    /// </summary>
    public static BinaryLogWriter CreateFromFile(LogFileConfig fileConfig) {
        var stream = fileConfig.CreateStream();
        return new BinaryLogWriter(stream);
    }

    /// <summary>
    /// Creates a <see cref="BinaryLogWriter"/> writing binary log records to the specified file path.
    /// </summary>
    public static BinaryLogWriter CreateFromFile(string filePath, bool append = true) {
        var fileConfig = append ? LogFileConfig.Append(filePath) : LogFileConfig.Replace(filePath);
        return CreateFromFile(fileConfig);
    }

    public void Write(NKLogRecord record) {
        if (_disposed || _output == null)
            return;

        try {
            NKLogRecordSerializer.Serialize(_output, record);
            _output.Flush();
        }
        catch {
            // Fail silently to prevent logger crashes
        }
    }

    public void Dispose() {
        if (_disposed) 
            return;
        
        _output?.Dispose();
        
        _output   = null;
        _disposed = true;
        
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync() {
        if (_disposed) 
            return;
        
        if (_output != null)
            await _output.DisposeAsync();

        _output   = null;
        _disposed = true;
        
        GC.SuppressFinalize(this);
    }
}