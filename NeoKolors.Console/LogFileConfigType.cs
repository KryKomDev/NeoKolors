//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.IO;

namespace NeoKolors.Console;

/// <summary>
/// Specifies the configuration options for handling log files in the application.
/// </summary>
public enum LogFileConfigType {
    
    /// <summary>
    /// No log file is created. Often used when a custom <see cref="TextWriter"/> for the logger is used.
    /// </summary>
    CUSTOM,
    
    /// <summary>
    /// Overwrites the existing log file with new log content.
    /// Any pre-existing data in the log file will be discarded.
    /// </summary>
    REPLACE,

    /// <summary>
    /// Appends new log content to the existing log file.
    /// Previously logged data in the file will remain unchanged, and new content will be added at the end.
    /// </summary>
    APPEND,

    /// <summary>
    /// Creates a new log file maintaining a count-based naming convention.
    /// Each time this configuration is used, a sequentially numbered log file is generated.
    /// </summary>
    NEW_COUNT,

    /// <summary>
    /// Creates a new log file with a name based on the current date and time.
    /// Preserves existing log files without overwriting them.
    /// </summary>
    NEW_DATETIME,

    /// <summary>
    /// Creates a new log file using a unique hash value combined with the current date and time.
    /// Ensures log files have distinct names to avoid overwriting existing logs.
    /// </summary>
    NEW_HASH_DATETIME
}