﻿namespace NeoKolors.ConsoleGraphics.TUI.Style;

public class VerticalAlignProperty : IStyleProperty<VerticalAlignData> {
    public VerticalAlignData Value { get; }
    public static string GetStaticName() => "vertical-align";
    public string GetName() => GetStaticName();
    public static VerticalAlignData GetStaticDefault() => VerticalAlignDirection.TOP;
    public VerticalAlignData GetDefault() => GetStaticDefault();
    public VerticalAlignProperty(VerticalAlignDirection value) => Value = value;
}

public class HorizontalAlignProperty : IStyleProperty<HorizontalAlignData> {
    public HorizontalAlignData Value { get; }
    public static string GetStaticName() => "horizontal-align";
    public string GetName() => GetStaticName();
    public static HorizontalAlignData GetStaticDefault() => HorizontalAlignDirection.LEFT;
    public HorizontalAlignData GetDefault() => GetStaticDefault();
    public HorizontalAlignProperty(HorizontalAlignDirection value) => Value = value;
}

public class VerticalAlignItemsProperty : IStyleProperty<VerticalAlignData> {
    public VerticalAlignData Value { get; }
    public static string GetStaticName() => "vertical-align";
    public string GetName() => GetStaticName();
    public static VerticalAlignData GetStaticDefault() => VerticalAlignDirection.TOP;
    public VerticalAlignData GetDefault() => GetStaticDefault();
    public VerticalAlignItemsProperty(VerticalAlignDirection value) => Value = value;
}

public class HorizontalAlignItemsProperty : IStyleProperty<HorizontalAlignData> {
    public HorizontalAlignData Value { get; }
    public static string GetStaticName() => "horizontal-align-items";
    public string GetName() => GetStaticName();
    public static HorizontalAlignData GetStaticDefault() => HorizontalAlignDirection.LEFT;
    public HorizontalAlignData GetDefault() => GetStaticDefault();
    public HorizontalAlignItemsProperty(HorizontalAlignDirection value) => Value = value;
}

public class HorizontalAlignData {
    public HorizontalAlignDirection Data { get; }
    public HorizontalAlignData(HorizontalAlignDirection data) => Data = data;
    public static implicit operator HorizontalAlignData(HorizontalAlignDirection value) => new(value);
    public static implicit operator HorizontalAlignDirection(HorizontalAlignData value) => value.Data;
}

public class VerticalAlignData {
    public VerticalAlignDirection Data { get; }
    public VerticalAlignData(VerticalAlignDirection data) => Data = data;
    public static implicit operator VerticalAlignData(VerticalAlignDirection value) => new(value);
    public static implicit operator VerticalAlignDirection(VerticalAlignData value) => value.Data;
}

public enum HorizontalAlignDirection {
    LEFT,
    CENTER,
    RIGHT
}

public enum VerticalAlignDirection {
    TOP,
    CENTER,
    BOTTOM
}