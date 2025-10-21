//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System;
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

public static partial class LoggerLevelExtensions {
    extension(LoggerLevel l) {
        public bool LogCrit => l.HasFlag(LoggerLevel.CRITICAL);
        public bool LogError => l.HasFlag(LoggerLevel.ERROR);
        public bool LogWarn => l.HasFlag(LoggerLevel.WARNING);
        public bool LogInfo => l.HasFlag(LoggerLevel.INFORMATION);
        public bool LogDebug => l.HasFlag(LoggerLevel.DEBUG);
        public bool LogTrace => l.HasFlag(LoggerLevel.TRACE);
    }
}