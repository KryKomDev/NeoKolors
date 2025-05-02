//
// NeoKolors
// Copyright (c) 2025 KryKom
//

// ReSharper disable UnusedParameter.Local

namespace NeoKolors.Common;

#if !NET9_0_OR_GREATER

[AttributeUsage(AttributeTargets.Method)]
internal class OverloadResolutionPriorityAttribute : Attribute {
    public OverloadResolutionPriorityAttribute(int priority) { }
}

#endif