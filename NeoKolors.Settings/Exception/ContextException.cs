//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Exception;

public class ContextException : System.Exception {

    private ContextException(string message) : base(message) { }

    public static ContextException KeyNotFound(string key) {
        return new ContextException($"Could not return context value. The context key '{key}' was not found.");
    }

    public static ContextException KeyDuplicate(string key) {
        return new ContextException($"Could not add a new context value. The context key '{key}' is already exists.");
    }

    public static ContextException OutOfRange(int index, int length) {
        return new ContextException($"Could not return context value. The inputted index ({index}) is out of range. " +
                                    $"Index must be greater than or equal to 0 and less than {length}.");
    }
}