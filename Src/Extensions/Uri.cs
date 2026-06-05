// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="Uri"/> instances and path strings.
/// </summary>
public static class UriExtensions {
    
    extension(Uri) {
        
        /// <summary>
        /// Determines whether the specified path or URI string is local (i.e., a file path or a path without scheme).
        /// </summary>
        /// <param name="path">The path or URI string to inspect.</param>
        /// <returns><c>true</c> if the path is local; otherwise, <c>false</c>.</returns>
        public static bool IsLocal(string path) {
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
                return uri.IsFile;
    
            // If it's not a valid absolute URI, check if it's a local path
            return Path.IsPathRooted(path) || !path.Contains("://");
        }
    }
}