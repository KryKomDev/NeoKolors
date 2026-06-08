using System.Text;
using NeoKolors.Console;
using NeoKolors.Console.Ansi.Mouse;
using NeoKolors.Console.Input;
using NeoKolors.Tui;
using NeoKolors.Tui.Fonts.Assets;
using static NeoKolors.Console.LoggerLevel;

namespace NKChess;

internal static class Program {
    
    static Program() {
        var installDir = AppContext.BaseDirectory;

        if (!Directory.Exists(installDir))
            Directory.CreateDirectory(installDir); 
        
        var logsDir = Path.Combine(installDir, "logs");
        
        if (!Directory.Exists(logsDir))
            Directory.CreateDirectory(logsDir);

        NKDebug.Logger.FileConfig = LogFileConfig.NewDatetime($"{logsDir}/{{0}}.log");

        NKDebug.Logger.SimpleMessages = true;
    }
    
    static void Main() {
        Console.OutputEncoding = Encoding.UTF8;
        NKDebug.Logger.Level = CRITICAL | ERROR | WARNING | INFORMATION | DEBUG | TRACE;
        NKDebug.ExceptionFormatting = true;
        NKDebug.Logger.IndentMessage = new LoggerConfig.InlineIndent();
        NKDebug.Logger.MessageHighlightLine = false;
        NKDebug.RedirectFatalToLog = true;
        NKDebug.EnableExceptionInterruption();
        
        AssetsProvider.RegisterFonts();
        
        // var mainView = new MainView();
        //
        // var app = new NKApplication(new NKAppConfig(
        //     mouseReportProtocol: MouseReportProtocol.SGR,
        //     mouseReportLevel: MouseReportLevel.ALL,
        //     rendering: RenderingConfig.Limited(144),
        //     ctrlCForceQuits: false,
        //     interruptCombination: new KeyEventArgs(KeyCode.Q, KeyModifiers.LEFT_CTRL, 'q')
        // ), mainView);
        //
        // app.KeyEvent += e => NKDebug.Debug(e.ToString());
        //
        // app.Start();
        
        var page = new ChessTuiApp();
        
        var config = new NKAppConfig(
            rendering: RenderingConfig.Limited(140)
        );
        
        var app = new NKApplication(config, page);
        page.SetApplication(app);
        app.Start();
    }
}