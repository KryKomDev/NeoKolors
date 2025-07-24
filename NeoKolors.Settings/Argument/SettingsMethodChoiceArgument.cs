// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Common.Util;
using NeoKolors.Settings.Builder;

namespace NeoKolors.Settings.Argument;

/// <summary>
/// Represents an argument that allows selecting a single choice from a predefined set of string options.
/// </summary>
public class SettingsMethodChoiceArgument : SingleSelectArgument<string>, IXsdArgument {
    public SettingsMethodChoiceArgument(string[] options, int defaultValue = 0) : base(options, defaultValue) { }
    public SettingsMethodChoiceArgument(string[] options, string defaultValue) : base(options, defaultValue) { }

    /// <summary>
    /// Creates a new <see cref="SettingsMethodChoiceArgument"/> from the provided <see cref="SettingsMethodGroup"/>.
    /// </summary>
    /// <param name="group">The settings group containing options to initialize the argument with.</param>
    /// <returns>A new instance of <see cref="SettingsMethodChoiceArgument"/> based on the options from the settings group.</returns>
    public static SettingsMethodChoiceArgument FromGroup(SettingsMethodGroup group) =>
        new(group.Options.Select(option => option.Name).ToArray());

    // This is technically unnecessary, but it is faster as it does not call ToString
    public new string ToXsd() =>
        $"""
         <xsd:simpleType>
             <xsd:restriction base="xsd:string">
                 {Options.Select(s => $"<xsd:enumeration value=\"{s}\"/>").Join("\n").PadLinesLeft(8)}
             </xsd:restriction>
         </xsd:simpleType>
         """;
}