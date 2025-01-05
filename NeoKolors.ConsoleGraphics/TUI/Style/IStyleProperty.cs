namespace NeoKolors.ConsoleGraphics.TUI.Style;

public interface IStyleProperty<out T> where T : class {
    public T Value { get; }
    public static abstract string GetStaticName();
    public string GetName();
    public static abstract T GetStaticDefault();
    public T GetDefault();
}