// ReSharper disable CheckNamespace

#if !NET5_0_OR_GREATER

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
internal sealed class NotNullAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class DoesNotReturnIfAttribute : Attribute {
    public bool ReturnValue { get; }

    public DoesNotReturnIfAttribute(bool returnValue) => ReturnValue = returnValue;
}

#endif