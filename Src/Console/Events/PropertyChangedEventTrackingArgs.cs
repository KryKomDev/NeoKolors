// NeoKolors
// Copyright (c) KryKom 2026

using System.ComponentModel;

namespace NeoKolors.Console;

public class PropertyChangedEventTrackingArgs<T> :
    PropertyChangedEventArgs,
    IPropertyChangedEventTrackingArgs<T> 
{

    public T? OldValue { get; }
    public T? NewValue { get; }

    public PropertyChangedEventTrackingArgs(
        string? propertyName,
        T?      oldValue,
        T?      newValue
    ) : base(
        propertyName
    ) {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

public interface IPropertyChangedEventTrackingArgs<out T> {
    public T?      OldValue     { get; }
    public T?      NewValue     { get; }
    public string? PropertyName { get; }
}