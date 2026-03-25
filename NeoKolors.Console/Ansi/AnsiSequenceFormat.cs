//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace NeoKolors.Console.Ansi;

/// <summary>
/// Represents a format used to validate and match ANSI sequences based on a defined pattern and maximum length.
/// </summary>
internal record AnsiSequenceFormat {
    private readonly string _format;
    private readonly string _terminator;
    private readonly int    _maxLength;
    
    private AnsiSequenceFormat([StringSyntax(StringSyntaxAttribute.Regex)] 
        string format, 
        int    maxLength, 
        string terminator)
    {
        _format     = format;
        _maxLength  = maxLength;
        _terminator = terminator;
    }

    /// <summary>
    /// Determines whether the specified string matches the ANSI sequence format
    /// based on the defined pattern and maximum length.
    /// </summary>
    /// <param name="value">
    /// The string value to be validated against the ANSI sequence format.
    /// The input value should not contain the starting escape character.
    /// </param>
    /// <returns>
    /// True if the string matches the format and its length is within the defined maximum; otherwise, false.
    /// </returns>
    public MatchType Matches(string value) {
        if (string.IsNullOrEmpty(value))
            return MatchType.NO_MATCH;
        
        if (value.Length > _maxLength)
            return MatchType.TOO_LONG; 
        
        return value.EndsWith(_terminator) && Regex.IsMatch(value[..^_terminator.Length], _format)
            ? MatchType.MATCH
            : MatchType.NO_MATCH;
    }
    
    public enum MatchType {
        MATCH,
        NO_MATCH,
        TOO_LONG
    }

    private const string DEC_FORMAT       = @"\[?\d+;[12]";
    private const string OSC_FORMAT       = @"\]\d+;.+?";
    private const string WIN_FORMAT       = @"\[\d;\d+;\d+";
    private const string WIN_STATE_FORMAT = @"\[[12]";
    private const string TITLE_FORMAT     = @"\][Ll].+?";

    public static AnsiSequenceFormat Dec      { get; } = new(DEC_FORMAT,       9,            "$y");
    public static AnsiSequenceFormat Osc      { get; } = new(OSC_FORMAT,       int.MaxValue, "\a");
    public static AnsiSequenceFormat Win      { get; } = new(WIN_FORMAT,       25,           "t");
    public static AnsiSequenceFormat WinState { get; } = new(WIN_STATE_FORMAT, 4,            "t");
    public static AnsiSequenceFormat Title    { get; } = new(TITLE_FORMAT,     int.MaxValue, "\a");
}