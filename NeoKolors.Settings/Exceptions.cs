//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exception;

namespace NeoKolors.Settings;

public static class Exceptions {
    
    /// <summary>
    /// argument input was in invalid format
    /// </summary>
    /// <param name="targetType">target type stored by the argument</param>
    /// <param name="sourceValue">input value</param>
    /// <param name="message">message produced by the input parser</param>
    public static ArgumentInputFormatException
        ArgumentInputFormat(Type targetType, string sourceValue, string message) =>
        new(targetType, sourceValue, message);

    /// <summary>
    /// a supplied key was not found in the context
    /// </summary>
    /// <param name="key">the supplied key</param>
    public static ContextException ContextKeyNotFound(string key) => ContextException.KeyNotFound(key);

    /// <summary>
    /// tried to add a key that already exists in the context
    /// </summary>
    /// <param name="key">the supplied duplicate key</param>
    public static ContextException ContextKeyDuplicate(string key) => ContextException.KeyDuplicate(key);

    /// <summary>
    /// tried to access an index that is out of range
    /// </summary>
    /// <param name="index">supplied index</param>
    /// <param name="length">count of arguments in the context</param>
    public static ContextException ContextOutOfRange(int index, int length) => ContextException.OutOfRange(index, length);

    /// <summary>
    /// argument input was invalid
    /// </summary>
    /// <param name="cause">cause of the exception</param>
    public static InvalidArgumentInputException ArgumentInput(string cause) => new(cause);

    /// <summary>
    /// argument input was of invalid type
    /// </summary>
    /// <param name="expected">expected type</param>
    /// <param name="actual">actual type</param>
    public static InvalidArgumentInputTypeException ArgumentInputType(Type expected, Type actual) =>
        new(expected, actual);

    /// <summary>
    /// tried to switch to a node that doesn't exist
    /// </summary>
    /// <param name="name">supplied name</param>
    public static SettingsBuilderException SwitchInvalidNodeName(string name) =>
        SettingsBuilderException.SwitchInvalidNodeName(name);

    /// <summary>
    /// tried to switch to a node using index that is out of range
    /// </summary>
    /// <param name="index">supplied index</param>
    /// <param name="length">count of nodes in the builder</param>
    public static SettingsBuilderException SwitchNodeIndexOutOfRange(int index, int length) =>
        SettingsBuilderException.SwitchNodeIndexOutOfRange(index, length);

    /// <summary>
    /// tried to access a node that doesn't exist
    /// </summary>
    /// <param name="name">supplied name</param>
    public static SettingsBuilderException InvalidNodeName(string name) =>
        SettingsBuilderException.InvalidNodeName(name);

    /// <summary>
    /// tried to access a node using index that is out of range
    /// </summary>
    /// <param name="index">supplied index</param>
    /// <param name="length">count of nodes in the builder</param>
    public static SettingsBuilderException NodeIndexOutOfRange(int index, int length) =>
        SettingsBuilderException.NodeIndexOutOfRange(index, length);
    
    /// <summary>
    /// tried to switch to an option that doesn't exist
    /// </summary>
    /// <param name="optionName">supplied name</param>
    public static SettingsGroupException SwitchInvalidOptionName(string optionName) =>
        SettingsGroupException.SwitchInvalidOptionName(optionName);
    
    /// <summary>
    /// tried to switch to an option using index that is out of range
    /// </summary>
    /// <param name="index">supplied index</param>
    /// <param name="length">count of options in the group</param>
    public static SettingsGroupException SwitchOptionIndexOutOfRange(int index, int length) =>
        SettingsGroupException.SwitchOptionIndexOutOfRange(index, length);

    /// <summary>
    /// no available options in the group
    /// </summary>
    /// <param name="groupName">name of the empty group</param>
    public static SettingsGroupException NoOptionsAvailable(string groupName) =>
        SettingsGroupException.NoOptionsAvailable(groupName);

    /// <summary>
    /// tried to access an option that doesn't exist
    /// </summary>
    /// <param name="optionName">supplied name</param>
    public static SettingsGroupException InvalidOptionName(string optionName) =>
        SettingsGroupException.InvalidOptionName(optionName);

    /// <summary>
    /// tried to add an option that already exists in the group
    /// </summary>
    /// <param name="optionName">supplied name</param>
    public static SettingsGroupException DuplicateOption(string optionName) =>
        SettingsGroupException.DuplicateOption(optionName);

    /// <summary>
    /// context of the group is not initialized
    /// </summary>
    /// <param name="groupName">name of the group</param>
    public static SettingsGroupException GroupUninitializedContext(string groupName) =>
        SettingsGroupException.UninitializedContext(groupName);

    /// <summary>
    /// context merging delegate was not set in a group
    /// </summary>
    /// <param name="groupName">name of the group</param>
    public static SettingsGroupException GroupMergeDelegateNotSet(string groupName) =>
        SettingsGroupException.ParseDelegateNotSet(groupName);
    
    /// <summary>
    /// an error occurred while auto-merging the context of a group
    /// </summary>
    /// <param name="groupName">name of the group</param>
    /// <param name="contextMessage">message of the error</param>
    public static SettingsGroupException GroupAutoMergeContext(string groupName, string contextMessage) =>
        SettingsGroupException.AutoParseContextException(groupName, contextMessage);

    /// <summary>
    /// context merging delegate was not set in a group
    /// </summary>
    /// <param name="optionName">name of the option</param>
    public static SettingsGroupOptionException OptionMergeDelegateNotSet(string optionName) =>
        SettingsGroupOptionException.ParseContextNotSet(optionName);

    /// <summary>
    /// an error occurred while auto-merging the context of an option
    /// </summary>
    /// <param name="optionName">name of the group</param>
    /// <param name="contextMessage">message of the error</param>
    public static SettingsGroupOptionException OptionAutoMergeContext(string optionName, string contextMessage) =>
        SettingsGroupOptionException.AutoParseContextException(optionName, contextMessage);
    
    /// <summary>
    /// no result constructor has been set in a node
    /// </summary>
    /// <param name="nodeName">name of the node</param>
    public static SettingsNodeException NoResultConstructor(string nodeName) =>
        SettingsNodeException.NoResultConstructor(nodeName);

    /// <summary>
    /// a node returns an invalid result type
    /// </summary>
    /// <param name="nodeName">name of the node</param>
    /// <param name="nodeResultType">expected result type of the node</param>
    /// <param name="actualResultType">actual result type</param>
    public static SettingsNodeException InvalidResultType(string nodeName, Type nodeResultType, Type actualResultType) =>
        SettingsNodeException.InvalidResultType(nodeName, nodeResultType, actualResultType);

    /// <summary>
    /// a node returns an invalid result type
    /// </summary>
    /// <param name="nodeName">name of the node</param>
    /// <param name="nodeResultType">expected result type of the node</param>
    public static SettingsNodeException InvalidResultType(string nodeName, Type nodeResultType) =>
        SettingsNodeException.InvalidResultType(nodeName, nodeResultType);
    
    /// <summary>
    /// tried to access a group that does not exist in the node
    /// </summary>
    /// <param name="nodeName">name of the node</param>
    /// <param name="groupName">supplied group name</param>
    public static SettingsNodeException InvalidGroupName(string nodeName, string groupName) =>
        SettingsNodeException.InvalidGroupName(nodeName, groupName);
}