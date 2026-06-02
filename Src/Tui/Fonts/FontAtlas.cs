// NeoKolors
// Copyright (c) 2026 KryKom

using System.Reflection;
using NeoKolors.Console;
using NeoKolors.Tui.Fonts.Serialization;

namespace NeoKolors.Tui.Fonts;

public static class FontAtlas {
    private static readonly NKLogger LOGGER = NKDebug.GetLogger("FontAtlas");
    
    private static readonly Dictionary<string, IAsciiFont> FONTS = new();

    static FontAtlas() {
        foreach (var res in Assembly.GetCallingAssembly().GetManifestResourceNames()) {
            LOGGER.Debug($"Found resource: '{res}'");
        }
        
        LoadBuiltin();
    }
    
    private static void LoadBuiltin() {
        Add("Default", IAsciiFont.Default);
        Add("Jitter",  new JitterFont());
    }

    private static void AddInternal(string name) => Add(name, NKFontSerializer.ReadInternal(name) ?? IAsciiFont.Default);

    public static void Add(string key, IAsciiFont value) {
        LOGGER.Info($"Adding '{key}'.");
        FONTS.Add(key, value);
    }

    public static IAsciiFont Get(string key) => FONTS[key];
    public static bool ContainsFont(string key) => FONTS.ContainsKey(key);
    public static bool ContainsValue(IAsciiFont value) => FONTS.ContainsValue(value);
    
    public static bool TryAdd(string key, IAsciiFont value) {
        if (FONTS.TryAdd(key, value)) {
            LOGGER.Info($"Added '{key}'.");
            return true;
        }

        LOGGER.Warn($"Could not add '{key}'.");
        return false;
    }

    public static bool TryGet(string key, out IAsciiFont? value) => FONTS.TryGetValue(key, out value);

    public static int Count => FONTS.Count;
    public static Dictionary<string, IAsciiFont>.KeyCollection Keys => FONTS.Keys;
    public static Dictionary<string, IAsciiFont>.ValueCollection Values => FONTS.Values;
}