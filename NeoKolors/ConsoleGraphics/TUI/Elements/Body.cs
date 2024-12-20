namespace NeoKolors.ConsoleGraphics.TUI.Elements;

public class Body : Div {
    public Body(IElement[] content) : base(content, []) { }
    public new static string GetTag() => "body";
}