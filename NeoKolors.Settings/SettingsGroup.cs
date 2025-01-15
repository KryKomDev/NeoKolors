//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.ArgumentTypes;
using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings;

public class SettingsGroup : ICloneable {
    public string Name { get; }
    public Context GroupContext { get; }
    private readonly List<SettingsGroupOption> options;
    public List<SettingsGroupOption> Options {
        get {
            if (options.Count < 1) throw SettingsGroupException.NoOptionsAvailable(Name);
            return options;
        }
    }
    public Action<Context, Context>? CustomParseContext { get; private set; }
    public bool AutoParseContext { get; private set; } = true;
    public OptionSwitch OptionSwitch { get; private set; }
    

    /// <summary>
    /// automatically adds group context arguments to node context arguments
    /// </summary>
    private void AutoMerge(in Context nodeContext) {
        try {
            nodeContext.Add(GroupContext);
        }
        catch (ContextException e) {
            throw SettingsGroupException.AutoParseContextException(Name, e.Message);
        }
    }
    
    public SettingsGroupOption this[string name] {
        get {
            foreach (var o in Options) if (o.Name == name) return o;
            throw SettingsGroupException.InvalidOptionName(name);
        }
    }
    
    
    private SettingsGroup(string name,
        Context groupContext,
        List<SettingsGroupOption> options,
        OptionSwitch optionSwitch,
        Action<Context, Context>? customParseContext, 
        bool autoParseContext) 
    {
        Name = name;
        GroupContext = (Context)groupContext.Clone();
        this.options = options.Select(o => (SettingsGroupOption)o.Clone()).ToList();
        OptionSwitch = optionSwitch;
        CustomParseContext = customParseContext;
        AutoParseContext = autoParseContext;
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
        options = new List<SettingsGroupOption>();
        OptionSwitch = new OptionSwitch();
    }

    
    /// <summary>
    /// adds an option to the group
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    /// <exception cref="SettingsGroupException"></exception>
    public SettingsGroup Option(SettingsGroupOption option) {
        foreach (var o in options) {
            if (o.Name == option.Name) {
                throw SettingsGroupException.DuplicateOption(option.Name);
            }
        }
        
        options.Add(option);
        OptionSwitch.Add(option);

        return this;
    }

    
    /// <summary>
    /// sets a custom context parsing delegate
    /// </summary>
    public SettingsGroup Merges(Action<Context, Context> parse) {
        AutoParseContext = false;
        CustomParseContext = parse;
        return this;
    }

    
    /// <summary>
    /// enables auto parsing of the group context <seealso cref="AutoMerge"/>
    /// </summary>
    public SettingsGroup EnableAutoMerge() {
        AutoParseContext = true;
        return this;
    }

    /// <summary>
    /// merges context of this group to context of its parent node
    /// </summary>
    /// <exception cref="SettingsGroupException">parsing delegate is not set and auto-parsing is disabled</exception>
    public void MergeContext(in Context nodeContext) {

        SettingsGroupOption o = Options[OptionSwitch.Index];
        o.MergeContext(GroupContext);
        
        if (AutoParseContext) {
            AutoMerge(nodeContext);
        }
        else {
            if (CustomParseContext is null) throw SettingsGroupException.ParseDelegateNotSet(Name);
            
            CustomParseContext(GroupContext, nodeContext);
        }
    }

    /// <summary>
    /// selects an input option using index
    /// </summary>
    public void Select(int index) => OptionSwitch = OptionSwitch.Select(index);

    /// <summary>
    /// selects an input option using the name of the option
    /// </summary>
    public void Select(string name) => OptionSwitch = OptionSwitch.Select(name);

    public object Clone() {
        return new SettingsGroup(Name, GroupContext, Options, OptionSwitch, CustomParseContext, AutoParseContext);
    }
}