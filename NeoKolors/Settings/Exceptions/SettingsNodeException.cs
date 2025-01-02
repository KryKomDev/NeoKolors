//
// NeoKolors
// Copyright (c) 2024 KryKom
//

namespace NeoKolors.Settings.Exceptions;

public class SettingsNodeException : Exception {
    private SettingsNodeException(string message) : base(message) { }

    public static SettingsNodeException NoResultConstructor(string nodeName) {
        return new SettingsNodeException($"Could not return result of settings node '{nodeName}'. Result builder is not initialized.");
    }
}