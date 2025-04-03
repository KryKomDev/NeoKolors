using System.Text;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui;
using NeoKolors.Tui.Fonts;
using static NeoKolors.Common.TextStyles;

namespace Testing;

public static class NeoKolors {
    public static void Main() {
        Console.OutputEncoding = Encoding.UTF8;
        Debug.LogAll();
        // Debug.Level = Debug.DebugLevel.FATAL | Debug.DebugLevel.ERROR | Debug.DebugLevel.WARN;
        Debug.EnableAutoFancy = true;

        Debug.Fatal("fuck");
        Debug.Error("fuck");
        Debug.Warn("what");
        Debug.Info("ok");
        Debug.Log("ok");
        
        var s = new ConsoleScreen();
        s.Rect(new Rectangle(2, 2, 5, 5), new NKColor(NKConsoleColor.MAGENTA), 
            BorderStyle.GetAscii(NKConsoleColor.BLUE, NKConsoleColor.MAGENTA));
        
        Console.WriteLine("adshoj".AddColor(ConsoleColor.DarkBlue));
        Console.WriteLine("asdhoj".AddColor(NKConsoleColor.DARK_BLUE));
        
        // s.Render();

        // var c = new AppConfig(false, new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, true), true, 
        //     toggleDebugLogCombination: new ConsoleKeyInfo('d', ConsoleKey.D, false, true, true));
        //
        // var a = new Application(c);
        // var v = new View();
        // a.AddView(v);
        // a.Start();

        // Console.SetOut(new ConsoleScreen());

        // Application a = new Application(c);
        // a.Start();

        /*

        Rectangle r = new Rectangle(2, 2, 80, 31);
        BorderProperty.BorderData borderData = new BorderProperty.BorderData(new Color(ConsoleColor.Green), BorderProperty.BorderStyle.NORMAL);

        // Color color = new Color(ConsoleColor.Green);
        // BorderProperty.WriteBorder(r, border, color);

        StyleBlock s = new StyleBlock("sd",
            new ColorProperty(ConsoleColor.White),
            new BorderProperty(borderData),
            new CheckboxProperty(CheckboxProperty.CheckboxStyle.SWITCH),
            new BackgroundColorProperty(new Color(ConsoleColor.Black)),
            // new HorizontalAlignItemsProperty(HorizontalAlignDirection.CENTER),
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
                   "exercitation ullamco laboris nisi ut aliquid ex ea commodi consequat.").AddStyles();

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

        Context c = new();
        IArgument arg = new StringArgument(allowSpecial: false, minLength: 5, maxLength: 15);
        arg <<= "hello";
        c.Add("b", arg);

        var b = new StringInteractiveElement();

        System.Console.CursorVisible = true;*/
    } 
}

public enum Test {
    T1,
    T2,
    T3,
    T4,
    T5,
}