using System.Text;
using NeoKolors.Console;
using NeoKolors.Console.Ansi.Mouse;
using NeoKolors.Console.Input;
using NeoKolors.Tui;
using NeoKolors.Tui.Fonts.Assets;
using static NeoKolors.Console.LoggerLevel;

namespace LogViewer;

class Program {
    static void Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;
        NKDebug.Logger.Level = CRITICAL | ERROR | WARNING | INFORMATION | DEBUG | TRACE;
        NKDebug.ExceptionFormatting = true;
        NKDebug.Logger.SimpleMessages = true;
        NKDebug.Logger.FileConfig = LogFileConfig.Replace("./fv.log");
        NKDebug.Logger.IndentMessage = new LoggerConfig.InlineIndent();
        NKDebug.Logger.MessageHighlightLine = false;
        NKDebug.RedirectFatalToLog = true;
        NKDebug.EnableExceptionInterruption();
        
        AssetsProvider.RegisterFonts();
        
        var mainView = new MainView();
        
        var app = new NKApplication(new NKAppConfig(
            mouseReportProtocol: MouseReportProtocol.SGR,
            mouseReportLevel: MouseReportLevel.ALL,
            rendering: RenderingConfig.Limited(144),
            ctrlCForceQuits: false,
            interruptCombination: new KeyEventArgs(KeyCode.Q, KeyModifiers.LEFT_CTRL, 'q')
        ), mainView);

        app.KeyEvent += e => NKDebug.Debug(e.ToString());
        
        app.Start();
    }
}