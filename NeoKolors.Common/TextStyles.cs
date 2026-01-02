//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common;

/// <summary>
/// style types
/// </summary>
[Flags]
public enum TextStyles : byte {
    NONE          = 0,
    BOLD          = 1 << 0,
    FAINT         = 1 << 1,
    ITALIC        = 1 << 2,
    UNDERLINE     = 1 << 3,
    BLINK         = 1 << 4,
    NEGATIVE      = 1 << 5,
    INVISIBLE     = 1 << 6,
    STRIKETHROUGH = 1 << 7,
}