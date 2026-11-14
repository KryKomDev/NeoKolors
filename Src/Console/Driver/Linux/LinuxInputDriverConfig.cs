//
// NeoKolors
// Copyright (c) 2026 KryKom
//

#if NK_ENABLE_NATIVE_IO

using NeoKolors.Console.Ansi;

namespace NeoKolors.Console;

/// <summary>
///     Represents configuration options for the native Linux input driver.
/// </summary>
public sealed record LinuxInputDriverConfig : InputDriverConfig {

    public LinuxInputDriverConfig() { }

    public LinuxInputDriverConfig(
        bool                reportFocus         = true,
        bool                reportResize        = true,
        bool                bracketedPaste      = false,
        ReportedMouseEvents mouseConfig         = ReportedMouseEvents.NONE,
        MouseReportProtocol mouseReportProtocol = MouseReportProtocol.SGR,
        bool                ctrlCForceQuits     = true,
        TimeSpan?           refreshInterval     = null,
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
        ctrlCForceQuits,
        refreshInterval
    ) { }
}

#endif
