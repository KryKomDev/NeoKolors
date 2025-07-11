// NeoKolors
// Copyright (c) 2025 KryKom

using static System.AttributeTargets;

// ReSharper disable InconsistentNaming

#if NETSTANDARD2_0

// ReSharper disable once CheckNamespace
namespace System.Runtime.Versioning;

[AttributeUsage(Assembly | Class | Constructor | AttributeTargets.Enum | Event | Field | Method | Module | Property, AllowMultiple = true, Inherited = false)]
internal class SupportedOSPlatformAttribute : Attribute {
    public SupportedOSPlatformAttribute(string windows) { }
}

#endif