//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common;

/// <summary>
/// style types
/// </summary>
[Flags]
public enum TextStyles {
    BOLD = 1,
    ITALIC = 2,
    UNDERLINE = 4,
    FAINT = 8,
    NEGATIVE = 16,
    STRIKETHROUGH = 32
}