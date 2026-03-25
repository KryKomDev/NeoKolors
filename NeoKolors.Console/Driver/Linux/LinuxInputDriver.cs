// NeoKolors
// Copyright (c) 2025 KryKom

#if NK_ENABLE_NATIVE_IO

using NeoKolors.Console.Driver.Dotnet;

namespace NeoKolors.Console.Driver.Linux;

public class LinuxInputDriver : DotnetInputDriver {
    // Currently, the DotnetInputDriver (using System.Console.ReadKey) provides 
    // the best balance of compatibility and performance on Linux for .NET apps.
    // Future expansion can implement raw 'libc' read calls here if needed.
}

#endif