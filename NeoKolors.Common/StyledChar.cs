//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common;

file readonly struct StyledChar : IEquatable<StyledChar> {
    public char Char { get; }
    public TextStyles Style { get; }

    public StyledChar(char c, TextStyles style) {
        Char  = c;
        Style = style;
    }

    public StyledChar() {
        Char = '\0';
        Style = TextStyles.NONE;
    }

    public bool Equals(StyledChar other) => Char == other.Char && Style == other.Style;
    public override bool Equals(object? obj) => obj is StyledChar other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Char, (int)Style);
    public static bool operator ==(StyledChar left, StyledChar right) => left.Equals(right);
    public static bool operator !=(StyledChar left, StyledChar right) => !left.Equals(right);

    public void Deconstruct(out char c, out TextStyles style) {
        c     = Char;
        style = Style;
    }
    
    public override string ToString() => $"{Style.GetEscSeq()}{Char}";
}