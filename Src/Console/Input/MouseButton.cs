// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Console.Input;

/// <summary>
/// Represents the various buttons and states of a mouse.
/// This includes standard mouse buttons, additional buttons,
/// scrolling actions, and release states for more detailed input handling.
///
/// <b>NOTE:</b> This enum is compliant with the Xterm mouse event encoding standard. 
/// </summary>
public enum MouseButton {
    LEFT       = 0,
    MIDDLE     = 1,
    RIGHT      = 2,
    RELEASE    = 3,
    WHEEL_UP   = 0 + 64,
    WHEEL_DOWN = 1 + 64,
    MB6        = 2 + 64,
    MB7        = 3 + 64,
    MB8        = 0 + 128,
    MB9        = 1 + 128,
    MB10       = 2 + 128,
    MB11       = 3 + 128,
}