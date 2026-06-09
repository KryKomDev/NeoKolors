//
// NeoKolors
// Copyright (c) 2026 KryKom
//

namespace NeoKolors.Console.Driver;

public abstract class InputDriverConfig {
    
    public bool                        ReportFocus  { get; set; }
    public bool                        ReportResize { get; set; }
    public virtual ReportedMouseEvents MouseConfig  { get; set; }

    protected InputDriverConfig(bool reportFocus, ReportedMouseEvents mouseConfig, bool reportResize) {
        ReportFocus  = reportFocus;
        MouseConfig  = mouseConfig;
        ReportResize = reportResize;
    }
}