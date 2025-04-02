﻿//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Console;

/// <summary>
/// determines what logging messages will be printed
/// </summary>
[Flags]
public enum DebugLevel {
    FATAL = 1,
    ERROR = 2,
    WARN = 4,
    INFO = 8,
    DEBUG = 16
}