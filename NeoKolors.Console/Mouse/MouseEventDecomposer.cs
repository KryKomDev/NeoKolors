// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;
using static System.ConsoleModifiers;
using static NeoKolors.Console.Mouse.MouseEventType;

namespace NeoKolors.Console.Mouse;

internal static class MouseEventDecomposer {
    /// <summary>
    /// Decomposes raw UTF-8 encoded mouse event data into detailed mouse event arguments.
    /// </summary>
    /// <param name="type">The character representing the raw event type.</param>
    /// <param name="x">The character representing the raw X-coordinate.</param>
    /// <param name="y">The character representing the raw Y-coordinate.</param>
    /// <returns>A <see cref="MouseEventArgs"/> instance containing the parsed mouse event type,
    /// modifiers, movement status, and position.</returns>
    internal static MouseEventArgs DecomposeUtf8(char type, char x, char y) {
        var t = GetType(type - 32);
        var pos = new Point2D(x - 32, y - 32);
        
        return new MouseEventArgs(t.Btn, t.Mods, t.Move, pos, t is { Btn: MouseButton.RELEASE, Move: false });
    }

    /// <summary>
    /// Decomposes raw console key information into a detailed representation of a mouse event.
    /// </summary>
    /// <param name="type">The raw character representing the mouse event type.</param>
    /// <param name="x">The raw character representing the X-coordinate.</param>
    /// <param name="y">The raw character representing the Y-coordinate.</param>
    /// <returns>A <see cref="MouseEventArgs"/> instance containing the decomposed mouse event type, modifiers,
    /// movement flag, and coordinates.</returns>
    internal static MouseEventArgs DecomposeUtf8(ConsoleKeyInfo type, ConsoleKeyInfo x, ConsoleKeyInfo y) {
        var t = GetType(type.KeyChar - 32);
        var pos = new Point2D(RemapCoordinate(x) - 1, RemapCoordinate(y) - 1);
        
        return new MouseEventArgs(t.Btn, t.Mods, t.Move, pos, t is { Btn: MouseButton.RELEASE, Move: false });
    }

    /// <summary>
    /// Decomposes SGR (Select Graphic Rendition) encoded mouse event data into detailed mouse event arguments.
    /// </summary>
    /// <param name="type">An integer representing the raw event type, including button and modifier states.</param>
    /// <param name="x">The X-coordinate of the mouse event in absolute terms.</param>
    /// <param name="y">The Y-coordinate of the mouse event in absolute terms.</param>
    /// <param name="f">A character indicating whether the event represents a button press ('M') or a button release ('m').</param>
    /// <returns>A <see cref="MouseEventArgs"/> instance containing the parsed mouse event type,
    /// modifiers, movement status, position, and release status.</returns>
    internal static MouseEventArgs DecomposeSGR(int type, int x, int y, char f) {
        var t = GetType(type);
        var pos = new Point2D(x, y);
        var rel = f == 'm';

        return new MouseEventArgs(t.Btn, t.Mods, t.Move, pos, rel);
    }

    /// <summary>
    /// Retrieves the mouse event details, including button, modifiers, and movement status,
    /// based on the provided type value.
    /// </summary>
    /// <param name="type">An integer representing the encoded mouse event type.</param>
    /// <returns>A tuple containing the mouse button (<see cref="MouseButton"/>),
    /// modifiers (<see cref="ConsoleModifiers"/>), and a boolean indicating whether the event is a
    /// movement event.</returns>
    private static (MouseButton Btn, ConsoleModifiers Mods, bool Move) GetType(int type) {
        var flags = (MouseEventFlags)type;
        var shift = flags.GetHasShift();
        var             alt   = flags.GetHasAlt();
        var             ctrl  = flags.GetHasCtrl();
        var             move  = flags.GetHasMove();

        var cm = (shift ? Shift   : 0) | 
                 (alt   ? Alt     : 0) | 
                 (ctrl  ? Control : 0);
        
        var button = (MouseButton)(type & ~(4 + 8 + 16 + 32));
        return (button, cm, move);
    }

    /// <summary>
    /// Maps a raw console key input to its corresponding coordinate value.
    /// </summary>
    /// <param name="k">The <see cref="ConsoleKeyInfo"/> instance representing the raw key input.</param>
    /// <returns>An integer representing the mapped coordinate value.</returns>
    private static int RemapCoordinate(ConsoleKeyInfo k) {
        if (k.Key == ConsoleKey.Backspace) return 94;
        if (k.Modifiers.HasFlag(Alt)) return k.KeyChar + 32;
        return k.KeyChar - 32;
    }

    
    #region LEGACY CODE
    
    /// <summary>
    /// Decomposes raw console key information to generate detailed mouse event information.
    /// </summary>
    /// <param name="rawEvType">The raw <see cref="ConsoleKeyInfo"/> representing the mouse event type.</param>
    /// <param name="x">The raw <see cref="ConsoleKeyInfo"/> representing the X-coordinate.</param>
    /// <param name="y">The raw <see cref="ConsoleKeyInfo"/> representing the Y-coordinate.</param>
    /// <returns>A <see cref="MouseEventInfo"/> instance containing the decomposed mouse event type, modifiers, and coordinates.</returns>
    [Obsolete($"Use {nameof(MouseEventDecomposer)}.{nameof(DecomposeUtf8)} instead.")]
    private static MouseEventInfo LEGACY__DecomposeUtf8(ConsoleKeyInfo rawEvType, ConsoleKeyInfo x, ConsoleKeyInfo y) {
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
    [Obsolete($"Use {nameof(MouseEventDecomposer)}.{nameof(DecomposeSGR)} instead.")]
    private static MouseEventInfo LEGACY__DecomposeSGR(int rawEvType, int x, int y, bool press) {
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

    private static ConsoleModifiers None => 0;
    
    #endregion
}