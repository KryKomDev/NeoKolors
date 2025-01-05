using NeoKolors.ConsoleGraphics.TUI.Style;
using NeoKolors.Settings.ArgumentTypes;

namespace NeoKolors.ConsoleGraphics.TUI.Elements.Interactive;

public class StringInteractiveElement : IInteractiveElement<StringArgument> {
    
    public string[] Selectors { get; }
    public StyleBlock Style { get; set; }
    
    public void UpdateStyle(StyleBlock style) => Style = style;
    public StringArgument Argument { get; }
    private string Input { get; set; } = "";

    public void Draw(Rectangle rect) {
        throw new NotImplementedException();
    }

    public int ComputeHeight(int width) {
        throw new NotImplementedException();
    }

    public int ComputeWidth(int height) {
        throw new NotImplementedException();
    }

    public static string GetTag() {
        throw new NotImplementedException();
    }

    public void Interact(ConsoleKeyInfo keyInfo) {
        
    }
}