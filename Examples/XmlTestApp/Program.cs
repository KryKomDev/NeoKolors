using System.Text;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Console.Ansi.Mouse;
using NeoKolors.Console.Input;
using NeoKolors.Tui;
using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Extensions;
using NeoKolors.Tui.Styles.Properties;
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
        
        var mainView = new MainView();
        var dom = new NKDom(mainView);
        
        var app = new NKApplication(new NKAppConfig(
            mouseReportProtocol: MouseReportProtocol.SGR,
            mouseReportLevel: MouseReportLevel.ALL,
            rendering: RenderingConfig.Limited(144)
        ), mainView);

        const int maxSlide = 4;
        
        int slide = 0;
        app.KeyEvent += k => {
            if (k.Key is KeyCode.SPACE or KeyCode.RETURN or KeyCode.ARROW_RIGHT) {
                var ns = slide + 1;
                
                if (ns <= maxSlide) {
                    GotoSlide(slide, ns, dom);
                    slide = ns;
                }
            }
            else if (k.Key is KeyCode.BACKSPACE or KeyCode.ARROW_LEFT) {
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

        dom.GetElementsByType(typeof(TextBlock)).Apply(new PaddingProperty(0, 0, 0, 1));
        
        dom.GetElementsByClass("Slide").Apply(
            new WidthProperty(Dimension.Stretch), 
            new HeightProperty(Dimension.Stretch),
            new PaddingProperty(4.Ch, 2.Ch),
            new BackgroundColorProperty(NKColor.Inherit) 
        );

        dom.GetElementsByClass("Heading").Apply(
            new MarginBottomProperty(2.Ch)
        );
        
        app.Start();
    }

    private static void GotoSlide(int last, int next, IDom dom) {
        dom.GetElementById(last.ToString())?.Style.Set(new VisibleProperty(false));
        dom.GetElementById(next.ToString())?.Style.Set(new VisibleProperty(true));
    }
}