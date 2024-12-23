//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Console;
using NeoKolors.ConsoleGraphics.Settings;
using NeoKolors.ConsoleGraphics.Settings.ArgumentType;
using NeoKolors.ConsoleGraphics.TUI;
using NeoKolors.ConsoleGraphics.TUI.Elements;
using NeoKolors.ConsoleGraphics.TUI.Style;

namespace NeoKolors.Common;

public class Kolors {
    public static void Main() {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        Debug.Level = Debug.DebugLevel.ALL;
        // Debug.Fatal("fatal");
        // Debug.Error("error");
        // Debug.Warn("warn");
        // Debug.Info("info");
        // Debug.Msg("debug");

        // for (int i = 0; i < System.Console.WindowHeight; i++) {
        //     System.Console.WriteLine($"{i}{new string(' ', System.Console.WindowWidth - i.ToString().Length)}");
        // }

        System.Console.Clear();
        
        Rectangle r = new Rectangle(2, 2, 80, 31);
        BorderProperty.Border border = new BorderProperty.Border(new Color(ConsoleColor.Green), BorderProperty.BorderStyle.NORMAL);
        
        // Color color = new Color(ConsoleColor.Green);
        // BorderProperty.WriteBorder(r, border, color);

        StyleBlock s = new StyleBlock("sd",
            new ColorProperty(ConsoleColor.White),
            new BorderProperty(border),
            new BackgroundColorProperty(new Color(ConsoleColor.Black)),
            // new HorizontalAlignItemsProperty(HorizontalAlignDirection.CENTER),
            new VerticalAlignItemsProperty(VerticalAlignDirection.TOP),
            new MarginProperty(new MarginProperty.MarginData(
                new SizeValue(0, SizeValue.UnitType.CHAR),
                new SizeValue(0, SizeValue.UnitType.PERCENT),
                new SizeValue(0, SizeValue.UnitType.CHAR),
                new SizeValue(0, SizeValue.UnitType.CHAR))),
            new PaddingProperty(new PaddingProperty.PaddingData(
                new SizeValue(1, SizeValue.UnitType.CHAR),
                new SizeValue(0, SizeValue.UnitType.CHAR),
                new SizeValue(1, SizeValue.UnitType.CHAR),
                new SizeValue(0, SizeValue.UnitType.CHAR)))
            );
        
        System.Console.CursorVisible = false;

        string str = StringEffects.AddTextStyles(
            "<b>Lorem</b> <i>ipsum</i> <u>dolor</u> <s>sit</s> <f>amet</f>, <n>consectetur</n> adipisici elit, sed eiusmod tempor incidunt ut labore et" +
            " dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquid ex ea commodi consequat.");
        
        
        Text t1 = new Text(str, ["sd"], s);
        Text t2 = new Text(str, ["sd"], s);
        
        // t1.Draw(r);

        Div d = new Div([t1, t2, t1], ["s"], "Divinator");


        d.UpdateStyle(new StyleBlock("sd",
            new DisplayProperty(DisplayProperty.DisplayType.FLEX),
            new FlexFlowProperty(FlexFlowProperty.FlexDirection.ROW_REVERSE, false),
            new BackgroundColorProperty(ConsoleColor.Black),
            new ListStyleProperty(ListStyleProperty.ListStyle.SQUARE, ConsoleColor.Magenta),
            new PaddingProperty(new PaddingProperty.PaddingData(
                new SizeValue(1, SizeValue.UnitType.CHAR),
                new SizeValue(1, SizeValue.UnitType.CHAR),
                new SizeValue(1, SizeValue.UnitType.CHAR),
                new SizeValue(1, SizeValue.UnitType.CHAR)))
        ));
        
        d.Draw(r);
        
        System.Console.SetCursorPosition(System.Console.WindowWidth - 1, System.Console.WindowHeight - 1);
        // System.Console.WriteLine(r.Width);
        
        // for (int y = 0; y < System.Console.WindowHeight - 10; y++) {
        //     ConsoleColors.PrintlnColored(new string(' ', System.Console.WindowWidth - 80), ConsoleColor.DarkBlue, ConsoleColor.Red);
        // }
        
        // System.Console.SetCursorPosition(10, 10);
        // System.Console.Write("asdsaddsasad");
        
        Context c = new();
        IArgumentType arg = new StringArgumentType(allowSpecial: false, minLength: 5, maxLength: 15);
        arg <<= "hello";
        c.Add("b", arg);

        var b = new StringGraphicElement(12, 5, "Hello Int", (StringArgumentType?)c["b"]);
        // var b2 = new BoolGraphicElement(12, 6, "Bool Elem");
        // b.Draw(12, 5);
        // b2.Draw(12, 6);
        // b.Selected = true;
        
        // ConsoleKeyInfo key;
        //
        // do {
        //     System.Console.SetCursorPosition(0, System.Console.WindowHeight - 1);
        //     key = System.Console.ReadKey();
        //     b.Interact(key);
        //     b.Draw(b.GridX, b.GridY);
        // } 
        // while (key.Key != ConsoleKey.Escape);
        
        System.Console.CursorVisible = true;
    }
}