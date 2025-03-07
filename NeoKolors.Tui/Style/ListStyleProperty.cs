using NeoKolors.Common;

namespace NeoKolors.Tui.Style;

public class ListStyleProperty : IStyleProperty<ListStyleProperty.ListStyleData> {
    public ListStyleData Value { get; }
    public static string GetStaticName() => "list-style";
    public string GetName() => GetStaticName();
    public static ListStyleData GetStaticDefault() => new(ListStyle.ASTERISK);
    public ListStyleData GetDefault() => GetStaticDefault();
    
    public ListStyleProperty(ListStyleData value) => Value = value;
    public ListStyleProperty(ListStyle style, Color? color) => Value = new ListStyleData(style, color);

    public class ListStyleData {
        public ListStyle Style { get; }
        public Color Color { get; }

        public ListStyleData(ListStyle style, Color? color = null) {
            Style = style;
            Color = color ?? ConsoleColor.Gray;
        }

        public static implicit operator ListStyleData(ListStyle style) => new(style);
        public static implicit operator ListStyle(ListStyleData data) => data.Style;

        public string GetPoint() {
            return Style switch {
                ListStyle.NONE => "".AddColor(Color),
                ListStyle.ASTERISK => "*".AddColor(Color),
                ListStyle.ASCII_ARROW => ">".AddColor(Color),
                ListStyle.ARROW => "→".AddColor(Color),
                ListStyle.CIRCLE => "○".AddColor(Color),
                ListStyle.CIRCLE_FILL => "●".AddColor(Color),
                ListStyle.SQUARE => "□".AddColor(Color),
                ListStyle.SQUARE_FILL => "■".AddColor(Color),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public enum ListStyle {
        NONE,
        ASTERISK,
        ASCII_ARROW,
        ARROW,
        CIRCLE,
        CIRCLE_FILL,
        SQUARE,
        SQUARE_FILL,
    }
}