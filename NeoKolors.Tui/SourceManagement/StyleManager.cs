// NeoKolors
// Copyright (c) 2025 KryKom

using System.Reflection;
using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.SourceManagement;

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
    
    private static readonly HashSet<Type> STYLES = [];

    static StyleManager() {
        AssemblyManager.OnAssemblyLoaded += LoadStyles;
    }
    
    private static void LoadStyles(Assembly assembly) {
        LOGGER.Info($"Loading styles from assembly '{assembly.GetName().Name}'.");
        foreach (var t in assembly.GetTypes()) {
            if (!IStyleProperty.IsStyle(t)) continue;
            
            STYLES.Add(t);
            LOGGER.Trace($"Registered style '{t.Name}'.");
        }
    }
    
    public static Type[] GetStyles() => STYLES.ToArray();
}