// NeoKolors 
// Copyright (c) KryKom 2025

#include <iostream>

#ifdef _WIN32
    #include <windows.h>
    #define EXPORT __declspec(dllexport)
#else
    #include <sys/ioctl.h>
    #include <signal.h>
    #include <unistd.h>
    #define EXPORT __attribute__((visibility("default")))
#endif

// Define a function pointer type for the C# callback
typedef void (*ResizeCallback)(int rows, int cols);
ResizeCallback globalCallback = nullptr;

#ifndef _WIN32
// Unix Signal Handler
void handle_sigwinch(int sig) {
    struct winsize w;
    ioctl(STDOUT_FILENO, TIOCGWINSZ, &w);
    if (globalCallback) {
        globalCallback(w.ws_row, w.ws_col);
    }
}
#endif

extern "C" {
    EXPORT void RegisterResizeCallback(ResizeCallback callback) {
        globalCallback = callback;

        #ifdef _WIN32
            // Windows doesn't use SIGWINCH. 
            // Usually, C# developers use Console.WindowWidth in a loop 
            // or ReadConsoleInput, but for this C++ bridge:
            std::cout << "Windows: Use .NET Console events for better stability." << std::endl;
        #else
            // Register the Unix signal
            signal(SIGWINCH, handle_sigwinch);
        #endif
    }
}