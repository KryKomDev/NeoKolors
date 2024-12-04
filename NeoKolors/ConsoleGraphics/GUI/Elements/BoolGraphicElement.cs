//
// NeoKolors
// by KryKom 2024
//

using NeoKolors.Console;
using NeoKolors.ConsoleGraphics.Settings;
using NeoKolors.ConsoleGraphics.Settings.ArgumentType;

namespace NeoKolors.ConsoleGraphics.GUI.Elements;

public class BoolGraphicElement : IGraphicElement {
    public int GridX { get; set; }
    public int GridY { get; set; }
    public int Width { get; init; }
    public int Height { get; init; }
    public string Name { get; init; }
    public bool Selected { get; set; } = false;

    private readonly BoolArgumentType argument = Arguments.Bool();

    public void Draw(int x, int y) {
        System.Console.SetCursorPosition(x, y);

        if (Selected)
            ConsoleColors.PrintColored($"{Name}: ", 0xffffff);
        else
            System.Console.Write($"{Name}: ");

        if ((bool)argument.GetValue())
            ConsoleColors.PrintComplexColored("[*yy*r] *nn*r ", ("*y", Debug.InfoColor), ("*n", Debug.ErrorColor),
                ("*r", -1));
        else
            ConsoleColors.PrintComplexColored(" *yy*r [*nn*r]", ("*y", Debug.InfoColor), ("*n", Debug.ErrorColor),
                ("*r", -1));
    }

    public void Interact(ConsoleKeyInfo keyInfo) {
        
        switch (keyInfo.Key) {
            case ConsoleKey.J or ConsoleKey.Y:
                argument.SetValue(true);
                break;
            case ConsoleKey.K or ConsoleKey.N:
                argument.SetValue(false);
                break;
        }
    }

    public object GetValue() {
        return argument.GetValue();
    }

    public BoolGraphicElement(int x, int y, string name) {
        Name = name;
        GridX = x;
        GridY = y;
        argument.SetValue(false);
        Width = Name.Length + 8;
        Height = 1;
    }
}