//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.Runtime.InteropServices;
using Xunit;

namespace NeoKolors.Console.Tests;

/// <summary>
/// Attribute that marks a test method to be executed only on the Windows platform.
/// On non-Windows platforms, the test is automatically skipped by xUnit.
/// </summary>
public sealed class WindowsFactAttribute : FactAttribute {
    public WindowsFactAttribute() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            Skip = "Test is only supported on Windows.";
        }
    }
}

/// <summary>
/// Attribute that marks a theory test method to be executed only on the Windows platform.
/// On non-Windows platforms, the test is automatically skipped by xUnit.
/// </summary>
public sealed class WindowsTheoryAttribute : TheoryAttribute {
    public WindowsTheoryAttribute() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            Skip = "Test is only supported on Windows.";
        }
    }
}

/// <summary>
/// Attribute that marks a test method to be executed only on the Linux platform.
/// On non-Linux platforms, the test is automatically skipped by xUnit.
/// </summary>
public sealed class LinuxFactAttribute : FactAttribute {
    public LinuxFactAttribute() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            Skip = "Test is only supported on Linux.";
        }
    }
}

/// <summary>
/// Attribute that marks a theory test method to be executed only on the Linux platform.
/// On non-Linux platforms, the test is automatically skipped by xUnit.
/// </summary>
public sealed class LinuxTheoryAttribute : TheoryAttribute {
    public LinuxTheoryAttribute() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            Skip = "Test is only supported on Linux.";
        }
    }
}
