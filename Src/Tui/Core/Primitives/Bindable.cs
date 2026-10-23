// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Tui.Core;

public readonly union Bindable<T>(T, Func<T>, Signal<T>) {
    public T Get() {
        return Value switch {
            T staticValue    => staticValue,
            Func<T> factory  => factory(),
            Signal<T> signal => signal.Value,
            _                => default!
        };
    }
    
    public T AsT => Get();

    public bool TrySet(T value) {
        if (Value is Signal<T> signal) {
            signal.Value = value;
            return true;
        }
        return false;
    }

    public void Set(T value) {
        if (Value is Signal<T> signal) {
            signal.Value = value;
        }
    }
    
    public static implicit operator T(Bindable<T> bindable) => bindable.Get();
}