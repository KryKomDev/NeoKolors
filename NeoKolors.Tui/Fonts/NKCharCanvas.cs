// NeoKolors
// Copyright (c) 2025 KryKom

/*
using System.Runtime.CompilerServices;
using System.Text;
using Metriks;

namespace NeoKolors.Tui.Fonts;
/// <summary>
/// Represents a two-dimensional character canvas designed for rendering and manipulation of text elements.
/// </summary>
/// <remarks>
/// This class provides methods to handle the layout, writing, and clearing of characters on a canvas.
/// It serves as a default implementation of the ICharCanvas interface and operates on a fixed-origin grid structure.
/// </remarks>
public class NKCharCanvas : ICharCanvas {

    private readonly Plane<char?> _canvas = new();

    public char? this[int x, int y] => _canvas[x, y];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char? UncoordinatedGet(int x, int y) => _canvas.UncoordinatedGet(x, y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UncoordinatedSet(int x, int y, char? value) => _canvas.UncoordinatedSet(x, y, value);

    public char?[,] GetChars() => _canvas.ToArray();
    
    public int Width => _canvas.XSize;
    public int Height => _canvas.YSize;

    public string[] GetLines() {
        List<string> lines = [];

        for (int y = 0; y < _canvas.YSize; y++) {
            StringBuilder sb = new();
            
            for (int x = 0; x < _canvas.XSize; x++) {
                sb.Append(_canvas.UncoordinatedGet(x, y));
            }
            
            lines.Add(sb.ToString());
        }
        
        return lines.ToArray();
    }

    public void Write() {
        for (int y = 0; y < _canvas.YSize; y++) {
            StringBuilder sb = new();
            
            for (int x = 0; x < _canvas.XSize; x++) {
                sb.Append(_canvas.UncoordinatedGet(x, y) ?? ' ');
            }
            
            Stdio.WriteLine(sb.ToString());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _canvas.Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PlaceGlyph(int x, int y, IGlyph glyph) 
        => _canvas.Place(glyph.Glyph, new Point2D(x, y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PlaceCanvas(int x, int y, ICharCanvas canvas) 
        => _canvas.Place(canvas.GetChars(), new Point2D(x, y));

    public void PlaceString(int x, int y, string s) {
        var arr = new char?[s.Length, 1];

        for (int xi = 0; xi < s.Length; xi++) {
            arr[xi, 0] = s[xi];
        }
        
        _canvas.Place(arr, new Point2D(x, y));
    }
}
*/