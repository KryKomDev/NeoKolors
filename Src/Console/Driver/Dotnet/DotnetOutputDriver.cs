//
// NeoKolors
// Copyright (c) 2026 KryKom
//

namespace NeoKolors.Console.Driver.Dotnet;

/// <summary>
/// Represents an implementation of the <see cref="IOutputDriver"/> interface
/// for writing output using the .NET standard output.
/// </summary>
/// <remarks>
/// This driver interacts with the .NET standard output mechanism to write
/// character data or formatted strings. It ensures proper disposal of
/// output resources when no longer needed.
/// </remarks>
public class DotnetOutputDriver : IOutputDriver {
    private bool _disposed = false;

    public void Write(ReadOnlySpan<char> value) => Stdio.Write(value.ToArray());
    public void Write(string value) => Stdio.Write(value);

    ~DotnetOutputDriver() {
        Dispose();
    }
    
    public void Dispose() {
        if (_disposed) 
            return;

        _disposed = true;
        Stdio.Out.Close();
        GC.SuppressFinalize(this);
    }
}