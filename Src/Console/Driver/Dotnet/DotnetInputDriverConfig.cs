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
        get;
        set {
            SetMouseReportProtocol(field, value);

            field = value;
            LOGGER.Info($"Mouse reporting protocol set to {value}");
        }
    }

    public override ReportedMouseEvents MouseConfig { 
        get;
        set {
            SetReportedMouseEvents(field, value);
            
            field = value;
            LOGGER.Info($"Mouse reporting level set to {value}");
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
        
        SetMouseReportProtocol(MouseReportProtocol.X10, reportProtocol);
        SetReportedMouseEvents(ReportedMouseEvents.NONE, mouseConfig);
    }

    public DotnetInputDriverConfig() : this(true) { }

    private static void SetMouseReportProtocol(MouseReportProtocol oldValue, MouseReportProtocol newValue) {
        var d = oldValue switch {
            MouseReportProtocol.X10        => null,
            MouseReportProtocol.UTF8       => MOUSE_EV_UTF8_OFF,
            MouseReportProtocol.SGR        => MOUSE_EV_SGR_OFF,
            MouseReportProtocol.SGR_PIXELS => MOUSE_EV_SGR_PIXELS_OFF,
            _                              => throw new ArgumentOutOfRangeException(nameof(oldValue), oldValue, null),
        };

        var e = newValue switch {
            MouseReportProtocol.X10        => null,
            MouseReportProtocol.UTF8       => MOUSE_EV_UTF8_ON,
            MouseReportProtocol.SGR        => MOUSE_EV_SGR_ON,
            MouseReportProtocol.SGR_PIXELS => MOUSE_EV_SGR_PIXELS_ON,
            _                              => throw new ArgumentOutOfRangeException(nameof(newValue), newValue, null),
        };
            
        // disable the original protocol
        if (d != null) 
            NKConsole.OutputDriver.Write(d);
            
        // enable the new protocol
        if (e != null)
            NKConsole.OutputDriver.Write(e);
    }
    
    private static void SetReportedMouseEvents(ReportedMouseEvents oldValue, ReportedMouseEvents newValue) {
        var d = oldValue switch { 
               ReportedMouseEvents.NONE    => null,
            <= ReportedMouseEvents.DOWN    => MOUSE_EV_ON_P_OFF,
            <= ReportedMouseEvents.RELEASE => MOUSE_EV_ON_PR_OFF,
            <= ReportedMouseEvents.DRAG    => MOUSE_EV_ON_PRD_OFF,
            <= ReportedMouseEvents.ALL     => MOUSE_EV_ON_ALL_OFF,
            _                              => throw new ArgumentOutOfRangeException(nameof(oldValue), oldValue, null),
        };
        
        var e = newValue switch { 
               ReportedMouseEvents.NONE    => null,
            <= ReportedMouseEvents.DOWN    => MOUSE_EV_ON_P_ON,
            <= ReportedMouseEvents.RELEASE => MOUSE_EV_ON_PR_ON,
            <= ReportedMouseEvents.DRAG    => MOUSE_EV_ON_PRD_ON,
            <= ReportedMouseEvents.ALL     => MOUSE_EV_ON_ALL_ON,
            _                              => throw new ArgumentOutOfRangeException(nameof(newValue), newValue, null),
        };
        
        // disable the original level
        if (d != null) 
            NKConsole.OutputDriver.Write(d);
            
        // enable the new level
        if (e != null)
            NKConsole.OutputDriver.Write(e);
    }
}