using System.Text;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Console.Mouse;
using NeoKolors.Tui;
using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Extensions;
using NeoKolors.Tui.Fonts.Serialization;
using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Styles.Values;
using static NeoKolors.Console.LoggerLevel;

namespace XmlTestApp;

internal static class Program {
    public static void Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;
        
        NKDebug.Logger.Level = CRITICAL | ERROR | WARNING | INFORMATION | DEBUG | TRACE;
        NKDebug.ExceptionFormatting = true;
        NKDebug.Logger.SimpleMessages = true;
        NKDebug.Logger.FileConfig = LogFileConfig.Replace(@"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\Examples\XmlTestApp\xml.log");
        NKDebug.Logger.IndentMessage = new LoggerConfig.InlineIndent();
        NKDebug.Logger.MessageHighlightLine = false;
        NKDebug.RedirectFatalToLog = true;
        NKDebug.EnableExceptionInterruption();

        NKFontSerializer.CreateArchive(@"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\Builtin\Dummy1", 
            @"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\Builtin\Dummy1.nkf");

        NKFontSerializer.CreateArchive(@"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\Builtin\Future", 
            @"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\Builtin\Future.nkf");

        NKFontSerializer.CreateArchive(@"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\Builtin\Bytesized", 
            @"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\NeoKolors.Tui\Fonts\Builtin\Bytesized.nkf");

        
        var loader = new XmlDomLoader();
        var dom = loader.LoadFromFile(@"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\Examples\XmlTestApp\App.xml");
        
        var app = new NKApplication(new NKAppConfig(
            mouseReportProtocol: MouseReportProtocol.SGR,
            mouseReportLevel: MouseReportLevel.ALL,
            rendering: RenderingConfig.Limited(144)
            // rendering: RenderingConfig.Lazy()
        ), dom.BaseElement);

        const int maxSlide = 3;
        
        int slide = 0;
        app.KeyEvent += k => {
            if (k.Key is ConsoleKey.Spacebar or ConsoleKey.Enter or ConsoleKey.RightArrow) {
                var ns = slide + 1;
                
                if (ns <= maxSlide) {
                    GotoSlide(slide, ns, dom);
                    slide = ns;
                }
            }
            else if (k.Key is ConsoleKey.Backspace or ConsoleKey.LeftArrow) {
                var ns = slide - 1;
                
                if (ns >= 0) {
                    GotoSlide(slide, ns, dom);
                    slide = ns;
                }
            }
        };
        
        app.MouseEvent += k => {
            if (k.Button is MouseButton.WHEEL_DOWN) {
                var ns = slide + 1;
                
                if (ns <= maxSlide) {
                    GotoSlide(slide, ns, dom);
                    slide = ns;
                }
            }
            else if (k.Button is MouseButton.WHEEL_UP) {
                var ns = slide - 1;
                
                if (ns >= 0) {
                    GotoSlide(slide, ns, dom);
                    slide = ns;
                }
            }
        };

        dom.GetElementsByType(typeof(Text)).Apply(new PaddingProperty(0, 0, 0, 1));
       
        var logo = dom.GetElementById("logo");

        logo?.Style.Set(new CheckerBckgProperty(new CheckerBckg(NKConsoleColor.RED, NKConsoleColor.BLUE, false)));
        
        dom.GetElementsByClass("Slide").Apply(
            new WidthProperty(Dimension.Stretch), 
            new HeightProperty(Dimension.Stretch),
            new PaddingProperty(4.Ch, 2.Ch),
            new BackgroundColorProperty(NKColor.Default) 
            // new BorderProperty(BorderStyle.GetRounded())
        );

        dom.GetElementsByType(typeof(Heading)).Apply(
            new MarginBottomProperty(2.Ch)
        );

        dom.GetElementsByType(typeof(List)).Apply(
            ListPointProperty.PointLarger
        );

        // var x = (Button)dom.GetElementById("X")!;
        // x.OnClick += _ => app.Stop();
        
        app.Start();
    }

    private static void GotoSlide(int last, int next, IDom dom) {
        dom.GetElementById(last.ToString())?.Style.Set(new VisibleProperty(false));
        dom.GetElementById(next.ToString())?.Style.Set(new VisibleProperty(true));
    }
}