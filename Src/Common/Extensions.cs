//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common;

/// <summary>
/// Provides extension methods for applying styles and colors to strings and characters
/// using the NeoKolors framework.
/// </summary>
public static class Extensions {
    
    /// <summary>
    /// Calculates the distance between the specified value and a given range defined by minimum and maximum bounds.
    /// </summary>
    /// <param name="value">The integer value to evaluate against the range.</param>
    /// <param name="min">The minimum boundary of the range.</param>
    /// <param name="max">The maximum boundary of the range.</param>
    /// <returns>
    /// A non-negative integer representing the distance between the value and the range.
    /// Returns 0 if the value is within the range.
    /// </returns>
    public static int RangeDist(this int value, int min, int max) =>
        value < min ? min - value : value > max ? value - max : 0;

    public static string ToString(ConsoleKeyInfo key) => 
        $"{(key.HasCtrl  ? "Ctrl + " : "")}" +
        $"{(key.HasAlt   ? "Alt + " : "")}" +
        $"{(key.HasShift ? "Shift + " : "")}" + 
        $"{key.Key.ToString()} => '{key.KeyChar}'";


    extension(uint rgb) {
        public byte R => (byte)(rgb >> 16);
        public byte G => (byte)(rgb >> 8);
        public byte B => (byte)(rgb >> 0);
    }
}