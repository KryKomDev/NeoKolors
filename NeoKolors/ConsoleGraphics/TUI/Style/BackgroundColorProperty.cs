using NeoKolors.Common;

namespace NeoKolors.ConsoleGraphics.TUI.Style;

public class BackgroundColorProperty : IStyleProperty<Color> {
    public Color Value { get; }
    
    public static string GetStaticName() => "background-color";
    public string GetName() => GetStaticName();
    public static Color GetStaticDefault() => new(ConsoleColor.Black);
    public Color GetDefault() => GetStaticDefault();
    
    public BackgroundColorProperty(Color value) => Value = value;
}