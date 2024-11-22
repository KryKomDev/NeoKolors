//
// NeoKolors
// by KryKom 2024
//

namespace NeoKolors.ConsoleGraphics.Settings;

public interface ISettingsNode<out TResult, in TSource> {
    string Name { get; }
    public ISettingsNode<TResult, TSource> Argument(string argumentName, ArgumentType.IArgumentType argumentType);
    public ISettingsNode<TResult, TSource> Group(SettingsGroup group);
    public ISettingsNode<TResult, TSource> OnParse(Func<Context, TSource> execute);
    public TResult Execute(params (string groupName, string optionName)[] config);
    public ISettingsNode<TResult, TSource> Clone();
    
    internal static SettingsNode<T> Empty<T>() {
        SettingsNode<T> empty = SettingsNode<T>.New("empty");
        empty.Argument("empty", Arguments.
            PlainText("This node is empty. " +
                      "For more information watch this video: https://youtu.be/dQw4w9WgXcQ?si=7aQZOGrCBTLmLRih"));
        
        return empty;
    }
}