//
// NeoKolors
// Copyright (c) 2026 KryKom
//

namespace NeoKolors.Console;

/// <summary>
///     Represents configuration options for the Windows input driver.
/// </summary>
public sealed record WinInputDriverConfig : InputDriverConfig {

    public WinInputDriverConfig() {
        MouseReportLevel = ReportedMouseEvents.ALL;
    }

    public WinInputDriverConfig(
        bool                reportFocus         = true,
        ReportedMouseEvents mouseConfig         = ReportedMouseEvents.ALL,
        bool                reportResize        = true,
        TimeSpan?           refreshInterval     = null,
        TimeSpan?           vtRequestTimeout    = null,
        bool                ctrlCForceQuits     = true,
        int                 vtRequestMaxRetries = 1
    ) : base(
        reportFocus,
        mouseConfig,
        reportResize,
        vtRequestTimeout,
        vtRequestMaxRetries,
        ctrlCForceQuits: ctrlCForceQuits,
        refreshInterval: refreshInterval
    ) { }
}
