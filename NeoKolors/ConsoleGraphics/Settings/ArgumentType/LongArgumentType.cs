//
// NeoKolors
// by KryKom 2024
//

using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings.ArgumentType;

/// <summary>
/// Int64 Argument Type
/// </summary>
public sealed class LongArgumentType : IArgumentType {
    
    private long? value;
    public long Min { get; }
    public long Max { get; }
    private long defaultValue;

    internal LongArgumentType(long min = Int64.MinValue, long max = Int64.MaxValue, long defaultValue = 0) {
        Min = min;
        Max = max;
        this.defaultValue = defaultValue;
        value = defaultValue;
    }
    
    public string GetInputType() {
        return "Long";
    }

    public string GetStringValue() {
        if (value == null) throw new SettingsBuilderException("Accessing value that has not been set.");

        return ((int)value).ToString();
    }

    public object GetValue() {
        if (value == null) throw new SettingsBuilderException("Accessing value that has not been set.");

        return value;
    }

    public void SetValue(object v) {
        if (v.GetType() != typeof(long)) throw new SettingsArgumentException(v.GetType(), typeof(long));
        if ((long)v < Min || (long)v > Max) 
            throw new SettingsArgumentException($"Value of {v} must be greater than {Min} and smaller than {Max}.");

        value = (long)v;
    }

    public IArgumentType Clone() {
        LongArgumentType clone = new LongArgumentType(Min, Max) {
            value = value
        };
        
        return clone;
    }

    public void Reset() {
        value = defaultValue;
    }

    public override string ToString() {
        return $"{{\"type\": \"long\", " +
               $"\"min\": {Min}, " +
               $"\"max\": {Max}, " +
               $"\"value\": {(value == null ? "\"null\"" : value)}}}}}";
    }
}


/// <summary>
/// Unsigned Int64 Argument Type
/// </summary>
public sealed class UnsignedLongArgumentType : IArgumentType {

    private ulong? value;
    public ulong Min { get; }
    public ulong Max { get; }
    private ulong defaultValue;

    internal UnsignedLongArgumentType(ulong min = ulong.MinValue, ulong max = ulong.MaxValue, ulong defaultValue = 0) {
        Min = min;
        Max = max;
        this.defaultValue = defaultValue;
        value = defaultValue;
    }
    
    public string GetInputType() {
        return "ULong";
    }

    public string GetStringValue() {
        if (value == null) throw new SettingsBuilderException("Accessing value that has not been set.");

        return ((int)value).ToString();
    }

    public object GetValue() {
        if (value == null) throw new SettingsBuilderException("Accessing value that has not been set.");

        return value;
    }

    public void SetValue(object v) {
        if (v.GetType() != typeof(ulong)) throw new SettingsArgumentException(v.GetType(), typeof(ulong));
        if ((ulong)v < Min || (ulong)v > Max) 
            throw new SettingsArgumentException($"Value of {v} must be greater than {Min} and smaller than {Max}.");
        
        value = (ulong)v;
    }
    
    public IArgumentType Clone() {
        UnsignedLongArgumentType clone = new UnsignedLongArgumentType(Min, Max) {
            value = value
        };
        
        return clone;
    }

    public void Reset() {
        value = defaultValue;
    }

    public override string ToString() {
        return $"{{\"type\": \"ulong\", " +
               $"\"min\": {Min}, " +
               $"\"max\": {Max}, " +
               $"\"value\": {(value == null ? "\"null\"" : value)}}}}}";
    }
}