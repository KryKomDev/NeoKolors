//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings;

/// <summary>
/// Settings Node Class <br/>
/// single settings node object
/// </summary>
/// <typeparam name="TResult">result class</typeparam>
public class SettingsNode<TResult> : ISettingsNode<TResult, TResult> {
    
    private string name { get; }
    public string Name => name;
    public Context context { get; private set; } = new();
    public List<SettingsGroup>? groups { get; private set; } = null;
    private Func<Context, TResult>? executionDelegate = null;
    private bool isEmpty = false;


    /// <summary>
    /// creates a new instance
    /// </summary>
    public static SettingsNode<TResult> New(string name) {
        return new SettingsNode<TResult>(name);
    }

    /// <summary>
    /// constructor, checks if type has a parameterless constructor
    /// </summary>
    /// <exception cref="InvalidTypeException">
    /// no parameterless constructor available for <see cref="TResult"/>
    /// </exception>
    private SettingsNode(string name) {
        if (typeof(TResult).GetConstructors().All(c => c.GetParameters().Length != 0))
            throw new InvalidTypeException($"Cannot create settings node for type {typeof(TResult).Name}. " +
                                           $"Class must have a public parameterless constructor.");
        
        this.name = name;
    }


    /// <summary>
    /// adds an argument to the settings node
    /// </summary>
    /// <param name="argumentName">name of the argument</param>
    /// <param name="argumentType">type of the argument</param>
    public ISettingsNode<TResult, TResult> Argument(string argumentName, ArgumentType.IArgumentType argumentType) {
        context.Add(argumentName, argumentType);
        return this;
    }


    /// <summary>
    /// creates a new group of settings
    /// </summary>
    /// <param name="group">added group option</param>
    public ISettingsNode<TResult, TResult> Group(SettingsGroup group) {
        groups ??= [];

        if (!group.IsFullyInitialized())
            throw new SettingsBuilderException($"Group '{group}' in '{name}' is not fully initialized.");
        
        if (groups.Any(g => g.Name == group.Name)) {
            throw new SettingsBuilderException($"Group '{group.Name}' in '{name}' already exists.");
        }
        
        groups.Add(group);
        context.Add(group.groupContext ?? 
                    throw new SettingsBuilderException($"Output context of group '{group.Name}' in '{name}' is null."));

        return this;
    }


    /// <summary>
    /// sets the <see cref="executionDelegate"/> that creates a
    /// <see cref="TResult"/> instance from <see cref="context"/>
    /// </summary>
    public ISettingsNode<TResult, TResult> OnParse(Func<Context, TResult> execute) {
        executionDelegate = execute;
        return this;
    }
    

    /// <summary>
    /// parses the arguments to an object of type <see cref="TResult"/>
    /// </summary>
    public TResult Execute(params (string groupName, string optionName)[] config) {
        if (isEmpty)
            throw new SettingsBuilderException("Node is empty. Cannot execute.");
        
        if (executionDelegate is null)
            throw new SettingsBuilderException("Execution delegate is not set.");

        if (groups is null || groups.Count == 0)
            return executionDelegate(context);
        
        foreach (SettingsGroup group in groups) {
            foreach ((string groupName, string optionName) c in config) {
                if (group.Name != c.groupName) continue;
                
                group.Execute(c.optionName, context);
            }
        }

        return executionDelegate(context);
    }


    /// <summary>
    /// sets the value of an argument
    /// </summary>
    public void SetValue(string argumentName, object value) {
        context[argumentName].SetValue(value);
    }


    public void SetGroupOptionValue(string groupName, string optionName, string argumentName, object value) {
        if (groups is null) {
            throw new SettingsBuilderException("No groups have been set.");
        }
        
        foreach (var g in groups) {
            if (g.Name == groupName) {
                g[optionName].SetValue(argumentName, value);
                return;
            }
        }
    }
    
    public ISettingsNode<TResult, TResult> Clone() {
        SettingsNode<TResult> clone = new SettingsNode<TResult>(name) {
            context = (Context)context.Clone(),
            executionDelegate = executionDelegate
        };

        if (groups == null) return clone;
        
        clone.groups = [];
        foreach (SettingsGroup g in groups) {
            clone.groups.Add((SettingsGroup)g.Clone());
        }
        
        return clone;
    }

    public override string ToString() {
        string output = $"{{\"name\": \"{name}\", \"arguments\": [";

        for (int i = 0; i < context.Length; i++) {
            var argument = context.GetAtIndex(i); 
            output += $"{{\"name\": \"{argument.name}\", \"value\": {argument.value}}}" + 
                      (i == context.Length - 1 ? "" : ", ");
        }

        output += "], \"groups\": [";

        if (groups == null) {
            output += "]}";
            return output;
        }
        
        for (int i = 0; i < groups.Count; i++) {
            output += groups[i] + (i == groups.Count - 1 ? "" : ", ");
        }

        output += "]}";

        return output;
    }
}