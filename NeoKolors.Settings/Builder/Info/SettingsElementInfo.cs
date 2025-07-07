// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Settings.Argument;
using OneOf;

namespace NeoKolors.Settings.Builder.Info;

public readonly struct SettingsElementInfo {
    
    public string Name => _element.Match(a => a.Name, g => g.Name);
    private readonly OneOf<ArgumentInfo, SettingsMethodGroupInfo> _element;
    
    public SettingsElementInfo(string name, SettingsMethodGroup group, string? description = null) => 
        _element = new SettingsMethodGroupInfo(name, group, description);

    public SettingsElementInfo(string name, IArgument argument, string? description = null) => 
        _element = new ArgumentInfo(name, argument, description);

    public bool IsArgument => _element.IsT0;
    public bool IsGroup => _element.IsT1;
    
    public IArgument AsArgument => _element.AsT0.Argument;
    public SettingsMethodGroup AsGroup => _element.AsT1.Group;
    
    public override int GetHashCode() => 
        _element.Match(
            a => a.GetHashCode(), 
            g => g.GetHashCode()
        );

    public string ToXsd() {
        return _element.Match(
            info => info.ToXsd(), 
            group => group.ToXsd()
        );
    }
}