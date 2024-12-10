//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings;

/// <summary>
/// groups options of different inputs of settings  
/// </summary>
public sealed class SettingsGroup : ICloneable {
    private string name { get; }
    public string Name => name;
    public Context? groupContext { get; private init; } = null;
    private List<SettingsGroupOption>? options { get; set; } = null;
    public SettingsGroupOption[] Options => 
        options?.ToArray() ?? throw new SettingsBuilderException("No options have been set.");

    private Action<Context, Context>? parseContext = null;
    private bool autoParseContext = false;
    
    private static readonly Action<Context, Context> AUTO_PARSE = (cin, cout) => {
        for (int i = 0; i < cin.Length; i++) {
            var v = cin.GetAtIndex(i);
            cout[v.name].SetValue(v.value.GetStringValue());
        }
    };
    
    public SettingsGroupOption this[string name] {
        get {
            if (options is null) {
                throw new SettingsBuilderException("No options have been set.");
            }
            
            foreach (var o in options) {
                if (o.Name == name) return o;
            }
            
            throw new SettingsBuilderException($"Option with name '{name}' doesn't exist in group '{this.name}'.");
        }
    }

    /// <summary>
    /// creates a new instance of <see cref="SettingsGroup"/>
    /// </summary>
    /// <param name="name">name of the new group</param>
    /// <param name="outputContext">unified output context of the group</param>
    public static SettingsGroup New(string name, params (string name, ArgumentType.IArgumentType argument)[] outputContext) {

        if (outputContext == null)
            throw new SettingsBuilderException($"Output context is not initialized for group '{name}'.");
        
        SettingsGroup sg = new SettingsGroup(name) {
            groupContext = new Context()
        };
        
        sg.groupContext.AddRange(outputContext);
        
        return sg;
    }

    public static SettingsGroup New(string name, Context outputContext) {
        if (outputContext is null) {
            throw new SettingsBuilderException($"Output context is not initialized for group '{name}'.");
        }
        
        SettingsGroup sg = new SettingsGroup(name) {
            groupContext = outputContext
        };
        
        return sg;
    }
    
    /// <summary>
    /// basic constructor
    /// </summary>
    /// <param name="name">group name</param>
    /// <param name="outputContext">context that all available options must parse to</param>
    public SettingsGroup(string name, params (string name, ArgumentType.IArgumentType argumentType)[] outputContext) {
        this.name = name;

        this.groupContext ??= new Context();
        
        foreach ((string name, ArgumentType.IArgumentType argumentType) c in outputContext) {
            this.groupContext.Add(c.name, c.argumentType);
        }
    }
    
    /// <summary>
    /// private name constructor
    /// </summary>
    private SettingsGroup(string name) => this.name = name;

    /// <summary>
    /// adds an option  
    /// </summary>
    /// <param name="option">added option</param>
    public SettingsGroup Option(SettingsGroupOption option) {
        options ??= [];

        foreach (SettingsGroupOption o in options.Where(o => o.Name == option.Name)) {
            throw new SettingsBuilderException($"A settings option with the same ({o.Name}) name already exists.");
        }
        
        options.Add(option);
        
        return this;
    }


    /// <summary>
    /// sets the node context variables to group context variables 
    /// </summary>
    /// <param name="parse">parameter 1: group context, parameter 2: node context</param>
    public SettingsGroup OnParse(Action<Context, Context> parse) {
        parseContext = parse;
        return this;
    }

    /// <summary>
    /// enables auto parsing of context, matches context values with same name
    /// </summary>
    public SettingsGroup EnableAutoParse() {
        autoParseContext = true;
        return this;
    }

    /// <summary>
    /// parses the arguments to a context
    /// </summary>
    /// <exception cref="SettingsBuilderException">
    /// no available options exist, no option with provided name exists
    /// </exception>
    public void Execute(string optionName, in Context outputContext) {
        
        if (options == null)
            throw new SettingsBuilderException($"No options have been set for group '{name}'.");

        if (groupContext is null)
            throw new SettingsBuilderException($"Output context is not initialized for group '{name}'.");
        
        if (autoParseContext) {
            parseContext = AUTO_PARSE;
        }
        
        if (parseContext is null)
            throw new SettingsBuilderException($"No parsing method has been set for group '{name}'.");


        foreach (var o in options) {
            if (o.Name != optionName) continue;
            
            o.Execute(outputContext);
            parseContext(groupContext, outputContext);
            return;
        }
        
        throw new SettingsBuilderException($"Option '{optionName}' does not exist in group '{name}'.");
    }

    /// <summary>
    /// whether all properties have been properly initialized
    /// </summary>
    public bool IsFullyInitialized() => groupContext is { Length: > 0 } && options is { Count: > 0 };
    
    /// <summary>
    /// sets the value of an argument of an option
    /// </summary>
    public void SetValue(string optionName, string argumentName, object value) {
        if (options is null) {
            throw new SettingsBuilderException($"No options have been set in group '{name}'.");
        }
        
        foreach (var o in options) {
            if (o.Name == optionName) {
                o.SetValue(argumentName, value);
                return;
            }
        }
        
        throw new SettingsBuilderException($"No option named '{optionName}' found in group '{name}'.");
    }
    
    public object Clone() {
        SettingsGroup sg = new SettingsGroup(name) {
            parseContext = parseContext,
            groupContext = (Context?)groupContext?.Clone(),
            autoParseContext = autoParseContext
        };

        if (options == null) throw new SettingsBuilderException($"Options of group '{name}' cannot be uninitialized.");
        
        sg.options = new List<SettingsGroupOption>();
        sg.options.AddRange(options);
        
        return sg;
    }

    public override string ToString() {
        string output = $"{{\"name\": \"{name}\", \"outputContext\": [";

        if (groupContext != null) {
            for (int i = 0; i < groupContext.Length; i++) {
                var argument = groupContext.GetAtIndex(i); 
                output += $"{{\"name\": \"{argument.name}\", \"value\": {argument.value}}}" + 
                          (i == groupContext.Length - 1 ? "" : ", ");
            }
        }

        output += "], \"options\": [";
        
        if (options != null && options.Count != 0) {
            for (var i = 0; i < options.Count; i++) {
                output += options[i] + (i == options.Count - 1 ? "" : ", ");
            }
        }
        
        output += "]}";
        
        return output;
    }
}