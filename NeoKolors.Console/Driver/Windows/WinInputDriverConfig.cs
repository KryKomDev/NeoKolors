//
// NeoKolors
// Copyright (c) 2026 KryKom
//

namespace NeoKolors.Console.Driver.Windows;

public class WinInputDriverConfig : InputDriverConfig {
    
    public TimeSpan RefreshInterval  { get; }
    public TimeSpan VTRequestTimeout { get; }
    public bool     CtrlCForceQuits  { get; }
    
    public WinInputDriverConfig(
        bool                reportFocus      = true,
        ReportedMouseEvents mouseConfig      = ReportedMouseEvents.ALL,
        bool                reportResize     = true,
        TimeSpan?           refreshInterval  = null,
        TimeSpan?           vtRequestTimeout = null,
        bool                ctrlCForceQuits  = true
    ) : base(
        reportFocus,
        mouseConfig,
        reportResize
    ) {
        RefreshInterval  = refreshInterval  ?? TimeSpan.FromMilliseconds(10);
        VTRequestTimeout = vtRequestTimeout ?? TimeSpan.FromMilliseconds(100);
        CtrlCForceQuits  = ctrlCForceQuits;
    }
}