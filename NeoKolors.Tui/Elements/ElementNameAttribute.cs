namespace NeoKolors.Tui.Elements;

[AttributeUsage(AttributeTargets.Class)]
public class ElementNameAttribute : Attribute {
    public string Name { get; }

    public ElementNameAttribute(string name) {
        Name = name;
    }
}