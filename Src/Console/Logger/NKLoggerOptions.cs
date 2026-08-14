// NeoKolors
// Copyright (c) krystof 2026

using static NeoKolors.Console.NKLogLevel;

namespace NeoKolors.Console;

/// <summary>
/// Configuration options for <see cref="NKLoggerProvider"/> and <see cref="NKLogger"/>.
/// </summary>
public class NKLoggerOptions {

    private ILogWriter _writer = new AnsiLogWriter();

    /// <summary>
    /// Gets or sets whether logging is enabled. Default is true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the active log level flags. Defaults to all log levels enabled.
    /// </summary>
    public NKLogLevel Level { get; set; } = CRITICAL | ERROR | WARNING | INFORMATION | DEBUG | TRACE;

    /// <summary>
    /// Gets or sets the <see cref="ILogWriter"/> instance used to output log records.
    /// Defaults to an <see cref="AnsiLogWriter"/>.
    /// </summary>
    public ILogWriter Writer {
        get => _writer;
        set => _writer = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets or sets the ANSI logger configuration used when creating a default <see cref="AnsiLogWriter"/>.
    /// </summary>
    public AnsiLoggerConfig AnsiConfig {
        get => (_writer as AnsiLogWriter)?.Config ?? field;
        set {
            field = value ?? throw new ArgumentNullException(nameof(value));
            if (_writer is AnsiLogWriter ansiWriter) {
                ansiWriter.Config = value;
            }
        }
    } = new();

    /// <summary>
    /// Configures logging to write formatted text log records simultaneously to Console and a log file.
    /// </summary>
    public NKLoggerOptions UseFileLogging(LogFileConfig fileConfig, TextLoggerConfig? textConfig = null) {
        var fileWriter = TextLogWriter.CreateFromFile(fileConfig, textConfig);
        _writer = new CompositeLogWriter(_writer, fileWriter);
        return this;
    }

    /// <summary>
    /// Configures logging to write formatted text log records simultaneously to Console and a log file path.
    /// </summary>
    public NKLoggerOptions UseFileLogging(string filePath, TextLoggerConfig? textConfig = null, bool append = true) {
        var fileConfig = append ? LogFileConfig.Append(filePath) : LogFileConfig.Replace(filePath);
        return UseFileLogging(fileConfig, textConfig);
    }

    /// <summary>
    /// Configures logging to write log records simultaneously to Console and a binary log file.
    /// </summary>
    public NKLoggerOptions UseBinaryFileLogging(LogFileConfig fileConfig) {
        var binaryWriter = BinaryLogWriter.CreateFromFile(fileConfig);
        _writer = new CompositeLogWriter(_writer, binaryWriter);
        return this;
    }

    /// <summary>
    /// Configures logging to output only to a log file instead of the Console.
    /// </summary>
    public NKLoggerOptions UseFileOnly(LogFileConfig fileConfig, TextLoggerConfig? textConfig = null) {
        _writer = TextLogWriter.CreateFromFile(fileConfig, textConfig);
        return this;
    }
}
