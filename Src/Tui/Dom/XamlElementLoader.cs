// NeoKolors
// Copyright (c) 2026 KryKom

using System.Globalization;
using System.Reflection;
using Portable.Xaml;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Dom;

/// <summary>
/// A desktop-grade XAML loader for the UWP-inspired NeoKolors TUI framework.
/// Reimplemented using Portable.Xaml.
/// </summary>
public class XamlElementLoader {
    private static readonly NKLogger LOGGER = NKDebug.GetLogger<XamlElementLoader>();

    private readonly Dictionary<string, Type> _elementMap;

    public XamlElementLoader(params Assembly[] assemblies) {
        _elementMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        /*
        // Ensure Font Assets assembly is loaded and registered
        try {
            var assetsAssembly = Assembly.Load("NeoKolors.Tui.Fonts.Assets");
            if (assetsAssembly != null) {
                var providerType = assetsAssembly.GetType("NeoKolors.Tui.Fonts.Assets.AssetsProvider");
                if (providerType != null) {
                    var registerMethod = providerType.GetMethod("RegisterFonts", BindingFlags.Public | BindingFlags.Static);
                    if (registerMethod != null) {
                        registerMethod.Invoke(null, null);
                    }
                }
            }
        }
        catch (Exception ex) {
            LOGGER.Warn($"Failed to load built-in font assets assembly dynamically: {ex.Message}");
        }*/

        RegisterElements(typeof(IElement).Assembly);

        foreach (var a in assemblies) {
            RegisterElements(a);
        }
    }

    private void RegisterElements(Assembly sourceAssembly) {
        var types = sourceAssembly.GetTypes()
            .Where(t => typeof(IElement).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false });

        foreach (var type in types) {
            var attr = type.GetCustomAttribute<ElementNameAttribute>();

            if (attr != null) {
                _elementMap[attr.Name] = type;
            }

            _elementMap.TryAdd(type.Name, type);
        }
    }

    private class ParserFrame {
        public object Instance { get; }
        public XamlType Type { get; }
        public List<IElement> Children { get; } = new();
        public XamlMember? CurrentMember { get; set; }
        public string? CurrentMemberName { get; set; }
        public object? MemberValue { get; set; }
        public string? InitializationText { get; set; }

        public ParserFrame(object instance, XamlType type) {
            Instance = instance;
            Type = type;
        }
    }

    public NKDom Load(string xaml) {
        using var stringReader = new StringReader(xaml);
        using var xamlReader = new XamlXmlReader(stringReader);

        return LoadFromReader(xamlReader);
    }

    public NKDom LoadFromFile(string path) {
        using var streamReader = new StreamReader(path);
        using var xamlReader = new XamlXmlReader(streamReader);

        return LoadFromReader(xamlReader);
    }

    private NKDom LoadFromReader(XamlReader xamlReader) {
        var stack = new Stack<ParserFrame>();
        object? rootObject = null;

        while (xamlReader.Read()) {
            switch (xamlReader.NodeType) {
                case XamlNodeType.StartObject: {
                    var xamlType = xamlReader.Type;
                    var type = xamlType.UnderlyingType;

                    if (type == null) {
                        _elementMap.TryGetValue(xamlType.Name, out type);
                    }

                    if (type == null) {
                        throw new InvalidOperationException($"Unknown XAML element: {xamlType.Name}");
                    }

                    object instance;

                    try {
                        instance = Activator.CreateInstance(type)!;
                    }
                    catch (Exception ex) {
                        throw new InvalidOperationException($"Type {type.Name} has no suitable parameterless constructor.", ex);
                    }

                    var newFrame = new ParserFrame(instance, xamlType);
                    stack.Push(newFrame);

                    break;
                }

                case XamlNodeType.StartMember: {
                    if (stack.Count == 0) break;

                    var member = xamlReader.Member;
                    var frame = stack.Peek();
                    frame.CurrentMember = member;
                    frame.CurrentMemberName = member.Name;

                    break;
                }

                case XamlNodeType.Value: {
                    if (stack.Count == 0) break;

                    var frame = stack.Peek();
                    var val = xamlReader.Value;

                    if (frame.CurrentMember != null) {
                        frame.MemberValue = val;
                    }

                    break;
                }

                case XamlNodeType.EndMember: {
                    if (stack.Count == 0) break;

                    var frame = stack.Peek();
                    var member = frame.CurrentMember;
                    var memberName = frame.CurrentMemberName;
                    var val = frame.MemberValue;

                    if (member != null && val != null && memberName != null) {
                        bool isMemberTagAnElement = _elementMap.ContainsKey(memberName);
                        bool isUnknownContent = memberName.Equals("_UnknownContent", StringComparison.OrdinalIgnoreCase);

                        if (isMemberTagAnElement || isUnknownContent) {
                            // Skip applying nested element tags or UnknownContent as a property
                        }
                        else if (
                            member.IsDirective && member == XamlLanguage.Initialization || 
                            memberName.Equals("_Initialization", StringComparison.OrdinalIgnoreCase)) 
                        {
                            frame.InitializationText = val.ToString() ?? "";
                        }
                        else if (memberName.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                            memberName.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                            (member.IsDirective && member.Name.Equals("Name", StringComparison.OrdinalIgnoreCase))) {
                            if (frame.Instance is IElement el) {
                                el.Info.Id = val.ToString();
                            }
                        }
                        else if (member.IsAttachable || memberName.Contains('.')) {
                            string attachedName = memberName.Contains('.') ? memberName : $"{member.DeclaringType.Name}.{member.Name}";

                            if (frame.Instance is IElement el) {
                                SetAttachedProperty(el, attachedName, val.ToString() ?? "");
                            }
                        }
                        else {
                            if (frame.Instance is IElement el) {
                                SetProperty(el, memberName, val.ToString() ?? "");
                            }
                            else {
                                SetPropertyObject(frame.Instance, memberName, val);
                            }
                        }
                    }

                    frame.CurrentMember     = null!;
                    frame.CurrentMemberName = null!;
                    frame.MemberValue       = null!;

                    break;
                }

                case XamlNodeType.EndObject: {
                    if (stack.Count == 0) break;

                    var poppedFrame = stack.Pop();
                    var instance = poppedFrame.Instance;

                    // Handle text initialization content if present
                    if (!string.IsNullOrEmpty(poppedFrame.InitializationText) && instance is INode node && poppedFrame.Children.Count == 0) {
                        try {
                            Type? nodeInterface = instance.GetType().GetInterfaces()
                                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INode<>));
                            
                            if (nodeInterface != null) {
                                Type expectedType = nodeInterface.GetGenericArguments()[0];
                                object? convertedText = ConvertValue(poppedFrame.InitializationText, expectedType);
                                node.SetChildNode(convertedText);
                            }
                            else {
                                node.SetChildNode(poppedFrame.InitializationText);
                            }
                        }
                        catch (Exception ex) {
                            LOGGER.Error($"Failed to set child node content: {ex.Message}");
                        }
                    }

                    // Perform children assembly
                    if (instance is IElement element) {
                        AssembleElement(element, poppedFrame.Children);
                    }

                    if (stack.Count == 0) {
                        rootObject = instance;
                    }
                    else {
                        var parentFrame = stack.Peek();

                        if (parentFrame.CurrentMember != null && parentFrame.CurrentMemberName != null) {
                            string parentMemberName = parentFrame.CurrentMemberName;
                            bool isMemberTagAnElement = _elementMap.ContainsKey(parentMemberName);

                            if (parentMemberName.Equals("Children", StringComparison.OrdinalIgnoreCase) ||
                                parentMemberName.Equals("_Items", StringComparison.OrdinalIgnoreCase) ||
                                parentMemberName.Equals("_UnknownContent", StringComparison.OrdinalIgnoreCase) ||
                                parentFrame.CurrentMember == XamlLanguage.Items ||
                                parentFrame.CurrentMember == XamlLanguage.UnknownContent ||
                                isMemberTagAnElement) {
                                if (instance is IElement el) {
                                    parentFrame.Children.Add(el);
                                }
                            }
                            else {
                                ApplyMemberValue(parentFrame.Instance, parentFrame.CurrentMember, parentFrame.CurrentMemberName, instance);
                            }
                        }
                        else {
                            if (instance is IElement el) {
                                parentFrame.Children.Add(el);
                            }
                        }
                    }

                    break;
                }
            }
        }

        if (rootObject == null) {
            throw new InvalidOperationException("XAML document has no root.");
        }

        if (rootObject is not IElement rootElement) {
            throw new InvalidOperationException($"Root object is not an IElement: {rootObject.GetType().Name}");
        }

        return new NKDom(rootElement);
    }

    private void ApplyMemberValue(object instance, XamlMember member, string memberName, object value) {
        if (instance is IElement element && value is string strVal) {
            if (member.IsDirective) {
                if (memberName.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                    memberName.Equals("Key", StringComparison.OrdinalIgnoreCase)) {
                    element.Info.Id = strVal;

                    return;
                }
            }
            else if (member.IsAttachable || memberName.Contains('.')) {
                string attachedName = memberName.Contains('.') ? memberName : $"{member.DeclaringType.Name}.{member.Name}";
                SetAttachedProperty(element, attachedName, strVal);

                return;
            }
            else {
                SetProperty(element, memberName, strVal);

                return;
            }
        }

        SetPropertyObject(instance, memberName, value);
    }

    private void AssembleElement(IElement element, List<IElement> childrenList) {
        // Post-process Grid/RelativePanel/Panel children layout mappings and classes
        int gridRow = 0;
        int gridCol = 0;
        int gridRowSpan = 1;
        int gridColSpan = 1;
        bool hasGridProps = false;

        foreach (var c in element.Info.Classes) {
            if (c.StartsWith("Grid.Row=", StringComparison.OrdinalIgnoreCase)) {
                if (int.TryParse(c.Substring(9), out var r)) {
                    gridRow = r;
                    hasGridProps = true;
                }
            }
            else if (c.StartsWith("Grid.Column=", StringComparison.OrdinalIgnoreCase)) {
                if (int.TryParse(c.Substring(12), out var col)) {
                    gridCol = col;
                    hasGridProps = true;
                }
            }
            else if (c.StartsWith("Grid.RowSpan=", StringComparison.OrdinalIgnoreCase)) {
                if (int.TryParse(c.Substring(13), out var rs)) {
                    gridRowSpan = rs;
                    hasGridProps = true;
                }
            }
            else if (c.StartsWith("Grid.ColumnSpan=", StringComparison.OrdinalIgnoreCase)) {
                if (int.TryParse(c.Substring(16), out var cs)) {
                    gridColSpan = cs;
                    hasGridProps = true;
                }
            }
        }

        if (hasGridProps) {
            element.Style.Set(new GridAlignProperty(new Rectangle(gridCol, gridRow, gridColSpan, gridRowSpan)));
        }

        if (childrenList.Count > 0) {
            if (element is Grid grid) {
                foreach (var child in childrenList) {
                    int row = 0;
                    int col = 0;
                    int rowSpan = 1;
                    int colSpan = 1;

                    var gridAlign = child.Style.Get<GridAlignProperty>().Value;
                    row = gridAlign.LowerY;
                    col = gridAlign.LowerX;

                    foreach (var c in child.Info.Classes) {
                        if (c.StartsWith("Grid.RowSpan=", StringComparison.OrdinalIgnoreCase)) {
                            int.TryParse(c[13..], out rowSpan);
                        }
                        else if (c.StartsWith("Grid.ColumnSpan=", StringComparison.OrdinalIgnoreCase)) {
                            int.TryParse(c[16..], out colSpan);
                        }
                    }

                    grid.AddChild(child, row, col, rowSpan, colSpan);
                }
            }
            else if (element is RelativePanel relativePanel) {
                foreach (var child in childrenList) {
                    relativePanel.AddChild(child);
                }

                foreach (var child in childrenList) {
                    foreach (var c in child.Info.Classes) {
                        if (c.StartsWith("RelativePanel.RightOf=", StringComparison.OrdinalIgnoreCase)) {
                            var targetId = c.Substring(22);
                            var target = childrenList.FirstOrDefault(x => x.Info.Id == targetId);
                            if (target != null) relativePanel.SetRightOf(child, target);
                        }
                        else if (c.StartsWith("RelativePanel.Below=", StringComparison.OrdinalIgnoreCase)) {
                            var targetId = c.Substring(20);
                            var target = childrenList.FirstOrDefault(x => x.Info.Id == targetId);
                            if (target != null) relativePanel.SetBelow(child, target);
                        }
                        else if (c.StartsWith("RelativePanel.LeftOf=", StringComparison.OrdinalIgnoreCase)) {
                            var targetId = c.Substring(21);
                            var target = childrenList.FirstOrDefault(x => x.Info.Id == targetId);
                            if (target != null) relativePanel.SetLeftOf(child, target);
                        }
                        else if (c.StartsWith("RelativePanel.Above=", StringComparison.OrdinalIgnoreCase)) {
                            var targetId = c.Substring(20);
                            var target = childrenList.FirstOrDefault(x => x.Info.Id == targetId);
                            if (target != null) relativePanel.SetAbove(child, target);
                        }
                    }
                }
            }
            else if (element is Panel panel) {
                foreach (var child in childrenList) {
                    panel.AddChild(child);
                }
            }
            else if (element is INode container) {
                if (childrenList.Count == 1) {
                    container.SetChildNode(childrenList[0]);
                }
                else {
                    container.SetChildNode(childrenList.ToArray());
                }
            }
        }
    }

    public static void SetPropertyObject(IElement element, string name, object value) {
        var prop = element.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (prop != null) {
            try {
                if (typeof(System.Collections.IList).IsAssignableFrom(prop.PropertyType)) {
                    var list = prop.GetValue(element) as System.Collections.IList;
                    if (list != null) {
                        list.Add(value);
                        return;
                    }
                }
                if (prop.CanWrite) {
                    prop.SetValue(element, value);
                }
            }
            catch (Exception e) {
                LOGGER.Warn($"Failed to set C# property object '{name}' on '{element.GetType().Name}': {e.Message}");
            }
        }
        else {
            LOGGER.Warn($"C# Property object '{name}' not found on '{element.GetType().Name}'.");
        }
    }

    public static void SetPropertyObject(object instance, string name, object value) {
        var prop = instance.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (prop != null) {
            try {
                if (typeof(System.Collections.IList).IsAssignableFrom(prop.PropertyType)) {
                    var list = prop.GetValue(instance) as System.Collections.IList;
                    if (list != null) {
                        list.Add(value);
                        return;
                    }
                }
                if (prop.CanWrite) {
                    prop.SetValue(instance, value);
                }
            }
            catch (Exception e) {
                LOGGER.Warn($"Failed to set C# property object '{name}' on '{instance.GetType().Name}': {e.Message}");
            }
        }
        else {
            LOGGER.Warn($"C# Property object '{name}' not found on '{instance.GetType().Name}'.");
        }
    }

    public static void SetAttachedProperty(IElement element, string fullName, string value) {
        var parts = fullName.Split('.');
        var containerName = parts[0];
        var propertyName = parts[1];
        element.Info.AddClass($"{containerName}.{propertyName}={value}");
    }

    public static void SetProperty(IElement element, string name, string value) {
        if (name.Equals("Class", StringComparison.OrdinalIgnoreCase)) {
            element.Info.AddClass(value);

            return;
        }

        var prop = element.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (prop != null && prop.CanWrite) {
            try {
                var convertedValue = ConvertValue(value, prop.PropertyType);
                prop.SetValue(element, convertedValue);
                return;
            }
            catch (Exception e) {
                LOGGER.Warn($"Failed to set C# property '{name}' on '{element.GetType().Name}': {e.Message}");
            }
        }

        if (!IStyleProperty.TryGetByName(name, out var type)) {
            LOGGER.Warn($"Style property '{name}' is not registered and '{element.GetType().Name}' has no matching C# property.");
            return;
        }

        try {
            Type target;

            if (IStyleProperty.IsPartial(type)) {
                var partial = (IPartialStyleProperty?)Activator.CreateInstance(type);

                if (partial == null) return;

                target = partial.ValueType;
            }
            else {
                var full = (IStyleProperty?)Activator.CreateInstance(type);

                if (full == null) return;

                target = full.ValueType;
            }

            var convertedValue = ConvertValue(value, target);
            var property = (IStyleProperty?)Activator.CreateInstance(type, convertedValue);

            if (property != null)
                element.Style.Set(property);
        }
        catch (Exception e) {
            LOGGER.Warn($"Failed to set style property '{name}' on '{element.GetType().Name}': {e.Message}");
        }
    }

    public static void SetChildNodeContent(object parent, object child) {
        if (parent is INode node) {
            node.SetChildNode(child);
        }
    }

    public static void SetChildNodeText(object instance, string text) {
        if (instance is INode node) {
            try {
                Type? nodeInterface = instance.GetType().GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INode<>));
                
                if (nodeInterface != null) {
                    Type expectedType = nodeInterface.GetGenericArguments()[0];
                    object? convertedText = ConvertValue(text, expectedType);
                    node.SetChildNode(convertedText);
                }
                else {
                    node.SetChildNode(text);
                }
            }
            catch (Exception ex) {
                LOGGER.Error($"Failed to set child node content: {ex.Message}");
            }
        }
    }

    public static void RegisterEvent(object element, string eventName, object target, string handlerMethodName) {
        var ev = element.GetType().GetEvent(eventName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (ev == null) {
            LOGGER.Warn($"Event '{eventName}' not found on '{element.GetType().Name}'.");
            return;
        }

        if (ev.EventHandlerType == null) {
            LOGGER.Warn($"Event '{eventName}' on '{element.GetType().Name}' does not have a valid handler type.");
            return;
        }

        var method = target.GetType().GetMethod(handlerMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase);
        if (method == null) {
            LOGGER.Warn($"Method '{handlerMethodName}' not found on '{target.GetType().Name}'.");
            return;
        }

        Delegate del;
        try {
            del = method.IsStatic
                ? Delegate.CreateDelegate(ev.EventHandlerType, method) 
                : Delegate.CreateDelegate(ev.EventHandlerType, target, method);
        }
        catch {
            try {
                var invokeMethod = ev.EventHandlerType.GetMethod("Invoke");
                if (invokeMethod == null) {
                    throw new InvalidOperationException($"Event handler type {ev.EventHandlerType.Name} does not have an Invoke method.");
                }

                var delegateParameters = invokeMethod.GetParameters()
                    .Select(p => System.Linq.Expressions.Expression.Parameter(p.ParameterType))
                    .ToArray();

                var methodParams = method.GetParameters();
                var callArgs = new List<System.Linq.Expressions.Expression>();
                for (int i = 0; i < methodParams.Length && i < delegateParameters.Length; i++) {
                    callArgs.Add(delegateParameters[i]);
                }

                System.Linq.Expressions.Expression body;
                if (method.IsStatic) {
                    body = System.Linq.Expressions.Expression.Call(method, callArgs);
                }
                else {
                    body = System.Linq.Expressions.Expression.Call(
                        System.Linq.Expressions.Expression.Constant(target),
                        method,
                        callArgs);
                }
                
                var lambda = System.Linq.Expressions.Expression.Lambda(ev.EventHandlerType, body, delegateParameters);
                del = lambda.Compile();
            }
            catch (Exception ex) {
                LOGGER.Warn($"Failed to bind event '{eventName}' to method '{handlerMethodName}': {ex.Message}");
                return;
            }
        }

        ev.AddEventHandler(element, del);
    }

    public static object? ConvertValue(string value, Type targetType) {
        if (targetType == typeof(object)) return value;
        if (targetType == typeof(AnsiString)) return new AnsiString(value);
        if (targetType == typeof(string)) return value;
        if (targetType == typeof(int)) return int.Parse(value);
        if (targetType == typeof(float)) return float.Parse(value);
        if (targetType == typeof(double)) return double.Parse(value);
        if (targetType == typeof(bool)) return bool.Parse(value);
        if (targetType.IsEnum) return ParseEnum(targetType, value);
        if (targetType == typeof(IAsciiFont)) return ParseFont(value);

        if (targetType == typeof(SkiaSharp.SKImage)) {
            if (string.IsNullOrEmpty(value) || !File.Exists(value)) return null;

            return SkiaSharp.SKImage.FromEncodedData(value);
        }

        if (typeof(IParsableValue).IsAssignableFrom(targetType)) {
            var inst = (IParsableValue?)Activator.CreateInstance(targetType);

            if (inst == null)
                throw new InvalidOperationException($"Cannot convert '{value}' to type {targetType.Name}");

            return inst.Parse(value, CultureInfo.InvariantCulture);
        }

        throw new InvalidOperationException($"Unsupported type conversion from '{value}' to type {targetType.Name}");
    }

    private static IAsciiFont ParseFont(string name) {
        if (!FontAtlas.TryGet(name, out var res))
            res = IAsciiFont.Default;

        return res ?? IAsciiFont.Default;
    }

    private static object? ParseEnum(Type targetType, string value) {
        var res = Enum.TryParse(targetType, value, true, out var result);

        if (res) return result;

        res = Enum.TryParse(targetType, value.Replace('_', '-'), true, out result);

        return res ? result : null;
    }
}