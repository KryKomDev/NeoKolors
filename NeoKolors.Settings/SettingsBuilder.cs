//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings;

public class SettingsBuilder<TResult> : ISettingsBuilder<TResult> where TResult : class {
    
    public ISettingsNode[] Nodes { get; }
    public NodeSwitch Switch { get; set; }
    public string Name { get; }

    private SettingsBuilder(string name, ISettingsNode[] nodes) {
        Nodes = nodes;
        Name = name;
        Switch = new NodeSwitch(nodes);
    }

    public ISettingsNode this[int index] {
        get {
            try {
                return Nodes[index];
            }
            catch (IndexOutOfRangeException) {
                throw SettingsBuilderException.NodeIndexOutOfRange(index, Nodes.Length);
            }
        }    
    }
    
    public ISettingsNode this[string name] {
        get {
            foreach (var n in Nodes) {
                if (n.Name == name) return n;
            }
            
            throw SettingsBuilderException.InvalidNodeName(name);
        }
    }

    /// <summary>
    /// creates a new instance
    /// </summary>
    public static SettingsBuilder<TResult> Build(string name, params ISettingsNode[] nodes) {
        return new SettingsBuilder<TResult>(name, nodes);
    }

    /// <summary>
    /// returns the result of the settings
    /// </summary>
    public TResult GetResult() {
        var n = Nodes[Switch.Index];
        return (TResult)n.GetResult();
    }
    

    public object Clone() {
        ISettingsNode[] newNodes = new ISettingsNode[Nodes.Length];

        for (int i = 0; i < Nodes.Length; i++) {
            newNodes[i] = (ISettingsNode)Nodes[i].Clone();
        }
        
        return new SettingsBuilder<TResult>(Name, newNodes) { Switch = Switch };
    }
    
    public void Select(string nodeName) => Switch = Switch.Select(nodeName);
    public void Select(int nodeIndex) => Switch = Switch.Select(nodeIndex);
    public override string ToString() =>
        $"{{\"name\": \"{Name}\", " +
        $"\"switch\": {Switch}, " +
        $"\"nodes\": [{string.Join(", ", Nodes.Select(n => n.ToString()))}]}}";
}