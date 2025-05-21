using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using NeoKolors;
using NeoKolors.Common;
using NeoKolors.Common.Util;
using NeoKolors.Console;
using NeoKolors.Settings.Argument;
using NeoKolors.Tui;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.SourceManagement;
using NeoKolors.Tui.Styles;
using SkiaSharp;
using static NeoKolors.Common.NKConsoleColor;
using static NeoKolors.Common.TextStyles;
using static NeoKolors.Console.LoggerLevel;

namespace Testing;

// git ls-files | grep '\.cs' | xargs wc -l

public static class NeoKolors {
    private const string LOREM = "Lorem ipsum dolor sit amet, consectetur adipisici elit, sed eiusmod tempor" +
                                 " incidunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud" +
                                 " exercitation ullamco laboris nisi ut aliquid ex ea commodi consequat. Quis aute" +
                                 " iure reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla " +
                                 "pariatur. Excepteur sint obcaecat cupiditat non proident, sunt in culpa qui " +
                                 "officia deserunt mollit anim id est laborum.";
    
    public static void Main() {
        Console.OutputEncoding = Encoding.UTF8;
        NKDebug.Logger.Level = FATAL | ERROR | WARN | INFO | DEBUG | TRACE;
        NKDebug.ExceptionFormatting = true;
        NKDebug.Logger.SimpleMessages = true;
        NKDebug.Logger.FileConfig = LogFileConfig.Replace(@"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\Examples\Testing\log.log");
        
        NKConsole.WriteTable(["FColor", "BColor", "Styles"], NKStyle.Default, new NKStyle(C_ALLOY_ORANGE, NKColor.Inherit, BOLD));
        
        return;
        
        using var s = new ConsoleScreen();

        NKCharFontReader fr = new NKCharFontReader(@"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\BuiltIn\Basic.nkf");
        var f = fr.ReadFont();
        NKImageFontReader ifr = new NKImageFontReader(@"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\BuiltIn\radio.nkf");
        var f2 = ifr.ReadFont();
        // s.DrawText("Hello, World!", 7, 4, f, new NKStyle(GRAYSCALE_0, NKColor.Inherit));

        // SKBitmap bmp = SKBitmap.Decode(@"C:\Users\krystof\Desktop\peng.png"); 
        // SKBitmap bmp = SKBitmap.Decode(@"C:\Users\krystof\Pictures\Screenshots\Screenshot 2024-01-02 092440.png");
        // SKBitmap bmp2 = new SKBitmap(bmp.Width / 4, bmp.Height / 2);
        // bmp.ScalePixels(bmp2, new SKSamplingOptions(SKFilterMode.Linear));
        // bmp2.Encode(File.OpenWrite(@"C:\Users\krystof\Desktop\math meme half.webp"), SKEncodedImageFormat.Webp, 100);

        Paragraph h = new Paragraph("Hello, World!");
        h.Style.OverrideWith(new StyleCollection(
            new FontProperty(f),
            new OverflowProperty(OverflowType.VISIBLE_ALL),
            new ColorProperty(NKPalettes.OldSchool[2]),
            new PaddingProperty(top: SizeValue.Pixels(-1))
        ));
        
        var sty = new StyleCollection(
            new ColorProperty(NKPalettes.OldSchool[5]),
            new BackgroundColorProperty(NKPalettes.OldSchool[4]),
            new BorderProperty(BorderStyle.Inset(NKPalettes.OldSchool[5], NKPalettes.OldSchool[3])),
            // new MarginProperty(SizeValue.Pixels(1), SizeValue.Pixels(1), SizeValue.Pixels(1), SizeValue.Pixels(1)),
            new PaddingProperty(SizeValue.Pixels(1), SizeValue.Pixels(1), SizeValue.Pixels(1), SizeValue.Pixels(1))
        );
        
        Paragraph p0 = new Paragraph("No key was pressed.");
        p0.Style.OverrideWith(sty);
        p0.KeyPressHandler = (_, args) => {
            p0.Content = $"Pressed Key: {Extensions.ToString(args.PressedKey)}";
        };
        p0.Style.OverrideWith(new StyleCollection(new MinWidth(SizeValue.Chars(30))));
        
        Paragraph p1 = new Paragraph($"Current window size is: {Console.WindowWidth}x{Console.WindowHeight}");
        p1.Style.OverrideWith(sty);
        p1.ResizeHandler = _ => {
            p1.Content = $"Size: {Console.WindowWidth}x{Console.WindowHeight}";
        };
        
        var time = new Paragraph("Hello, World!");
        time.Style.OverrideWith(sty);
        time.OnRender = () => {
            time.Content = $"Time: {DateTime.Now:R}";
        };

        var d = new NamedDiv("Test div");
        d.Style.OverrideWith(new StyleCollection(
            new NDivTitleProperty(new NKStyle(NKPalettes.OldSchool[1], NKColor.Inherit), HorizontalAlign.CENTER),
            new DisplayProperty(DisplayType.FLEX),
            new BackgroundColorProperty(NKPalettes.OldSchool[4]),
            new BorderProperty(BorderStyle.Outset(NKPalettes.OldSchool[5], NKPalettes.OldSchool[3])),
            // new MarginProperty(SizeValue.Pixels(1), SizeValue.Pixels(1), SizeValue.Pixels(1), SizeValue.Pixels(1)),
            new PaddingProperty(SizeValue.Pixels(1), SizeValue.Pixels(1), SizeValue.Pixels(1), SizeValue.Pixels(1))
        ));
        d.Children.Add(p0);
        d.Children.Add(p1);
        
        // Body b = new Body(d, time);
        // Body b = new Body(h, p0, p1, time);
        // Body b = new Body(h);
        Body b = new Body(h, d, time);
        b.Style.OverrideWith(new StyleCollection(
            new DisplayProperty(DisplayType.BLOCK),
            new BorderProperty(),
            new PaddingProperty(SizeValue.Pixels(1), SizeValue.Pixels(1), SizeValue.Pixels(1), SizeValue.Pixels(1)),
            new BackgroundColorProperty(NKPalettes.OldSchool[0])   
        ));
        
        Application a = new Application(b, new AppConfig(
            interruptCombination: new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, true), 
            lazyRender: false, 
            maxUpdatesPerSecond: 5), s);
        
        a.KeyEvent += p0.HandleKeyPress;
        a.ResizeEvent += p1.HandleResize;
        
        // AppEventBus.SetSourceApplication(a);
        // AppEventBus.SubscribeToKeyEvent(p.HandleKeyPress);
        
        a.Start();
        
        s.Dispose();

        // var p = new Paragraph("Hello, World");


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

    private const string LOREM_CZ =
        "Bolest sama o sobě je skvělá věc, je důsledkem adipiscující elity, ale stává se to i v době porodu a velké" +
        " bolesti. Neboť, abychom se dostali k věci, kdo z nás vykonává nějakou práci kromě toho, aby z ní měl" +
        " nějaký užitek? Ale kdo by právem kritizoval potěšení za to, že chce být trochu bolestí? Ať od toho uteče a" +
        " ať se nikdo nenarodí. Kromě toho, že jsou zaslepeni touhou, nepokračují; jsou na vině, kdo opouštějí své" +
        " povinnosti, což obměkčuje duši, to jest práce práce.";
}

public enum Test {
    T1,
    T2,
    T3,
    T4,
    T5,
}