//
// NeoKolors
// Copyright (c) 2025 KryKom
//

// ReSharper disable CheckNamespace

#if !NET5_0_OR_GREATER && !NETSTANDARD2_1

namespace System.Diagnostics.CodeAnalysis {
    
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    internal sealed class NotNullAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class DoesNotReturnIfAttribute : Attribute {
        public bool ReturnValue { get; }

        public DoesNotReturnIfAttribute(bool returnValue) => ReturnValue = returnValue;
    }
}

#endif

#if !NET9_0_OR_GREATER

namespace NeoKolors.Common {
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
    public class OverloadResolutionPriorityAttribute : Attribute {
        
        /// <summary>
        /// The priority of the member.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverloadResolutionPriorityAttribute"/> class.
        /// </summary>
        /// <param name="priority">The priority of the attributed member. Higher numbers are prioritized, lower numbers
        /// are deprioritized. 0 is the default if no attribute is present.</param>
        public OverloadResolutionPriorityAttribute(int priority) {
            Priority = priority;
        }
    }
}

#endif

#if !NET8_0_OR_GREATER

namespace NeoKolors.Common {
    
    /// <summary>
    /// does literally nothing, just makes the code cleaner
    /// </summary>
    internal class StringSyntaxAttribute : Attribute {
        public string Format { get; }

        // ReSharper disable once InconsistentNaming
        public const string CompositeFormat = "CompositeFormat";
        public StringSyntaxAttribute(string format) {
            Format = format;
        }
    }
}

#endif