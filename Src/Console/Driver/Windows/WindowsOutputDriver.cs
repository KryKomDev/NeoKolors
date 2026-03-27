// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

namespace NeoKolors.Console.Driver.Windows;

/// <summary>
/// The <c>WindowsOutputDriver</c> class provides an implementation of the <see cref="IOutputDriver"/> interface
/// for handling output operations specifically on Windows platforms. This class ensures thread safety during
/// output operations and manages resource cleanup appropriately.
/// </summary>
/// <remarks>
/// This class is designed to work in conjunction with other platform-specific output drivers and is selected
/// dynamically based on the operating system environment. Internally, it uses the <see cref="WindowsOutput"/>
/// class for low-level output handling on Windows.
/// </remarks>
/// <threadsafety>
/// This class ensures thread safety for output operations through the use of synchronization mechanisms.
/// </threadsafety>
/// <example>
/// The <c>WindowsOutputDriver</c> class is not intended to be used directly by the end-user, but rather
/// as part of the NeoKolors framework's internal configuration for platform-specific outputs.
/// </example>
public class WindowsOutputDriver : IOutputDriver {
    
    private          bool          _disposed = false;
    private readonly LockObject    _lock     = new();
    private readonly WindowsOutput _output;

    public WindowsOutputDriver() {
        _output = new WindowsOutput();
    }
    
    public void Write(ReadOnlySpan<char> value) {
        lock (_lock) {
            _output.Write(value);
        }
    }

    public void Dispose() {
        if (_disposed)
            return;
        
        _output.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}