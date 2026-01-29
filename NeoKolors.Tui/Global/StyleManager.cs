// NeoKolors
// Copyright (c) 2025 KryKom

using System.Reflection;
using NeoKolors.Tui.Styles.Properties;
using NameTypeTuple = (string Name, System.Type Type);

namespace NeoKolors.Tui.Global;

/// <summary>
/// Manages and provides access to a collection of styles within the application.
/// </summary>
/// <remarks>
/// The <c>StyleManager</c> class serves as a registry for storing and retrieving style types.
/// It maintains a static collection of styles and provides a method to retrieve them.
/// Logging support for the class is facilitated by the <c>NKLogger</c>.
/// </remarks>
public class StyleManager {
    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(StyleManager));
    
    private static readonly HashSet<NameTypeTuple> STYLES = new(new NameTypeTupleComparer());

    static StyleManager() {
        AssemblyManager.OnAssemblyLoaded += LoadStyles;
        
        foreach (var a in AssemblyManager.GetAssemblies()) {
            LoadStyles(a);
        }
    }
    
    private static void LoadStyles(Assembly assembly) {
        LOGGER.Info($"Loading styles from assembly '{assembly.GetName().Name}'.");
        foreach (var t in assembly.GetTypes()) {
            if (!IStyleProperty.IsStyle(t)) continue;
            
            STYLES.Add((t.Name, t));
            LOGGER.Trace($"Registered style '{t.Name}'.");
        }
    }

    public static Type? GetTypeByName(string name) =>
        STYLES.TryGetValue((name, null!), out var t) || STYLES.TryGetValue((name + "Property", null!), out t)
            ? t.Type : null;

    public static Type[] GetStyles() => STYLES.Select(t => t.Type).ToArray();
    
    private class NameTypeTupleComparer : IEqualityComparer<NameTypeTuple> {
        public bool Equals(NameTypeTuple x, NameTypeTuple y) => x.Name == y.Name;
        public int GetHashCode(NameTypeTuple obj) => obj.Name.GetHashCode();
    }
}