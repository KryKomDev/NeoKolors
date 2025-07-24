//
// NeoKolors
// Copyright (c) 2025 KryKom
//

#if ALLOW_SECONDARY_CONTEXT
#undef ALLOW_SECONDARY_CONTEXT
#endif

#define ALLOW_SECONDARY_CONTEXT 

using NeoKolors.Common;
using NeoKolors.Common.Util;
using NeoKolors.Console;
using NeoKolors.Tui.Events;

namespace NeoKolors.Tui;

public partial class ConsoleScreen {
    
    private NKStyle[,] _styles;
    private char[,] _chars;
    private bool[,] _changes;
    
    /// <summary>
    /// Array that holds all styles of the screen.
    /// When accessing with <c>[x, y]</c>, <c>x</c> represents the x-th column from left to right
    /// and <c>y</c> represents y-th row from top to bottom.
    /// </summary>
    private NKStyle[,] Styles => _styles;

    /// <summary>
    /// sets the style of a character at the given position
    /// </summary>
    /// <param name="x">x coordinate</param>
    /// <param name="y">y coordinate</param>
    /// <param name="style">the style to be set</param>
    public void SetStyle(int x, int y, NKStyle style) {
        _styles[x, y] = style;
        _changes[x, y] = true;
    }

    /// <summary>
    /// Retrieves the style of a character at the given position.
    /// </summary>
    /// <param name="x">The x-coordinate of the character.</param>
    /// <param name="y">The y-coordinate of the character.</param>
    /// <param name="style">The style of the character at the specified position.</param>
    public void GetStyle(int x, int y, out NKStyle style) {
        style = _styles[x, y];
        _changes[x, y] = true;
    }

    /// <summary>
    /// Retrieves the style of a character at the specified position.
    /// </summary>
    /// <param name="x">The x-coordinate of the position.</param>
    /// <param name="y">The y-coordinate of the position.</param>
    /// <returns>The style of the character at the specified position.</returns>
    public NKStyle GetStyle(int x, int y) => _styles[x, y];

    /// <summary>
    /// Array that holds all characters of the screen.
    /// When accessing with <c>[x, y]</c>, <c>x</c> represents the x-th column from left to right
    /// and <c>y</c> represents y-th row from top to bottom.
    /// </summary>
    private char[,] Chars => _chars;

    /// <summary>
    /// Sets the character at the specified position.
    /// </summary>
    /// <param name="x">The x-coordinate of the character's position.</param>
    /// <param name="y">The y-coordinate of the character's position.</param>
    /// <param name="c">The character to be set at the specified position.</param>
    public void SetChar(int x, int y, char c) {
        _chars[x, y] = c;
        _changes[x, y] = true;
    }

    /// <summary>
    /// Retrieves the character at the given position.
    /// </summary>
    /// <param name="x">The x-coordinate of the character position.</param>
    /// <param name="y">The y-coordinate of the character position.</param>
    /// <param name="c">The character at the specified position.</param>
    public void GetChar(int x, int y, out char c) {
        c = _chars[x, y];
        _changes[x, y] = true;
    }

    /// <summary>
    /// Retrieves the character at the specified position.
    /// </summary>
    /// <param name="x">The x-coordinate of the character's position.</param>
    /// <param name="y">The y-coordinate of the character's position.</param>
    /// <returns>The character at the specified position.</returns>
    public char GetChar(int x, int y) => _chars[x, y];

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
    // ReSharper disable once RedundantDefaultMemberInitializer
    public bool TestingMode { get; set; } = false;

    /// <summary>
    /// if true, the application is in screen mode, else in console (logging) mode.
    /// settings the property automatically sets the terminal context
    /// </summary>
    public bool ScreenMode { 
        get;
        set {
            if (value) {
                #if ALLOW_SECONDARY_CONTEXT
                NKConsole.EnableAltBuffer();
                LOGGER.Trace("Secondary context mode enabled.");
                #endif

                System.Console.CursorVisible = false;
                Render();
            }
            else {
                #if ALLOW_SECONDARY_CONTEXT
                NKConsole.DisableAltBuffer();
                LOGGER.Trace("Secondary context mode disabled.");
                #endif
                
                System.Console.CursorVisible = true;
                UpdateConsole();
            }

            field = value;
        } 
    }

    /// <summary>
    /// toggles the <see cref="ScreenMode"/>
    /// </summary>
    public void ToggleScreenMode() => ScreenMode = !ScreenMode;

    /// <summary>
    /// determines what pixels had been changed
    /// </summary>
    private bool[,] PixelChanges => _changes;


    /// <summary>
    /// standard output 
    /// </summary>
    private TextWriter StdOut { get; }
    
    /// <summary>
    /// updates the screen size
    /// </summary>
    public void Resize(ResizeEventArgs args) {
        Width = System.Console.WindowWidth;
        Height = System.Console.WindowHeight;

        // resize the arrays
        // null check is not needed
        List2D.Resize(ref _styles!, Width, Height);
        List2D.Resize(ref _chars!, Width, Height);
        List2D.Resize(ref _changes!, Width, Height);
        List2D.Fill(_changes, true);
        
        LOGGER.Info($"ConsoleScreen has been resized to {Width}x{Height}.");
    }
    
    
    /// <summary>
    /// initializes the screen
    /// </summary>
    /// <returns>method supposed to be used as an event handler for terminal size changes</returns>
    public ConsoleScreen(TextWriter? standardOutput = null, bool testingMode = false) {
        TestingMode = testingMode;
        StdOut = standardOutput ?? System.Console.Out;

        Width = TestingMode ? 1 : System.Console.WindowWidth;
        Height = TestingMode ? 1 : System.Console.WindowHeight;
        
        LOGGER.Info($"Setting up new ConsoleScreen with width {Width} and height {Height}.");

        _styles = new NKStyle[Width, Height];
        _chars = new char[Width, Height];
        _changes = new bool[Width, Height];
        
        ScreenMode = true;

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                _chars[x, y] = ' ';
                _styles[x, y] = new NKStyle(NKColor.Default, NKColor.Default, TextStyles.NONE);
                _changes[x, y] = true;
            }
        }
        
        LOGGER.Info("ConsoleScreen successfully initialized.");
    }

    public new void Dispose() {
        _styles = List2D.Empty<NKStyle>();
        _chars = List2D.Empty<char>();
        _changes = List2D.Empty<bool>();
        _output = string.Empty;
        ScreenMode = false;
        LOGGER.Trace("ConsoleScreen disposed.");
    }

    public override ValueTask DisposeAsync() {
        try {
            Dispose();
            return default;
        }
        catch (Exception exc) {
            return new ValueTask(Task.FromException(exc));
        }
    }
}