//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Console.Ansi;

namespace NeoKolors.Console;

/// <summary>
///     Represents configuration options for console input drivers.
/// </summary>
public record InputDriverConfig {

    public bool ReportFocus { get; init; } = true;
    public bool ReportResize { get; init; } = true;
    public bool BracketedPaste { get; init; } = false;
    public ReportedMouseEvents MouseReportLevel { get; init; } = ReportedMouseEvents.NONE;
    public MouseReportProtocol MouseReportProtocol { get; init; } = MouseReportProtocol.SGR;
    public bool CtrlCForceQuits { get; init; } = true;
    public TimeSpan RefreshInterval { get; init; } = TimeSpan.FromMilliseconds(10);
    public TimeSpan VTRequestTimeout { get; init; } = TimeSpan.FromMilliseconds(100);
    public int VTRequestMaxRetries { get; init; } = 1;

    public InputDriverConfig() { }

    public InputDriverConfig(
        bool                reportFocus         = true,
        ReportedMouseEvents mouseConfig         = ReportedMouseEvents.NONE,
        bool                reportResize        = true,
        TimeSpan?           vtRequestTimeout    = null,
        int                 vtRequestMaxRetries = 1,
        bool                bracketedPaste      = false,
        MouseReportProtocol mouseReportProtocol = MouseReportProtocol.SGR,
        bool                ctrlCForceQuits     = true,
        TimeSpan?           refreshInterval     = null
    ) {
        ReportFocus         = reportFocus;
        MouseReportLevel    = mouseConfig;
        ReportResize        = reportResize;
        BracketedPaste      = bracketedPaste;
        MouseReportProtocol = mouseReportProtocol;
        CtrlCForceQuits     = ctrlCForceQuits;
        RefreshInterval     = refreshInterval ?? TimeSpan.FromMilliseconds(10);
        VTRequestTimeout    = vtRequestTimeout ?? TimeSpan.FromMilliseconds(100);
        VTRequestMaxRetries = vtRequestMaxRetries;
    }
}
