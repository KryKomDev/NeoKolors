// NeoKolors
// Copyright (c) 2025 KryKom

// ReSharper disable InconsistentNaming


// ReSharper disable once CheckNamespace
// ReSharper disable once EmptyNamespace
namespace System.Runtime.Versioning;

// ReSharper disable once RedundantUsingDirective
using static AttributeTargets;

#if NETSTANDARD2_0

[AttributeUsage(Assembly | Class | Constructor | Enum | Event | Field | Method | Module | Property, AllowMultiple = true, Inherited = false)]
internal class SupportedOSPlatformAttribute : Attribute {
    // ReSharper disable once UnusedParameter.Local
    public SupportedOSPlatformAttribute(string windows) { }
}


#endif

#if !NET8_0_OR_GREATER

[AttributeUsage(Parameter)]
internal class NotNullWhenAttribute : Attribute {
    // ReSharper disable once UnusedParameter.Local
    public NotNullWhenAttribute(bool returnValue) { }
}

#endif