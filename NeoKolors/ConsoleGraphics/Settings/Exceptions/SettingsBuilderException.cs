//
// NeoKolors
// Copyright (c) 2024 KryKom
//

namespace NeoKolors.ConsoleGraphics.Settings.Exceptions;

public class SettingsBuilderException : Exception {
    public SettingsBuilderException(string message) : base($"Failed to build settings. {message}") { }
}