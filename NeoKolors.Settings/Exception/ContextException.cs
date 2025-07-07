//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using JetBrains.Annotations;

namespace NeoKolors.Settings.Exception;

public class ContextException : System.Exception {

    private ContextException(string message) : base(message) { }

    [Pure]
    public static ContextException KeyNotFound(string key) => 
        new($"Could not return context value. The context key '{key}' was not found.");

    [Pure]
    public static ContextException KeyDuplicate(string key) => 
        new($"Could not add a new context value. The context key '{key}' is already exists.");

    [Pure]
    public static ContextException OutOfRange(int index, int length) =>
        new($"Could not return context value. The inputted index ({index}) is out of range. " +
            $"Index must be greater than or equal to 0 and less than {length}.");

    [Pure]
    public static ContextException InvalidType(Type expected, Type actual) =>
        new($"Tried to get context value of type '{expected.FullName}' " +
            $"but got '{actual.FullName}'.");

    [Pure]
    public static ContextException ContextLocked() =>
        new("Cannot add new context values. Context is locked.");
}