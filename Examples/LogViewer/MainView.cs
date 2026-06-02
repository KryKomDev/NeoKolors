using NeoKolors.Console;
using NeoKolors.Tui.Elements;

namespace LogViewer;

public partial class MainView {
    public MainView() {
        InitializeComponent();
        
        G.RowDefinitions.Add(GridLength.Auto);
        G.RowDefinitions.Add(GridLength.Star());

        G.ColumnDefinitions.Add(GridLength.Chars(15));
        G.ColumnDefinitions.Add(GridLength.Star());
    }

    public static void LogHello() {
        NKDebug.Debug("Hellow!");
    }

    public void FuckYou() {
        NKDebug.Debug("Fuck yeah!");
    }
}
