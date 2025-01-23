# NeoKolors.Settings
this package includes a settings building system for easier automation of instance building

## Usage
this section will explain on an example how the system works step by step

---

### Example
Let's say you have a class `C` and you want to create a settings that will output an instance of it.
`C` has an `Int32` field `value` whose allowed values are >= -10 and <= 10.

```csharp
var builder = SettingsBuidler<C>.Build("c-builder",
    SettingsNode<C>.New("default")
        .Argument("field", Arguments.Integer(-10, 10))
        .Constructs(context => new C((int)context["field"].Get()))
);

var result = builder.GetResult();
```

In the example above we created a `SettingsBuilder` that outputs `C`. As you can see `builder`
has a `SettingsNode` inside of the same type. `SettingsNode` named `"default"` contains an argument 
and a result constructor. The argument named `"field"` is of type integer and its value can range from -10
up to 10 (same as values of `C.value`). The result constructor is a `Func<Context, TResult>` (in our case 
`TResult` is `C`) that uses the constructor of `C` and `Context` to output a new instance with the `value` 
field set.

---

### SettingsNode
Settings node can have arguments, and groups. You have already seen the creation of a single argument 
(more types in the [Arguments](#arguments) section). `SettingsBuilder`s can contain more than just one 
`SettingsNode`. You can have how many `SettingsNode`s as you want, but too many nodes can result in 
confusion. Instead of a large amount of nodes with very similar inputs you can create a group and let
the user decide which type of input they want to use (example in the [SettingsGroup](#settingsgroup)
section). 

---

### Arguments
As of now NeoKolors.Settings contains 8 argument types all stored in [ArgumentTypes](ArgumentTypes) 
directory.

The types contain:
* `bool` → [BoolArgument](ArgumentTypes/BoolArgument.cs)
* `double` → [DoubleArgument](ArgumentTypes/DoubleArgument.cs)
* `float` → [FloatArgument](ArgumentTypes/FloatArgument.cs)
* `int` → [IntegerArgument](ArgumentTypes/IntegerArgument.cs)
* `uint` → [UIntegerArgument](ArgumentTypes/IntegerArgument.cs)
* `long` → [LongArgument](ArgumentTypes/LongArgument.cs)
* `ulong` → [ULongArgument](ArgumentTypes/LongArgument.cs)
* `string` → [StringArgument](ArgumentTypes/StringArgument.cs)
* single selection → [SingleSelectArgument](ArgumentTypes/SingleSelectArgument.cs)
* multiple selection → [MultiSelectArgument](ArgumentTypes/MultiSelectArgument.cs)
* path -> [PathArgument](ArgumentTypes/PathArgument.cs)

All these basic arguments can be created using factory methods from [Arguments.cs](Arguments.cs)

---

### SettingsGroup
Groups are a way to let the user decide how to input a set of values into the settings.

An example could be authentication: 
```csharp
class Auth
{
    public string Name { get; }
    public string Passcode { get; }
    
    public Auth(string name, string passcode) 
    {
        Name = name;
        Passcode = passcode;
    }
}

var builder = SettingsBuilder<Auth>.Build("auth",
    SettingsNode<Auth>.New("default")
        .Argument("name", Arguments.String())
        .Group(SettingsGroup
            .New("passcode", 
                ("passcode", Arguments.String()))
            .Option(SettingsGroupOption
                .New("pin")
                .Argument("pin", Arguments.Int(0, 9999))
                .Merges((cin, cout) => 
                {
                    cout["passcode"] <<= cin["pin"].Get().ToString();
                })
            )
            .Option(SettingsGroupOption
                .New("password")
                .Argument("password", Arguments.String())
                .Merges((cin, cout) => 
                {
                    cout["passcode"] <<= cin["password"].Get();
                })
            )
            .EnableAutoMerge()
        )
        .Constructs(context => 
        {
            return new Auth((string)context["name"].Get(), (string)context["passcode"].Get());
        })
);
```

As you can see we created a group named passcode that contains two options, pin and password, 
that are then merged to the group context from where it is automatically merged to the node's context. 