using NeoKolors.ConsoleGraphics.GUI;
using NeoKolors.ConsoleGraphics.GUI.Elements;
using NeoKolors.ConsoleGraphics.Settings;
using NeoKolors.ConsoleGraphics.Settings.ArgumentType;

namespace NeoKolors.Common;

public class Kolors {
    public static void Main() {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        // Debug.Level = Debug.DebugLevel.ALL;
        // Debug.Fatal("fatal");
        // Debug.Error("error");
        // Debug.Warn("warn");
        // Debug.Info("info");
        // Debug.Trace("debug")
        
        System.Console.CursorVisible = false;

        
        // string name = "AhoyString";
        // System.Console.Write(name);
        // System.Console.WriteLine($": ┌{new string('─', 50)}┐");
        // for (int i = 0; i < 7; i++) {
        //     System.Console.WriteLine($"{new string(' ', name.Length + 2)}│{new string(' ', 50)}│");
        // }
        // System.Console.WriteLine($"{new string(' ', name.Length + 2)}└{new string('─', 50)}┘");
        
        for (int y = 0; y < System.Console.WindowHeight + 10; y++) {
            System.Console.WriteLine(new string(' ', System.Console.WindowWidth));
        }
        
        Context c = new();
        IArgumentType arg = new StringArgumentType(allowSpecial: false, minLength: 5, maxLength: 15);
        arg <<= "hello";
        c.Add("b", arg);

        var b = new StringGraphicElement(12, 5, "Hello Int", (StringArgumentType?)c["b"]);
        var b2 = new BoolGraphicElement(12, 6, "Bool Elem");
        b.Draw(12, 5);
        b2.Draw(12, 6);
        b.Selected = true;
        
        ConsoleKeyInfo key;
        
        do {
            System.Console.SetCursorPosition(0, System.Console.WindowHeight - 1);
            key = System.Console.ReadKey();
            b.Interact(key);
            b.Draw(b.GridX, b.GridY);
        } 
        while (key.Key != ConsoleKey.Escape);
    }
}