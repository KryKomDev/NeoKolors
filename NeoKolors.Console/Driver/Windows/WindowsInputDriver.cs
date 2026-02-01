// NeoKolors
// Copyright (c) 2025 KryKom

#pragma warning disable CS0067

#if NK_ENABLE_NATIVE_INPUT

using Metriks;
using NeoKolors.Console.Events;
using NeoKolors.Console.Mouse;
using static NeoKolors.Console.Driver.Windows.NativeConsole;

namespace NeoKolors.Console.Driver.Windows;

public class WindowsInputDriver : IInputDriver {

    public event MouseEventHandler? Mouse;
    public event KeyEventHandler? Key;
    public event FocusInEventHandler? FocusIn;
    public event FocusOutEventHandler? FocusOut;
    public event PasteEventHandler? Paste;
    public event WinOpsResponseEventHandler? WinOpsResponse;
    public event DecReqResponseEventHandler? DecReqResponse;

    private bool _running;
    private Thread? _inputThread;
    private readonly IntPtr _hInput;
    private uint _originalMode;
    private readonly NKLogger _logger = NKDebug.GetLogger("WindowsInputDriver");

    public WindowsInputDriver() {
        _hInput = GetStdHandle(STD_INPUT_HANDLE);
    }

    public void Start() {
        if (_running) return;
        
        // Setup Console Mode
        if (GetConsoleMode(_hInput, out _originalMode)) {
            uint newMode = (_originalMode | ENABLE_MOUSE_INPUT | ENABLE_WINDOW_INPUT | ENABLE_EXTENDED_FLAGS) & ~ENABLE_QUICK_EDIT_MODE;
            // Determine if we should also disable line input/echo for true raw mode?
            // Usually yes for TUI.
            // newMode &= ~(ENABLE_LINE_INPUT | ENABLE_ECHO_INPUT); 
            // Leaving them might be safer if we only want to add capabilities, but usually TUI wants control.
            // For now, focus on Quick Edit and Mouse.
            SetConsoleMode(_hInput, newMode);
        } else {
            _logger.Error("Failed to get console mode.");
        }

        _running = true;
        _inputThread = new Thread(Loop) {
            IsBackground = true,
            Priority = ThreadPriority.BelowNormal,
            Name = "NeoKolors Windows Input Interceptor"
        };
        _inputThread.Start();
    }

    public void Stop() {
        if (!_running) return;
        _running = false;
        
        // Restore Console Mode
        SetConsoleMode(_hInput, _originalMode);
    }

    public void Dispose() {
        Stop();
    }

    private void Loop() {
        var buffer = new INPUT_RECORD[10];
        
        while (_running) {
            try {
                // Wait for input events or timeout (to check _running flag)
                uint waitResult = WaitForSingleObject(_hInput, 100);

                if (waitResult == WAIT_OBJECT_0) {
                    // Input available
                    if (GetNumberOfConsoleInputEvents(_hInput, out uint count) && count > 0) {
                        if (ReadConsoleInput(_hInput, buffer, (uint)buffer.Length, out uint read)) {
                            for (int i = 0; i < read; i++) {
                                ProcessRecord(buffer[i]);
                            }
                        }
                    }
                }
                else if (waitResult == WAIT_FAILED) {
                    _logger.Error("WaitForSingleObject failed.");
                    Thread.Sleep(100);
                }
            } catch (Exception e) {
                _logger.Error($"Error in Windows input loop: {e.Message}");
                Thread.Sleep(100);
            }
        }
    }

    private void ProcessRecord(INPUT_RECORD record) {
        switch (record.EventType) {
            case KEY_EVENT:
                HandleKey(record.KeyEvent);
                break;
            case MOUSE_EVENT:
                HandleMouse(record.MouseEvent);
                break;
            case WINDOW_BUFFER_SIZE_EVENT:
                HandleResize(record.WindowBufferSizeEvent);
                break;
            case FOCUS_EVENT:
                HandleFocus(record.FocusEvent);
                break;
        }
    }

    private void HandleKey(KEY_EVENT_RECORD k) {
        // We only care about KeyDown usually, unless we want KeyUp events?
        // System.Console.ReadKey only fires on Down.
        if (!k.bKeyDown) return;

        // Map control keys
        bool shift = (k.dwControlKeyState & 0x0010) != 0; // SHIFT_PRESSED
        bool alt = (k.dwControlKeyState & 0x0001) != 0 || (k.dwControlKeyState & 0x0002) != 0; // LEFT_ALT or RIGHT_ALT
        bool ctrl = (k.dwControlKeyState & 0x0004) != 0 || (k.dwControlKeyState & 0x0008) != 0; // LEFT_CTRL or RIGHT_CTRL

        var keyInfo = new ConsoleKeyInfo(k.uChar, (ConsoleKey)k.wVirtualKeyCode, shift, alt, ctrl);
        
        _ = Task.Run(() => Key?.Invoke(keyInfo));
    }

    private void HandleMouse(MOUSE_EVENT_RECORD m) {
        var x = m.dwMousePosition.X;
        var y = m.dwMousePosition.Y;
        
        // Modifiers
        ConsoleModifiers mods = 0;
        if ((m.dwControlKeyState & 0x0010) != 0) mods |= ConsoleModifiers.Shift;
        if ((m.dwControlKeyState & 0x0008) != 0 || (m.dwControlKeyState & 0x0004) != 0) mods |= ConsoleModifiers.Control;
        if ((m.dwControlKeyState & 0x0002) != 0 || (m.dwControlKeyState & 0x0001) != 0) mods |= ConsoleModifiers.Alt;
        
        bool moved = (m.dwEventFlags & 0x0001) != 0; // MOUSE_MOVED
        bool wheeled = (m.dwEventFlags & 0x0004) != 0; // MOUSE_WHEELED
        
        var btn = MouseButton.RELEASE;
        bool release = false;
        
        if (wheeled) {
            long delta = (short)((m.dwButtonState >> 16) & 0xFFFF);
            btn = delta > 0 ? MouseButton.WHEEL_UP : MouseButton.WHEEL_DOWN;
        }
        else {
            // Check buttons
            if ((m.dwButtonState & 0x01) != 0) btn = MouseButton.LEFT;
            else if ((m.dwButtonState & 0x04) != 0) btn = MouseButton.MIDDLE; // Note: Windows Middle is 0x04 usually? Left=1, Right=2, Middle=4.
            else if ((m.dwButtonState & 0x02) != 0) btn = MouseButton.RIGHT;
            
            if (btn == MouseButton.RELEASE && !moved) {
                // If not moved and no button, it's a release (if event flags are 0)
                release = true;
            }
        }

        var args = new MouseEventArgs(
            button: btn,
            modifiers: mods,
            move: moved,
            position: new Point2D(x, y),
            release: release
        );

        _ = Task.Run(() => Mouse?.Invoke(args));
    }

    private void HandleResize(WINDOW_BUFFER_SIZE_RECORD r) {
        // Translate to WinOpsResponseArgs.WinSize?
        // Or just let the native resize handler deal with it?
        // NKConsole has WinOpsResponse event for explicit requests.
        // But getting a free update is nice.
        _ = Task.Run(() => WinOpsResponse?.Invoke(WinOpsResponseArgs.WinSize(new Size2D(r.dwSize.X, r.dwSize.Y))));
    }

    private void HandleFocus(FOCUS_EVENT_RECORD f) {
        if (f.bSetFocus)
            _ = Task.Run(() => FocusIn?.Invoke());
        else
            _ = Task.Run(() => FocusOut?.Invoke());
    }
}

#endif