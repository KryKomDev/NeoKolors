// NeoKolors
// Copyright (c) 2025 KryKom

using JetBrains.Annotations;

namespace NeoKolors.Extensions;

public static class UriExtensions {
    
    extension(Uri) {
        
        public static bool IsLocal(string path) {
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
                return uri.IsFile;
    
            // If it's not a valid absolute URI, check if it's a local path
            return Path.IsPathRooted(path) || !path.Contains("://");
        }
    }
}