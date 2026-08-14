//
// NeoKolors
// Copyright (c) 2026 KryKom
//

namespace NeoKolors.Console;

public abstract class InputDriverConfig {
    
    public bool                        ReportFocus  { get; set; }
    public bool                        ReportResize { get; set; }
    public virtual ReportedMouseEvents MouseReportLevel  { get; set; }

    protected InputDriverConfig(bool reportFocus, ReportedMouseEvents mouseConfig, bool reportResize) {
        ReportFocus  = reportFocus;
        MouseReportLevel  = mouseConfig;
        ReportResize = reportResize;
    }
}