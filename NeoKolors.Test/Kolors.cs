//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.ConsoleGraphics;
using NeoKolors.ConsoleGraphics.Elements.Interactive;
using NeoKolors.ConsoleGraphics.Style;
using NeoKolors.ConsoleGraphics;
using NeoKolors.ConsoleGraphics.Elements;
using NeoKolors.ConsoleGraphics.Elements.Interactive;
using NeoKolors.ConsoleGraphics.Elements.InteractiveOld;
using NeoKolors.ConsoleGraphics.Style;
using NeoKolors.Settings;
using NeoKolors.Settings.ArgumentTypes;

namespace NeoKolors.Test;

public class Kolors {
    public static void Main() {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        Debug.Level = Debug.DebugLevel.ALL;
        // Debug.Fatal("fatal");
        // Debug.Error("error");
        // Debug.Warn("warn");
        // Debug.Info("info");
        // Debug.Msg("debug");

        System.Console.Clear();

        Rectangle r = new Rectangle(2, 2, 80, 31);
        BorderProperty.BorderData borderData = new BorderProperty.BorderData(new Color(ConsoleColor.Green), BorderProperty.BorderStyle.NORMAL);

        // Color color = new Color(ConsoleColor.Green);
        // BorderProperty.WriteBorder(r, border, color);

        StyleBlock s = new StyleBlock("sd",
            new ColorProperty(ConsoleColor.White),
            new BorderProperty(borderData),
            new CheckboxProperty(CheckboxProperty.CheckboxStyle.SWITCH),
            new BackgroundColorProperty(new Color(ConsoleColor.Black)),
            // new HorizontalAlignItemsProperty(HorizontalAlignDirection.CENTER),w
            new VerticalAlignItemsProperty(VerticalAlignDirection.TOP),
            new MarginProperty(new MarginProperty.MarginData(
                new SizeValue(0, SizeValue.SizeOptions.UNIT_CHAR),
                new SizeValue(0, SizeValue.SizeOptions.UNIT_PERCENT),
                new SizeValue(0, SizeValue.SizeOptions.UNIT_CHAR),
                new SizeValue(0, SizeValue.SizeOptions.UNIT_CHAR))),
            new PaddingProperty(new PaddingProperty.PaddingData(
                new SizeValue(1, SizeValue.SizeOptions.UNIT_CHAR),
                new SizeValue(0, SizeValue.SizeOptions.UNIT_CHAR),
                new SizeValue(1, SizeValue.SizeOptions.UNIT_CHAR),
                new SizeValue(0, SizeValue.SizeOptions.UNIT_CHAR)))
            );

        System.Console.CursorVisible = false;

        var str = ("<b>Lorem</b> <i>ipsum</i> <u>dolor</u> <s>sit</s> <f>amet</f>, <n>consectetur</n> adipisici elit, " +
                   "sed eiusmod tempor incidunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud " +
                   "exercitation ullamco laboris nisi ut aliquid ex ea commodi consequat.").AddTextStyles();

        var t1 = new Text(str, ["sd"], s);
        var t2 = new Text(str, ["sd"], s);

        var bief = new BoolInteractiveElement("cus", [], new BoolArgument(), s);
        var biet = new BoolInteractiveElement("cus", [], new BoolArgument(), s);
        biet.Argument.Set(true);

        // t1.Draw(r);

        var d = new Div([bief, biet, biet, bief, bief], ["s"], "Divinator");

        d.UpdateStyle(new StyleBlock("sd",
            new BorderProperty(BorderProperty.BorderStyle.NORMAL),
            // new DisplayProperty(DisplayProperty.DisplayType.FLEX),
            // new FlexFlowProperty(FlexFlowProperty.FlexDirection.ROW_REVERSE, false),
            new BackgroundColorProperty(ConsoleColor.Black),
            new ListStyleProperty(ListStyleProperty.ListStyle.SQUARE, ConsoleColor.Magenta),
            new PaddingProperty(new PaddingProperty.PaddingData(
                new SizeValue(1, SizeValue.SizeOptions.UNIT_CHAR),
                new SizeValue(1, SizeValue.SizeOptions.UNIT_CHAR),
                new SizeValue(1, SizeValue.SizeOptions.UNIT_CHAR),
                new SizeValue(1, SizeValue.SizeOptions.UNIT_CHAR)))
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
        IArgument arg = new StringArgument(allowSpecial: false, minLength: 5, maxLength: 15);
        arg <<= "hello";
        c.Add("b", arg);

        var b = new StringGraphicElement(12, 5, "Hello Int", (StringArgument)c["b"]);
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