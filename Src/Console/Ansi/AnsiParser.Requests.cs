// NeoKolors
// Copyright (c) 2025 KryKom

// #define NK_DISABLE_ANSI_TIMEOUT

using System.Diagnostics.CodeAnalysis;
using System.Text;
using NeoKolors.Console.Events;

namespace NeoKolors.Console.Ansi;

/// <summary>
/// Provides functionality for parsing ANSI escape codes from VT input.
/// </summary>
/// <remarks>
/// The AnsiParser is a base class providing functionality for parsing the ANSI response escape sequences.
/// Implementing classes only have to implement the reading methods. 
/// </remarks>
public abstract partial class AnsiParser {
    private static readonly NKLogger LOGGER = NKDebug.GetLogger<AnsiParser>();
    
    private readonly LockObject _lock = new();
    private readonly TimeSpan   _timeout;
    private readonly TimeSpan   _retryInterval;

    protected AnsiParser(TimeSpan? timeout = null, TimeSpan? retryInterval = null) {
        _timeout       = timeout       ?? TimeSpan.FromMilliseconds(100);
        _retryInterval = retryInterval ?? TimeSpan.FromMilliseconds(1);
    }

    public ParserResult Parse(in VTQuery expected, out VTQuery? response) {
        LOGGER.Debug($"Trying to read VTQuery {expected.ToString()}.");
        
        var format = expected.Type switch {
            VTQueryType.INVALID    => throw new ArgumentException(),
            VTQueryType.DEC        => AnsiSequenceFormat.Dec,
            VTQueryType.OSC        => AnsiSequenceFormat.Osc,
            VTQueryType.WIN        => AnsiSequenceFormat.Win,
            VTQueryType.WIN_STATE  => AnsiSequenceFormat.WinState,
            VTQueryType.WIN_TITLE  => AnsiSequenceFormat.Title,
            VTQueryType.ICON_TITLE => AnsiSequenceFormat.Title,
            _                              => throw new ArgumentOutOfRangeException()
        };

        StringBuilder sb;
        
        lock (_lock) {
            if (!TryReadNext(out var escape)) {
                response = null;
                return ParserResult.TIMEOUT;
            }

            if (escape != '\e') {
                response = null;
                return ParserResult.INVALID;
            }
            
            if (ReadSequence(out response, format, out sb, out var parserResult)) 
                return parserResult;
        }
        
        var success = ExtractResult(sb.ToString(), in expected, out response);
        
        ReleaseRead(success);
        
        return success 
            ? ParserResult.SUCCESS 
            : ParserResult.WRONG_SEQUENCE;
    }

    private bool ReadSequence(
        out VTQuery?       response,
        AnsiSequenceFormat format, 
        out StringBuilder  sb,
        out ParserResult   parserResult) 
    {
        var match = AnsiSequenceFormat.MatchType.NO_MATCH;
        sb = new StringBuilder();
            
        while (match != AnsiSequenceFormat.MatchType.MATCH) {
            if (!TryReadNext(out var c)) {
                response = null;
                parserResult = ParserResult.INVALID;
                return true;
            }

            sb.Append(c);

            match = format.Matches(sb.ToString());

            if (match == AnsiSequenceFormat.MatchType.TOO_LONG) {
                LOGGER.Debug("Too long '{0}'", sb.ToString().Replace("\e", "\\e").Replace("\a", "\\a"));
                response = null;
                parserResult = ParserResult.INVALID;
                return true;
            }
        }

        // success, these are ignored
        response = null;
        parserResult = ParserResult.INVALID;
        return false;
    }

    private static bool ExtractResult(string s, in VTQuery expected, out VTQuery? response) {
        return expected.Type switch {
            VTQueryType.DEC        => ExtractResult_Dec(s, in expected, out response),
            VTQueryType.OSC        => ExtractResult_Osc(s, in expected, out response),
            VTQueryType.WIN        => ExtractResult_Win(s, in expected, out response),
            VTQueryType.WIN_STATE  => ExtractResult_WinState(s, out response),
            VTQueryType.WIN_TITLE  => ExtractResult_Title(s, in expected, out response),
            VTQueryType.ICON_TITLE => ExtractResult_Title(s, in expected, out response),
            VTQueryType.INVALID    => throw new ArgumentException(),
            _                              => throw new ArgumentOutOfRangeException()
        };
    }

    private static bool ExtractResult_Dec(string s, in VTQuery expected, out VTQuery? response) {
        var parts = s[2..^3].Split(';');
        
        if (parts.Length != 2) {
            response = null;
            // throw?
        }

        var mode = int.Parse(parts[0]);
        var res  = int.Parse(parts[1]);

        if (mode != (int)expected.DecMode) {
            response = null;
            return false;
        }

        response = expected with { DecResponse = (DecReqResponseType)res };
        return true;
    }
    
    // TODO: maybe create OSC response parser if there are any responses?
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private static bool ExtractResult_Osc(string s, in VTQuery expected, out VTQuery? response) {
        response = null;
        return false;
    }
    
    private static bool ExtractResult_Win(string s, in VTQuery expected, out VTQuery? response) {
        var parts = s[1..^1].Split(';');
        
        if (parts.Length != 3) {
            response = null;
            LOGGER.Error($"A suspicious sequence encountered: \\e{s}");
            return false;
            // throw?
        }

        var mode = int.Parse(parts[0]);
        var y    = int.Parse(parts[1]);
        var x    = int.Parse(parts[2]);

        if (!(
            mode == (int)expected.WinMode      || 
            mode == (int)expected.WinMode - 10 || 
            mode == (int)expected.WinMode - 9
        )) {
            response = null;
            return false;
        }

        response = expected with { WinResponse = (x, y) };
        return true;
    }
    
    private static bool ExtractResult_Title(string s, in VTQuery expected, out VTQuery? response) {
        var type  = s[1];
        var title = s[2..^1];

        (VTQuery? Res, bool Ret) a = (type, expected.Type) switch {
            ('l', VTQueryType.WIN_TITLE)  => (VTQuery.WinTitle (title), true),
            ('L', VTQueryType.ICON_TITLE) => (VTQuery.IconTitle(title), true),
            _                             => (null,                     false)
        };
        
        response = a.Res;
        return a.Ret;
    }
    
    private static bool ExtractResult_WinState(string s, out VTQuery? response) {
        (VTQuery? Res, bool Ret) a = s switch {
            "[1t" => (VTQuery.WinState(false), true),
            "[2t" => (VTQuery.WinState(true),  true),
            _     => (null, false)
        };

        response = a.Res;
        return a.Ret;
    }

    public enum ParserResult : byte {
        SUCCESS = 0,
        INVALID = 1,
        TIMEOUT = 2,
        WRONG_SEQUENCE = 3,
    }
}