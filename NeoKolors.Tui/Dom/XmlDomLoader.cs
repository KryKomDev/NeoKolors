// NeoKolors
// Copyright (c) 2025 KryKom

using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Dom;

public class XmlDomLoader {
    private static readonly NKLogger LOGGER = NKDebug.GetLogger<XmlDomLoader>();
    
    private readonly Dictionary<string, Type> _elementMap;

    public XmlDomLoader(params Assembly[] assemblies) {
        _elementMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        RegisterElements(typeof(IElement).Assembly);
        
        foreach (var a in assemblies) {
            RegisterElements(a);
        }
    }

    private void RegisterElements(Assembly sourceAssembly) {
        var types = sourceAssembly.GetTypes()
            .Where(t => typeof(IElement).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

        foreach (var type in types) {
            var attr = type.GetCustomAttribute<ElementNameAttribute>();
            if (attr != null) {
                _elementMap[attr.Name] = type;
            }
            
            if (!_elementMap.TryAdd(type.Name, type))
                LOGGER.Warn("Element type '{0}' with name '{1}' already registered", type, type.Name);
        }
    }

    public NKDom Load(string xml) {
        var doc = XDocument.Parse(xml);
        if (doc.Root == null) throw new InvalidOperationException("XML document has no root.");
        
        var rootElement = ParseElement(doc.Root);
        return new NKDom(rootElement);
    }
    
    public NKDom LoadFromFile(string path) {
        var doc = XDocument.Load(path);
        if (doc.Root == null) throw new InvalidOperationException("XML document has no root.");
        
        var rootElement = ParseElement(doc.Root);
        return new NKDom(rootElement);
    }

    private IElement ParseElement(XElement xmlElement) {
        var tagName = xmlElement.Name.LocalName;
        if (!_elementMap.TryGetValue(tagName, out var type)) {
            throw new InvalidOperationException($"Unknown element: {tagName}");
        }

        IElement element;

        var textContent = xmlElement.Nodes().OfType<XText>()
            .Select(t => t.Value)
            .Aggregate("", (current, next) => current + next)
            .Trim();
            
        var stringCtor  = type.GetConstructor([typeof(string)]);
        var defaultCtor = type.GetConstructor(Type.EmptyTypes);
        var paramsCtor  = type.GetConstructor([typeof(IElement[])]);

        if (stringCtor != null && (defaultCtor == null || !string.IsNullOrEmpty(textContent))) {
            element = (IElement)stringCtor.Invoke([textContent]);
            textContent = null;
        } 
        else if (defaultCtor != null) {
            element = (IElement)Activator.CreateInstance(type)!;
        } 
        else if (paramsCtor != null) {
             element = (IElement)paramsCtor.Invoke([Array.Empty<IElement>()]);
        } 
        else {
            throw new InvalidOperationException($"Type {type.Name} has no suitable constructor (Parameterless, String, or IElement[]).");
        }

        // Apply Properties from Attributes
        foreach (var attr in xmlElement.Attributes()) {
            SetProperty(element, attr.Name.LocalName, attr.Value);
        }
        
        // Handle Children
        if (xmlElement.HasElements) {
             var children = xmlElement.Elements().Select(ParseElement).ToList();
             
             if (children.Count > 0) {
                 element.AddChild(children.ToArray());
             }
        } 
        else if (!string.IsNullOrEmpty(textContent)) {
            try {
                element.SetChildren(textContent);
            }
            catch {
                // ignored
            }
        }

        return element;
    }

    private void SetProperty(IElement element, string name, string value) {
        if (name.Equals("Class", StringComparison.OrdinalIgnoreCase)) {
            element.Info.AddClass(value);
            return;
        }

        if (name.Equals("Id", StringComparison.OrdinalIgnoreCase)) {
            element.Info.Id = value;
            return;
        }
        
        var prop = element.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        
        if (prop != null && prop.CanWrite) {
            try {
               var convertedValue = ConvertValue(value, prop.PropertyType);
               prop.SetValue(element, convertedValue);
            } 
            catch (Exception e) {
                LOGGER.Error(e);
            }
        }
        else if (IStyleProperty.TryGetByName(name, out var type) && typeof(IStyleProperty<,>).IsAssignableFrom(type)) {
            try {
                var target = type!.GenericTypeArguments[0];
                var convertedValue = ConvertValue(value, target);
                element.Style.Set((IStyleProperty)Activator.CreateInstance(type, convertedValue));
            } 
            catch (Exception e) {
                LOGGER.Error(e);
            }
        }
    }

    private object? ConvertValue(string value, Type targetType) {
         if (targetType == typeof(string)) return value;
         if (targetType == typeof(int)) return int.Parse(value);
         if (targetType == typeof(float)) return float.Parse(value);
         if (targetType == typeof(double)) return double.Parse(value);
         if (targetType == typeof(bool)) return bool.Parse(value);
         if (targetType.IsEnum) return Enum.Parse(targetType, value, true);
         if (targetType == typeof(IFont)) return ParseFont(value);
         if (typeof(IParsableValue).IsAssignableFrom(targetType)) {
             var inst = (IParsableValue)Activator.CreateInstance(targetType);
             
             if (inst == null) 
                 throw new InvalidOperationException($"Cannot convert {value} to type {targetType.Name}");
             
             return inst.Parse(value, CultureInfo.InvariantCulture);
         }
         
         throw new InvalidOperationException($"Unsupported type conversion: {targetType.Name}");
    }

    private static IFont ParseFont(string name) {
        if (!FontAtlas.TryGet(name, out var res)) 
            res = IFont.Default;

        return res ?? IFont.Default;
    }
}
