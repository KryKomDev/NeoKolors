// NeoKolors
// Copyright (c) 2025 KryKom

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
        Decompose(rawEvType, out var type, out var mods);
        return new MouseEventInfo(type, mods, RemapCoordinate(x), RemapCoordinate(y));
    }
    
    internal static MouseEventInfo DecomposeSGR(ConsoleKeyInfo rawEvType, ConsoleKeyInfo x, ConsoleKeyInfo y) {
        throw new NotImplementedException();
    }

    private static void Decompose(ConsoleKeyInfo rawEvType, out MouseEventType type, out ConsoleModifiers mods) {
        
        // char = 32 + button + motion * 32 + shift * 4 + alt * 8 + ctrl * 16;
        // 0 = left, 1 = middle, 2 = right, 3 = release, wheel up = 64, wheel down = 65
        
        int raw = rawEvType.KeyChar - 32;
        bool moved, shift, alt, ctrl, wheel;

        // check if action is wheel
        if (raw >= 64) {
            wheel = true;
            raw -= 64;
        }
        else {
            wheel = false;
        }

        // check if action is motion
        if (raw >= 32) {
            moved = true;
            raw -= 32;
        }
        else {
            moved = false;
        }

        // check if ctrl
        if (raw >= 16) {
            ctrl = true;
            raw -= 16;
        }
        else {
            ctrl = false;
        }

        // check if alt
        if (raw >= 8) {
            alt = true;
            raw -= 8;
        }
        else {
            alt = false;
        }

        // check if shift
        if (raw >= 4) {
            shift = true;
            raw -= 4;
        }
        else {
            shift = false;
        }

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
    
    private static int RemapCoordinate(ConsoleKeyInfo k) {
        if (k.Key == ConsoleKey.Backspace) return 94;
        if (k.Modifiers.HasFlag(Alt)) return k.KeyChar + 31;
        return k.KeyChar - 33;
    }

    private static ConsoleModifiers None => 0;
}