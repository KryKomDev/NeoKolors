//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Settings.Argument;

namespace NeoKolors.Settings;

public interface ISettingsNode<out TResult> : ICloneable where TResult : class {
    public string Name { get; }
    public Func<Context, TResult>? ResultConstructor { get; }
    public TResult GetResult();
    public ISettingsNode<TResult> Argument(string name, IArgument argument);
    public ISettingsNode<TResult> Group(SettingsGroup group);
}