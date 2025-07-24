// NeoKolors
// Copyright (c) 2025 KryKom
//
using System.Collections;
using System.Runtime.CompilerServices;
using NeoKolors.Common.Util;
using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Builder.Delegate;
using NeoKolors.Settings.Builder.Exception;
using NeoKolors.Settings.Builder.Info;

namespace NeoKolors.Settings.Builder.Collection;

public class SettingsElementCollection : IEnumerable<ISettingsElementInfo> {
    
    private static string METHOD_OPTION_ARG_FORMAT;

    private readonly HashSet<ISettingsElementInfo> _elements = [];

    public void Argument(string name, IArgument argument, string? description = null) {
        if (_elements.Add(new ArgumentInfo(name, argument, description))) return;
        throw SettingsBuilderException.DuplicateElement(name);
    }

    public void Group(string name, SettingsMethodGroupSupplier group, string? description = null) {
        var g = group(new SettingsMethodGroup());
        var info = new SettingsMethodGroupInfo(name, g, description);
        
        if (_elements.Contains(info)) throw SettingsBuilderException.DuplicateElement(name);
            
        _elements.Add(new ArgumentInfo(NameArg(name), g.GetChoiceArgument()));
        _elements.Add(info);
    }

    public ISettingsElementInfo[] Elements => _elements.ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<ISettingsElementInfo> GetEnumerator() => _elements.GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    
    // --- static ---

    static SettingsElementCollection() {
        METHOD_OPTION_ARG_FORMAT = "{0}Methods";
        TestFormat();
    } 

    public static void SetMethodOptionArgFormat(string format) {
        METHOD_OPTION_ARG_FORMAT = format;
        TestFormat();
    }

    public static string GetMethodOptionArgFormat() => METHOD_OPTION_ARG_FORMAT;

    private static void TestFormat() {
        string t1 = NameArg("t1");
        string t2 = NameArg("t2");
        
        if (t1 == t2) 
            throw new SettingsBuilderException("Format must be a format string with at least one argument.");
        
        if (!t1.IsValidIdentifier() || !t2.IsValidIdentifier()) 
            throw new SettingsBuilderException("Format must be a valid identifier.");
    }
    
    private static string NameArg(string groupName) => string.Format(METHOD_OPTION_ARG_FORMAT, groupName);
}