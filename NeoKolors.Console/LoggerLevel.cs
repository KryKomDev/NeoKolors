//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using HasFlagExtension;

namespace NeoKolors.Console;

/// <summary>
/// determines what logging messages will be printed
/// </summary>
[Flags]
[HasFlagPrefix("Log")]
public enum LoggerLevel {
    NONE = 0,
    CRITICAL = 1,
    ERROR = 2,
    WARNING = 4,
    INFORMATION = 8,
    DEBUG = 16,
    TRACE = 32
}