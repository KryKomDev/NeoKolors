// NeoKolors
// Copyright (c) 2025 KryKom

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// ReSharper disable InconsistentNaming

namespace NeoKolors.Console;

file static class ConsoleAccelerator {


    [SupportedOSPlatform("windows")]
    internal static class Windows {

        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD {
            public short X;
            public short Y;

            public COORD(short X, short Y) {
                this.X = X;
                this.Y = Y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SMALL_RECT {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct CHAR_INFO {
            [FieldOffset(0)]
            public char UnicodeChar;
            
            [FieldOffset(0)]
            public byte AsciiChar;
            
            [FieldOffset(2)]
            public short Attributes; // Combination of FOREGROUND_COLOR and BACKGROUND_COLOR flags
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteConsoleOutput(
            IntPtr hConsoleOutput,
            [MarshalAs(UnmanagedType.LPArray), In] CHAR_INFO[] lpBuffer,
            COORD dwBufferSize,
            COORD dwBufferCoord,
            ref SMALL_RECT lpWriteRegion
        );

        // Standard Handles
        internal const int STD_INPUT_HANDLE = -10;
        internal const int STD_OUTPUT_HANDLE = -11;
        internal const int STD_ERROR_HANDLE = -12;

        private static readonly IntPtr COUT_HANDLE;
        private static readonly IntPtr CERR_HANDLE;
        
        static Windows() {
            
            // Set the output handle to the console window.
            // This is required to allow the WriteConsole function to work.
            COUT_HANDLE = GetStdHandle(STD_OUTPUT_HANDLE);
            if (COUT_HANDLE == IntPtr.Zero) {
                throw new Exception("Failed to get console handle.");
            }
            
            // Set the output handle to the console window.
            // This is required to allow the WriteConsole function to work.
            CERR_HANDLE = GetStdHandle(STD_ERROR_HANDLE);
            if (CERR_HANDLE == IntPtr.Zero) {
                throw new Exception("Failed to get error handle.");
            }
        }
        
        internal static void WriteAt(short x, short y, string text, short attributes) {

            // Prepare CHAR_INFO array
            var buffer = new CHAR_INFO[text.Length];
            for (int i = 0; i < text.Length; i++) {
                buffer[i].UnicodeChar = text[i];
                buffer[i].Attributes = attributes;
            }

            var bufferSize = new COORD((short)text.Length, 1); // For a single line of text
            var bufferCoord = new COORD(0, 0); // Start writing from (0,0) of the buffer

            var writeRegion = new SMALL_RECT {
                Left = x,
                Top = y,
                Right = (short)(x + text.Length - 1),
                Bottom = y
            };

            // Call the API function
            bool success = WriteConsoleOutput(COUT_HANDLE, buffer, bufferSize, bufferCoord, ref writeRegion);

            if (!success) {
                System.Console.WriteLine($"Unhandled exception: {Marshal.GetLastWin32Error()}");
            }
        }
    }
    
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("freebsd")]
    internal static class Ncurses {
        
        // On Linux/macOS, the ncurses library is typically named libncurses.so or libncurses.dylib.
        // Sometimes it's libncursesw (wide character support) or a specific version like libncurses.so.5.
        // We'll use "ncurses" as a common alias, but you might need to adjust this depending on your system.
        // For wide character support (recommended for general text), use "ncursesw".
        private const string LibNcurses = "ncursesw"; // Or "ncurses" if "ncursesw" doesn't work
        
        // --- Basic Initialization/Deinitialization ---
        [DllImport(LibNcurses)]
        internal static extern IntPtr initscr(); // Initializes the screen and curses mode

        [DllImport(LibNcurses)]
        internal static extern int endwin(); // Restores original terminal mode

        // --- Output Functions ---
        [DllImport(LibNcurses)]
        internal static extern int printw(string format, params object[] args); 

        [DllImport(LibNcurses)]
        internal static extern int addstr(string str); // Adds a string at the current cursor position

        [DllImport(LibNcurses)]
        internal static extern int mvaddstr(int y, int x, string str); // Moves cursor to (y, x) then adds string

        [DllImport(LibNcurses)]
        internal static extern int refresh(); // Updates the physical screen to show changes

        [DllImport(LibNcurses)]
        internal static extern int clear(); // Clears the screen

        [DllImport(LibNcurses)]
        internal static extern int move(int y, int x); // Moves the cursor to (y, x)

        // --- Input Functions ---
        [DllImport(LibNcurses)]
        internal static extern int getch(); // Gets a single character input

        [DllImport(LibNcurses)]
        internal static extern void noecho(); // Disables echoing of characters typed by the user
        
        [DllImport(LibNcurses)]
        internal static extern void cbreak(); // Disables line buffering, allowing character-at-a-time input

        [DllImport(LibNcurses)]
        internal static extern void keypad(IntPtr win, bool bf); // Enables keypad mode (for arrow keys, etc.).
                                                                // For stdscr (main window), pass initscr() result.
        
        // --- Colors ---
        // Make sure you call start_color() before using any color functions.
        // Always call use_default_colors() if you want to use the terminal's default background/foreground.
        [DllImport(LibNcurses)]
        internal static extern int start_color();

        [DllImport(LibNcurses)]
        internal static extern int init_pair(short pair, short f, short b); // Defines a color pair
        
        [DllImport(LibNcurses)]
        internal static extern int attron(int attrs); // Turns on attributes (like A_BOLD, COLOR_PAIR(n))

        [DllImport(LibNcurses)]
        internal static extern int attroff(int attrs); // Turns off attributes

        // Color Constants (short values 0-7 for basic colors)
        internal const short COLOR_BLACK = 0;
        internal const short COLOR_RED = 1;
        internal const short COLOR_GREEN = 2;
        internal const short COLOR_YELLOW = 3;
        internal const short COLOR_BLUE = 4;
        internal const short COLOR_MAGENTA = 5;
        internal const short COLOR_CYAN = 6;
        internal const short COLOR_WHITE = 7;

        // Attribute Constants (these can be combined)
        internal const int A_NORMAL = 0;
        internal const int A_STANDOUT = 0x00010000;
        internal const int A_UNDERLINE = 0x00020000;
        internal const int A_REVERSE = 0x00040000;
        internal const int A_BLINK = 0x00080000;
        internal const int A_BOLD = 0x00100000;
        internal const int A_ALTCHARSET = 0x00200000;
        internal const int A_CHARTEXT = 0x00FF; // Bitmask for the character part
        internal const int A_ATTRIBUTES = ~A_CHARTEXT; // Bitmask for the attribute part
        
        // Helper to get color pair attribute (ncurses uses shift bit)
        internal static int COLOR_PAIR(int n) => (n << 8);
    }
}

