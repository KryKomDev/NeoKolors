// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Fonts.Serialization;

namespace NeoKolors.Tui.Fonts;

public static class FontAtlas {
    private static readonly Dictionary<string, IFont> FONTS = new();

    static FontAtlas() {
        LoadBuiltin();
    }
    
    private static void LoadBuiltin() {
        Add("Default", IFont.Default);
        AddInternal("Bytesized");
        AddInternal("Dummy1");
        AddInternal("Future");
    }

    private static void AddInternal(string name) => Add(name, NKFontSerializer.ReadInternal(name) ?? IFont.Default);

    public static void Add(string key, IFont value) => FONTS.Add(key, value);
    public static IFont Get(string key) => FONTS[key];
    public static bool ContainsFont(string key) => FONTS.ContainsKey(key);
    public static bool ContainsValue(IFont value) => FONTS.ContainsValue(value);
    public static bool TryAdd(string key, IFont value) => FONTS.TryAdd(key, value);
    public static bool TryGet(string key, out IFont? value) => FONTS.TryGetValue(key, out value);

    public static int Count => FONTS.Count;
    public static Dictionary<string, IFont>.KeyCollection Keys => FONTS.Keys;
    public static Dictionary<string, IFont>.ValueCollection Values => FONTS.Values;
}