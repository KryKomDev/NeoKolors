//
// NeoKolors
// Copyright (c) 2026 KryKom
//

namespace NeoKolors.Console.Driver.Windows;

public class WinInputDriverConfig : InputDriverConfig {
    
    public TimeSpan RefreshInterval { get; }
    public bool     CtrlCForceQuits { get; }
    
    public WinInputDriverConfig(
        bool                reportFocus,
        ReportedMouseEvents mouseConfig,
        bool                reportResize,
        TimeSpan            refreshInterval,
        bool                ctrlCForceQuits  = true
    ) : base(
        reportFocus,
        mouseConfig,
        reportResize
    ) {
        RefreshInterval = refreshInterval;
        CtrlCForceQuits = ctrlCForceQuits;
    }
}