using System.Text;
using NeoKolors.Console;
using NeoKolors.Tui;

namespace NKChess;

internal static class Program {
    
    static Program() {
        Console.OutputEncoding = Encoding.UTF8;
        
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
        var page = new ChessTuiApp();
        
        var config = new NKAppConfig(
            rendering: RenderingConfig.Limited(140)
        );
        
        var app = new NKApplication(config, page);
        page.SetApplication(app);
        app.Start();
    }
}