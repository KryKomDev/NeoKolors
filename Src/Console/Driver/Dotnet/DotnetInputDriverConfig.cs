//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Console.Ansi;

namespace NeoKolors.Console;

/// <summary>
///     Represents configuration options for the DotnetInputDriver.
/// </summary>
public sealed record DotnetInputDriverConfig : InputDriverConfig {

    public DotnetInputDriverConfig() {
        ReportResize = false;
    }

    public DotnetInputDriverConfig(
        bool                reportFocus         = true,
        bool                reportResize        = false,
        bool                bracketedPaste      = false,
        ReportedMouseEvents mouseConfig         = ReportedMouseEvents.NONE,
        MouseReportProtocol mouseReportProtocol = MouseReportProtocol.SGR,
        bool                ctrlCForceQuits     = true,
        TimeSpan?           vtRequestTimeout    = null,
        int                 vtRequestMaxRetries = 1
    ) : base(
        reportFocus,
        mouseConfig,
        reportResize,
        vtRequestTimeout,
        vtRequestMaxRetries,
        bracketedPaste,
        mouseReportProtocol,
        ctrlCForceQuits
    ) { }
}
