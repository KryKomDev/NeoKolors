using NeoKolors.Settings.Argument;

namespace NeoKolors.Settings;

/// <summary>
/// factory methods for arguments, <see cref="IArgument{T}"/>
/// </summary>
public static class Arguments {
    
    public static BoolArgument Bool() => new();
    
    public static StringArgument String(uint minLength = 0,
        uint maxLength = UInt32.MaxValue,
        string defaultValue = "",
        bool allowSpaces = true,
        bool allowNewlines = true,
        bool allowSpecial = true,
        bool allowNumbers = true,
        bool allowUpper = true,
        bool allowLower = true,
        bool countVisibleOnly = true,
        Func<string, string?>? customValidate = null) => 
        new(minLength,
            maxLength,
            defaultValue,
            allowSpaces,
            allowNewlines,
            allowSpecial,
            allowNumbers,
            allowUpper,
            allowLower,
            countVisibleOnly,
            customValidate);
    
    public static IntegerArgument Integer(int min = int.MinValue,
        int max = int.MaxValue,
        int defaultValue = 0,
        Func<int, string?>? customValidate = null) => 
        new(min, 
            max,
            defaultValue,
            customValidate);
    
    public static UIntegerArgument UInteger(uint min = uint.MinValue,
        uint max = uint.MaxValue,
        uint defaultValue = 0,
        Func<uint, string?>? customValidate = null) => 
        new(min, 
            max, 
            defaultValue, 
            customValidate);
    
    public static LongArgument Long(long min = long.MinValue,
        long max = long.MaxValue,
        long defaultValue = 0,
        Func<long, string?>? customValidate = null) =>
        new(min,
            max,
            defaultValue,
            customValidate);

    public static ULongArgument ULong(ulong min = ulong.MinValue,
        ulong max = ulong.MaxValue,
        ulong defaultValue = 0,
        Func<ulong, string?>? customValidate = null) =>
        new(min,
            max,
            defaultValue,
            customValidate);
    
    public static FloatArgument Float(float min = float.MinValue,
        float max = float.MaxValue,
        float defaultValue = 0,
        Func<float, string?>? customValidate = null) =>
        new(min,
            max,
            defaultValue,
            customValidate);
    
    public static DoubleArgument Double(double min = double.MinValue,
        double max = double.MaxValue,
        double defaultValue = 0,
        Func<double, string?>? customValidate = null) =>
        new(min,
            max,
            defaultValue,
            customValidate);
}