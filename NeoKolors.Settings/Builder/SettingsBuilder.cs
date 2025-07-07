// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Common.Util;
using NeoKolors.Settings.Builder.Collection;
using NeoKolors.Settings.Builder.Delegate;
using NeoKolors.Settings.Builder.Exception;

namespace NeoKolors.Settings.Builder;

public class SettingsBuilder<TResult> : ISettingsBuilder<TResult> {
    
    private ISettingsNodeCollection<TResult> _nodes;
    private NodeSwitch<TResult> _switch;

    public SettingsBuilder(ISettingsNodeCollection<TResult> nodes) {
        _nodes = nodes;
        _switch = new NodeSwitch<TResult>(_nodes);
    }

    public SettingsBuilder(SettingsNodeCollectionSupplier<TResult> supplier) {
        _nodes = supplier(new SettingsNodeCollection<TResult>());
        _switch = new NodeSwitch<TResult>(_nodes);
    }
    
    public Context Parse() => _switch.Get().Node.Parse();
    public TResult Execute() => _switch.Get().Node.Execute();
    public TResult Execute(Context context) => _switch.Get().Node.Execute(context);

    public ISettingsBuilder<TResult> Nodes(SettingsNodeCollectionSupplier<TResult> supplier) {
        _nodes = supplier(_nodes);
        return this;
    }

    object? ISettingsBuilder.Execute() => Execute();
    object? ISettingsBuilder.Execute(Context context) => Execute(context);
    
    public ISettingsBuilder Nodes(SettingsNodeCollectionSupplier supplier) {
        _nodes = supplier(_nodes) as ISettingsNodeCollection<TResult> ?? throw SettingsBuilderException.InvalidNodeType();
        return this;   
    }

    public string ToXsd() => _nodes.Nodes.Select(n => n.ToXsd()).Join("\n");
    public object Clone() => MemberwiseClone();
}