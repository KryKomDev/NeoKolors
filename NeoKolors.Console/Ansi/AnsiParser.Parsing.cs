// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using NeoKolors.Console.Ansi.Mouse;
using NeoKolors.Console.Events;

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
            ']' => ParseOsc(),
            _   => null
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
            _   => ParseGeneric(),
        };
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
            'c' => ParsePrimaryAttr (seq),
            'y' => ParseDecReq      (seq),
            'n' => ParseDeviceStatus(seq),
            _   => null
        };
    }

    private AnsiRecord? ParseGraphicsAttr(string sequence) {
        throw new NotImplementedException("Graphics attributes not implemented.");
    }

    private AnsiRecord? ParsePrimaryAttr(string sequence) {
        var parameters = sequence[..^2].Split(';');

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
        var parameters = sequence[..^3].Split(';');
        
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

    private AnsiRecord? ParseGeneric() {
        throw new NotImplementedException();
    }

    private AnsiRecord? ParseOsc() {
        throw new NotImplementedException();
    }

    private AnsiRecord? ParseMouseSGR() {
        if (!TryReadUntil(out var seq, 'M', 'm'))
            return null;

        var parameters = seq[..^2].Split(';');

        if (parameters.Length != 3)
            return null;

        if (!int.TryParse(parameters[0], out var b) ||
            !int.TryParse(parameters[1], out var x) ||
            !int.TryParse(parameters[2], out var y)
        ) return null;

        return new AnsiRecord(MouseEventDecomposer.DecomposeSGR(b, x, y, seq[^1]));
    }
}