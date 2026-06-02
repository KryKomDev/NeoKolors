// NeoKolors
// Copyright (c) 2026 KryKom

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace NeoKolors.Tui.Generators;

[Generator]
public class XamlSourceGenerator : IIncrementalGenerator {

    private static readonly DiagnosticDescriptor XAML_PARSE_ERROR = new(
        id: "NKXAML001",
        title: "XAML Parsing Error",
        messageFormat: "XML parsing failed: {0}",
        category: "XamlCompiler",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MISSING_CLASS_ERROR = new(
        id: "NKXAML002",
        title: "Missing C# Target Class",
        messageFormat: "The root element of a XAML file must specify an x:Class attribute indicating the target C# partial class",
        category: "XamlCompiler",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor INVALID_IDENTIFIER_ERROR = new(
        id: "NKXAML003",
        title: "Invalid C# Identifier",
        messageFormat: "The Name or x:Name value '{0}' is not a valid C# identifier",
        category: "XamlCompiler",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor INVALID_TAG_ERROR = new(
        id: "NKXAML004",
        title: "Invalid Element Tag",
        messageFormat: "The tag name '{0}' is not a valid C# type identifier",
        category: "XamlCompiler",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly HashSet<string> KNOWN_EVENTS = new(StringComparer.OrdinalIgnoreCase) {
        "OnClick",
        "OnRelease",
        "OnHover",
        "OnHoverOut",
        "ContentChanged",
        "GotFocus",
        "LostFocus",
        "IsEnabledChanged",
        "HeaderChanged",
        "ValueChanged",
        "OnRender",
        "CheckedStateChanged"
    };

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var xamlFiles = context.AdditionalTextsProvider
            .Where(static text => text.Path.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase) || 
                                 text.Path.EndsWith(".nkxaml", StringComparison.OrdinalIgnoreCase));

        var parsedFiles = xamlFiles.Select(static (text, cancellationToken) => {
            var content = text.GetText(cancellationToken)?.ToString();
            if (string.IsNullOrEmpty(content)) return null;
            return new XamlFileDescriptor(text.Path, content!);
        }).Where(static desc => desc is not null).Select(static (desc, _) => desc!);

        context.RegisterSourceOutput(parsedFiles, Generate);
    }

    private static bool IsValidIdentifier(string name) {
        if (string.IsNullOrEmpty(name)) return false;
        if (!char.IsLetter(name[0]) && name[0] != '_') return false;
        for (int i = 1; i < name.Length; i++) {
            if (!char.IsLetterOrDigit(name[i]) && name[i] != '_') return false;
        }
        return true;
    }

    private static Location GetLocation(string filePath, XObject obj) {
        var lineInfo = (IXmlLineInfo)obj;
        if (lineInfo != null && lineInfo.HasLineInfo()) {
            var line = lineInfo.LineNumber;
            var col = lineInfo.LinePosition;
            var linePositionStart = new LinePosition(line - 1, col - 1);
            var linePositionEnd = new LinePosition(line - 1, col - 1);
            var lineSpan = new LinePositionSpan(linePositionStart, linePositionEnd);
            var textSpan = new TextSpan(0, 0);
            return Location.Create(filePath, textSpan, lineSpan);
        }
        return Location.None;
    }

    private static void ValidateXml(XElement element, string filePath, SourceProductionContext context) {
        var tag = element.Name.LocalName;
        if (!tag.Contains('.') && !IsValidIdentifier(tag)) {
            context.ReportDiagnostic(Diagnostic.Create(INVALID_TAG_ERROR, GetLocation(filePath, element), tag));
        }

        foreach (var attr in element.Attributes()) {
            var localName = attr.Name.LocalName;
            var isName = localName.Equals("Name", StringComparison.OrdinalIgnoreCase);
            var isXamlName = attr.Name.NamespaceName.Contains("xaml") && localName.Equals("Name", StringComparison.OrdinalIgnoreCase);
            
            if (isName || isXamlName) {
                if (!IsValidIdentifier(attr.Value)) {
                    context.ReportDiagnostic(Diagnostic.Create(INVALID_IDENTIFIER_ERROR, GetLocation(filePath, attr), attr.Value));
                }
            }
        }

        foreach (var child in element.Elements()) {
            ValidateXml(child, filePath, context);
        }
    }

    private static void FindNamedElements(XElement element, List<NamedElementInfo> namedElements) {
        foreach (var attr in element.Attributes()) {
            var attrName = attr.Name.LocalName;
            var isName = attrName.Equals("Name", StringComparison.OrdinalIgnoreCase);
            var isId = attrName.Equals("Id", StringComparison.OrdinalIgnoreCase);
            var isXamlName = attr.Name.NamespaceName.Contains("xaml") && attrName.Equals("Name", StringComparison.OrdinalIgnoreCase);

            if (isName || isId || isXamlName) {
                if (element.Parent == null) {
                    continue; // Skip root name mapping
                }

                var nameValue = attr.Value;
                if (!IsValidIdentifier(nameValue)) continue;

                var typeName = element.Name.LocalName;
                namedElements.Add(new NamedElementInfo(nameValue, typeName));
                break;
            }
        }

        foreach (var child in element.Elements()) {
            if (child.Name.LocalName.Contains('.')) continue;
            FindNamedElements(child, namedElements);
        }
    }

    private static void Generate(SourceProductionContext context, XamlFileDescriptor desc) {
        XDocument doc;
        try {
            using var stringReader = new StringReader(desc.Content);
            doc = XDocument.Load(stringReader, LoadOptions.SetLineInfo);
        }
        catch (XmlException ex) {
            var linePositionStart = new LinePosition(ex.LineNumber - 1, ex.LinePosition - 1);
            var linePositionEnd = new LinePosition(ex.LineNumber - 1, ex.LinePosition - 1);
            var lineSpan = new LinePositionSpan(linePositionStart, linePositionEnd);
            var textSpan = new TextSpan(0, 0);
            var location = Location.Create(desc.Path, textSpan, lineSpan);
            context.ReportDiagnostic(Diagnostic.Create(XAML_PARSE_ERROR, location, ex.Message));
            return;
        }

        var root = doc.Root;
        if (root == null) return;

        var xClassAttr = root.Attributes().FirstOrDefault(a => 
            a.Name.LocalName.Equals("Class", StringComparison.OrdinalIgnoreCase) ||
            (a.Name.NamespaceName.Contains("xaml") && a.Name.LocalName.Equals("Class", StringComparison.OrdinalIgnoreCase)));

        if (xClassAttr == null) {
            context.ReportDiagnostic(Diagnostic.Create(MISSING_CLASS_ERROR, GetLocation(desc.Path, root)));
            return;
        }

        // Validate element tags and identifiers
        ValidateXml(root, desc.Path, context);

        var fullClassName = xClassAttr.Value;
        var rootTag = root.Name.LocalName;
        var lastDot = fullClassName.LastIndexOf('.');
        var ns = lastDot > 0 ? fullClassName.Substring(0, lastDot) : "";
        var className = lastDot > 0 ? fullClassName.Substring(lastDot + 1) : fullClassName;

        var namedElements = new List<NamedElementInfo>();
        FindNamedElements(root, namedElements);

        var elementToVarName = new Dictionary<XElement, string>();
        var elementIdToVarName = new Dictionary<string, string>();
        var varCounter = 0;

        // Walk the tree to map all elements to variable names and build ID-to-variable map
        BuildVarNameMap(root, "this", elementToVarName, elementIdToVarName, ref varCounter);

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("using NeoKolors.Tui.Dom;");
        sb.AppendLine("using NeoKolors.Tui.Elements;");
        sb.AppendLine();

        if (!string.IsNullOrEmpty(ns)) {
            sb.AppendLine($"namespace {ns} {{");
        }

        sb.AppendLine($"    public partial class {className} : global::NeoKolors.Tui.Elements.{rootTag} {{");
        sb.AppendLine();

        foreach (var e in namedElements) {
            sb.AppendLine($"        public global::NeoKolors.Tui.Elements.{e.TypeName} {e.Name} {{ get; private set; }}");
        }

        sb.AppendLine();
        sb.AppendLine("        private void InitializeComponent() {");

        var bodyBuilder = new StringBuilder();
        var elementCounter = 0;
        CompileElement(root, "this", true, bodyBuilder, elementToVarName, elementIdToVarName, ref elementCounter);

        sb.Append(bodyBuilder);

        sb.AppendLine("        }");
        sb.AppendLine("    }");

        if (!string.IsNullOrEmpty(ns)) {
            sb.AppendLine("}");
        }

        context.AddSource($"{className}.g.cs", sb.ToString());
    }

    private static void BuildVarNameMap(
        XElement element, 
        string currentVarName, 
        Dictionary<XElement, string> elementToVarName, 
        Dictionary<string, string> elementIdToVarName, 
        ref int varCounter) 
    {
        elementToVarName[element] = currentVarName;

        var nameAttr = element.Attributes().FirstOrDefault(a => 
            a.Name.LocalName.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
            a.Name.LocalName.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
            (a.Name.NamespaceName.Contains("xaml") && a.Name.LocalName.Equals("Name", StringComparison.OrdinalIgnoreCase)));
        
        if (nameAttr != null && IsValidIdentifier(nameAttr.Value)) {
            elementIdToVarName[nameAttr.Value] = currentVarName;
        }

        foreach (var child in element.Elements()) {
            if (child.Name.LocalName.Contains('.')) {
                foreach (var grandChild in child.Elements()) {
                    varCounter++;
                    BuildVarNameMap(grandChild, $"el_{varCounter}", elementToVarName, elementIdToVarName, ref varCounter);
                }
            }
            else {
                varCounter++;
                BuildVarNameMap(child, $"el_{varCounter}", elementToVarName, elementIdToVarName, ref varCounter);
            }
        }
    }

    private static void CompileElement(
        XElement element, 
        string currentVarName, 
        bool isRoot, 
        StringBuilder sb, 
        Dictionary<XElement, string> elementToVarName, 
        Dictionary<string, string> elementIdToVarName, 
        ref int varCounter) 
    {
        var tag = element.Name.LocalName;
        var typePrefix = (tag == "Trigger" || tag == "Setter") 
            ? "global::NeoKolors.Tui.Styles." 
            : "global::NeoKolors.Tui.Elements.";

        if (!isRoot) {
            sb.AppendLine($"            var {currentVarName} = new {typePrefix}{tag}();");
        }

        // Parse and set attributes/properties
        foreach (var attr in element.Attributes()) {
            var localName = attr.Name.LocalName;
            var isClass = localName.Equals("Class", StringComparison.OrdinalIgnoreCase);
            var isXamlClass = attr.Name.NamespaceName.Contains("xaml") && localName.Equals("Class", StringComparison.OrdinalIgnoreCase);
            
            // Skip namespace definitions and Class attribute
            if (attr.Name.NamespaceName.StartsWith("http://www.w3.org/2000/xmlns") || 
                localName.Equals("xmlns", StringComparison.OrdinalIgnoreCase) ||
                isClass || isXamlClass) 
            {
                continue;
            }

            var isName = localName.Equals("Name", StringComparison.OrdinalIgnoreCase);
            var isId = localName.Equals("Id", StringComparison.OrdinalIgnoreCase);
            var isXamlName = attr.Name.NamespaceName.Contains("xaml") && localName.Equals("Name", StringComparison.OrdinalIgnoreCase);

            if (KNOWN_EVENTS.Contains(localName)) {
                sb.AppendLine($"            global::NeoKolors.Tui.Dom.XamlElementLoader.RegisterEvent({currentVarName}, {ToLiteral(localName)}, this, {ToLiteral(attr.Value)});");
            }
            else if (isName || isId || isXamlName) {
                sb.AppendLine($"            {currentVarName}.Info.Id = {ToLiteral(attr.Value)};");
                if (!isRoot && IsValidIdentifier(attr.Value)) {
                    sb.AppendLine($"            this.{attr.Value} = {currentVarName};");
                }
            }
            else if (localName.Contains('.')) {
                // Attached property
                sb.AppendLine($"            global::NeoKolors.Tui.Dom.XamlElementLoader.SetAttachedProperty({currentVarName}, {ToLiteral(localName)}, {ToLiteral(attr.Value)});");
            }
            else {
                if (tag == "Trigger" || tag == "Setter") {
                    sb.AppendLine($"            {currentVarName}.{localName} = {ToLiteral(attr.Value)};");
                }
                else {
                    // Standard property or style property
                    sb.AppendLine($"            global::NeoKolors.Tui.Dom.XamlElementLoader.SetProperty({currentVarName}, {ToLiteral(localName)}, {ToLiteral(attr.Value)});");
                }
            }
        }

        // Set child node text content if no element children exist
        if (!element.HasElements && !string.IsNullOrWhiteSpace(element.Value)) {
            sb.AppendLine($"            global::NeoKolors.Tui.Dom.XamlElementLoader.SetChildNodeText({currentVarName}, {ToLiteral(element.Value.Trim())});");
        }

        // Process standard children and property-elements
        foreach (var child in element.Elements()) {
            if (child.Name.LocalName.Contains('.')) {
                // Property-element syntax (e.g. `<Button.Content>`)
                var propName = child.Name.LocalName.Substring(child.Name.LocalName.IndexOf('.') + 1);
                foreach (var grandChild in child.Elements()) {
                    varCounter++;
                    var grandChildVarName = elementToVarName[grandChild];
                    CompileElement(grandChild, grandChildVarName, false, sb, elementToVarName, elementIdToVarName, ref varCounter);
                    sb.AppendLine($"            global::NeoKolors.Tui.Dom.XamlElementLoader.SetPropertyObject({currentVarName}, {ToLiteral(propName)}, {grandChildVarName});");
                }
            }
            else {
                // Standard child element
                varCounter++;
                var childVarName = elementToVarName[child];
                CompileElement(child, childVarName, false, sb, elementToVarName, elementIdToVarName, ref varCounter);

                // Parent/Assemble child to parent
                var parentTag = element.Name.LocalName;
                if (parentTag == "Trigger" || parentTag == "TriggerBase") {
                    sb.AppendLine($"            global::NeoKolors.Tui.Dom.XamlElementLoader.SetPropertyObject({currentVarName}, \"Setters\", {childVarName});");
                }
                else if (parentTag == "Grid") {
                    var rowAttr = child.Attributes().FirstOrDefault(a => a.Name.LocalName == "Grid.Row")?.Value ?? "0";
                    var colAttr = child.Attributes().FirstOrDefault(a => a.Name.LocalName == "Grid.Column")?.Value ?? "0";
                    var rowSpanAttr = child.Attributes().FirstOrDefault(a => a.Name.LocalName == "Grid.RowSpan")?.Value ?? "1";
                    var colSpanAttr = child.Attributes().FirstOrDefault(a => a.Name.LocalName == "Grid.ColumnSpan")?.Value ?? "1";

                    int.TryParse(rowAttr, out var row);
                    int.TryParse(colAttr, out var col);
                    int.TryParse(rowSpanAttr, out var rowSpan);
                    int.TryParse(colSpanAttr, out var colSpan);

                    sb.AppendLine($"            {currentVarName}.AddChild({childVarName}, {row}, {col}, {rowSpan}, {colSpan});");
                }
                else if (parentTag == "RelativePanel") {
                    sb.AppendLine($"            {currentVarName}.AddChild({childVarName});");
                }
                else if (parentTag == "StackPanel" || parentTag == "Canvas" || parentTag == "MenuBar" || parentTag == "Page") {
                    sb.AppendLine($"            {currentVarName}.AddChild({childVarName});");
                }
                else {
                    sb.AppendLine($"            ((global::NeoKolors.Tui.Dom.INode){currentVarName}).SetChildNode({childVarName});");
                }
            }
        }

        // Handle RelativePanel alignments at the end of RelativePanel processing
        if (tag == "RelativePanel") {
            foreach (var child in element.Elements()) {
                if (child.Name.LocalName.Contains('.')) continue;

                var childVarName = elementToVarName[child];

                var rightOfAttr = child.Attributes().FirstOrDefault(a => a.Name.LocalName == "RelativePanel.RightOf")?.Value;
                var belowAttr = child.Attributes().FirstOrDefault(a => a.Name.LocalName == "RelativePanel.Below")?.Value;
                var leftOfAttr = child.Attributes().FirstOrDefault(a => a.Name.LocalName == "RelativePanel.LeftOf")?.Value;
                var aboveAttr = child.Attributes().FirstOrDefault(a => a.Name.LocalName == "RelativePanel.Above")?.Value;

                if (!string.IsNullOrEmpty(rightOfAttr) && elementIdToVarName.TryGetValue(rightOfAttr!, out var rightTarget)) {
                    sb.AppendLine($"            {currentVarName}.SetRightOf({childVarName}, {rightTarget});");
                }
                if (!string.IsNullOrEmpty(belowAttr) && elementIdToVarName.TryGetValue(belowAttr!, out var belowTarget)) {
                    sb.AppendLine($"            {currentVarName}.SetBelow({childVarName}, {belowTarget});");
                }
                if (!string.IsNullOrEmpty(leftOfAttr) && elementIdToVarName.TryGetValue(leftOfAttr!, out var leftTarget)) {
                    sb.AppendLine($"            {currentVarName}.SetLeftOf({childVarName}, {leftTarget});");
                }
                if (!string.IsNullOrEmpty(aboveAttr) && elementIdToVarName.TryGetValue(aboveAttr!, out var aboveTarget)) {
                    sb.AppendLine($"            {currentVarName}.SetAbove({childVarName}, {aboveTarget});");
                }
            }
        }
    }

    private static string ToLiteral(string input) {
        return "@\"" + input.Replace("\"", "\"\"") + "\"";
    }
}

internal class XamlFileDescriptor {
    public string Path { get; }
    public string Content { get; }

    public XamlFileDescriptor(string path, string content) {
        Path = path;
        Content = content;
    }
}

internal class NamedElementInfo {
    public string Name { get; }
    public string TypeName { get; }

    public NamedElementInfo(string name, string typeName) {
        Name = name;
        TypeName = typeName;
    }
}
