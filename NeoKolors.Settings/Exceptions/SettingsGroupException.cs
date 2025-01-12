//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Exceptions;

public class SettingsGroupException : Exception {
    private SettingsGroupException(string message) : base(message) { }

    /// <summary>
    /// invalid option name was supplied to the <see cref="OptionSwitch"/>
    /// </summary>
    public static SettingsGroupException SwitchInvalidOptionName(string optionName) => 
        new($"Could not switch to node of name '{optionName}'. Option with that name doesn't exist.");

    /// <summary>
    /// invalid option index was supplied to the <see cref="OptionSwitch"/>
    /// </summary>
    public static SettingsGroupException SwitchOptionIndexOutOfRange(int index, int length) =>
        new($"Could not switch to node of index {index}. Index out of range. " +
            $"Index must be greater than or equal to 0 and less than {length}.");

    /// <summary>
    /// no options were set in a group
    /// </summary>
    public static SettingsGroupException NoOptionsAvailable(string groupName) => 
        new($"No options are available in the group '{groupName}'. Group must contain at least one option.");

    /// <summary>
    /// an option with the supplied name does not exist
    /// </summary>
    public static SettingsGroupException InvalidOptionName(string optionName) => 
        new($"Could not return option of name '{optionName}'. Option with that name doesn't exist.");

    /// <summary>
    /// a <see cref="SettingsGroupOption"/> with the same name already exists in the same <see cref="SettingsGroup"/>
    /// </summary>
    public static SettingsGroupException DuplicateOption(string optionName) => 
        new($"Could not add option '{optionName}' to the group. Option with the same name already exists.");

    /// <summary>
    /// <see cref="SettingsGroup.GroupContext"/> was not initialized properly
    /// </summary>
    public static SettingsGroupException UninitializedContext(string groupName) => 
        new($"Context of group '{groupName}' is not initialized. Context must have at leas one element to count as initialized.");

    /// <summary>
    /// <see cref="SettingsGroup.CustomParseContext"/> was not set while <see cref="SettingsGroup.AutoParseContext"/> was disabled
    /// </summary>
    public static SettingsGroupException ParseDelegateNotSet(string groupName) =>
        new($"Could not parse delegate for group '{groupName}'. Parse delegate must be set when AutoParseContext is disabled. " +
            $"To set the delegate call Merges(Action<Context, Context>) on the group.");
    
    /// <summary>
    /// auto-parsing of group context failed, for more see <see cref="Context.Add(Context)"/>
    /// </summary>
    public static SettingsGroupException AutoParseContextException(string groupName, string contextMessage) => 
        new($"Could not auto-parse context of group '{groupName}'. {contextMessage}");
}