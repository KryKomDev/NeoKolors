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
                catch { }
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
    Log.LogError("Could not find compiled CompileFontsTask flags in fonts assembly.");
    return false;
}

var taskInstance = Activator.CreateInstance(taskType);
taskType.GetProperty("BuildEngine")?.SetValue(taskInstance, BuildEngine);

if (!string.IsNullOrEmpty(SourceDir)) {
    taskType.GetProperty("SourceDir")?.SetValue(taskInstance, SourceDir);
}

if (!string.IsNullOrEmpty(OutputDir)) {
    taskType.GetProperty("OutputDir")?.SetValue(taskInstance, OutputDir);
}

if (Fonts != null) {
    var fontsProp = taskType.GetProperty("Fonts");
    
    if (fontsProp != null) {
        if (fontsProp.PropertyType.IsInstanceOfType(Fonts)) {
            fontsProp.SetValue(taskInstance, Fonts);
        }
        else {
            var targetElemType = fontsProp.PropertyType.GetElementType() ?? typeof(ITaskItem);
            var targetArray = Array.CreateInstance(targetElemType, Fonts.Length);
            
            for (int i = 0; i < Fonts.Length; i++) {
                targetArray.SetValue(Fonts[i], i);
            }
           
            fontsProp.SetValue(taskInstance, targetArray);
        }
    }
}

// Execute the compiled task
var executeMethod = taskType.GetMethod("Execute");
if (executeMethod == null) {
    Log.LogError("Execute method not found on CompileFontsTask.");
    return false;
}

bool success = false;
try {
    success = (bool)executeMethod.Invoke(taskInstance, null);
    
    if (success) {
        var outputFilesProp = taskType.GetProperty("OutputFiles");
        
        if (outputFilesProp != null) {
            var rawOutputs = outputFilesProp.GetValue(taskInstance);
            
            if (rawOutputs is ITaskItem[] directItems) {
                OutputFiles = directItems;
            } 
            else if (rawOutputs is System.Collections.IEnumerable enumerable) {
                var list = new List<ITaskItem>();
               
                foreach (var obj in enumerable) {
                    if (obj is ITaskItem ti) {
                        list.Add(ti);
                    } 
                    else if (obj != null) {
                        var itemSpecProp = obj.GetType().GetProperty("ItemSpec");
                        var itemSpec = itemSpecProp?.GetValue(obj)?.ToString() ?? string.Empty;
                        var taskItem = new Microsoft.Build.Utilities.TaskItem(itemSpec);
                        var cloneMethod = obj.GetType().GetMethod("CloneCustomMetadata");
                       
                        if (cloneMethod != null) {
                            var metaDict = cloneMethod.Invoke(obj, null) as System.Collections.IDictionary;
                          
                            if (metaDict != null) {
                                foreach (System.Collections.DictionaryEntry entry in metaDict) {
                                    taskItem.SetMetadata(entry.Key.ToString(), entry.Value?.ToString());
                                }
                            }
                        }
                        list.Add(taskItem);
                    }
                }
                OutputFiles = list.ToArray();
            }
        }
    }
} 
catch (Exception ex) {
    Log.LogError("Critical Error executing CompileFontsTask: " + ex.Message + "\n" + ex.StackTrace);
}

OutputFiles ??= [];

if (!success) {
    return false;
}
