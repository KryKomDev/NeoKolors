// NeoKolors
// Copyright (c) 2025 KryKom

using System.Runtime.InteropServices;
using static System.Console;

// ReSharper disable All

namespace NeoKolors.Console.Native;

internal static class ResizeHandler {
    
    internal delegate void ResizeCallback(int rows, int cols);

    // Import the native function
    [DllImport("NativeResizeHandler", CallingConvention = CallingConvention.Cdecl)]
    public static extern void RegisterResizeCallback(ResizeCallback callback);

    // Keep the delegate alive so GC doesn't collect it
    private static ResizeCallback _callbackReference;

    static ResizeHandler() {
        _callbackReference = OnWindowResized;
    }
    
    public static void Main()
    {
        _callbackReference = OnWindowResized;

        WriteLine("Listening for resize events... (Press Ctrl+C to exit)");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            WatchWindowsResize();
        }
        else
        {
            // Unix specific: Use our C++ Signal Handler
            RegisterResizeCallback(_callbackReference);
            
            // Keep the main thread alive
            while (true) { Thread.Sleep(1000); }
        }
    }
    
    static void OnWindowResized(int rows, int cols) {
        WriteLine($"[Signal] Terminal resized to: {rows} rows x {cols} cols");
    }

    static void WatchWindowsResize()
    {
        int lastWidth = WindowWidth;
        int lastHeight = WindowHeight;

        while (true) {
            if (WindowWidth != lastWidth || WindowHeight != lastHeight) {
                lastWidth = WindowWidth;
                lastHeight = WindowHeight;
                OnWindowResized(lastHeight, lastWidth);
            }
            
            Thread.Sleep(100);
        }
    }
}