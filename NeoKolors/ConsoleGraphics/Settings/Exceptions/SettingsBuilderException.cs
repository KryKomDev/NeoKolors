//
// NeoKolors
// by KryKom 2024
//

namespace NeoKolors.ConsoleGraphics.Settings.Exceptions;

public class SettingsBuilderException : Exception {
    public SettingsBuilderException(string message) : base($"Failed to build settings. {message}") { }
}