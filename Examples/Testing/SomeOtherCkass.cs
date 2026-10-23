// NeoKolors
// Copyright (c) krystof 2026

using System.Diagnostics.CodeAnalysis;

namespace Testing;

public class SomeOtherCkass {
    [DoesNotReturn]
    public static void Crash() {
        var a = Convert.ToInt32("asda");
    }
}