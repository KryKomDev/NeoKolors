//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using HasFlagExtension;
using NeoKolors.Console.Driver.Windows;
using static NeoKolors.Console.Input.KeyModifiers;

namespace NeoKolors.Console.Input;

/// <summary>
/// Represents a set of modifier keys that can be used in combination with other keys
/// to provide additional input options. This enumeration is marked with the <see cref="FlagsAttribute"/>
/// to allow a bitwise combination of its member values.
///
/// <b>NOTE:</b> The enum's values are compliant to the values of <see cref="WinImports.WinKeyModifiers"/>.
/// </summary>
[Flags]
[FlagGroup("Alt",  "Has")]
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

public static partial class KeyModifiersExtensions {
    
    private const KeyModifiers SWITCHES = NUMLOCK | SCROLL_LOCK | CAPS_LOCK | ENHANCED;
    
    extension(KeyModifiers mods) {
        
        /// <summary>
        /// Determines whether the current <see cref="KeyModifiers"/> matches the given <paramref name="other"/>
        /// based on the specified criteria for ignoring key side and switch states.
        /// </summary>
        /// <param name="other">The <see cref="KeyModifiers"/> value to compare against the current instance.</param>
        /// <param name="ignoreSide">
        /// A boolean indicating whether the side-specific keys (e.g., left or right Alt/Ctrl) should be ignored
        /// during the comparison. If true, the side-specific distinction is ignored.
        /// </param>
        /// <param name="ignoreSwitches">
        /// A boolean indicating whether toggle-state keys such as NumLock, CapsLock,
        /// ScrollLock, and Enhanced should be ignored during the comparison. If true,
        /// these states will not factor into the result.
        /// </param>
        /// <returns>
        /// A boolean indicating whether the current <see cref="KeyModifiers"/> matches the
        /// <paramref name="other"/> based on the provided comparison criteria.
        /// Returns true if the modifiers match; otherwise, false.
        /// </returns>
        public bool Matches(KeyModifiers other, bool ignoreSide = true, bool ignoreSwitches = true) {
            return (ignoreSide, ignoreSwitches) switch {
                (true, true) => // ignore sides and switches
                    !(mods.GetHasAlt()   ^ other.GetHasAlt())  &&
                    !(mods.GetHasCtrl()  ^ other.GetHasCtrl()) &&
                    !(mods.GetHasShift() ^ other.GetHasShift()),
                (true, false) => // ignore sides
                    !(mods.GetHasAlt()   ^ other.GetHasAlt())   &&
                    !(mods.GetHasCtrl()  ^ other.GetHasCtrl())  &&
                    !(mods.GetHasShift() ^ other.GetHasShift()) &&
                    (mods & SWITCHES) == (other & SWITCHES),
                (false, true) => // ignore switches
                    (mods & ~SWITCHES) == (other & ~SWITCHES),
                (false, false) => // do not ignore anything!!!
                    mods == other
            };
        }
    }
}