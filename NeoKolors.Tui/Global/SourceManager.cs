//
// NeoKolors
// Copyright (c) 2025 KryKom
//
/*
using System.Reflection;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Styles;
using IStylePropertyV2 = NeoKolors.Tui.Styles.Properties.IStyleProperty;

namespace NeoKolors.Tui.SourceManagement;

/// <summary>
/// manages the assemblies that are a source of classes for the application 
/// </summary>
public static class SourceManager {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(SourceManager));
    
    private static readonly List<Assembly> ASSEMBLIES = [];
    private static readonly Dictionary<string, Type> STYLES = [];
    private static readonly Dictionary<string, Type> STYLES_V2 = [];
    private static readonly Dictionary<string, Type> ELEMENTS = [];

    static SourceManager() {
        LoadAssembly(Assembly.GetAssembly(typeof(SourceManager)) 
                     ?? throw new ApplicationException("Could not load the NeoKolors.Tui base assembly."));
    } 

    /// <summary>
    /// adds an assembly to the list of assemblies that are a source of
    /// classes (like styles and elements) for the application
    /// </summary>
    /// <param name="assembly">the assembly to be added</param>
    /// <exception cref="InvalidStyleNameException">a style with the same name already exists</exception>
    public static void LoadAssembly(Assembly assembly) {
        LOGGER.Debug($"Loading assembly {assembly.GetName().Name}");
        
        foreach (var t in assembly.GetTypes()) {
            if (IStylePropertyV2.IsStyle(t))
                AddStyle(t);
            else if (IElement.IsElement(t)) 
                AddElement(t);
        }
        
        ASSEMBLIES.Add(assembly);
    }

    private static void AddStyle(Type t) {
        var a = t.GetCustomAttribute<StylePropertyNameAttribute>();
                
        if (a is null) {
            string name = IStyleProperty.GetName(t);
                
            try {
                STYLES.Add(name, t);
            }
            catch (ArgumentException) {
                throw InvalidStyleNameException.Duplicate(name);
            }
                    
            LOGGER.Trace($"Registered style '{name}'.");
            return;
        }
                
        try {
            STYLES.Add(a.Name, t);
        }
        catch (ArgumentException) {
            throw InvalidStyleNameException.Duplicate(a.Name);
        }
        
        LOGGER.Trace($"Registered style '{a.Name}'.");
    }

    private static void AddStyleV2(Type t) {
        var a = t.GetCustomAttribute<StylePropertyNameAttribute>();
                
        if (a is null) {
            string name = IStylePropertyV2.GetName(t);
                
            try {
                STYLES_V2.Add(name, t);
            }
            catch (ArgumentException) {
                throw InvalidStyleNameException.Duplicate(name);
            }
                    
            LOGGER.Trace($"Registered style '{name}'.");
            return;
        }
                
        try {
            STYLES_V2.Add(a.Name, t);
        }
        catch (ArgumentException) {
            throw InvalidStyleNameException.Duplicate(a.Name);
        }
        
        LOGGER.Trace($"Registered style '{a.Name}'.");
    }
    
    private static void AddElement(Type t) {
        var a = t.GetCustomAttribute<ElementNameAttribute>();
                
        if (a is null) {
            string name = IElement.GetName(t);
                
            try {
                STYLES.Add(name, t);
            }
            catch (ArgumentException) {
                throw InvalidElementNameException.Duplicate(name);
            }
                 
            LOGGER.Trace($"Registered elementOld '{name}'.");
            return;
        }
                
        try {
            STYLES.Add(a.Name, t);
        }
        catch (ArgumentException) {
            throw InvalidElementNameException.Duplicate(a.Name);
        }

        LOGGER.Trace($"Registered elementOld '{a.Name}'.");
        _ = t.GetCustomAttribute<ApplicableStylesAttribute>();
    }

    public static Assembly[] GetAssemblies() => ASSEMBLIES.ToArray();
    
    /// <summary>
    /// returns the type of the style property with the given name
    /// </summary>
    /// <param name="name">the name of the style property</param>
    /// <exception cref="InvalidStyleNameException">style with the given name not found</exception>
    public static Type GetStyle(string name) {
        try {
            return STYLES[name];
        }
        catch (KeyNotFoundException) {
            throw InvalidStyleNameException.NotFound(name);
        }
    }

    /// <summary>
    /// Retrieves the type of the elementOld with the specified name.
    /// </summary>
    /// <param name="name">The name of the elementOld to retrieve.</param>
    /// <returns>The type of the elementOld associated with the given name.</returns>
    /// <exception cref="InvalidElementNameException">
    /// Thrown when an elementOld with the specified name cannot be found.
    /// </exception>
    public static Type GetElement(string name) {
        try {
            return ELEMENTS[name];
        }
        catch (KeyNotFoundException) {
            throw InvalidElementNameException.NotFound(name);
        }
    }
}*/