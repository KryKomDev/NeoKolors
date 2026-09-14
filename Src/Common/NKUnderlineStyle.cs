// NeoKolors
// Copyright (c) krystof 2026

using System.Text;

namespace NeoKolors.Common;

/// <summary>
/// Represents an underline style with customizable color and type for text styling.
/// </summary>
/// <remarks>
/// This structure encapsulates properties for both the color and the style of the underline,
/// allowing fine-grained control over the appearance of underlined text.
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = 4)]
public readonly record struct NKUnderlineStyle {

    private const uint COLOR_MASK          = 0x03FFFFFFu;
    private const byte UNDERLINE_TYPE_MASK = 0x1C;
    private const byte INHERIT_TYPE_MASK   = 0x20;

    [FieldOffset(0)] private readonly uint _color;
    [FieldOffset(3)] private readonly byte _flags;

    public NKColor Color {
        get => NKColor.FromRaw(_color & COLOR_MASK);
        init {
            var raw = value.GetRaw() & COLOR_MASK;
            _color = (_color & ~COLOR_MASK) | raw;
        }
    }

    public NKUnderlineType Type {
        get => (NKUnderlineType)((_flags & UNDERLINE_TYPE_MASK) >> 2);
        init => _flags = (byte)((_flags & ~UNDERLINE_TYPE_MASK) | (((byte)value << 2) & UNDERLINE_TYPE_MASK));
    }

    public bool InheritType {
        get => (_flags & INHERIT_TYPE_MASK) != 0;
        init => _flags = (byte)(value ? (_flags | INHERIT_TYPE_MASK) : (_flags & ~INHERIT_TYPE_MASK));
    }

    public NKUnderlineStyle(NKColor color, NKUnderlineType flags, bool inheritType = false) {
        _color = color.GetRaw() & COLOR_MASK;
        _flags = (byte)((_flags & ~(UNDERLINE_TYPE_MASK | INHERIT_TYPE_MASK)) 
                      | (((byte)flags << 2) & UNDERLINE_TYPE_MASK)
                      | (inheritType ? INHERIT_TYPE_MASK : 0));
    }

    public string GetEscSeq() {
        var sb = new StringBuilder();

        sb.Append(EscapeCodes.GetUnderline(Type));

        if (Color != NKColor.Inherit)
            sb.Append(Color.Underline);

        return sb.ToString();
    }

    public NKUnderlineStyle With(NKUnderlineStyle other) {
        var u = other.Color.IsInherit ? Color : other.Color;
        var t = other.InheritType ? Type : other.Type;
        
        return new NKUnderlineStyle(u, t, other.InheritType);
    }

    public override string ToString() {
        var sb = new StringBuilder();
        
        sb.Append("NKUnderlineStyle { ");
        PrintMembers(sb);
        sb.Append(" }");
        
        return sb.ToString();
    }

    private bool PrintMembers(StringBuilder builder) {
        builder.Append("Color = ");
        Color.AppendMembers(builder);
        builder.Append(", Type = ");
        builder.Append(Type);
        
        return true;
    }
}