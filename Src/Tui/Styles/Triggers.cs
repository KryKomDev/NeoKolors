// NeoKolors
// Copyright (c) 2026 KryKom

using System.Reflection;
using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Styles;

/// <summary>
/// Represents a setter that applies a value to a property when a trigger is active.
/// </summary>
public class Setter {
    public string Property { get; set; } = string.Empty;
    public object Value { get; set; } = null!;
}

/// <summary>
/// The base class for all triggers that conditionally apply setters to an element.
/// </summary>
public abstract class TriggerBase {
    public List<Setter> Setters { get; set; } = new();

    /// <summary>
    /// Determines whether the trigger is currently active for the specified element.
    /// </summary>
    public abstract bool IsActive(IElement element);
}

/// <summary>
/// A property-based trigger that activates when a property matches a specified value.
/// </summary>
public class Trigger : TriggerBase {
    public string Property { get; set; } = string.Empty;
    public object Value { get; set; } = null!;

    public override bool IsActive(IElement element) {
        if (string.IsNullOrEmpty(Property)) return false;

        // 1. Try public C# property on the element class (like IsFocused, IsHovered, etc.)
        var prop = element.GetType().GetProperty(Property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (prop != null && prop.CanRead) {
            var val = prop.GetValue(element);

            if (val == null) return Value == null;

            try {
                var convertedTarget = Convert.ChangeType(Value, prop.PropertyType);

                return val.Equals(convertedTarget);
            }
            catch {
                return val.ToString()?.Equals(Value.ToString(), StringComparison.OrdinalIgnoreCase) ?? false;
            }
        }

        // 2. Try style property inside element.Style
        if (Properties.IStyleProperty.TryGetByName(Property, out var type)) {
            var styleProp = element.Style[type];

            if (styleProp == null)
                return Value == null;

            try {
                var convertedTarget = Convert.ChangeType(Value, styleProp.ValueType);

                return styleProp.Value.Equals(convertedTarget);
            }
            catch {
                return styleProp.Value.ToString()?.Equals(Value.ToString(), StringComparison.OrdinalIgnoreCase) ?? false;
            }
        }

        return false;
    }
}