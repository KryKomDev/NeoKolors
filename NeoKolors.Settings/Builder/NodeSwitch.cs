// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Settings.Builder.Collection;
using NeoKolors.Settings.Builder.Exception;
using NeoKolors.Settings.Builder.Info;

namespace NeoKolors.Settings.Builder;

public struct NodeSwitch<TResult> {
    
    private readonly ISettingsNodeCollection<TResult> _nodes;
    private int _index = 0;

    public NodeSwitch(ISettingsNodeCollection<TResult> nodes) => _nodes = nodes;

    public void Select(string name) {
        var ns = _nodes.Nodes;
        for (int i = 0; i < ns.Length; i++) {
            if (ns[i].Name != name) continue;
            _index = i;
            break;
        }
        throw SettingsExecutionException.InvalidNodeName(name);
    }
    
    public SettingsNodeInfo<TResult> Get() => _nodes.Nodes[_index];
}

public struct NodeSwitch {
    
    private readonly ISettingsNodeCollection _nodes;
    private int _index = 0;

    public NodeSwitch(ISettingsNodeCollection nodes) => _nodes = nodes;

    public void Select(string name) {
        var ns = _nodes.Nodes;
        for (int i = 0; i < ns.Length; i++) {
            if (ns[i].Name != name) continue;
            _index = i;
            break;
        }
        throw SettingsExecutionException.InvalidNodeName(name);
    }
    
    public SettingsNodeInfo Get() => _nodes.Nodes[_index];
}