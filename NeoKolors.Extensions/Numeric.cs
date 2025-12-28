// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Extensions;

public static class Numeric {
    
    extension(int value) {
        
        /// <summary>
        /// Determines whether the current numeric value is within the specified range, inclusive of the range's boundaries.
        /// If the range's boundaries are from the end of the range, the range is considered to be open on that side
        /// (value is excluded). <br/>
        /// <b>WARNING:</b> This works only for ranges with non-negative start and end values
        /// (from-end indices are detected by checking if the index value is negative).
        /// </summary>
        /// <param name="range">The range to check the numeric value against.</param>
        /// <returns>True if the numeric value lies within the range; otherwise, false.</returns>
        /// <example>
        /// <code>
        /// var t1 = 5.Belongs(1..10);    // true
        /// var t2 = 11.Belongs(1..10);   // false
        /// var t3 = 10.Belongs(1..10);   // true
        /// var t4 = 10.Belongs(10..20);  // true
        /// var t5 = 10.Belongs(10..10);  // true
        /// var t6 = 10.Belongs(^10..20); // false
        /// var t7 = 10.Belongs(1..^10);  // false
        /// </code>
        /// </example>
        public bool Belongs(Range range) {
            range = new Range(
                range.Start.IsFromEnd
                    ? range.Start.Value + 1 
                    : range.Start.Value, 
                range.End.IsFromEnd 
                    ? range.End.Value - 1 
                    : range.End.Value
            );
            
            return value >= range.Start.Value && value <= range.End.Value;
        }
    }

    extension(int) {
        public static bool operator ==(int left, Range right) => Belongs(left, right);
        public static bool operator !=(int left, Range right) => !Belongs(left, right);
    }

    extension(int val) {
        public int Clamp(int min, int max) => Math.Clamp(val, min, max);
    }

    extension(Math) {
        public static int DClamp(int val, int b0, int b1) => Math.Clamp(val, Math.Min(b0, b1), Math.Max(b0, b1));
    }
}