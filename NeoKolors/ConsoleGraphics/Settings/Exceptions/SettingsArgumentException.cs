//
// NeoKolors
// by KryKom 2024
//

namespace NeoKolors.ConsoleGraphics.Settings.Exceptions;

public class SettingsArgumentException : Exception {
    public SettingsArgumentException(ArgumentType.IArgumentType argumentType, string parserMessage) : 
        base($"Could not parse argument of type {argumentType.GetInputType()}. {parserMessage}") { }
    
    public SettingsArgumentException(string message) : 
        base($"Could not set value to argument. Source value does not match the rules of the argument. {message}") { }
    
    public SettingsArgumentException(ArgumentType.IArgumentType type1, ArgumentType.IArgumentType type2) : 
        base($"Could not copy argument value. Argument types must match. " +
             $"Instead they were {type1.GetType().Name}, {type2.GetType().Name}.") { }

    public SettingsArgumentException(Type type1, Type type2) :
        base($"Could not set value to argument. Value types must match. " +
             $"Instead they were {type1.Name}, {type2.Name}.") { }
}