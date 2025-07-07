//
// NeoKolors
// Copyright (c) 2025 KryKom
//

// ReSharper disable UnusedParameter.Local

#if !NET9_0_OR_GREATER

namespace NeoKolors.Common;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
public class OverloadResolutionPriorityAttribute : Attribute {
    public OverloadResolutionPriorityAttribute(int priority) { }
}

#endif