using System.Text;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui.Fonts.Assets;
using WinImports = NeoKolors.Console.WinImports;

namespace Testing;

// git ls-files | grep '\.cs' | xargs wc -l

public static class Program {
    private const string LOREM = "Lorem ipsum dolor sit amet, consectetur adipisici elit, sed eiusmod tempor" +
        " incidunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud"                   +
        " exercitation ullamco laboris nisi ut aliquid ex ea commodi consequat. Quis aute"                    +
        " iure reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla "                          +
        "pariatur. Excepteur sint obcaecat cupiditat non proident, sunt in culpa qui "                        +
        "officia deserunt mollit anim id est laborum.";
    
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
        
        var logStream = new FileStream("./logs/log.blog", FileMode.Create, FileAccess.Write);

        NKDebug.Logger.Writer = new CompositeLogWriter(
            new BinaryLogWriter(logStream),
            TextLogWriter.CreateFromFile("./logs/log.log", append: false),
            new AnsiLogWriter()
        );
    }

    private static void ConTest() {
        Console.CursorVisible = false;

        var inp = new WindowsInput();

        while (true) {
            if (!inp.TryGetInputs(out var r))
                continue;

            foreach (var i in r) {
                if (i is not { Type: WinImports.WinEventType.KEY, Key.Down: true })
                    goto Write;

                switch (i.Key.VirtualKeyCode) {
                    case WinImports.WinVirtualKeyCode.F when i.Key.Modifiers.GetHasLeftCtrl(): {
                        Console.Write("\e[16t");

                        break;
                    }
                    case WinImports.WinVirtualKeyCode.C when i.Key.Modifiers.GetHasLeftCtrl(): {
                        return;
                    }
                }

                Write:

                Console.WriteLine(i);
            }
        }
    }

    public static void Main(string[] args) {
        // DumpDiagnostics();
        NKDebug.SetLogAll();

        try {
            Console.OutputEncoding = Encoding.Unicode;
        }
        catch (IOException) { }

        NKDebug.SetLogAll();
        NKDebug.ExceptionFormatting = true;
        NKDebug.RedirectFatalToLog  = true;

        NKDebug.Debug($"OS: {Environment.OSVersion.VersionString}, Platform: {Environment.OSVersion.Platform}");
        NKDebug.Info($"Used I/O drivers: {NKConsole.InputDriver.GetType().Name}/{NKConsole.OutputDriver.GetType().Name}");
        NKDebug.Info($"VT Platform: {NKConsole.GetVTInfo()}");

        AssetsProvider.RegisterFonts();

        var asdasd = "e\u0301";

        NKConsole.WriteLine(EscapeCodes.GetLink("https://youtube.com/", "You", "yt"));
        NKConsole.WriteLine(EscapeCodes.GetLink("https://youtube.com/", "Tube", "yt"));

        NKConsole.ReadKey();

        NKConsole.WriteLine(asdasd.Normalize(NormalizationForm.FormD));
        NKConsole.WriteLine(asdasd.Length);

        foreach (var ca in asdasd) {
            Console.Write(ca);
        }

        // ExceptionFormatter.ShowExceptionNamespace = false;
        // ExceptionFormatter.ShowMethodNamespace    = false;
        // ExceptionFormatter.ShowFilePath           = false;
        // ExceptionFormatter.ShowSourceSnippet      = false;
        ExceptionFormatter.DimSystemFrames  = false;
        ExceptionFormatter.StackTraceIndent = 3;

        NKConsole.WriteLine();
        
        try {
            SomeOtherCkass.Crash();
        }
        catch (Exception e) {
            ExceptionFormatter.Print(e);
        }

        NKConsole.ReadKey();

        return;

        // while (true) {
        //     if (inp.HasInput()) {
        //         var i = inp.ReadInput().First();
        //         if (i is {
        //                 Flags: WinImports.EventType.KEY,
        //                 Key.VirtualKeyCode: WinImports.VirtualKeyCode.C
        //             }
        //             && i.Key.Modifiers.GetHasLeftCtrl()
        //         ) {
        //             return;  
        //         }
        //
        //         if (i.Flags == WinImports.EventType.MOUSE) {
        //             var (xc, yc) = i.Mouse.Coord;
        //             Console.Clear();
        //             NKConsole.SetCursorPosition(xc, yc);
        //             Console.Write("x");
        //         }
        //     }
        //     
        //     Thread.Sleep(10);
        // }
    }

    private const string LOREM_CZ =
        "Bolest sama o sobě je skvělá věc, je důsledkem adipiscující elity, ale stává se to i v době porodu a velké"  +
        " bolesti. Neboť, abychom se dostali k věci, kdo z nás vykonává nějakou práci kromě toho, aby z ní měl"       +
        " nějaký užitek? Ale kdo by právem kritizoval potěšení za to, že chce být trochu bolestí? Ať od toho uteče a" +
        " ať se nikdo nenarodí. Kromě toho, že jsou zaslepeni touhou, nepokračují; jsou na vině, kdo opouštějí své"   +
        " povinnosti, což obměkčuje duši, to jest práce práce.";
}