// NeoKolors
// Copyright (c) 2025 KryKom
//
using System.Collections;
using System.Runtime.CompilerServices;
using NeoKolors.Settings.Builder.Delegate;
using NeoKolors.Settings.Builder.Exception;
using NeoKolors.Settings.Builder.Info;

namespace NeoKolors.Settings.Builder.Collection;

public class SettingsNodeCollection<TResult> : ISettingsNodeCollection<TResult> {
    
    private readonly HashSet<SettingsNodeInfo<TResult>> _nodes = [];
    public int Count => _nodes.Count;

    public ISettingsNodeCollection<TResult> Node(string name, SettingsNodeSupplier<TResult> supplier) {
        var res = new SettingsNodeInfo<TResult>(name, supplier(GenericSettingsFactory<TResult>.DefaultNodeProvider()));
        if (_nodes.Add(res)) return this;
        throw SettingsBuilderException.DuplicateNode(name);
    }

    public ISettingsNodeCollection Node(string name, SettingsNodeSupplier supplier) {
        var node = supplier(GenericSettingsFactory<TResult>.DefaultNodeProvider());
        if (node is not SettingsNode<TResult> n)
            throw SettingsBuilderException.InvalidNodeType();
        var res = new SettingsNodeInfo<TResult>(name, n);
        if (_nodes.Add(res)) return this;
        throw SettingsBuilderException.DuplicateNode(name);
    }

    public ISettingsNodeCollection<TResult> Node(SettingsNodeInfo<TResult> info) {
        if (_nodes.Add(info)) return this;
        throw SettingsBuilderException.DuplicateNode(info.Name);
    }

    public ISettingsNodeCollection Node(SettingsNodeInfo info) {
        if (info.Node is not SettingsNode<TResult> n)
            throw SettingsBuilderException.InvalidNodeType();
        var res = new SettingsNodeInfo<TResult>(info.Name, n, info.Description);
        if (_nodes.Add(res)) return this;
        throw SettingsBuilderException.DuplicateNode(info.Name);
    }

    public SettingsNodeInfo<TResult>[] Nodes {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _nodes.ToArray();
    }

    SettingsNodeInfo[] ISettingsNodeCollection.Nodes => 
        _nodes.Select(n => new SettingsNodeInfo(n.Name, n.Node, n.Description)).ToArray();

    IEnumerator<SettingsNodeInfo> IEnumerable<SettingsNodeInfo>.GetEnumerator() =>
        _nodes.Select(n => new SettingsNodeInfo(n.Name, n.Node, n.Description)).GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<SettingsNodeInfo<TResult>> GetEnumerator() => _nodes.GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class SettingsNodeCollection : ISettingsNodeCollection {
    
    private readonly HashSet<SettingsNodeInfo> _nodes = [];
    public int Count => _nodes.Count;

    public ISettingsNodeCollection Node(string name, SettingsNodeSupplier supplier) {
        if (!_nodes.Add(new SettingsNodeInfo(name, supplier(SettingsFactory.DefaultNodeProvider()))))
            throw SettingsBuilderException.DuplicateNode(name);
        return this;
    }

    public ISettingsNodeCollection Node(SettingsNodeInfo info) {
        if (_nodes.Add(info)) return this;
        throw SettingsBuilderException.DuplicateNode(info.Name);
    }

    public SettingsNodeInfo[] Nodes {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _nodes.ToArray();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<SettingsNodeInfo> GetEnumerator() => _nodes.GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}