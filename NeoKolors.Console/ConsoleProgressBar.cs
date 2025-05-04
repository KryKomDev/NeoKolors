//
// NeoKolors
// Copyright (c) 2025 KryKom
//

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Local
namespace NeoKolors.Console;

/// <summary>
/// creates a progress bar in the console
/// </summary>
[Obsolete]
public class ConsoleProgressBar {
    
    private readonly int barSize;
    private readonly int minValue;
    private readonly int maxValue;
    private int currentValue;
    private bool showData;
    private char beginChar;
    private char endChar;
    private char barChar;
    
    private const int OFFSET = 1;
    
    /// <summary>
    /// creates a progress bar shown in console
    /// </summary>
    /// <param name="minValue">starting point value</param>
    /// <param name="maxValue">ending point value</param>
    /// <param name="barSize">size of the bar in characters</param>
    /// <param name="showData">whether the exact numbers will be shown</param>
    /// <param name="barStyle">style of the bar</param>
    public ConsoleProgressBar(int minValue, int maxValue, int barSize = 40, 
        bool showData = true, BarStyle barStyle = BarStyle.CLASSIC) {
        
        this.barSize = barSize;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.showData = showData;
        SetStyle(barStyle);
        System.Console.CursorVisible = false;
        StartProgressBar();
    }
    
    /// <summary>
    /// creates a progress bar shown in console
    /// </summary>
    /// <param name="minValue">starting point value</param>
    /// <param name="maxValue">ending point value</param>
    /// <param name="barSize">size of the bar in characters</param>
    /// <param name="showData">whether the exact numbers will be shown</param>
    /// <param name="barStyle">style of the bar</param>
    public ConsoleProgressBar(int minValue, int maxValue, (char start, char bar, char end) barStyle, 
        int barSize = 40, bool showData = true) {
        
        this.barSize = barSize;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.showData = showData;
        beginChar = barStyle.start;
        barChar = barStyle.bar;
        endChar = barStyle.end;
        System.Console.CursorVisible = false;
        StartProgressBar();
    }

    public void EndProgressBar() {
        System.Console.WriteLine();
        System.Console.CursorVisible = true;
    }

    /// <summary>
    /// add this method to your event handler to update the progress bar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">
    /// if typeof <see cref="ProgressBarArgs"/> determines what state it is currently at,
    /// else adds 1 to current value
    /// </param>
    public void OnProgressUpdate(object? sender, EventArgs e) {
        if (e.GetType() == typeof(ProgressBarArgs)) {
            currentValue = ((ProgressBarArgs)e).Value;
        }
        else {
            currentValue++;
        }

        UpdateProgressBar();
    }

    private void SetStyle(BarStyle barStyle) {
        switch (barStyle) {
            case BarStyle.CLASSIC:
                beginChar = CLASSIC_START;
                barChar = CLASSIC_BAR;
                endChar = CLASSIC_END;
                break;
            case BarStyle.MODERN:
                beginChar = MODERN_START;
                barChar = MODERN_BAR;
                endChar = MODERN_END;
                break;
            case BarStyle.MODERN_DOUBLE:
                beginChar = MODERN_START;
                barChar = MODERN_DOUBLE_BAR;
                endChar = MODERN_END;
                break;
            case BarStyle.MODERN_THIN:
                beginChar = MODERN_START;
                barChar = MODERN_THIN_BAR;
                endChar = MODERN_END;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(barStyle), barStyle, null);
        }
    }
    
    private void StartProgressBar() {
        PrintBarStart();

        for (int i = 0; i < barSize; i++) {
            PrintBarPart(false);
        }
        
        PrintBarEnd();

        System.Console.CursorLeft = OFFSET;
    }

    private void UpdateProgressBar() {
        int steps = (int)((1 - (float)(maxValue - minValue - currentValue) / (maxValue - minValue)) * barSize);
        System.Console.CursorLeft = OFFSET;
        
        for (int i = 0; i < steps; i++) {
            PrintBarPart(true);
        }
    }

    private void PrintBarPart(bool state) {
        ConsoleColors.Write("" + barChar, state ? NKDebug.Logger.InfoColor : NKDebug.Logger.ErrorColor);
    }

    private void PrintBarStart() {
        System.Console.Write(beginChar);
    }
    
    private void PrintBarEnd() {
        System.Console.Write(endChar);
    }

    public enum BarStyle {
        CLASSIC,
        MODERN,
        MODERN_DOUBLE,
        MODERN_THIN
    }

    private const char CLASSIC_START = '[';
    private const char CLASSIC_BAR = '=';
    private const char CLASSIC_END = ']';
    
    private const char MODERN_START = ' ';
    private const char MODERN_END = ' ';
    private const char MODERN_BAR = '━';
    private const char MODERN_DOUBLE_BAR = '═';
    private const char MODERN_THIN_BAR = '─';
}