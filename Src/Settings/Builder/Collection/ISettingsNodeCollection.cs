// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Builder.Delegate;
using NeoKolors.Settings.Builder.Info;

namespace NeoKolors.Settings.Builder.Collection;

public interface ISettingsNodeCollection<TResult> : IEnumerable<SettingsNodeInfo<TResult>>, ISettingsNodeCollection {
    public new SettingsNodeInfo<TResult>[] Nodes { get; }
    public new int Count { get; }
    public ISettingsNodeCollection<TResult> Node(string name, SettingsNodeSupplier<TResult> supplier);
    public ISettingsNodeCollection<TResult> Node(SettingsNodeInfo<TResult> info);
}

public interface ISettingsNodeCollection : IEnumerable<SettingsNodeInfo> {
    public SettingsNodeInfo[] Nodes { get; }
    public int Count { get; }
    public ISettingsNodeCollection Node(string name, SettingsNodeSupplier supplier);
    public ISettingsNodeCollection Node(SettingsNodeInfo info);
}