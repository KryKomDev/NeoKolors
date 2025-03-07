using NeoKolors.Tui.Style;
using NeoKolors.Settings.Argument;

namespace NeoKolors.Tui.Elements.Interactive;

public class StringInteractiveElement : IInteractiveElement<StringArgument> {
    
    public string[] Selectors { get; }
    public StyleBlock Style { get; set; }
    
    public void UpdateStyle(StyleBlock style) => Style = style;
    public StringArgument Argument { get; }
    private string Input { get; set; } = "";

    public void Render(Rectangle rect) {
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