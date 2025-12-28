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
    ITALIC        = 1 << 1,
    UNDERLINE     = 1 << 2,
    FAINT         = 1 << 3,
    NEGATIVE      = 1 << 4,
    STRIKETHROUGH = 1 << 5
}