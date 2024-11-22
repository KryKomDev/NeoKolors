//
// NeoKolors
// by KryKom 2024
//

using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings.ArgumentType;

/// <summary>
/// Plain Text Argument Type <br/>
/// not actually an argument type, returns a static text
/// </summary>
internal class PlainTextArgumentType : IArgumentType {

    private readonly string text;

    public PlainTextArgumentType(string text) {
        this.text = text;
    }
    
    public string GetInputType() {
        return "Empty";
    }

    public string GetStringValue() {
        return text;
    }

    public object GetValue() {
        return text;
    }

    public void SetValue(object v) {
        throw new SettingsBuilderException("Cannot set empty argument type.");
    }

    public IArgumentType Clone() {
        return (IArgumentType)MemberwiseClone();
    }
}