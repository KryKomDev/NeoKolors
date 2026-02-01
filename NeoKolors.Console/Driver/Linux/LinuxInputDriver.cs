// NeoKolors
// Copyright (c) 2025 KryKom

#if NK_ENABLE_NATIVE_INPUT

using NeoKolors.Console.Driver.DotNet;

namespace NeoKolors.Console.Driver.Linux;

public class LinuxInputDriver : DotNetInputDriver {
    // Currently, the DotNetInputDriver (using System.Console.ReadKey) provides 
    // the best balance of compatibility and performance on Linux for .NET apps.
    // Future expansion can implement raw 'libc' read calls here if needed.
}

#endif