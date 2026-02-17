using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Console.Mouse;
using NeoKolors.Extensions;
using NeoKolors.Tui;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Elements.Caching;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Extensions;
using NeoKolors.Tui.Fonts.Serialization;
using NeoKolors.Tui.Rendering;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Values;
using static System.ConsoleModifiers;
using static NeoKolors.Console.LoggerLevel;
using Rectangle = NeoKolors.Tui.Rectangle;

namespace Testing;

// git ls-files | grep '\.cs' | xargs wc -l

public static class Program {
    private const string LOREM = "Lorem ipsum dolor sit amet, consectetur adipisici elit, sed eiusmod tempor" +
                                 " incidunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud" +
                                 " exercitation ullamco laboris nisi ut aliquid ex ea commodi consequat. Quis aute" +
                                 " iure reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla " +
                                 "pariatur. Excepteur sint obcaecat cupiditat non proident, sunt in culpa qui " +
                                 "officia deserunt mollit anim id est laborum.";

    private const string TEST =
        """
           ▀██▄▄██▀                                                                   ██                                                                                     ▄▄▄   
              ▀▀               ██                                                    ▀▀                                                                                     ██ ██
           ▄████████     ███   ▀▀   ▄████████    ▄████████     ███     ███▄▄▄▄   ▄██   ▄        ████████▄     ▄████████ ███▄▄▄▄         ▄██████▄      ███      ▄████████     ▀▀▀     
          ███    ███ ▀█████████▄   ███    ███   ███    ███ ▀█████████▄ ███▀▀▀██▄ ███   ██▄      ███   ▀███   ███    ███ ███▀▀▀██▄      ███    ███ ▀█████████▄ ███    ███ ███    █▄ 
          ███    █▀     ▀███▀▀██   ███    ███   ███    █▀     ▀███▀▀██ ███   ███ ███▄▄▄███      ███    ███   ███    █▀  ███   ███      ███    ███    ▀███▀▀██ ███    █▀  ███    ███ 
          ███            ███   ▀   ███    ███   ███            ███   ▀ ███   ███ ▀▀▀▀▀▀███      ███    ███  ▄███▄▄▄     ███   ███      ███    ███     ███   ▀ ███        ███    ███ 
        ▀███████████     ███     ▀███████████ ▀███████████     ███     ███   ███ ▄██   ███      ███    ███ ▀▀███▀▀▀     ███   ███      ███    ███     ███     ███        ███    ███ 
                 ███     ███       ███    ███          ███     ███     ███   ███ ███   ███      ███    ███   ███    █▄  ███   ███      ███    ███     ███     ███    █▄  ███    ███ 
           ▄█    ███     ███       ███    ███    ▄█    ███     ███     ███   ███ ███   ███      ███   ▄███   ███    ███ ███   ███      ███    ███     ███     ███    ███ ███    ███ 
         ▄████████▀     ▄████▀     ███    █▀   ▄████████▀     ▄████▀    ▀█   █▀   ▀█████▀       ████████▀    ██████████  ▀█   █▀        ▀██████▀     ▄████▀   ████████▀  ████████▀  
                                                                                                                                                                                    
         :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)     :)
        """;

    public enum E {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5
    }

    static Program() {
        Console.OutputEncoding = Encoding.Unicode;
    }
    
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public static void Main() {
        
        NKDebug.Logger.Level = CRITICAL | ERROR | WARNING | INFORMATION | DEBUG | TRACE;
        NKDebug.ExceptionFormatting = true;
        NKDebug.RedirectFatalToLog = true;
        NKDebug.Logger.SimpleMessages = true;
        NKDebug.Logger.FileConfig = LogFileConfig.Replace(@"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\Examples\Testing\loga.log");
        NKDebug.Logger.IndentMessage = new LoggerConfig.InlineIndent();
        NKDebug.Logger.MessageHighlightLine = false;

        var style = new NKStyle(NKColor.FromRgb(1, 2, 3), NKColor.Inherit, TextStyles.ALL);
        Console.WriteLine(style.ToBitmapString());
        
        NKFontSerializer.CreateArchive(@"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\Builtin\Dummy1", 
            @"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\Builtin\Dummy1.nkf");
        
        var res = NKFontSerializer.ReadDir(
            @"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\Builtin\Dummy1");

        var res1 = NKFontSerializer.ReadDir(
            @"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\Builtin\Bytesized");

        var fut = NKFontSerializer.ReadDir(
            @"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\Builtin\Future");
        
        Console.CursorVisible = false;
        
        var canv = new NKCharScreen(Console.WindowWidth, Console.WindowHeight);
        
        // res1.PlaceString("Hello World How you all doin tonight I am doing pretty damn good", canv,
        //     new Rectangle(new Point(2, 2), new Size(canv.Width - 4, canv.Height - 4)));

        NKDebug.Info($"Console size: ({Console.WindowWidth}, {Console.WindowHeight})");
        
        var app = new NKApplication(
            new NKAppConfig(
                rendering: RenderingConfig.Limited(144), 
                mouseReportProtocol: MouseReportProtocol.UTF8
            ), 
            null!
        );
        
        AppEventBus.SetSourceApplication(app);
        
        var tt = new Text("Hello World How you doin?") {
            Style = new StyleCollection() {
                TextColor = NKConsoleColor.WHITE,
                Border = BorderStyle.GetNormal(NKConsoleColor.GRAY, NKConsoleColor.DARK_GRAY),
                BackgroundColor = NKConsoleColor.DARK_GRAY,
                Margin = new(2.Ch, 1.Ch),
                Padding = new(2.Ch, 1.Ch),
                GridAlign = new Rectangle(0, 1, 0, 1),
                Width = Dimension.Stretch,
                Height = Dimension.Stretch,
            }
        };
        
        var tt2 = new Text("Oi mae? wha a delighful weah, inni?") {
            Info = { Id = "tt2" },
            Style = new StyleCollection {
                TextColor = NKConsoleColor.WHITE,
                Border = BorderStyle.GetNormal(NKConsoleColor.GRAY, NKConsoleColor.DARK_GRAY),
                BackgroundColor = NKConsoleColor.DARK_GRAY,
                Margin = new(2.Ch, 1.Ch),
                Padding = new(2.Ch, 1.Ch),
                GridAlign = new Rectangle(1, 1, 1, 1),
                Width = Dimension.Stretch,
                Height = Dimension.Stretch,
            }
        };
        
        var lt = new Text("Lorem ipsum dolor sit amet, consectetur adipisici elit") {
            Style = new StyleCollection {
                TextColor = NKConsoleColor.WHITE,
                Border = BorderStyle.GetNormal(NKConsoleColor.GRAY, NKConsoleColor.DARK_GRAY),
                BackgroundColor = NKConsoleColor.DARK_GRAY,
                Margin = new(2.Ch, 1.Ch),
                Padding = new(2.Ch, 1.Ch),
                GridAlign = new Rectangle(2, 0, 2, 1),
                Width = Dimension.Stretch,
                Height = Dimension.Stretch,
            }
        };
        
        var vlt = new Text("Lorem ipsum dolor sit amet, consectetur adipisici elit, sed eiusmod tempor incidunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquid ex ea commodi consequat.") {
            Info = { Id = "Tested" },
            Style = new StyleCollection {
                TextColor = NKConsoleColor.WHITE,
                Border = BorderStyle.GetNormal(NKConsoleColor.GRAY, NKConsoleColor.DARK_GRAY),
                BackgroundColor = NKConsoleColor.DARK_GRAY,
                Margin = new(2.Ch, 1.Ch),
                Padding = new(2.Ch, 1.Ch),
                GridAlign = new Rectangle(0, 0, 1, 0),
                Width = Dimension.Stretch,
                Height = Dimension.Stretch,
            }
        };

        var fp = new Text("čus jak se máš!?") {
            Style = new StyleCollection {
                Font = fut,
                TextColor = NKConsoleColor.BLUE,
                Margin = new(2.Ch, 1.Ch),
                Padding = new(2.Ch, 1.Ch),
                // Border = BorderStyle.GetNormal(NKConsoleColor.DARK_GRAY),
                // BackgroundColor = NKConsoleColor.BLACK,
                TextAlign = new Align(HorizontalAlign.CENTER, VerticalAlign.TOP),
            }
        };

        // var sd = new NamedDiv("NAMEEE", vlt, tt, lt, tt2) {
        var sd = new Div(vlt, tt, lt, tt2) {
            Style = new StyleCollection {
                BackgroundColor = NKConsoleColor.BLACK,
                Direction = Direction.LEFT_TO_RIGHT,
                Height = 50.Vh,
                // TitlePadding = 2.Ch,
                // TitleAlign = HorizontalAlign.RIGHT,
                Grid = new GridDimensions(
                    [Dimension.Percent(20), Dimension.Auto, Dimension.Percent(30)], 
                    [Dimension.Percent(50), Dimension.Auto]
                ),
                EnableGrid = true,
            }
        };

        // var l = new List(vlt, tt, lt, tt2) {
        //     BackgroundColor = NKConsoleColor.BLACK,
        //     Point = ListPointProperty.PointLarger,
        //     PointStyle = new NKStyle(TextStyles.BOLD),
        //     PointOffset = 3,
        //     Padding = new(2.Ch, 1.Ch),
        // };

        var button = new Button("Click me") {
            Info = { Id = "Tested" },
            Style = new StyleCollection {
                Border = BorderStyle.GetThick(NKConsoleColor.BLACK, NKConsoleColor.GREEN),
                BackgroundColor = NKConsoleColor.GREEN,
                TextColor = NKConsoleColor.BLACK,
                Margin = new (1.Ch, 1.Ch, 1.Ch, 0),
                Padding = new(1.Ch, 0),
            }
        };

        var counter = new Text("Click count: 0");
        var stopwatch = new Text("Button not held...");
        
        var exit = new Button("EXIT") {
            Style = new StyleCollection {
            Border = BorderStyle.GetThick(NKConsoleColor.BLACK, NKConsoleColor.RED),
            BackgroundColor = NKConsoleColor.RED,
            TextColor = NKConsoleColor.BLACK,
            Margin = new (1.Ch, 1.Ch, 1.Ch, 0),
            Padding = new(1.Ch, 0),
        }};

        var x = new Button("X") {
            Style = new StyleCollection{
            Padding = new(1.Ch, 0),
            BackgroundColor = NKConsoleColor.RED,
            Position = new Position(100.Vw - 5.Ch, 0.Ch),
            TextColor = NKConsoleColor.BLACK,
        }};

        var min = new Button("_") {
            Style = new StyleCollection{
            Padding = new(1.Ch, 0),
            BackgroundColor = NKConsoleColor.BLUE,
            Position = new Position(100.Vw - 8.Ch, 0.Ch),
            TextColor = NKConsoleColor.BLACK,
        }};
        
        var info = new Div(counter, stopwatch) {
            Style = new StyleCollection {
                Padding = new Spacing(1.Ch),
            }
        };
        
        var buttons = new Div(button, info, exit) {
            Style = new StyleCollection {
                Direction = Direction.LEFT_TO_RIGHT,
            }
        };

        var input = new TextInput("label: ") {
            Info = { Id = "Input" },
            Style = {
                Border = BorderStyle.GetNormal()
            }
        };
        input.Select();
        
        var body = new Body(fp, sd, buttons, input, x, min) {
            Style = {
                Border = BorderStyle.GetNormal(NKConsoleColor.DARK_BLUE, NKColor.Default)
            }
        };

        app.Base = body;
        
        app.MouseEvent += m => tt.Content = "ME: " + m;
        app.KeyEvent   += k => tt.Content = "KE: " + k.AsString();
        tt2.OnRender   += () => tt2.Content = DateTime.Now.ToString("O");
        app.KeyEvent   += k => {
            if (k is { Key: ConsoleKey.Escape, Modifiers: Control }) app.Stop();
        };

        // var p = new Point2D(0, 0);
        // var lc = ' ';
        // app.MouseEvent += m => {
        //     var nlc = app.Screen[m.Position.X, m.Position.Y].Char;
        //     app.Screen[m.Position.X, m.Position.Y].Char = 'X';
        //     app.Screen[p.X, p.Y].Char = lc;
        //
        //     lc = nlc ?? ' ';
        //     p = new Point2D(m.Position.X, m.Position.Y);
        // };
        
        int c = 0;
        var swp = new Stopwatch();
        var swn = new Stopwatch();
        
        button.OnClick += arg => {
            
            switch (arg) {
                case MouseButton.RIGHT: c--; break;
                case MouseButton.WHEEL_DOWN: c--; break;
                default: c++; break;
            }

            if (arg is not MouseButton.WHEEL_DOWN and not MouseButton.WHEEL_UP) {
                if (arg is MouseButton.RIGHT)
                    swn.Start();
                else
                    swp.Start();
            }
            
            counter.Content = $"Click count: {c}";
        };

        button.OnRelease += _ => {
            swp.Stop();
            swn.Stop();
        };

        stopwatch.OnRender += () => stopwatch.Content = $"Time held: {(swp.Elapsed - swn.Elapsed):c}";

        exit.OnRelease += _ => app.Stop();
        x.OnRelease += _ => app.Stop();
        min.OnClick += _ => NKConsole.Minimize();

        vlt.OnHover += () => vlt.Style.BackgroundColor = NKConsoleColor.GRAY;
        vlt.OnHoverOut += () => vlt.Style.BackgroundColor = NKConsoleColor.DARK_GRAY;
        lt.OnHover += () => lt.Style.BackgroundColor = NKConsoleColor.GRAY;
        lt.OnHoverOut += () => lt.Style.BackgroundColor = NKConsoleColor.DARK_GRAY;
        tt.OnHover += () => tt.Style.BackgroundColor = NKConsoleColor.GRAY;
        tt.OnHoverOut += () => tt.Style.BackgroundColor = NKConsoleColor.DARK_GRAY;
        tt2.OnHover += () => tt2.Style.BackgroundColor = NKConsoleColor.GRAY;
        tt2.OnHoverOut += () => tt2.Style.BackgroundColor = NKConsoleColor.DARK_GRAY;
        
        app.Start();
        
        NKDebug.Debug("\nCache Hits : " + CacheAnalyzer.Hits + "\nCache Misses : " + CacheAnalyzer.Misses);
    }

    private const string LOREM_CZ =
        "Bolest sama o sobě je skvělá věc, je důsledkem adipiscující elity, ale stává se to i v době porodu a velké" +
        " bolesti. Neboť, abychom se dostali k věci, kdo z nás vykonává nějakou práci kromě toho, aby z ní měl" +
        " nějaký užitek? Ale kdo by právem kritizoval potěšení za to, že chce být trochu bolestí? Ať od toho uteče a" +
        " ať se nikdo nenarodí. Kromě toho, že jsou zaslepeni touhou, nepokračují; jsou na vině, kdo opouštějí své" +
        " povinnosti, což obměkčuje duši, to jest práce práce.";
}