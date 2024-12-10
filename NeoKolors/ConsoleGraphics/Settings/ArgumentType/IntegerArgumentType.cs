//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings.ArgumentType;

/// <summary>
/// Int32 Argument Type
/// </summary>
public sealed class IntegerArgumentType : IArgumentType {

    private int value;
    public int Min { get; }
    public int Max { get; }
    private readonly int defaultValue;

    internal IntegerArgumentType(int min = int.MinValue, int max = int.MaxValue, int defaultValue = 0) {
        Min = min;
        Max = max;
        this.defaultValue = defaultValue;
        value = defaultValue;
    }
    
    public string GetInputType() {
        return "Integer";
    }

    public string GetStringValue() {
        return value.ToString();
    }

    public object GetValue() {
        return value;
    }

    public void SetValue(object v) {
        if (v.GetType() != typeof(int)) throw new SettingsArgumentException(v.GetType(), typeof(int));
        if ((int)v < Min || (int)v > Max) 
            throw new SettingsArgumentException($"Value of {v} must be greater than {Min} and smaller than {Max}.");

        value = (int)v;
    }

    public IArgumentType Clone() {
        return (IArgumentType)MemberwiseClone();
    }

    public void Reset() {
        value = defaultValue;
    }

    public override string ToString() {
        return $"{{\"type\": \"int\", " +
               $"\"min\": {Min}, " +
               $"\"max\": {Max}, " +
               $"\"value\": {value}}}";
    }
    
    public static implicit operator int(IntegerArgumentType arg) => arg.value;
    public static int operator +(IntegerArgumentType a, IntegerArgumentType b) => a.value + b.value;
    public static int operator -(IntegerArgumentType a, IntegerArgumentType b) => a.value - b.value;
    public static int operator *(IntegerArgumentType a, IntegerArgumentType b) => a.value * b.value;
    public static int operator /(IntegerArgumentType a, IntegerArgumentType b) => a.value / b.value;
    public static int operator %(IntegerArgumentType a, IntegerArgumentType b) => a.value % b.value;

    public static IntegerArgumentType operator ++(IntegerArgumentType arg) {
        arg.value++;
        return arg;
    }

    public static IntegerArgumentType operator --(IntegerArgumentType arg) {
        arg.value--;
        return arg;
    }
}


/// <summary>
/// Unsigned Int32 Argument Type
/// </summary>
public sealed class UnsignedIntegerArgumentType : IArgumentType {

    private uint value;
    public uint Min { get; }
    public uint Max { get; }
    private readonly uint defaultValue;

    internal UnsignedIntegerArgumentType(uint min = uint.MinValue, uint max = uint.MaxValue, uint defaultValue = 0) {
        Min = min;
        Max = max;
        this.defaultValue = defaultValue;
        value = defaultValue;
    }
    
    public string GetInputType() {
        return "UInteger";
    }

    public string GetStringValue() {
        return ((int)value).ToString();
    }

    public object GetValue() {
        return value;
    }

    public void SetValue(object v) {
        if (v.GetType() != typeof(uint)) throw new SettingsArgumentException(v.GetType(), typeof(uint));
        if ((uint)v < Min || (uint)v > Max) 
            throw new SettingsArgumentException($"Value of {v} must be greater than {Min} and smaller than {Max}.");
        
        value = (uint)v;
    }

    public IArgumentType Clone() {
        return (IArgumentType)MemberwiseClone();
    }

    public void Reset() {
        value = defaultValue;
    }

    public override string ToString() {
        return $"{{\"type\": \"uint\", " +
               $"\"min\": {Min}, " +
               $"\"max\": {Max}, " +
               $"\"value\": {value}}}";
    }
    
    public static implicit operator uint(UnsignedIntegerArgumentType arg) => arg.value;
    public static uint operator +(UnsignedIntegerArgumentType a, UnsignedIntegerArgumentType b) => a.value + b.value;
    public static uint operator -(UnsignedIntegerArgumentType a, UnsignedIntegerArgumentType b) => a.value - b.value;
    public static uint operator *(UnsignedIntegerArgumentType a, UnsignedIntegerArgumentType b) => a.value * b.value;
    public static uint operator /(UnsignedIntegerArgumentType a, UnsignedIntegerArgumentType b) => a.value / b.value;
    public static uint operator %(UnsignedIntegerArgumentType a, UnsignedIntegerArgumentType b) => a.value % b.value;

    public static UnsignedIntegerArgumentType operator ++(UnsignedIntegerArgumentType arg) {
        arg.value++;
        return arg;
    }

    public static UnsignedIntegerArgumentType operator --(UnsignedIntegerArgumentType arg) {
        arg.value--;
        return arg;
    }
}