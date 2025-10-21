// NeoKolors
// Copyright (c) 2025 KryKom

using System;
using static System.ConsoleModifiers;
using static NeoKolors.Console.Mouse.MouseEventType;

namespace NeoKolors.Console.Mouse;

internal static class MouseEventDecomposer {
    
    /// <summary>
    /// Decomposes raw console key information to generate detailed mouse event information.
    /// </summary>
    /// <param name="rawEvType">The raw <see cref="ConsoleKeyInfo"/> representing the mouse event type.</param>
    /// <param name="x">The raw <see cref="ConsoleKeyInfo"/> representing the X-coordinate.</param>
    /// <param name="y">The raw <see cref="ConsoleKeyInfo"/> representing the Y-coordinate.</param>
    /// <returns>A <see cref="MouseEventInfo"/> instance containing the decomposed mouse event type, modifiers, and coordinates.</returns>
    internal static MouseEventInfo DecomposeUtf8(ConsoleKeyInfo rawEvType, ConsoleKeyInfo x, ConsoleKeyInfo y) {
        DecomposeX10(rawEvType.KeyChar, out var type, out var mods);
        return new MouseEventInfo(type, mods, RemapCoordinate(x) - 1, RemapCoordinate(y) - 1);
    }

    /// <summary>
    /// Decomposes raw SGR mouse input values into detailed mouse event information.
    /// </summary>
    /// <param name="rawEvType">The raw integer value representing the mouse event type.</param>
    /// <param name="x">The X-coordinate of the mouse event.</param>
    /// <param name="y">The Y-coordinate of the mouse event.</param>
    /// <param name="press">A boolean indicating whether the event involves a press action. Pressed = 'M', released = 'm', in case of none and movement -> 'M'</param>
    /// <returns>A <see cref="MouseEventInfo"/> instance containing the decomposed mouse event type, modifiers, and coordinates.</returns>
    internal static MouseEventInfo DecomposeSGR(int rawEvType, int x, int y, bool press) {
        DecomposeSGR(rawEvType, press, out var type, out var mods);
        return new MouseEventInfo(type, mods, x - 1, y - 1);
    }

    private static void DecomposeX10(int rawEvType, out MouseEventType type, out ConsoleModifiers mods) {
        
        // char = 32 + button + motion * 32 + shift * 4 + alt * 8 + ctrl * 16;
        // 0 = left, 1 = middle, 2 = right, 3 = release, wheel up = 64, wheel down = 65
        
        int raw = GetModifiers(rawEvType - 32, out var wheel, out var moved, out var ctrl, out var alt, out var shift);

        // compute modifiers
        mods = (shift ? Shift : 0) | (alt ? Alt : 0) | (ctrl ? Control : 0);
        
        // if action is wheel
        if (wheel) {
            type = raw switch {
                0 => WHEEL_UP,
                1 => WHEEL_DOWN,
                _ => UNKNOWN
            };
            return;
        }

        // if action is not wheel
        type = (raw, moved) switch {
            (0, false) => LEFT_PRESS,
            (1, false) => MIDDLE_PRESS,
            (2, false) => RIGHT_PRESS,
            (3, false) => RELEASE,
            (0, true) => LEFT_DRAG,
            (1, true) => MIDDLE_DRAG,
            (2, true) => RIGHT_DRAG,
            (3, true) => MOVE,
            _ => UNKNOWN
        };
    }

    private static void DecomposeSGR(int rawEvType, bool press, out MouseEventType type, out ConsoleModifiers mods) {
        // char = 32 + button + motion * 32 + shift * 4 + alt * 8 + ctrl * 16;
        // 0 = left, 1 = middle, 2 = right, 3 = release, wheel up = 64, wheel down = 65
        
        int raw = GetModifiers(rawEvType, out var wheel, out var moved, out var ctrl, out var alt, out var shift);
        
        // compute modifiers
        mods = (shift ? Shift : 0) | (alt ? Alt : 0) | (ctrl ? Control : 0);
        
        // if action is wheel
        if (wheel) {
            type = raw switch {
                0 => WHEEL_UP,
                1 => WHEEL_DOWN,
                _ => UNKNOWN
            };
            return;
        }

        type = (raw, moved, press) switch {
            (0, false, true) => LEFT_PRESS,
            (1, false, true) => MIDDLE_PRESS,
            (2, false, true) => RIGHT_PRESS,
            (0, false, false) => LEFT_RELEASE,
            (1, false, false) => MIDDLE_RELEASE,
            (2, false, false) => RIGHT_RELEASE,
            (3, false, false) => RELEASE,
            (0, true, true) => LEFT_DRAG,
            (1, true, true) => MIDDLE_DRAG,
            (2, true, true) => RIGHT_DRAG,
            (3, true, true) => MOVE,
            _ => UNKNOWN
        };
    }

    private static int GetModifiers(int raw, out bool wheel, out bool moved, out bool ctrl, out bool alt, out bool shift) {
        raw = HasWheel(raw, out wheel);
        raw = HasMovement(raw, out moved);
        raw = HasCtrl(raw, out ctrl);
        raw = HasAlt(raw, out alt);
        raw = HasShift(raw, out shift);
        
        return raw;
    }

    private static int HasWheel(int raw, out bool wheel) {
        if (raw >= 64) {
            wheel = true;
            raw -= 64;
        }
        else {
            wheel = false;
        }
        
        return raw;
    }

    private static int HasMovement(int raw, out bool moved) {
        if (raw >= 32) {
            moved = true;
            raw -= 32;
        }
        else {
            moved = false;
        }
        
        return raw;
    }

    private static int HasCtrl(int raw, out bool ctrl) {
        if (raw >= 16) {
            ctrl = true;
            raw -= 16;
        }
        else {
            ctrl = false;
        }
        
        return raw;
    }

    private static int HasAlt(int raw, out bool alt) {
        if (raw >= 8) {
            alt = true;
            raw -= 8;
        }
        else {
            alt = false;
        }
        
        return raw;
    }

    private static int HasShift(int raw, out bool shift) {
        if (raw >= 4) {
            shift = true;
            raw -= 4;
        }
        else {
            shift = false;
        }
        
        return raw;
    }
    
    private static int RemapCoordinate(ConsoleKeyInfo k) {
        if (k.Key == ConsoleKey.Backspace) return 94;
        if (k.Modifiers.HasFlag(Alt)) return k.KeyChar + 32;
        return k.KeyChar - 32;
    }

    private static ConsoleModifiers None => 0;
}