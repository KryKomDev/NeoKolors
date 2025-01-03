//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Console;
using NeoKolors.ConsoleGraphics.TUI;
using NeoKolors.ConsoleGraphics.TUI.Elements;
using NeoKolors.ConsoleGraphics.TUI.Elements.Interactive;
using NeoKolors.ConsoleGraphics.TUI.Style;
using NeoKolors.Settings;
using NeoKolors.Settings.ArgumentTypes;

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

        SettingsBuilder<IntegerArgument> builder = SettingsBuilder<IntegerArgument>.Build("int", 
            SettingsNode<IntegerArgument>
                .New("idk")
                .Group(SettingsGroup
                    .New("idk", 
                        ("min", Arguments.Integer()), 
                        ("max", Arguments.Integer()), 
                        ("default", Arguments.Integer()))
                    .Option(SettingsGroupOption
                        .New("a")
                        .Argument("min", Arguments.Integer(defaultValue: -123))
                        .Argument("max", Arguments.Integer(defaultValue: 123))
                        .Argument("default", Arguments.Integer(defaultValue: 5))
                        .EnableAutoMerge()
                    )
                    .Option(SettingsGroupOption
                        .New("b")
                        .Merges((cin, cout) => {
                            cout["min"] <<= -123;
                            cout["max"] <<= 123;
                            cout["default"] <<= -5;
                        })
                    )
                    .EnableAutoMerge()
                )
                .Constructs(context => 
                    Arguments.Integer(defaultValue: (int)context["default"].Get(), min: (int)context["min"].Get(), max: (int)
                        context["max"].Get())
                )
        );

        var a = Arguments.Integer();
        a <<= 123;

        builder["idk"]["idk"]["a"].Context["min"] <<= -1234;
        
        builder["idk"]["idk"].Select(1);
        
        // System.Console.WriteLine(builder.Nodes[0].Context["default"].Get());
        
        var i = builder.GetResult();
        System.Console.WriteLine(i);

        // SettingsBuilder<Kolors> builder = SettingsBuilder<Kolors>.Build("builder", 
        //     SettingsNode<Kolors>.New<Kolors>("")
        //                         .Argument("idk", (IArgument)new IntegerArgument())
        // ); 
        

        /*

        System.Console.Clear();

        Rectangle r = new Rectangle(2, 2, 80, 31);
        BorderProperty.BorderData borderData = new BorderProperty.BorderData(new Color(ConsoleColor.Green), BorderProperty.BorderStyle.NORMAL);

        // Color color = new Color(ConsoleColor.Green);
        // BorderProperty.WriteBorder(r, border, color);

        StyleBlock s = new StyleBlock("sd",
            new ColorProperty(ConsoleColor.White),
            new BorderProperty(borderData),
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

        var bie = new BoolInteractiveElement("cus", [], new BoolArgument(), s);

        // t1.Draw(r);

        var d = new Div([bie, bie, bie, bie], ["s"], "Divinator");

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

        var b = new StringGraphicElement(12, 5, "Hello Int", (StringArgument?)c["b"]);
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

        System.Console.CursorVisible = true;*/
    }
}