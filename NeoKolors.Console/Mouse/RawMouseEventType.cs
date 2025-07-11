// NeoKolors
// Copyright (c) 2025 KryKom

using static NeoKolors.Console.Mouse.XTermMouseModifiers;

namespace NeoKolors.Console.Mouse;

file enum RawMouseEventType {
    NONE_MOVE =         'C',
    NONE_LEFT_PRESS =   ' ',
    NONE_MIDDLE_PRESS = '!',
    NONE_RIGHT_PRESS =  '"',
    NONE_RELEASE =      '#',
    NONE_LEFT_DRAG =    '@',
    NONE_MIDDLE_DRAG =  'A',
    NONE_RIGHT_DRAG =   'B',
    NONE_WHEEL_UP =     '`',
    NONE_WHEEL_DOWN =   'a',

    CTRL_MOVE =         NONE_MOVE + CTRL,         // 'S'
    CTRL_LEFT_PRESS =   NONE_LEFT_PRESS + CTRL,   // '0'
    CTRL_MIDDLE_PRESS = NONE_MIDDLE_PRESS + CTRL, // '1',
    CTRL_RIGHT_PRESS =  NONE_RIGHT_PRESS + CTRL,  // '2',
    CTRL_RELEASE =      NONE_RELEASE + CTRL,      // '3',
    CTRL_LEFT_DRAG =    NONE_LEFT_DRAG + CTRL,    // 'P'
    CTRL_MIDDLE_DRAG =  NONE_MIDDLE_DRAG + CTRL,  // 'R',
    CTRL_RIGHT_DRAG =   NONE_RIGHT_DRAG + CTRL,   // 'Q',
    CTRL_WHEEL_UP =     NONE_WHEEL_UP + CTRL,     // 'p'
    CTRL_WHEEL_DOWN =   NONE_WHEEL_DOWN + CTRL,   // 'q',
    
    ALT_MOVE =         NONE_MOVE + ALT,         // 'K'
    ALT_LEFT_PRESS =   NONE_LEFT_PRESS + ALT,   // '('
    ALT_MIDDLE_PRESS = NONE_MIDDLE_PRESS + ALT, // ')',
    ALT_RIGHT_PRESS =  NONE_RIGHT_PRESS + ALT,  // '*',
    ALT_RELEASE =      NONE_RELEASE + ALT,      // '+',
    ALT_LEFT_DRAG =    NONE_LEFT_DRAG + ALT,    // 'H'
    ALT_MIDDLE_DRAG =  NONE_MIDDLE_DRAG + ALT,  // 'I',
    ALT_RIGHT_DRAG =   NONE_RIGHT_DRAG + ALT,   // 'J',
    ALT_WHEEL_UP =     NONE_WHEEL_UP + ALT,     // 'h'
    ALT_WHEEL_DOWN =   NONE_WHEEL_DOWN + ALT,   // 'i',
    
    CTRL_ALT_MOVE =         NONE_MOVE + CTRL + ALT,         // '['
    CTRL_ALT_LEFT_PRESS =   NONE_LEFT_PRESS + CTRL + ALT,   // '8'
    CTRL_ALT_MIDDLE_PRESS = NONE_MIDDLE_PRESS + CTRL + ALT, // '9',
    CTRL_ALT_RIGHT_PRESS =  NONE_RIGHT_PRESS + CTRL + ALT,  // ':',
    CTRL_ALT_RELEASE =      NONE_RELEASE + CTRL + ALT,      // ';',
    CTRL_ALT_LEFT_DRAG =    NONE_LEFT_DRAG + CTRL + ALT,    // 'X'
    CTRL_ALT_MIDDLE_DRAG =  NONE_MIDDLE_DRAG + CTRL + ALT,  // 'Y',
    CTRL_ALT_RIGHT_DRAG =   NONE_RIGHT_DRAG + CTRL + ALT,   // 'Z',
    CTRL_ALT_WHEEL_UP =     NONE_WHEEL_UP + CTRL + ALT,     // 'x'
    CTRL_ALT_WHEEL_DOWN =   NONE_WHEEL_DOWN + CTRL + ALT,   // 'y',
}

file enum XTermMouseModifiers {
    NONE = 0,
    SHIFT = 4,
    ALT = 8,
    CTRL = 16
}