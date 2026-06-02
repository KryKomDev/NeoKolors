//
// NeoKolors 
// Copyright (c) KryKom 2026
//

var assemblyPath = Path.GetFullPath(FontsAssemblyPath);
if (!File.Exists(assemblyPath)) {
    Log.LogError("Fonts assembly not found at: " + assemblyPath);
    return false;
}

// Index all resolved references by their assembly name
var refMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

foreach (var item in References) {
    var path = item.ItemSpec;
    var name = Path.GetFileNameWithoutExtension(path);
    refMap[name] = path;
}

// Resolve dependencies dynamically using the indexed references to avoid locking files on disk
AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
    var name = new AssemblyName(args.Name).Name;
    
    if (refMap.TryGetValue(name, out var path)) {
        try {
            var bytes = File.ReadAllBytes(path);
            
            return Assembly.Load(bytes);
        }
        catch { }
    }
    
    return null;
};

try {
    var alcType = Type.GetType("System.Runtime.Loader.AssemblyLoadContext, System.Runtime.Loader");
    
    if (alcType != null) {
        var currentAlc = alcType.GetMethod("GetLoadContext", new[] { typeof(Assembly) }).Invoke(null, new[] { Assembly.GetExecutingAssembly() });
        var resolvingEvent = alcType.GetEvent("Resolving");
        
        Func<object, AssemblyName, Assembly> resolveHandler = (alc, assemblyName) => {
            var name = assemblyName.Name;
            
            if (refMap.TryGetValue(name, out var path)) {
                try {
                    var bytes = File.ReadAllBytes(path);
                    var loadFromStream = alcType.GetMethod("LoadFromStream", new[] { typeof(Stream) });

                    using (var stream = new MemoryStream(bytes)) {
                        return (Assembly)loadFromStream.Invoke(alc, new[] { stream });
                    }
                }
                catch {
                    
                }
            }
            
            return null;
        };

        var delegateHandler = Delegate.CreateDelegate(resolvingEvent.EventHandlerType, resolveHandler.Target, resolveHandler.Method);
        resolvingEvent.AddMethod.Invoke(currentAlc, new[] { delegateHandler });
    }
} 
catch { }

// Load core assembly in-memory from bytes (no lock!)
var assemblyBytes = File.ReadAllBytes(assemblyPath);
var assembly = Assembly.Load(assemblyBytes);

// Reflectively instantiate the compiled CompileFontsTask
var taskType = assembly.GetType("NeoKolors.Tui.Fonts.Build.CompileFontsTask");

if (taskType == null) {
    Log.LogError("Could not find compiled CompileFontsTask type in fonts assembly.");
    return false;
}

var taskInstance = Activator.CreateInstance(taskType);
taskType.GetProperty("BuildEngine").SetValue(taskInstance, BuildEngine);
taskType.GetProperty("SourceDir").SetValue(taskInstance, SourceDir);

// Execute the compiled task
var executeMethod = taskType.GetMethod("Execute");
if (executeMethod == null) {
    Log.LogError("Execute method not found on CompileFontsTask.");
    return false;
}

bool success = false;
try {
    success = (bool)executeMethod.Invoke(taskInstance, null);
} 
catch (Exception ex) {
    Log.LogError("Critical Error executing CompileFontsTask: " + ex.Message + "\n" + ex.StackTrace);
}

if (!success) {
    return false;
}
