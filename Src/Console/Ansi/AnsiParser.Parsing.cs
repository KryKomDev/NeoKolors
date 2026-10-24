// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

namespace NeoKolors.Console.Ansi;

public abstract partial class AnsiParser {
    /// <summary>
    /// Parses input to determine if it is an ANSI escape sequence or another type of input
    /// and returns an <c>AnsiRecord</c> representing the parsed result.
    /// </summary>
    /// <returns>An instance of <c>AnsiRecord</c> representing the parsed input,
    /// or <c>null</c> if the input does not form a valid ANSI escape sequence.
    /// </returns>
    public AnsiRecord? Parse() {
        var first = Read();

        if (first.Char != '\e') {
            ReleaseRead(true);
            return new AnsiRecord(first);
        }

        // try to read the remainder of the sequence
        var result = ParseEscape();

        // if the thing was not an escseq, treat the keys as a valid input
        ReleaseRead(success: result != null);

        return result;
    }

    private AnsiRecord? ParseEscape() {
        if (!TryReadNext(out var intro))
            return null;

        return intro switch {
            '[' => ParseCsi(),
            'O' => ParseSs3(),
            ']' => ParseOsc(),
            _   => ParseAltKey(intro)
        };
    }

    private AnsiRecord? ParseCsi() {
        if (!TryReadNext(out var type))
            return null;

        return type switch {
            'O' => new AnsiRecord(false),
            'I' => new AnsiRecord(true),
            'M' => ParseMouseX10(),
            '?' => ParseQm(),
            '>' => ParseSecondaryAttr(),
            '<' => ParseMouseSGR(),
            _   => ParseCsiKeyOrSequence(type),
        };
    }

    private AnsiRecord? ParseCsiKeyOrSequence(char firstChar) {
        if (firstChar is >= 'A' and <= 'Z') {
            return MapLetterToKey(firstChar, KeyModifiers.NONE);
        }

        var sb = new System.Text.StringBuilder();
        sb.Append(firstChar);

        char terminator = '\0';
        while (TryReadNext(out var c)) {
            if (c is >= '@' and <= '~') {
                terminator = c;
                break;
            }

            sb.Append(c);
        }

        if (terminator == '\0')
            return null;

        var paramStr = sb.ToString();
        var parts = paramStr.Split(';');

        if (terminator == '~' && parts.Length > 0 && parts[0] == "200") {
            return ParseBracketedPaste();
        }

        var modifiers = KeyModifiers.NONE;
        if (parts.Length > 1 && int.TryParse(parts[1], out var modVal)) {
            int m = modVal - 1;
            if ((m & 1) != 0) modifiers |= KeyModifiers.SHIFT;
            if ((m & 2) != 0) modifiers |= KeyModifiers.LEFT_ALT;
            if ((m & 4) != 0) modifiers |= KeyModifiers.LEFT_CTRL;
        }

        if (terminator == '~') {
            if (parts.Length > 0 && int.TryParse(parts[0], out var code)) {
                return MapTildeCodeToKey(code, modifiers);
            }

            return null;
        }

        return MapLetterToKey(terminator, modifiers);
    }

    private static AnsiRecord? MapLetterToKey(char letter, KeyModifiers modifiers) {
        KeyCode code = letter switch {
            'A' => KeyCode.ARROW_UP,
            'B' => KeyCode.ARROW_DOWN,
            'C' => KeyCode.ARROW_RIGHT,
            'D' => KeyCode.ARROW_LEFT,
            'E' => KeyCode.CLEAR,
            'F' => KeyCode.END,
            'H' => KeyCode.HOME,
            'Z' => KeyCode.TAB,
            _   => KeyCode.NONE
        };

        if (letter == 'Z')
            modifiers |= KeyModifiers.SHIFT;

        if (code == KeyCode.NONE)
            return null;

        return new AnsiRecord(new KeyEventArgs(code, modifiers, '\0'));
    }

    private static AnsiRecord? MapTildeCodeToKey(int code, KeyModifiers modifiers) {
        KeyCode key = code switch {
            1 or 7 => KeyCode.HOME,
            2      => KeyCode.INSERT,
            3      => KeyCode.DELETE,
            4 or 8 => KeyCode.END,
            5      => KeyCode.PAGE_UP,
            6      => KeyCode.PAGE_DOWN,
            11     => KeyCode.F1,
            12     => KeyCode.F2,
            13     => KeyCode.F3,
            14     => KeyCode.F4,
            15     => KeyCode.F5,
            17     => KeyCode.F6,
            18     => KeyCode.F7,
            19     => KeyCode.F8,
            20     => KeyCode.F9,
            21     => KeyCode.F10,
            23     => KeyCode.F11,
            24     => KeyCode.F12,
            _      => KeyCode.NONE
        };

        if (key == KeyCode.NONE)
            return null;

        return new AnsiRecord(new KeyEventArgs(key, modifiers, '\0'));
    }

    private AnsiRecord? ParseSs3() {
        if (!TryReadNext(out var code))
            return null;

        KeyCode key = code switch {
            'A' => KeyCode.ARROW_UP,
            'B' => KeyCode.ARROW_DOWN,
            'C' => KeyCode.ARROW_RIGHT,
            'D' => KeyCode.ARROW_LEFT,
            'F' => KeyCode.END,
            'H' => KeyCode.HOME,
            'P' => KeyCode.F1,
            'Q' => KeyCode.F2,
            'R' => KeyCode.F3,
            'S' => KeyCode.F4,
            _   => KeyCode.NONE
        };

        if (key == KeyCode.NONE)
            return null;

        return new AnsiRecord(new KeyEventArgs(key, KeyModifiers.NONE, '\0'));
    }

    private static AnsiRecord? ParseAltKey(char intro) {
        var key = LinuxInput.CharToKeyEventArgs(intro);
        return new AnsiRecord(new KeyEventArgs(key.Key, key.Modifiers | KeyModifiers.LEFT_ALT, intro));
    }

    private AnsiRecord? ParseBracketedPaste() {
        var sb = new System.Text.StringBuilder();
        while (TryReadNext(out var c)) {
            if (c == '\e') {
                // ReSharper disable once RedundantAssignment
                // ReSharper disable once InlineOutVariableDeclaration
                char b1 = '\0', b2 = '\0', b3 = '\0', b4 = '\0', b5 = '\0';

                bool matched =
                    TryReadNext(out b1) && b1 == '[' &&
                    TryReadNext(out b2) && b2 == '2' &&
                    TryReadNext(out b3) && b3 == '0' &&
                    TryReadNext(out b4) && b4 == '1' &&
                    TryReadNext(out b5) && b5 == '~';

                if (matched) {
                    break;
                }

                sb.Append('\e');
                if (b1 != '\0') sb.Append(b1);
                if (b2 != '\0') sb.Append(b2);
                if (b3 != '\0') sb.Append(b3);
                if (b4 != '\0') sb.Append(b4);
                if (b5 != '\0') sb.Append(b5);
            }
            else {
                sb.Append(c);
            }
        }

        return new AnsiRecord(sb.ToString());
    }

    private AnsiRecord? ParseMouseX10() {
        if (!TryReadNext(out var b) ||
            !TryReadNext(out var x) ||
            !TryReadNext(out var y)
        ) return null;

        return new AnsiRecord(MouseEventDecomposer.DecomposeUtf8(b, x, y));
    }

    private AnsiRecord? ParseQm() {
        if (!TryReadUntil(out var seq, 'S', 'c', 'y', 'n'))
            return null;

        var last = seq[^1];

        return last switch {
            'S' => ParseGraphicsAttr(seq),
            'c' => ParsePrimaryAttr(seq),
            'y' => ParseDecReq(seq),
            'n' => ParseDeviceStatus(seq),
            _   => null
        };
    }

    private AnsiRecord? ParseGraphicsAttr(string sequence) {
        throw new NotImplementedException("Graphics attributes not implemented.");
    }

    private AnsiRecord? ParsePrimaryAttr(string sequence) {
        var parameters = sequence[..^1].Split(';');

        if (parameters.Length == 0)
            return null;

        // parse terminal mode 
        if (!int.TryParse(parameters[0], out var term))
            return null;

        // only terminal mode supplied
        if (parameters.Length == 1)
            return new AnsiRecord(VTQuery.PrimaryDA(new PDAResponse(term)));

        // type and capabilities supplied
        var capabilities = new int[parameters.Length - 1];

        for (int i = 0; i < parameters.Length - 1; i++) {
            if (!int.TryParse(parameters[i + 1], out var value))
                return null;

            capabilities[i] = value;
        }

        return new AnsiRecord(VTQuery.PrimaryDA(new PDAResponse(term, capabilities)));
    }

    private AnsiRecord? ParseDecReq(string sequence) {
        var parameters = sequence[..^1].Split(';');

        if (parameters.Length != 2)
            return null;

        if (!int.TryParse(parameters[0], out var mode) ||
            !int.TryParse(parameters[1], out var value)
        ) return null;

        return new AnsiRecord(VTQuery.Dec(mode, (DecReqResponseType)value));
    }

    private AnsiRecord? ParseDeviceStatus(string result) {
        throw new NotImplementedException("Device status not implemented.");
    }

    private AnsiRecord? ParseSecondaryAttr() {
        throw new NotImplementedException("Secondary attributes not implemented.");
    }

    private AnsiRecord? ParseOsc() {
        // OSC sequences (e.g. \e]0;title\x07 or \e]...ST)
        TryReadUntil(out _, '\x07', '\\');
        return null;
    }

    private AnsiRecord? ParseMouseSGR() {
        if (!TryReadUntil(out var seq, 'M', 'm'))
            return null;

        var parameters = seq[..^1].Split(';');

        if (parameters.Length != 3)
            return null;

        if (!int.TryParse(parameters[0], out var b) ||
            !int.TryParse(parameters[1], out var x) ||
            !int.TryParse(parameters[2], out var y)
        ) return null;

        return new AnsiRecord(MouseEventDecomposer.DecomposeSGR(b, x, y, seq[^1]));
    }
}