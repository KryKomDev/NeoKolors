//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Console;
using NeoKolors.Settings;
using NeoKolors.Settings.ArgumentTypes;

namespace NeoKolors.ConsoleGraphics.Elements.InteractiveOld;

public class BoolGraphicElement : IGraphicElement {
    public int GridX { get; set; }
    public int GridY { get; set; }
    public int Width { get; init; }
    public int Height { get; init; }
    public string Name { get; init; }
    public bool Selected { get; set; } = false;

    private readonly BoolArgument argument = Arguments.Bool();

    public void Draw(int x, int y) {
        System.Console.SetCursorPosition(x, y);

        if (Selected)
            ConsoleColors.PrintColored($"{Name}: ", 0xffffff);
        else
            System.Console.Write($"{Name}: ");

        if (argument.Value)
            ConsoleColors.PrintComplexColored("[*yy*r] *nn*r ", ("*y", Debug.InfoColor), ("*n", Debug.ErrorColor),
                ("*r", -1));
        else
            ConsoleColors.PrintComplexColored(" *yy*r [*nn*r]", ("*y", Debug.InfoColor), ("*n", Debug.ErrorColor),
                ("*r", -1));
    }

    public void Interact(ConsoleKeyInfo keyInfo) {
        
        switch (keyInfo.Key) {
            case ConsoleKey.J or ConsoleKey.Y:
                argument.Set(true);
                break;
            case ConsoleKey.K or ConsoleKey.N:
                argument.Set(false);
                break;
        }
    }

    public object Get() {
        return argument.Get();
    }

    public BoolGraphicElement(int x, int y, string name) {
        Name = name;
        GridX = x;
        GridY = y;
        argument.Set(false);
        Width = Name.Length + 8;
        Height = 1;
    }
}