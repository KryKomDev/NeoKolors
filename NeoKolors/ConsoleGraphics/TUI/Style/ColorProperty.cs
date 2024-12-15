using NeoKolors.Common;

namespace NeoKolors.ConsoleGraphics.TUI.Style;

public class ColorProperty : IStyleProperty<Color> {
    public Color Value { get; }
    
    public static string GetStaticName() => "color";
    public string GetName() => GetStaticName();
    public static Color GetStaticDefault() => new(ConsoleColor.Gray);
    public Color GetDefault() => GetStaticDefault();
    
    public ColorProperty(Color value) => Value = value;
    public ColorProperty(ConsoleColor c) => Value = new Color(c);
    public ColorProperty(int c) => Value = new Color(c);
}