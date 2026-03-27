//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.CodeAnalysis;
using NeoKolors.Settings.Argument;
using static NeoKolors.Settings.Argument.AllowedPathType;
using StringArgument = NeoKolors.Settings.Argument.StringArgument;

namespace NeoKolors.Settings;

/// <summary>
/// factory methods for arguments, <see cref="IArgument{T}"/>
/// </summary>
[ExcludeFromCodeCoverage]
public static class Arguments {

    /// <summary>
    /// creates a new boolean argument
    /// </summary>
    public static BoolArgument Bool() => new();

    /// <summary>
    /// creates a new string argument
    /// </summary>
    /// <param name="minLength">minimal length of the string to be allowed as a valid input</param>
    /// <param name="maxLength">maximal length of the string to be allowed as a valid input</param>
    /// <param name="defaultValue">default value of the argument</param>
    /// <param name="allowSpaces">whether spaces are allowed in the input string</param>
    /// <param name="allowNewlines">whether newlines are allowed in the input string</param>
    /// <param name="allowSpecial">
    /// whether special characters (every character except a-z A-Z 0-9 space and \n) are allowed in the input string
    /// </param>
    /// <param name="allowNumbers">whether numbers are allowed in the input string</param>
    /// <param name="allowUpper">whether upper characters are allowed in the input string</param>
    /// <param name="allowLower">whether lower characters are allowed in the input string</param>
    /// <param name="countVisibleOnly">whether to count only visible characters in the string to the total length</param>
    /// <param name="customValidate">custom string validation function, if value is valid returns null else string with the cause</param>
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

    /// <summary>
    /// creates a new integer argument
    /// </summary>
    /// <param name="min">minimal allowed value</param>
    /// <param name="max">maximal allowed value</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="customValidate">custom validation function, if value is valid returns null else string with the cause</param>
    public static IntegerArgument Integer(int min = int.MinValue,
        int max = int.MaxValue,
        int defaultValue = 0,
        Func<int, string?>? customValidate = null) =>
        new(min,
            max,
            defaultValue,
            customValidate);

    /// <summary>
    /// creates a new unsigned integer argument
    /// </summary>
    /// <param name="min">minimal allowed value</param>
    /// <param name="max">maximal allowed value</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="customValidate">custom validation function, if value is valid returns null else string with the cause</param>
    public static UIntegerArgument UInteger(uint min = uint.MinValue,
        uint max = uint.MaxValue,
        uint defaultValue = 0,
        Func<uint, string?>? customValidate = null) =>
        new(min,
            max,
            defaultValue,
            customValidate);

    /// <summary>
    /// creates a new long argument
    /// </summary>
    /// <param name="min">minimal allowed value</param>
    /// <param name="max">maximal allowed value</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="customValidate">custom validation function, if value is valid returns null else string with the cause</param>
    public static LongArgument Long(long min = long.MinValue,
        long max = long.MaxValue,
        long defaultValue = 0,
        Func<long, string?>? customValidate = null) =>
        new(min,
            max,
            defaultValue,
            customValidate);

    /// <summary>
    /// creates a new unsigned long argument
    /// </summary>
    /// <param name="min">minimal allowed value</param>
    /// <param name="max">maximal allowed value</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="customValidate">custom validation function, if value is valid returns null else string with the cause</param>
    public static ULongArgument ULong(ulong min = ulong.MinValue,
        ulong max = ulong.MaxValue,
        ulong defaultValue = 0,
        Func<ulong, string?>? customValidate = null) =>
        new(min,
            max,
            defaultValue,
            customValidate);

    /// <summary>
    /// creates a new float argument
    /// </summary>
    /// <param name="min">minimal allowed value</param>
    /// <param name="max">maximal allowed value</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="customValidate">custom validation function, if value is valid returns null else string with the cause</param>
    public static FloatArgument Float(float min = float.MinValue,
        float max = float.MaxValue,
        float defaultValue = 0,
        Func<float, string?>? customValidate = null) =>
        new(min,
            max,
            defaultValue,
            customValidate);

    /// <summary>
    /// creates a new double argument
    /// </summary>
    /// <param name="min">minimal allowed value</param>
    /// <param name="max">maximal allowed value</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="customValidate">custom validation function, if value is valid returns null else string with the cause</param>
    public static DoubleArgument Double(double min = double.MinValue,
        double max = double.MaxValue,
        double defaultValue = 0,
        Func<double, string?>? customValidate = null) =>
        new(min,
            max,
            defaultValue,
            customValidate);

    /// <summary>
    /// creates a new single selection (radio selection) argument
    /// </summary>
    /// <param name="defaultIndex">index of the default selected value</param>
    /// <param name="options">selectable options</param>
    public static SingleSelectArgument<T> SingleSelect<T>(int defaultIndex = 0, params T[] options) where T : notnull =>
        new(options, defaultIndex);

    /// <summary>
    /// creates a new single selection (radio selection) argument
    /// </summary>
    /// <param name="options">selectable options</param>
    /// <param name="defaultValue">value that will be selected by default</param>
    public static SingleSelectArgument<T> SingleSelect<T>(T[] options, T defaultValue) where T : notnull =>
        new(options, defaultValue);

    /// <summary>
    /// creates a new single selection (radio selection) argument from all members of an enum
    /// </summary>
    /// <param name="defaultValue">value that will be selected by default</param>
    /// <typeparam name="T">type of the enum</typeparam>
    public static SingleSelectArgument<T> SingleSelectEnum<T>(T? defaultValue = default) where T : Enum =>
        SingleSelectArgument<T>.FromEnum(defaultValue);

    /// <summary>
    /// creates a new multiple selection (checkbox selection) argument
    /// </summary>
    /// <param name="options">selectable options</param>
    /// <param name="defaultValues">values that are selected by default</param>
    public static MultiSelectArgument<T> MultiSelect<T>(T[] options, params T[] defaultValues) where T : notnull =>
        new(options, defaultValues);

    /// <summary>
    /// creates a new multiple selection (checkbox selection) argument from all members of an enum
    /// </summary>
    /// <param name="defaultValues">values that are selected by default</param>
    public static MultiSelectArgument<T> MultiSelectEnum<T>(params T[] defaultValues) where T : Enum =>
        MultiSelectArgument<T>.FromEnum(defaultValues);

    /// <summary>
    /// creates a new path argument
    /// </summary>
    /// <param name="defaultValue">the default path</param>
    /// <param name="allowedPathType">determines the allowed path types</param>
    public static PathArgument Path(
        string defaultValue = ".",
        AllowedPathType allowedPathType = FILE | DIRECTORY | NOT_EXISTING) =>
        new(defaultValue, allowedPathType);
    
    /// <summary>
    /// creates a new selection list argument, 
    /// </summary>
    /// <param name="values">choice values that can be added to the list</param>
    /// <param name="defaultValues">values that are selected by default</param>
    /// <typeparam name="T">element type</typeparam>
    public static SelectionListArgument<T> SelectionList<T>(T[] values, params T[] defaultValues) where T : notnull =>
        new(values, defaultValues);

    /// <summary>
    /// creates a new selection list argument from all members of an enum
    /// </summary>
    /// <param name="defaultValues">values that are selected by default</param>
    /// <typeparam name="T">enum type</typeparam>
    public static SelectionListArgument<T> SelectionListEnum<T>(params T[] defaultValues) where T : Enum =>
        SelectionListArgument<T>.FromEnum(defaultValues);
}