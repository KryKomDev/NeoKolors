//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

namespace NeoKolors.Tui;

public partial class ConsoleScreen {
    
    /// <summary>
    /// Array that holds all styles of the screen.
    /// When accessing with <c>[x, y]</c>, <c>x</c> represents the x-th column from left to right
    /// and <c>y</c> represents y-th row from top to bottom.
    /// </summary>
    private NKStyle[,] Styles { get; set; }

    /// <summary>
    /// Array that holds all characters of the screen.
    /// When accessing with <c>[x, y]</c>, <c>x</c> represents the x-th column from left to right
    /// and <c>y</c> represents y-th row from top to bottom.
    /// </summary>
    private char[,] Chars { get; set; }

    /// <summary>
    /// width of the screen 
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// height of the screen 
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// whether the Virtual Console is in testing mode
    /// </summary>
    public bool TestingMode { get; set; } = false;

    /// <summary>
    /// if true, application is in screen mode, else in console (logging) mode.
    /// settings the property automatically sets the terminal context
    /// </summary>
    public bool ScreenMode { 
        get;
        set {
            if (value) {
                EscapeCodes.EnableSecondary();
                Render();
            }
            else {
                EscapeCodes.DisableSecondary();
                UpdateConsole();
            }

            field = value;
        } 
    } = true;

    /// <summary>
    /// toggles the <see cref="ScreenMode"/>
    /// </summary>
    public void ToggleScreenMode() => ScreenMode = !ScreenMode;

    /// <summary>
    /// determines what pixels had been changed
    /// </summary>
    private bool[,] PixelChanges { get; set; }


    /// <summary>
    /// standard output 
    /// </summary>
    private TextWriter StandardOutput { get; }
    
    /// <summary>
    /// updates the screen size
    /// </summary>
    public void Resize() {
        Width = System.Console.WindowWidth;
        Height = System.Console.WindowHeight;
        
        Styles = new Common.NKStyle[Width, Height];
        Chars = new char[Width, Height];
        PixelChanges = new bool[Width, Height];

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Chars[x, y] = ' ';
            }
        }
    }
    
    
    /// <summary>
    /// initializes the screen
    /// </summary>
    /// <returns>method supposed to be used as an event handler for terminal size changes</returns>
    public ConsoleScreen(TextWriter? standardOutput = null, bool testingMode = false) {
        TestingMode = testingMode;
        StandardOutput = standardOutput ?? System.Console.Out;

        Width = TestingMode ? 1 : System.Console.WindowWidth;
        Height = TestingMode ? 1 : System.Console.WindowHeight;
        
        Styles = new NKStyle[Width, Height];
        Chars = new char[Width, Height];
        PixelChanges = new bool[Width, Height];

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Chars[x, y] = ' ';
                Styles[x, y] = new NKStyle();
            }
        }
    }
}