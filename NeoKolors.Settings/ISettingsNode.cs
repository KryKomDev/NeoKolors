//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.ArgumentTypes;

namespace NeoKolors.Settings;

public interface ISettingsNode<TResult> : ISettingsNode where TResult : class {
    public new TResult GetResult();
    public new ISettingsNode<TResult> Argument(string name, IArgument argument);
    public new ISettingsNode<TResult> Group(SettingsGroup group);
    public ISettingsNode<TResult> Constructs(Func<Context, TResult> resultConstructor);
}

public interface ISettingsNode : ICloneable {
    public string Name { get; }
    public Context Context { get; }
    public List<SettingsGroup> Groups { get; }
    public object GetResult();
    public ISettingsNode Argument(string name, IArgument argument);
    public ISettingsNode Group(SettingsGroup group);
    public ISettingsNode Constructs(Func<Context, object> resultConstructor);
    
    public SettingsGroup this[string name] { get; }
}