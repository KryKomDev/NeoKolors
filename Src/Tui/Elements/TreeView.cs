// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a node within a TreeView control.
/// </summary>
public class TreeViewNode {
    public string Header { get; set; }
    public bool IsExpanded { get; set; }
    public List<TreeViewNode> Children { get; } = new();
    public object? Tag { get; set; }

    public TreeViewNode(string? header) {
        Header = header ?? "";
    }
}

/// <summary>
/// A control that displays hierarchical data in a tree structure.
/// Shows standard connectors ├──/└── and folder toggles ►/▼.
/// </summary>
public class TreeView : Control<IReadOnlyList<TreeViewNode>>, INode<IReadOnlyList<TreeViewNode>> {
    
    public List<TreeViewNode> RootNodes { get; } = new();

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.Borderless,
        ReadOnly = true
    };

    public TreeView() : base(DefaultStyles) { }

    protected override Size MeasureOverride(Size availableSize) {
        int height = 0;
        int maxWidth = 0;

        foreach (var node in RootNodes) {
            MeasureNode(node, ref height, ref maxWidth, 0);
        }

        return new Size(maxWidth, height);
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;
        var contentPos = pos + RenderLayout.Content.Lower;
        var contentWidth = RenderLayout.Content.Width;

        int currentLine = 0;
        foreach (var rootNode in RootNodes) {
            RenderNode(canvas, rootNode, contentPos, contentWidth, ref currentLine, 0, "");
        }
    }

    private void RenderNode(
        ICharCanvas canvas, 
        TreeViewNode node, 
        Point startPoint, 
        int width, 
        ref int currentLine, 
        int depth, 
        string prefix) 
    {
        if (currentLine >= canvas.Height) return;

        var nodePos = startPoint + new Point(0, currentLine);

        // Draw node header with expansion toggle: e.g. "► System Settings" or "  ├── Guest"
        string toggleSymbol = "";
        if (node.Children.Count > 0) {
            toggleSymbol = node.IsExpanded ? "▼ " : "► ";
        }
        else if (depth > 0) {
            toggleSymbol = "  ";
        }

        string indent = prefix;
        string displayText = indent + toggleSymbol + node.Header;

        canvas.Place(displayText, nodePos, width, HorizontalAlign.LEFT);
        currentLine++;

        if (node.IsExpanded && node.Children.Count > 0) {
            for (int i = 0; i < node.Children.Count; i++) {
                bool isLast = i == node.Children.Count - 1;
                string childPrefix = depth == 0 ? "" : (isLast ? "└── " : "├── ");
                RenderNode(canvas, node.Children[i], startPoint, width, ref currentLine, depth + 1, prefix + childPrefix);
            }
        }
    }

    private void MeasureNode(TreeViewNode node, ref int height, ref int maxWidth, int depth) {
        height++;
        int nodeWidth = depth * 4 + 4 + node.Header.Length;
        maxWidth = Math.Max(maxWidth, nodeWidth);

        if (node.IsExpanded) {
            foreach (var child in node.Children) {
                MeasureNode(child, ref height, ref maxWidth, depth + 1);
            }
        }
    }

    public override ElementInfo Info { get; } = new();

    public override IReadOnlyList<TreeViewNode> GetChildNode() => RootNodes;

    public override void SetChildNode(IReadOnlyList<TreeViewNode> childNode) {
        RootNodes.Clear();
        if (childNode != null) {
            RootNodes.AddRange(childNode);
        }
        InvokeElementUpdated();
    }

    void INode.SetChildNode(object? child) {
        RootNodes.Clear();
        if (child == null) return;
        if (child is TreeViewNode node) {
            RootNodes.Add(node);
        }
        else if (child is IEnumerable<TreeViewNode> nodes) {
            RootNodes.AddRange(nodes);
        }
        InvokeElementUpdated();
    }
}
