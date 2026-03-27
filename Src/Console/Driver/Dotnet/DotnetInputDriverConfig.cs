//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Console.Ansi.Mouse;
using static NeoKolors.Common.EscapeCodes;

namespace NeoKolors.Console.Driver.Dotnet;

/// <summary>
/// Represents the configuration options for the DotnetInputDriver.
/// </summary>
public class DotnetInputDriverConfig : InputDriverConfig {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger<DotnetInputDriverConfig>();
    
    public bool BracketedPaste { get; set; }

    public MouseReportProtocol ReportProtocol {
        get => field;
        set {
            var d = field switch {
                MouseReportProtocol.X10        => null,
                MouseReportProtocol.UTF8       => MOUSE_EV_UTF8_OFF,
                MouseReportProtocol.SGR        => MOUSE_EV_SGR_OFF,
                MouseReportProtocol.SGR_PIXELS => MOUSE_EV_SGR_PIXELS_OFF,
                _                              => throw new ArgumentOutOfRangeException(nameof(ReportProtocol), field, null),
            };

            var e = value switch {
                MouseReportProtocol.X10        => null,
                MouseReportProtocol.UTF8       => MOUSE_EV_UTF8_ON,
                MouseReportProtocol.SGR        => MOUSE_EV_SGR_ON,
                MouseReportProtocol.SGR_PIXELS => MOUSE_EV_SGR_PIXELS_ON,
                _                              => throw new ArgumentOutOfRangeException(nameof(value), value, null),
            };
            
            // disable the original protocol
            if (d != null) 
                NKConsole.OutputDriver.Write(d);
            
            // enable the new protocol
            if (e != null)
                NKConsole.OutputDriver.Write(e);

            field = value;
            LOGGER.Info($"Mouse reporting protocol set to {value}");
        }
    }
    
    public bool CtrlCForceQuits { get; set; }

    public DotnetInputDriverConfig(
        bool                reportFocus     = true, 
        bool                reportResize    = false,
        bool                bracketedPaste  = false,
        ReportedMouseEvents mouseConfig     = ReportedMouseEvents.NONE,
        MouseReportProtocol reportProtocol  = MouseReportProtocol.SGR,
        bool                ctrlCForceQuits = true
    ) : base(
        reportFocus, 
        mouseConfig,
        reportResize
    ) {
        BracketedPaste  = bracketedPaste;
        ReportProtocol  = reportProtocol;
        CtrlCForceQuits = ctrlCForceQuits;
    }

    public DotnetInputDriverConfig() : this(true) { }
}