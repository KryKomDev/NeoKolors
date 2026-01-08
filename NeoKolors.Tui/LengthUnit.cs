// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui;

public enum LengthUnit {
    
    /// <summary>
    /// 2x1 characters
    /// </summary>
    PIXEL,
    
    /// <summary>
    /// a single character
    /// </summary>
    CHAR,
    
    /// <summary>
    /// percentage of the (parent) elementOld's size
    /// </summary>
    PERCENT,
    
    VIEWPORT_WIDTH,
    VIEWPORT_HEIGHT,
}

public static class LengthUnitExtensions {
    extension(LengthUnit u) {
        public string String => u switch {
            LengthUnit.PIXEL => "px",
            LengthUnit.CHAR => "ch",
            LengthUnit.PERCENT => "%",
            LengthUnit.VIEWPORT_WIDTH => "vw",
            LengthUnit.VIEWPORT_HEIGHT => "vh",
            _ => throw new ArgumentOutOfRangeException(nameof(u), u, null)
        };
    }
}