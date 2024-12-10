//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Console;
using NeoKolors.ConsoleGraphics.Settings;
using NeoKolors.ConsoleGraphics.Settings.ArgumentType;
using NeoKolors.ConsoleGraphics.TUI;
using NeoKolors.ConsoleGraphics.TUI.Elements;
using NeoKolors.ConsoleGraphics.TUI.GridSystem;

namespace NeoKolors.Common;

public class Kolors {
    public static void Main() {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        Debug.Level = Debug.DebugLevel.ALL;
        Debug.Fatal("fatal");
        Debug.Error("error");
        Debug.Warn("warn");
        Debug.Info("info");
        Debug.Msg("debug");

        Rectangle r = new Rectangle(10, 10, 0, 20);
        
        System.Console.CursorVisible = false;
        
        for (int y = 0; y < System.Console.WindowHeight + 10; y++) {
            System.Console.WriteLine(new string(' ', System.Console.WindowWidth));
        }
        
        Context c = new();
        IArgumentType arg = new StringArgumentType(allowSpecial: false, minLength: 5, maxLength: 15);
        arg <<= "hello";
        c.Add("b", arg);

        var b = new StringGraphicElement(12, 5, "Hello Int", (StringArgumentType?)c["b"]);
        // var b2 = new BoolGraphicElement(12, 6, "Bool Elem");
        // b.Draw(12, 5);
        // b2.Draw(12, 6);
        // b.Selected = true;
        
        // GridSection g = GridSection.New("Section", 0, 0, (0, 0), (1, 4), b);
        // GridSection g2 = GridSection.New("Section", 0, 0, (1, 0), (4, 1), b);
        // GridSection g3 = GridSection.New("Section", 0, 0, (1, 2), (4, 4), b);
        GridBuilder builder = new GridBuilder(4, 4);
        // builder.Build(g, g2, g3);
        // GridRenderer.Render(builder);
        
        GridRenderer.RenderGrid(builder);
        
        // ConsoleKeyInfo key;
        //
        // do {
        //     System.Console.SetCursorPosition(0, System.Console.WindowHeight - 1);
        //     key = System.Console.ReadKey();
        //     b.Interact(key);
        //     b.Draw(b.GridX, b.GridY);
        // } 
        // while (key.Key != ConsoleKey.Escape);
    }
}