//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Reflection;
using NeoKolors.Console;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ApplicableStylesAttribute : Attribute {

    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(ApplicableStylesAttribute));
    
    public Type[] Styles { get; }

    /// <summary>
    /// Represents an attribute used to specify the styles that can be applied to a class or struct.
    /// This attribute is designed for usage with elements implementing styling-related behaviors.
    /// </summary>
    public ApplicableStylesAttribute(params Type[] styles) {
        Styles = styles;
    }

    /// <summary>
    /// Represents an attribute used to define the types of styles that can be applied to a class or struct.
    /// This attribute can be used to annotate classes or structs implementing the <see cref="IElement"/>,
    /// specifying one or more style types that are applicable for the annotated element.
    /// </summary>
    public ApplicableStylesAttribute(params string[] styles) {
        Styles = GetTypes(styles);
    }

    /// <summary>
    /// Represents an attribute used to specify the styles that can be applied to a class or struct.
    /// This attribute is primarily used to define styling constraints for elements implementing
    /// styling-related behaviors or functionalities.
    /// </summary>
    /// <param name="baseType">the type from which the style constraints will be inherited</param>
    /// <param name="styles">the additional styles that can be applied to the annotated element</param>
    public ApplicableStylesAttribute(Type baseType, params Type[] styles) {
        var a = baseType.GetCustomAttribute<ApplicableStylesAttribute>();

        if (a == null) {
            Styles = styles;
            return;
        }
        
        var types = a.Styles.ToList();

        foreach (var s in styles) {
            if (!types.Contains(s)) {
                types.Add(s);
            }
        }
        
        Styles = types.ToArray();
    }

    /// <summary>
    /// Represents an attribute used to specify the styles that can be applied to a class or struct.
    /// This attribute is primarily used to define styling constraints for elements implementing
    /// styling-related behaviors or functionalities.
    /// </summary>
    /// <param name="baseType">the type from which the style constraints will be inherited</param>
    /// <param name="styles">the additional styles that can be applied to the annotated element</param>
    public ApplicableStylesAttribute(Type baseType, params string[] styles) {
        var a = baseType.GetCustomAttribute<ApplicableStylesAttribute>(true);

        if (a == null) {
            Styles = GetTypes(styles);
            return;
        }
        
        var types = a.Styles.ToList();

        foreach (var s in GetTypes(styles)) {
            if (!types.Contains(s)) {
                types.Add(s);
            }
        }
        
        Styles = types.ToArray();
    }

    /// <summary>
    /// Represents an attribute used to define the set of styles that can be applied to a class or struct.
    /// This attribute can be used to enhance type-specific styling configuration capabilities for elements
    /// that support customizable appearance and behavior.
    /// </summary>
    /// <param name="typeSet">a predefined set of types that can be applied to the annotated element</param>
    /// <param name="styles">the additional styles that can be applied to the annotated element</param>
    public ApplicableStylesAttribute(Predefined typeSet, params string[] styles) {
        var types = FromPredefined(typeSet).ToList();
        
        foreach (var s in GetTypes(styles)) {
            if (!types.Contains(s)) {
                types.Add(s);
            }
        }
        
        Styles = types.ToArray();
    }

    private static Type[] GetTypes(params string[] names) {
        List<Type> types = [];
        
        foreach (var s in names) {
            var t = IStyleProperty.GetType(s);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (t == null) {
                LOGGER.Warn($"Style {s} cannot be added to the list of applicable styles, as it does not exist.");
                continue;
            }
            
            types.Add(t);
        }
        
        return types.ToArray();
    }

    private static Type[] ContainerStyles() => GetTypes("flex-direction", "overflow", "justify-content", "grid");
    private static Type[] TextStyles() => GetTypes("color", "font", "overflow", "text-align");
    private static Type[] UniversalStyles() => GetTypes(
        "display", "min-width", "max-width", "min-height", "max-height", "display", "background-color", 
        "border", "padding", "margin", "grid-align");

    private static Type[] FromPredefined(Predefined p) {
        List<Type> types = [];
        
        if (p.HasFlag(Predefined.CONTAINER)) types.AddRange(ContainerStyles());
        if (p.HasFlag(Predefined.TEXT)) types.AddRange(TextStyles());
        if (p.HasFlag(Predefined.UNIVERSAL)) types.AddRange(UniversalStyles());
        
        return types.ToArray();
    }

    [Flags]
    public enum Predefined {
        
        /// <summary>
        /// display, min/max-width/height, display, border, padding, margin, background-color, grid-align
        /// </summary>
        UNIVERSAL = 1,
        
        /// <summary>
        /// flex-direction, overflow, justify-content, grid
        /// </summary>
        CONTAINER = 2,
        
        /// <summary>
        /// color, background, border, font
        /// </summary>
        TEXT = 4,
    }
}