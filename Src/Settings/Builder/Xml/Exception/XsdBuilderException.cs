// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Settings.Builder;

public class XsdBuilderException : System.Exception {
    private XsdBuilderException(string message) : base($"Could not build XSD. {message}") { }

    public static XsdBuilderException ArgumentXsdIncompatible(string name) =>
        new($"Argument '{name}' is XSD incompatible.");
}