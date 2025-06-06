//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Exception;

namespace NeoKolors.Settings;

public class SettingsGroupOption : ICloneable {
    public string Name { get; }
    public Context Context { get; }
    public Action<Context, Context>? CustomParseContext { get; private set; }
    public bool AutoParseContext { get; private set; }
    
    /// <summary>
    /// adds the <see cref="optionContext"/> to <see cref="groupContext"/>
    /// </summary>
    /// <exception cref="SettingsGroupOptionException">
    /// an argument from <see cref="optionContext"/> does not exist in the <see cref="groupContext"/>
    /// </exception>
    /// <seealso cref="NeoKolors.Settings.Context.Set(string,IArgument,bool)"/>
    private void AutoMerge(Context optionContext, Context groupContext) {
        try {
            groupContext.Set(optionContext);
        }
        catch (ContextException e) {
            throw SettingsGroupOptionException.AutoParseContextException(Name, e.Message);
        }
    }
    
    private SettingsGroupOption(string name) {
        Name = name;
        Context = new Context();
    }

    private SettingsGroupOption(string name, Context context, Action<Context, Context>? customParseContext, bool autoParseContext) {
        Name = name;
        Context = (Context)context.Clone();
        CustomParseContext = customParseContext;
        AutoParseContext = autoParseContext;
    }
        

    /// <summary>
    /// creates a new settings group option
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static SettingsGroupOption New(string name) => new(name);

    /// <summary>
    /// adds a new argument to the option context
    /// </summary>
    public SettingsGroupOption Argument(string name, IArgument argument) {
        Context.Add(name, argument);
        return this;
    }

    /// <summary>
    /// sets a parsing function that transfers the context of this option to the parent group's context
    /// </summary>
    /// <param name="parseContext">
    /// first argument represents this option's context,
    /// second is the parent group's context
    /// </param>
    public SettingsGroupOption Merges(Action<Context, Context> parseContext) {
        CustomParseContext = parseContext;
        AutoParseContext = false;
        return this;
    }

    /// <summary>
    /// enables context auto-parsing
    /// </summary>
    /// <seealso cref="AutoMerge"/>
    public SettingsGroupOption EnableAutoMerge() {
        AutoParseContext = true;
        return this;
    }

    /// <summary>
    /// merges the contexts of this option and its parent group
    /// </summary>
    public void MergeContext(in Context groupContext) {
        if (AutoParseContext) {
            AutoMerge(Context, groupContext);
        }
        else {
            if (CustomParseContext is null) throw SettingsGroupOptionException.ParseContextNotSet(Name);
            CustomParseContext(Context, groupContext);
        }
    }

    public object Clone() => new SettingsGroupOption(Name, Context, CustomParseContext, AutoParseContext);
    
    public override string ToString() => 
        $"{{\"name\": \"{Name}\", " +
        $"\"context\": {Context}," +
        $"\"auto-merge\": {AutoParseContext}}}";
}