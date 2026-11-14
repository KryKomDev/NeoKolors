//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.Runtime.InteropServices;
using NeoKolors.Common;

namespace NeoKolors.Console.Ansi;

/// <summary>
///     Represents a response or a query to a Virtual Terminal (VT).
///     This struct encapsulates the type of the response, data associated with the response,
///     and specific mode information when applicable.
/// </summary>
public readonly struct VTQuery : IEquatable<VTQuery> {

    private readonly VTQueryType _type;
    private readonly int         _mode;
    private readonly nint        _response;
    private readonly nint        _args;

    private object? GetResponse() {
        if (_response == nint.Zero)
            return null;

        var handle = GCHandle.FromIntPtr(_response);

        return handle.Target;
    }

    private int[]? GetArgs() {
        if (_args == nint.Zero)
            return null;

        var handle = GCHandle.FromIntPtr(_args);

        return (int[]?)handle.Target;
    }

    private object? Response {
        get => GetResponse();
        init {
            if (value == null) {
                _response = nint.Zero;
            }
            else {
                var handle = GCHandle.Alloc(value);
                _response = GCHandle.ToIntPtr(handle);
            }
        }
    }

    private int[]? Args {
        get => GetArgs();
        init {
            if (value == null || value.Length == 0) {
                _args = nint.Zero;
            }
            else {
                var handle = GCHandle.Alloc(value);
                _args = GCHandle.ToIntPtr(handle);
            }
        }
    }

    public VTQueryType Type => _type;

    public IReadOnlyList<int> Arguments => GetArgs() ?? [];

    public DecReqResponseType DecResponse {
        get =>
            Type is VTQueryType.DEC
                ? (DecReqResponseType)(Response ?? throw new InvalidOperationException("The query has no response."))
                : throw new InvalidOperationException($"Cannot convert a {_type} query to a DEC query response.");
        init {
            if (Type != VTQueryType.DEC)
                throw new InvalidOperationException($"Cannot convert a {_type} query to a DEC query response.");

            Response = value;
        }
    }

    public EscapeCodes.DecMode DecMode =>
        Type == VTQueryType.DEC
            ? (EscapeCodes.DecMode)_mode
            : throw new InvalidOperationException($"Cannot convert a {_type} query to a DEC query response.");

    public object? OscResponse {
        get =>
            Type is VTQueryType.OSC
                ? Response
                : throw new InvalidOperationException($"Cannot convert a {_type} query to an OSC query response.");
        init {
            if (Type != VTQueryType.OSC)
                throw new InvalidOperationException($"Cannot convert a {_type} query to an OSC query response.");

            Response = value;
        }
    }

    public EscapeCodes.OscMode OscMode =>
        Type == VTQueryType.OSC
            ? (EscapeCodes.OscMode)_mode
            : throw new InvalidOperationException($"Cannot convert a {_type} query to an OSC query response.");

    public (int Index, NKColor Color) RedefineColorResponse {
        get {
            if (Type != VTQueryType.OSC || OscMode != EscapeCodes.OscMode.REDEFINE_COLOR)
                throw new InvalidOperationException($"Cannot convert a {_type} query to a RedefineColor query response.");

            return Response switch {
                ValueTuple<int, NKColor> pair => pair,
                NKColor color                 => (Arguments.Count > 0 ? Arguments[0] : 0, color),
                _                             => throw new InvalidOperationException("The query has no valid RedefineColor response.")
            };
        }
    }

    public IReadOnlyDictionary<int, NKColor> RedefineColorsResponse {
        get {
            if (Type != VTQueryType.OSC || OscMode != EscapeCodes.OscMode.REDEFINE_COLOR)
                throw new InvalidOperationException($"Cannot convert a {_type} query to a RedefineColors query response.");

            return Response switch {
                IReadOnlyDictionary<int, NKColor> dict => dict,
                ValueTuple<int, NKColor> pair          => new Dictionary<int, NKColor> { { pair.Item1, pair.Item2 } },
                _                                      => throw new InvalidOperationException("The query has no valid RedefineColors response.")
            };
        }
    }

    public NKColor ColorResponse => RedefineColorResponse.Color;

    public (int X, int Y) WinResponse {
        get =>
            Type is VTQueryType.WIN
                ? ((int X, int Y))(Response ?? throw new InvalidOperationException("The query has no response."))
                : throw new InvalidOperationException($"Cannot convert a {_type} query to a WinOpt query response.");
        init {
            if (Type != VTQueryType.WIN)
                throw new InvalidOperationException($"Cannot convert a {_type} query to a WinOpt query response.");

            Response = value;
        }
    }

    public EscapeCodes.WinOpts WinMode =>
        Type == VTQueryType.WIN
            ? (EscapeCodes.WinOpts)_mode
            : throw new InvalidOperationException($"Cannot convert a {_type} query to an WinOpt query response.");

    public PDAResponse? PrimaryDAResponse =>
        Type == VTQueryType.PRIMARY_DA
            ? (PDAResponse?)(Response ?? throw new InvalidOperationException("The query has no response."))
            : throw new InvalidOperationException($"Cannot convert a {_type} query to a PrimaryDA query response.");


    public VTQuery() {
        _type     = VTQueryType.INVALID;
        _mode     = 0;
        _response = nint.Zero;
        _args     = nint.Zero;
    }

    private VTQuery(VTQueryType type, int mode, object? data = null, int[]? args = null) {
        _type    = type;
        _mode    = mode;
        Response = data;
        Args     = args;
    }

    private VTQuery(VTQueryType type, object? data = null, int[]? args = null) {
        _type    = type;
        _mode    = 0;
        Response = data;
        Args     = args;
    }

    private T As<T>(VTQueryType type) =>
        _type == type
            ? (T)(Response ?? throw new InvalidOperationException("The query has no response."))
            : throw new InvalidOperationException($"Cannot convert a {_type} query to a {_type} query response.");

    public bool   AsWinState  => As<bool>(VTQueryType.WIN_STATE);
    public string AsWinTitle  => As<string>(VTQueryType.WIN_TITLE);
    public string AsIconTitle => As<string>(VTQueryType.ICON_TITLE);

    public string GetEscSeq() {
        return _type switch {
            VTQueryType.INVALID      => throw new InvalidOperationException(),
            VTQueryType.DEC          => $"\e[?{_mode}$p",
            VTQueryType.OSC          => FormatOscSeq(),
            VTQueryType.WIN          => $"\e[{_mode}t",
            VTQueryType.WIN_STATE    => "\e[11t",
            VTQueryType.WIN_TITLE    => "\e]l",
            VTQueryType.ICON_TITLE   => "\e]L",
            VTQueryType.PRIMARY_DA   => "\e[c",
            VTQueryType.SECONDARY_DA => "\e[>c",
            _                        => throw new ArgumentOutOfRangeException()
        };
    }

    private string FormatOscSeq() {
        if (_mode == (int)EscapeCodes.OscMode.REDEFINE_COLOR) {
            var args = Arguments;
            if (args.Count == 0)
                return $"\e]{_mode};?\a";
            if (args.Count == 1)
                return $"\e]4;{args[0]};?\a";

            return $"\e]4;{string.Join(";", args.Select(a => $"{a};?"))}\a";
        }

        var generalArgs = Arguments;
        return generalArgs.Count > 0
            ? $"\e]{_mode};{string.Join(";", generalArgs)};?\a"
            : $"\e]{_mode};?\a";
    }

    public override string ToString() =>
        $"Type: {_type}, Mode: {_mode}" +
        (Arguments.Count > 0 ? $", Args: [{string.Join(", ", Arguments)}]" : "") +
        $", Data: {Response ?? _response}";


    // ---------------- FACTORY METHODS ---------------- // 

    // --- requests ---

    public static VTQuery RequestDec(EscapeCodes.DecMode mode) => new(VTQueryType.DEC, (int)mode);

    public static VTQuery RequestDec(int mode) => new(VTQueryType.DEC, mode);

    public static VTQuery RequestOsc(EscapeCodes.OscMode mode, params int[] args) => new(VTQueryType.OSC, (int)mode, null, args);

    public static VTQuery RequestOsc(int mode, params int[] args) => new(VTQueryType.OSC, mode, null, args);

    public static VTQuery RequestRedefineColor(int colorIndex) => RequestOsc(EscapeCodes.OscMode.REDEFINE_COLOR, colorIndex);

    public static VTQuery RequestRedefineColor(NKConsoleColor color) => RequestRedefineColor((int)color);

    public static VTQuery RequestRedefineColors(params int[] colorIndices) => RequestOsc(EscapeCodes.OscMode.REDEFINE_COLOR, colorIndices);

    public static VTQuery RequestRedefineColors(params NKConsoleColor[] colors) => RequestOsc(EscapeCodes.OscMode.REDEFINE_COLOR, colors.Select(c => (int)c).ToArray());

    public static VTQuery RequestWin(EscapeCodes.WinOpts mode) => new(VTQueryType.WIN, (int)mode);

    public static VTQuery RequestWin(int mode) => new(VTQueryType.WIN, mode);

    public static VTQuery RequestWinState    => new(VTQueryType.WIN_STATE);
    public static VTQuery RequestWinTitle    => new(VTQueryType.WIN_TITLE);
    public static VTQuery RequestIconTitle   => new(VTQueryType.ICON_TITLE);
    public static VTQuery RequestPrimaryDA   => new(VTQueryType.PRIMARY_DA);
    public static VTQuery RequestSecondaryDA => new(VTQueryType.SECONDARY_DA);


    // --- responses ---

    public static VTQuery Dec(EscapeCodes.DecMode mode, DecReqResponseType data) => new(VTQueryType.DEC, (int)mode, data);

    public static VTQuery Dec(int mode, DecReqResponseType data) => new(VTQueryType.DEC, mode, data);

    public static VTQuery Osc(EscapeCodes.OscMode mode, object? data, params int[] args) => new(VTQueryType.OSC, (int)mode, data, args);

    public static VTQuery Osc(int mode, object? data, params int[] args) => new(VTQueryType.OSC, mode, data, args);

    public static VTQuery RedefineColor(int colorIndex, NKColor color) =>
        new(VTQueryType.OSC, (int)EscapeCodes.OscMode.REDEFINE_COLOR, (colorIndex, color), [colorIndex]);

    public static VTQuery RedefineColors(IReadOnlyDictionary<int, NKColor> colors) =>
        new(VTQueryType.OSC, (int)EscapeCodes.OscMode.REDEFINE_COLOR, colors, colors.Keys.ToArray());

    public static VTQuery Win(EscapeCodes.WinOpts mode, (int X, int Y) data) => new(VTQueryType.WIN, (int)mode, data);

    public static VTQuery Win(int mode, (int X, int Y) data) => new(VTQueryType.WIN, mode, data);

    /// <summary>
    ///     Creates a new VT query response for a window state query.
    /// </summary>
    /// <param name="state">If true, the VT window is minimized, otherwise not.</param>
    public static VTQuery WinState(bool state) => new(VTQueryType.WIN_STATE, state);

    public static VTQuery WinTitle(string  title) => new(VTQueryType.WIN_TITLE, title);
    public static VTQuery IconTitle(string title) => new(VTQueryType.ICON_TITLE, title);

    /// <summary>
    ///     Creates a new VT query response for a Primary Device Attribute (PrimaryDA) query.
    /// </summary>
    /// <param name="response">
    ///     The PDAResponse object representing the device's primary attribute
    ///     information.
    /// </param>
    /// <returns>A VTQuery instance representing the PrimaryDA query response.</returns>
    public static VTQuery PrimaryDA(PDAResponse response) => new(VTQueryType.PRIMARY_DA, response);

    #region Equality and Matching

    public bool Equals(VTQuery other) {
        if (_type != other._type || _mode != other._mode)
            return false;

        var a1 = Arguments;
        var a2 = other.Arguments;
        if (a1.Count != a2.Count)
            return false;

        for (var i = 0; i < a1.Count; i++) {
            if (a1[i] != a2[i])
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is VTQuery other && Equals(other);

    public override int GetHashCode() {
        var hash = HashCode.Combine((int)_type, _mode);
        foreach (var arg in Arguments)
            hash = HashCode.Combine(hash, arg);
        return hash;
    }

    public static bool operator ==(VTQuery left, VTQuery right) => left.Equals(right);
    public static bool operator !=(VTQuery left, VTQuery right) => !left.Equals(right);

    /// <summary>
    ///     Determines whether this query matches the expected request type, mode, and parameters.
    /// </summary>
    public bool Matches(in VTQuery other) {
        if (Type != other.Type)
            return false;

        return Type switch {
            VTQueryType.DEC        => DecMode == other.DecMode,
            VTQueryType.OSC        => OscMode == other.OscMode && MatchesOscArgs(in other),
            VTQueryType.WIN        => WinMode == other.WinMode,
            VTQueryType.WIN_STATE  => true,
            VTQueryType.WIN_TITLE  => true,
            VTQueryType.ICON_TITLE => true,
            VTQueryType.PRIMARY_DA => true,
            _                      => false
        };
    }

    private bool MatchesOscArgs(in VTQuery other) {
        if (Arguments.Count == 0 || other.Arguments.Count == 0)
            return true;

        var otherArgs = other.Arguments;
        return Arguments.Any(a => otherArgs.Contains(a));
    }

    #endregion
}
