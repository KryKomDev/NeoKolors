//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Exceptions;

public class SettingsGroupOptionException : Exception {
    private SettingsGroupOptionException(string message) : base(message) {}

    public static SettingsGroupOptionException ParseContextNotSet(string optionName) => 
        new($"Could not parse context of option '{optionName}'. Custom parsing function was not set.");

    public static SettingsGroupOptionException AutoParseContextException(string optionName, string contextMessage) => 
        new($"Could not auto-parse context of option '{optionName}'. {contextMessage}");
}