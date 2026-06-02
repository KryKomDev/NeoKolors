// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a control that has a value within a specified range, such as a slider or progress bar.
/// </summary>
public abstract class RangeBase : Control<double> {
    
    private double _minimum;
    private double _maximum = 100.0;
    private double _value;
    private double _smallChange = 1.0;
    private double _largeChange = 10.0;

    public double Minimum {
        get => _minimum;
        set {
            if (_minimum == value) return;
            _minimum = value;
            CoerceValue();
        }
    }

    public double Maximum {
        get => _maximum;
        set {
            if (_maximum == value) return;
            _maximum = value;
            CoerceValue();
        }
    }

    public double Value {
        get => _value;
        set {
            var coerced = Math.Clamp(value, _minimum, _maximum);
            if (_value == coerced) return;
            var old = _value;
            _value = coerced;
            OnValueChanged(old, coerced);
        }
    }

    public double SmallChange {
        get => _smallChange;
        set => _smallChange = value;
    }

    public double LargeChange {
        get => _largeChange;
        set => _largeChange = value;
    }

    public event Action<RangeBase, double, double>? ValueChanged;

    protected RangeBase(StyleCollection defaultStyle) : base(defaultStyle) { }
    protected RangeBase() { }

    private void CoerceValue() {
        Value = Math.Clamp(_value, _minimum, _maximum);
    }

    protected virtual void OnValueChanged(double oldValue, double newValue) {
        ValueChanged?.Invoke(this, oldValue, newValue);
        InvokeElementUpdated();
    }

    public override double GetChildNode() => _value;

    public override void SetChildNode(double child) {
        Value = child;
    }
}
