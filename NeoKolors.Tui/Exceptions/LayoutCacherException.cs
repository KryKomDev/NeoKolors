// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Exceptions;

public sealed class LayoutCacherException : Exception {
    private LayoutCacherException(string message) : base(message) { }
    
    public static LayoutCacherException UnsetMax() => new("Cannot use compute layout cache. Layout is not set.");
    public static LayoutCacherException UnsetMin() => new("Cannot use min layout cache. Layout is not set.");
    public static LayoutCacherException UnsetRender() => new("Cannot use render layout cache. Layout is not set.");
}