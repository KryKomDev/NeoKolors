//
// NeoKolors
// by KryKom 2024
//

namespace NeoKolors.ConsoleGraphics.Settings.ArgumentType;

/// <summary>
/// Argument Type Interface <br/>
/// used in settings
/// </summary>
public interface IArgumentType {
    
    /// <summary>
    /// returns a string with the type's name
    /// </summary>
    public string GetInputType();

    /// <summary>
    /// returns the stringified value stored in an argument
    /// </summary>
    public string GetStringValue();
    
    /// <summary>
    /// returns the raw value stored in an argument
    /// </summary>
    public object GetValue();

    /// <summary>
    /// sets the value stored in an argument from a raw value
    /// </summary>
    public void SetValue(object v);

    /// <summary>
    /// clones the argument (all its field including its value)
    /// </summary>
    public IArgumentType Clone();
}