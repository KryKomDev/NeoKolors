//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using HasFlagExtension;
using NeoKolors.Console.Driver.Windows;

namespace NeoKolors.Console.Input;

/// <summary>
/// Represents a set of modifier keys that can be used in combination with other keys
/// to provide additional input options. This enumeration is marked with the <see cref="FlagsAttribute"/>
/// to allow a bitwise combination of its member values.
///
/// <b>NOTE:</b> The enum's values are compliant to the values of <see cref="WinImports.WinKeyModifiers"/>.
/// </summary>
[Flags]
[FlagGroup("Alt", "Has")]
[FlagGroup("Ctrl", "Has")]
public enum KeyModifiers {
    
    [ExcludeFlag] NONE = 0,

    [FlagGroup("Alt")]  RIGHT_ALT  = 1 << 0,
    [FlagGroup("Alt")]  LEFT_ALT   = 1 << 1,
    [FlagGroup("Ctrl")] RIGHT_CTRL = 1 << 2,
    [FlagGroup("Ctrl")] LEFT_CTRL  = 1 << 3,
    
    SHIFT       = 1 << 4,
    NUMLOCK     = 1 << 5,
    SCROLL_LOCK = 1 << 6,
    CAPS_LOCK   = 1 << 7,
    ENHANCED    = 1 << 8,
}