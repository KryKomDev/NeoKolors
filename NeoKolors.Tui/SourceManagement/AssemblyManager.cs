// NeoKolors
// Copyright (c) 2025 KryKom

using System.Reflection;

namespace NeoKolors.Tui.SourceManagement;

public static class AssemblyManager {
    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(AssemblyManager));
    
    private static readonly List<Assembly> ASSEMBLIES = [];
    
    public static event Action<Assembly>? OnAssemblyLoaded = delegate { };

    static AssemblyManager() {
        RegisterAssembly(Assembly.GetExecutingAssembly());
    }
    
    public static void RegisterAssembly(Assembly assembly) {
        ASSEMBLIES.Add(assembly);
        LOGGER.Info($"Registered assembly '{assembly.GetName().Name}'.");
        OnAssemblyLoaded?.Invoke(assembly);
    }

    public static Assembly[] GetAssemblies() => ASSEMBLIES.ToArray();
}