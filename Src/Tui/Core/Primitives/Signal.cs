// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Tui.Core;

public sealed class Signal<T> {
    private T _value;

    public event Action? Changed;

    public Signal(T initial) => _value = initial;

    public T Value {
        get => _value;
        set {
            if (EqualityComparer<T>.Default.Equals(_value, value))
                return;

            _value = value;
            Changed?.Invoke();
        }
    }

    public static implicit operator T(Signal<T> signal) => signal.Value;
}