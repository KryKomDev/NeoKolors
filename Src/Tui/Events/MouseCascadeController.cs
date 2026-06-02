// NeoKolors
// Copyright (c) 2026 KryKom

using System.Reflection;
using NeoKolors.Console.Input;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Events;

/// <summary>
/// Controls the propagation of mouse events (clicks, releases, hovers) down the element tree (Mouse Cascade).
/// </summary>
public class MouseCascadeController {
    private readonly IApplication _app;
    private List<IInteractableElement> _lastHoveredPath = new();
    private static readonly Dictionary<(Type, string), FieldInfo?> FIELD_CACHE = new();

    public MouseCascadeController(IApplication app) {
        _app = app;
    }

    /// <summary>
    /// Handles an incoming mouse event, performing hit-testing and invoking target interactable elements.
    /// </summary>
    public void HandleMouseEvent(MouseEventArgs m) {
        var root = _app.Base as IElement;
        if (root == null) return;

        var hit = HitTest(root, m.Position.X, m.Position.Y);
        
        // Build the path of interactable elements from the hit element up to the root
        var currentPath = new List<IInteractableElement>();
        if (hit != null) {
            var fullPath = new List<IElement>();
            if (FindPath(root, hit, fullPath)) {
                foreach (var el in fullPath) {
                    if (el is IInteractableElement ie) {
                        currentPath.Add(ie);
                    }
                }
            }
        }

        // Handle Hover / HoverOut
        // Trigger OnHoverOut for elements in the previous path that are not in the current path
        foreach (var ie in _lastHoveredPath) {
            if (!currentPath.Contains(ie)) {
                RaiseEvent<Action>(ie, "OnHoverOut");
                ie.IsHovered = false;
            }
        }

        // Trigger OnHover for elements in the current path that were not in the previous path
        foreach (var ie in currentPath) {
            if (!_lastHoveredPath.Contains(ie)) {
                RaiseEvent<Action>(ie, "OnHover");
                ie.IsHovered = true;
            }
        }

        _lastHoveredPath = currentPath;

        // Handle Click (Press)
        if (m.IsPress) {
            foreach (var ie in currentPath) {
                RaiseEvent<Action<MouseButton>>(ie, "OnClick", m.Button);
            }
        }

        // Handle Release
        if (m.Released || m.IsRelease) {
            foreach (var ie in currentPath) {
                RaiseEvent<Action<MouseButton>>(ie, "OnRelease", m.Button);
            }
        }
    }

    /// <summary>
    /// Recursively performs a hit-test to find the deepest visible child element under the specified coordinate.
    /// </summary>
    public static IElement? HitTest(IElement element, int x, int y) {
        // Skip invisible elements
        if (!element.Style.Get<VisibleProperty>().Value) {
            return null;
        }

        // Skip if coordinate is outside the element's RenderBounds
        if (!element.RenderBounds.Contains(x, y)) {
            return null;
        }

        // Traverse children in reverse order (topmost first)
        var childNode = element.GetChildNode();
        if (childNode is IElement child) {
            var hit = HitTest(child, x, y);
            if (hit != null) return hit;
        }
        else if (childNode is IEnumerable<IElement> children) {
            var childList = children.Where(c => c != null).ToList();
            for (int i = childList.Count - 1; i >= 0; i--) {
                var hit = HitTest(childList[i], x, y);
                if (hit != null) return hit;
            }
        }

        return element;
    }

    /// <summary>
    /// Recursively finds the visual path from a parent/root to a target element.
    /// </summary>
    public static bool FindPath(IElement current, IElement target, List<IElement> path) {
        if (current == target) {
            path.Add(current);
            return true;
        }

        path.Add(current);
        var childNode = current.GetChildNode();
        if (childNode is IElement child) {
            if (FindPath(child, target, path)) return true;
        }
        else if (childNode is IEnumerable<IElement> children) {
            foreach (var c in children) {
                if (c != null && FindPath(c, target, path)) return true;
            }
        }

        path.RemoveAt(path.Count - 1);
        return false;
    }

    private static void RaiseEvent<TDelegate>(object target, string eventName, params object[] args) where TDelegate : Delegate {
        var type = target.GetType();
        var key = (type, eventName);
        if (!FIELD_CACHE.TryGetValue(key, out var field)) {
            var currentType = type;
            while (currentType != null) {
                field = currentType.GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (field != null) break;
                currentType = currentType.BaseType;
            }
            FIELD_CACHE[key] = field;
        }

        if (field != null) {
            var del = field.GetValue(target) as TDelegate;
            del?.DynamicInvoke(args);
        }
    }
}
