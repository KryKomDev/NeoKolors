//
// NeoKolors
// Copyright (c) 2026 KryKom
//

namespace NeoKolors.Console.Events;

public enum VTQueryResponseType : byte {
    INVALID = 0,
    DEC,
    OSC,
    WIN_STATE,
    WIN_POS,
    WIN_SIZE,
    WIN_SIZE_PX,
    SCREEN_SIZE,
    LABEL,
    WIN_TITLE,
    ICON_TITLE
}