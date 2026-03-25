// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

namespace NeoKolors.Console.Ansi;

/// <summary>
/// Represents the capabilities of a terminal that supports Virtual Terminal (VT) features.
/// This enum is used to define and categorize the varying levels of functionality available
/// within the compatibility and feature set of VT-compliant terminals.
/// </summary>
[Flags]
public enum VTCapabilities : ushort {
    NONE                 = 0,
    COL_132              = 1 << 0,  // 1
    PRINTER              = 1 << 1,  // 2
    REGIS                = 1 << 2,  // 3
    SIXEL                = 1 << 3,  // 4 
    SELECTIVE_ERASE      = 1 << 4,  // 6
    USER_DEFINED_KEYS    = 1 << 5,  // 8
    NR_CHAR_SET          = 1 << 6,  // 9
    TECHNICAL_CHARS      = 1 << 7,  // 15
    LOCATOR_PORT         = 1 << 8,  // 16 
    VT_STATE_REPORT      = 1 << 9,  // 17
    USER_WINDOWS         = 1 << 10, // 18
    HORIZONTAL_SCROLL    = 1 << 11, // 21
    ANSI_COLOR           = 1 << 12, // 22
    RECTANGULAR_EDITING  = 1 << 13, // 28
    ANSI_TEXT_LOCATOR    = 1 << 14, // 29
}