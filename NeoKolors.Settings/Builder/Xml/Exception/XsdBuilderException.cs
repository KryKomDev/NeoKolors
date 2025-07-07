// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Settings.Builder.Xml.Exception;

using System;

public class XsdBuilderException : Exception {
    private XsdBuilderException(string message) : base($"Could not build XSD. {message}") { }

    public static XsdBuilderException ArgumentXsdIncompatible(string name) =>
        new($"Argument '{name}' is XSD incompatible.");
}