//
// NeoKolors
// Copyright (c) 2025 KryKom
//

// ReSharper disable CheckNamespace

namespace System.Diagnostics.CodeAnalysis {
    
    #if !NET5_0_OR_GREATER

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    internal sealed class NotNullAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class DoesNotReturnIfAttribute : Attribute {
        public bool ReturnValue { get; }

        public DoesNotReturnIfAttribute(bool returnValue) => ReturnValue = returnValue;
    }

    #endif

    #if !NET8_0_OR_GREATER

    /// <summary>
    /// does literally nothing, just makes the code cleaner
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class StringSyntaxAttribute : Attribute {

        public StringSyntaxAttribute(string syntax, params object?[] arguments) {
            Syntax = syntax;
            Arguments = arguments;
        }

        /// <summary>Gets the identifier of the syntax used.</summary>
        public string Syntax { get; }

        /// <summary>Optional arguments associated with the specific syntax employed.</summary>
        public object?[] Arguments { get; }

        /// <summary>The syntax identifier for strings containing composite formats for string formatting.</summary>
        public const string CompositeFormat = nameof(CompositeFormat);

        /// <summary>The syntax identifier for strings containing date format specifiers.</summary>
        public const string DateOnlyFormat = nameof(DateOnlyFormat);

        /// <summary>The syntax identifier for strings containing date and time format specifiers.</summary>
        public const string DateTimeFormat = nameof(DateTimeFormat);

        /// <summary>The syntax identifier for strings containing <see cref="Enum"/> format specifiers.</summary>
        public const string EnumFormat = nameof(EnumFormat);

        /// <summary>The syntax identifier for strings containing <see cref="Guid"/> format specifiers.</summary>
        public const string GuidFormat = nameof(GuidFormat);

        /// <summary>The syntax identifier for strings containing JavaScript Object Notation (JSON).</summary>
        public const string Json = nameof(Json);

        /// <summary>The syntax identifier for strings containing numeric format specifiers.</summary>
        public const string NumericFormat = nameof(NumericFormat);

        /// <summary>The syntax identifier for strings containing regular expressions.</summary>
        public const string Regex = nameof(Regex);

        /// <summary>The syntax identifier for strings containing time format specifiers.</summary>
        public const string TimeOnlyFormat = nameof(TimeOnlyFormat);

        /// <summary>The syntax identifier for strings containing <see cref="TimeSpan"/> format specifiers.</summary>
        public const string TimeSpanFormat = nameof(TimeSpanFormat);

        /// <summary>The syntax identifier for strings containing URIs.</summary>
        public const string Uri = nameof(Uri);

        /// <summary>The syntax identifier for strings containing XML.</summary>
        public const string Xml = nameof(Xml);
    }
    #endif

    #if !NET9_0_OR_GREATER

    /// <summary>
    /// Specifies the priority used during overload resolution for methods or constructors.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
    public class OverloadResolutionPriorityAttribute : Attribute {
        public OverloadResolutionPriorityAttribute(int priority) { }
    }

    #endif
}

namespace System.Runtime.Versioning {
    #if NETSTANDARD2_1
    
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SupportedOsPlatformAttribute : Attribute {
        public SupportedOsPlatformAttribute(string platform) { }
    }
    
    #endif
}

namespace System {
    #if NETSTANDARD2_0

    [AttributeUsage(AttributeTargets.Parameter)]
    public class NotNullWhenAttribute : Attribute {
        public NotNullWhenAttribute(bool returnValue) { }
    }
    #endif
}