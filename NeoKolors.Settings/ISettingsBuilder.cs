//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings;

public interface ISettingsBuilder<out TResult> : ICloneable where TResult : class {
    
    public ISettingsNode[] Nodes { get; }
    public NodeSwitch Switch { get; set; }
    public string Name { get; }
    public TResult GetResult();
}