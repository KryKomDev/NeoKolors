//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Attributes;

namespace NeoKolors.Settings;

public static class Extensions {
    public static string GetDisplayName(this Type t) => 
        DisplayTypeAttribute.GetName(t);
}