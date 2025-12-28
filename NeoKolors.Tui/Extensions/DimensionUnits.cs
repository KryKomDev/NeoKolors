// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Extensions;

public static class DimensionUnits {

    extension(int val) {
        public Dimension Ch => Dimension.Chars(val);
        public Dimension Vw => Dimension.ViewportWidth(val);
        public Dimension Vh => Dimension.ViewportHeight(val);
        public Dimension Pc => Dimension.Percent(val);
        public Dimension Px => Dimension.Pixels(val);
    }
}