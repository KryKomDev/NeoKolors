//
// NeoKolors
// Copyright (c) 2024 KryKom
//

namespace NeoKolors.Settings;

public class SettingsBuilder<TResult> : ISettingsBuilder<TResult> where TResult : class {
    
    public ISettingsNode<TResult>[] Nodes { get; }
    public NodeSwitch Switch { get; set; }
    public string Name { get; }

    private SettingsBuilder(string name, ISettingsNode<TResult>[] nodes) {
        Nodes = nodes;
        Name = name;

        // ReSharper disable once CoVariantArrayConversion
        Switch = new NodeSwitch(nodes);
    }

    /// <summary>
    /// creates a new instance
    /// </summary>
    public static SettingsBuilder<TResult> Build(string name, params ISettingsNode<TResult>[] nodes) {
        return new SettingsBuilder<TResult>(name, nodes);
    }

    /// <summary>
    /// returns the result of the settings
    /// </summary>
    public TResult GetResult() {
        var n = Nodes[Switch.Index];
        return n.GetResult();
    }
    

    public object Clone() {
        ISettingsNode<TResult>[] newNodes = new ISettingsNode<TResult>[Nodes.Length];

        for (int i = 0; i < Nodes.Length; i++) {
            newNodes[i] = (ISettingsNode<TResult>)Nodes[i].Clone();
        }
        
        return new SettingsBuilder<TResult>(Name, newNodes) { Switch = Switch };
    }
}