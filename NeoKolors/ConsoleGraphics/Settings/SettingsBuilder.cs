//
// NeoKolors
// by KryKom 2024
//

using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings;

/// <summary>
/// Settings Builder <br/>
/// builds settings for generator (or other) classes 
/// </summary>
/// <typeparam name="TResult">settings result class</typeparam>
public class SettingsBuilder<TResult> : ISettingsBuilder<TResult, TResult> {

    public string name { get; }
    public List<ISettingsNode<TResult, TResult>>? nodes { get; private set; } = null;
    private bool isEmpty = false;


    public ISettingsNode<TResult, TResult> this[string name] {
        get {
            if (nodes is null) {
                throw new SettingsBuilderException($"No nodes have been set in builder '{name}'");
            }

            foreach (var n in nodes) {
                if (n.Name == name) {
                    return n;
                }
            }
            
            throw new SettingsBuilderException($"No node with name '{name}' exists in the builder '{name}'");
        }
    } 
    
    
    /// <summary>
    /// constructor, checks if type has a parameterless constructor
    /// </summary>
    /// <exception cref="InvalidTypeException">
    /// no parameterless constructor available for <see cref="TResult"/>
    /// </exception>
    public SettingsBuilder(string name) {
        if (typeof(TResult).GetConstructors().All(c => c.GetParameters().Length != 0))
            throw new InvalidTypeException($"Cannot create settings node for type {typeof(TResult).Name}. " +
                                           $"Class must have a public parameterless constructor.");
        
        this.name = name;
    }
    
    /// <summary>
    /// adds new settings nodes to the builder
    /// </summary>
    /// <param name="nodes">node creation actions</param>
    public void Build(params ISettingsNode<TResult, TResult>[] nodes) {
        
        this.nodes ??= nodes.ToList();

        this.nodes.AddRange(nodes);
    }

    
    /// <summary>
    /// creates a new <see cref="SettingsBuilder{TResult}"/> with nodes and returns it
    /// </summary>
    public static ISettingsBuilder<TResult, TResult> Build(string name, params ISettingsNode<TResult, TResult>[] nodes) {
        SettingsBuilder<TResult> builder = new SettingsBuilder<TResult>(name) {
            nodes = nodes.ToList()
        };
        
        return builder;
    }

    /// <summary>
    /// converts the settings to an object of type <see cref="TResult"/>
    /// </summary>
    /// <exception cref="SettingsBuilderException">
    /// no settings have been configured,
    /// no matching settings node was found
    /// </exception>
    public TResult Execute(string nodeName, params (string groupName, string optionName)[] selectedOptions) {
        if (isEmpty)
            throw new SettingsBuilderException("SettingsBuilder is empty. Cannot execute.");
        
        if (nodes == null) 
            throw new SettingsBuilderException("Cannot execute settings before building them.");
        
        if (nodes.Count == 0) 
            throw new SettingsBuilderException("No settings nodes have been configured.");
        
        foreach (var n in nodes) {
            if (n.Name == nodeName) {
                return n.Execute(selectedOptions);
            }
        }

        throw new SettingsBuilderException("No matching settings node found.");
    }


    public ISettingsBuilder<TResult, TResult> Clone() {
        SettingsBuilder<TResult> clone = new SettingsBuilder<TResult>(name);
        
        if (nodes == null) return clone;
        
        clone.nodes = new List<ISettingsNode<TResult, TResult>>();
        
        foreach (ISettingsNode<TResult, TResult> n in nodes) {
            clone.nodes.Add(n.Clone());
        }

        return clone;
    }

    public override string ToString() {
        string output = $"{{\"name\": \"{name}\", \"nodes\": [";

        if (nodes == null) {
            output += "]}";
            return output;
        }
        
        for (int i = 0; i < nodes.Count; i++) {
            output += "" + nodes[i] + (i == nodes.Count - 1 ? "" : ", ");
        }
        
        output += "]}";
        
        return output;
    }
}