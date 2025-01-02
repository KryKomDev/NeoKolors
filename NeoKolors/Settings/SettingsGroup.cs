//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings;

public class SettingsGroup {
    public string Name { get; }
    public Context GroupContext { get; }
    public List<SettingsGroupOption> Options { get { if (field.Count < 1) throw SettingsGroupException.NoOptionsAvailable(Name); return field; } }
    public Action<Context, Context>? CustomParseContext { get; private set; }
    public bool AutoParseContext { get; private set; }
    public OptionSwitch OptionSwitch { get; private set; }
    
    /// <summary>
    /// automatically adds group context arguments to node context arguments
    /// </summary>
    private static readonly Action<Context, Context> AUTO_PARSE = (cin, cout) => {
        for (int i = 0; i < cin.Length; i++) {
            var v = cin.GetAtIndex(i);
            cout.Set(v.name, v.value, true);
        }
    };
    
    
    public SettingsGroupOption this[string name] {
        get {
            foreach (var o in Options) if (o.Name == name) return o;
            throw SettingsGroupException.InvalidOptionName(name);
        }
    }
    
    
    /// <summary>
    /// creates a new instance of <see cref="SettingsGroup"/>
    /// </summary>
    /// <param name="name">name of the new group</param>
    /// <param name="outputContext">unified output context of the group</param>
    public static SettingsGroup New(string name, params (string name, IArgument argument)[] outputContext) {
        if (outputContext is null || outputContext.Length < 1) 
            throw SettingsGroupException.UninitializedContext(name);
        
        SettingsGroup sg = new SettingsGroup(name, new Context(outputContext));
        return sg;
    }

    
    /// <summary>
    /// creates a new instance of <see cref="SettingsGroup"/>
    /// </summary>
    /// <param name="name">name of the group</param>
    /// <param name="outputContext">unified group context</param>
    /// <exception cref="SettingsGroupException">context is not properly initialized</exception>
    public static SettingsGroup New(string name, Context outputContext) {
        if (outputContext is null || outputContext.Length < 1)
            throw SettingsGroupException.UninitializedContext(name);
            
        SettingsGroup sg = new SettingsGroup(name, outputContext);
        return sg;
    }
    
    
    /// <summary>
    /// private constructor
    /// </summary>
    private SettingsGroup(string name, Context groupContext) {
        Name = name;
        GroupContext = groupContext;
        Options = new List<SettingsGroupOption>();
    }

    
    /// <summary>
    /// adds an option to the group
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    /// <exception cref="SettingsGroupException"></exception>
    public SettingsGroup Option(SettingsGroupOption option) {
        foreach (var o in Options) {
            if (o.Name == option.Name) {
                throw SettingsGroupException.DuplicateOption(option.Name);
            }
        }
        
        Options.Add(option);

        return this;
    }

    
    /// <summary>
    /// sets a custom context parsing delegate
    /// </summary>
    public SettingsGroup OnParse(Action<Context, Context> parse) {
        AutoParseContext = false;
        CustomParseContext = parse;
        return this;
    }

    
    /// <summary>
    /// enables auto parsing of the group context, <seealso cref="AUTO_PARSE"/>
    /// </summary>
    public SettingsGroup EnableAutoParse() {
        AutoParseContext = true;
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodeContext"></param>
    /// <exception cref="SettingsGroupException"></exception>
    public void MergeContext(in Context nodeContext) {

        SettingsGroupOption o = Options[OptionSwitch.Index];
        o.MergeContext(GroupContext);
        
        if (AutoParseContext) {
            AUTO_PARSE(GroupContext, nodeContext);
        }
        else {
            if (CustomParseContext is null) throw SettingsGroupException.ParseDelegateNotSet(Name);
            
            CustomParseContext(GroupContext, nodeContext);
        }
    }
}