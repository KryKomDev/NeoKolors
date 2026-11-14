//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Console.Ansi;

namespace NeoKolors.Console;

public delegate void VTQueryTimeoutHandler(VTQuery request, AnsiParser.ParserResult reason);
