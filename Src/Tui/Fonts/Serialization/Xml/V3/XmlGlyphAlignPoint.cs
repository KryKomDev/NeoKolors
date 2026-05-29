// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using Metriks;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

public readonly struct XmlGlyphAlignPoint {
    
    public Point2D Position { get; }
    public char    Id       { get; }

    public XmlGlyphAlignPoint(char id, Point2D point) {
        Position = point;
        Id       = id;
    }

    public static XmlGlyphAlignPoint[] ParseArray(string value) {
        if (string.IsNullOrEmpty(value)) {
            NKFontSerializer.LOGGER.Error("The value for AlignPoints is empty or null.");
            return [];
        }

        value = value.Trim();

        List<XmlGlyphAlignPoint> points = [];
        int start = 0;
        
        for (int i = 0; i < value.Length; i++) {
            if (value[i] is not ')')
                continue;

            var pointString = value.Substring(start, i - start);
            var point = Parse(pointString);

            if (point == null) {
                NKFontSerializer.LOGGER.Error($"The value '{value}' is not valid for AlignPoints.");
                return [];
            }
            
            points.Add(point.Value);

            for (int j = i; j < value.Length; j++) {
                if (value[j] is ' ' or ',')
                    continue;

                start = i = j + 1;
            }
        }

        return points.ToArray();
    }

    public static XmlGlyphAlignPoint? Parse(string value) {
        if (string.IsNullOrEmpty(value)) {
            NKFontSerializer.LOGGER.Error("The value for AlignPoints is empty or null.");
            return null;
        }

        if (value.Length < 5) {
            NKFontSerializer.LOGGER.Error($"The value '{value}' is not valid for AlignPoints.");
            return null;
        }
        
        var c = value[0];
        var coords = value[2..^2].Split(',');

        if (coords.Length == 2 &&
            int.TryParse(coords[0].Trim(), out var x) &&
            int.TryParse(coords[1].Trim(), out var y)
        ) return new XmlGlyphAlignPoint(c, new Point2D(x, y));
        
        NKFontSerializer.LOGGER.Error($"The value '{value}' is not valid for AlignPoints.");
        return null;
    }
}