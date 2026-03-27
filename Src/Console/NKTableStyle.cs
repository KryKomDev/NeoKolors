//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Console;

public enum NKTableStyle {
    
    /// <summary>
    /// Represents a console table style without borders or separators.
    /// This table type provides a minimalistic format, displaying data
    /// without any surrounding lines or grid structure.
    /// </summary>
    BORDERLESS,

    /// <summary>
    /// Represents a console table style using ASCII characters for borders and separators.
    /// This table type is suitable for simple, visually clear table formatting
    /// in environments that support plain text representation.
    /// </summary>
    ASCII,

    /// <summary>
    /// Represents a standard console table style with basic formatting,
    /// including borders and separators for a clean and readable layout.
    /// </summary>
    NORMAL
}