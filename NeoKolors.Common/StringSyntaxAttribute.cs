//
// NeoKolors
// Copyright (c) 2025 KryKom
//

#if !NET8_0_OR_GREATER

namespace NeoKolors.Common;

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
#endif