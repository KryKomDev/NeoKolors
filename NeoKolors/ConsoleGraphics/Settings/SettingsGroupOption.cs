//
// NeoKolors
// by KryKom 2024
//

using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings;

/// <summary>
/// single option of input method of settings
/// </summary>
public sealed class SettingsGroupOption : ICloneable {
    private string name { get; }
    public string Name => name;
    public Context context { get; init; } = new();
    public Action<Context, Context>? parseContext { get; private set; } = null;
    private bool autoParseContext { get; set; } = false;
    
    private static readonly Action<Context, Context> AUTO_PARSE = (cin, cout) => {
        for (int i = 0; i < cin.Length; i++) {
            var v = cin.GetAtIndex(i);
            cout[v.name].SetValue(v.value.GetStringValue());
        }
    };

    
    /// <summary>
    /// creates a new instance
    /// </summary>
    public static SettingsGroupOption New(string name) {
        return new SettingsGroupOption(name);
    }
    
    private SettingsGroupOption(string name) => this.name = name;

    /// <summary>
    /// adds a new argument
    /// </summary>
    /// <param name="name">name of the argument</param>
    /// <param name="argument">argument type</param>
    public SettingsGroupOption Argument(string name, ArgumentType.IArgumentType argument) {
        context.Add(name, argument);
        return this;
    }

    /// <summary>
    /// describes how to convert arguments from this option to shared group context
    /// </summary>
    /// <param name="parse">parameter 1: option context, parameter 2: shared group context</param>
    public SettingsGroupOption OnParse(Action<Context, Context> parse) {
        autoParseContext = false;
        parseContext = parse;
        return this;
    }

    /// <summary>
    /// enables auto parsing of context, matches context values with same name
    /// </summary>
    public SettingsGroupOption EnableAutoParse() {
        autoParseContext = true;
        return this;
    }

    /// <summary>
    /// parses the option context into the shared group context  
    /// </summary>
    /// <exception cref="SettingsBuilderException">
    /// no parsing delegate has been provided through the <see cref="OnParse"/> method
    /// </exception>
    public void Execute(in Context context) {

        if (autoParseContext) {
            parseContext = AUTO_PARSE;
        }
        
        if (parseContext == null) {
            throw new SettingsBuilderException(
                "Parsing delegate has not been provided. Use OnParse(Action<Context,Context> parse) method to do so.");
        }
        
        parseContext(this.context, context);
    }

    /// <summary>
    /// whether the option is fully initialized
    /// </summary>
    public bool IsFullyInitialized() {
        return parseContext != null;
    }
    
    /// <summary>
    /// sets the value of an argument
    /// </summary>
    public void SetValue(string argumentName, object value) {
        context[argumentName].SetValue(value);
    }
    
    public object Clone() {
        SettingsGroupOption sgo = new SettingsGroupOption(name) {
            parseContext = parseContext,
            context = (Context)context.Clone(),
            autoParseContext = autoParseContext
        };
        
        return sgo;
    }

    public override string ToString() {
        string output = $"{{\"name\": \"{name}\", \"context\": [";

        for (int i = 0; i < context.Length; i++) {
            (string name, ArgumentType.IArgumentType value) argument = context.GetAtIndex(i); 
            output += $"{{\"name\": \"{argument.name}\", \"value\": {argument.value}}}" + 
                      (i == context.Length - 1 ? "" : ", ");
        }
        
        output += "]}";
        
        return output;
    }
}