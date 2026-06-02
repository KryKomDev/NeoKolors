// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A layout panel that enables positioning of child elements relative to other sibling elements or the panel container.
/// </summary>
public class RelativePanel : Panel {
    
    private readonly Dictionary<IElement, IElement> _rightOf = new();
    private readonly Dictionary<IElement, IElement> _below = new();
    private readonly Dictionary<IElement, IElement> _leftOf = new();
    private readonly Dictionary<IElement, IElement> _above = new();

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        ReadOnly = true
    };

    public RelativePanel() : base(DefaultStyles) { }

    public void SetRightOf(IElement child, IElement target) {
        _rightOf[child] = target;
        InvokeElementUpdated();
    }

    public void SetBelow(IElement child, IElement target) {
        _below[child] = target;
        InvokeElementUpdated();
    }

    public void SetLeftOf(IElement child, IElement target) {
        _leftOf[child] = target;
        InvokeElementUpdated();
    }

    public void SetAbove(IElement child, IElement target) {
        _above[child] = target;
        InvokeElementUpdated();
    }

    protected override Size MeasureOverride(Size availableSize) {
        var resolved = ResolveLayouts(availableSize);
        int maxWidth = 0;
        int maxHeight = 0;

        foreach (var childRect in resolved.Values) {
            maxWidth = Math.Max(maxWidth, childRect.LowerX + childRect.Width);
            maxHeight = Math.Max(maxHeight, childRect.LowerY + childRect.Height);
        }

        return new Size(maxWidth, maxHeight);
    }

    protected override Size ArrangeOverride(Size finalSize) {
        var pos = RenderBounds.Lower;
        var resolvedPositions = ResolveLayouts(RenderLayout.Content.Size);

        foreach (var child in _children) {
            if (child == null) continue;
            if (resolvedPositions.TryGetValue(child, out var childRect)) {
                var childPos = pos + RenderLayout.Content.Lower + childRect.Lower;
                child.Arrange(new Rectangle(childPos, childRect.Size));
            }
        }
        return finalSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        foreach (var child in _children) {
            child?.Render(canvas);
        }
    }

    private Dictionary<IElement, Rectangle> ResolveLayouts(Size viewport) {
        var resolved = new Dictionary<IElement, Rectangle>();
        var visiting = new HashSet<IElement>();

        foreach (var child in _children) {
            if (child == null) continue;
            ResolveChild(child, viewport, resolved, visiting);
        }

        return resolved;
    }

    private void ResolveChild(
        IElement child, 
        Size viewport, 
        Dictionary<IElement, Rectangle> resolved, 
        HashSet<IElement> visiting) 
    {
        if (resolved.ContainsKey(child)) return;

        child.Measure(viewport);
        var size = child.DesiredSize;

        if (!visiting.Add(child)) {
            resolved[child] = new Rectangle(Point.Zero, size);
            return;
        }

        int x = 0;
        int y = 0;

        if (_rightOf.TryGetValue(child, out var rightOfTarget) && _children.Contains(rightOfTarget)) {
            ResolveChild(rightOfTarget, viewport, resolved, visiting);
            if (resolved.TryGetValue(rightOfTarget, out var targetRect)) {
                x = targetRect.LowerX + targetRect.Width;
            }
        }
        else if (_leftOf.TryGetValue(child, out var leftOfTarget) && _children.Contains(leftOfTarget)) {
            ResolveChild(leftOfTarget, viewport, resolved, visiting);
            if (resolved.TryGetValue(leftOfTarget, out var targetRect)) {
                x = targetRect.LowerX - size.Width;
            }
        }

        if (_below.TryGetValue(child, out var belowTarget) && _children.Contains(belowTarget)) {
            ResolveChild(belowTarget, viewport, resolved, visiting);
            if (resolved.TryGetValue(belowTarget, out var targetRect)) {
                y = targetRect.LowerY + targetRect.Height;
            }
        }
        else if (_above.TryGetValue(child, out var aboveTarget) && _children.Contains(aboveTarget)) {
            ResolveChild(aboveTarget, viewport, resolved, visiting);
            if (resolved.TryGetValue(aboveTarget, out var targetRect)) {
                y = targetRect.LowerY - size.Height;
            }
        }

        resolved[child] = new Rectangle(new Point(x, y), size);
        visiting.Remove(child);
    }

    public override ElementInfo Info { get; } = new();
}
