//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.ComponentModel;
using System.Runtime.CompilerServices;
using NeoKolors.Console.Ansi;

namespace NeoKolors.Console;

/// <summary>
/// Represents the configuration options for the DotnetInputDriver.
/// </summary>
public sealed class DotnetInputDriverConfig : InputDriverConfig, INotifyPropertyChanged {

    private static readonly NKLogger LOGGER = NKDebug.GetLogger<DotnetInputDriverConfig>();

    public bool BracketedPaste { get; set; }

    public MouseReportProtocol MouseReportProtocol {
        get;
        set => SetField(ref field, value);
    }

    public override ReportedMouseEvents MouseReportLevel {
        get;
        set => SetField(ref field, value);
    }

    public bool CtrlCForceQuits { get; set; }

    public DotnetInputDriverConfig(
        bool                reportFocus         = true,
        bool                reportResize        = false,
        bool                bracketedPaste      = false,
        ReportedMouseEvents mouseConfig         = ReportedMouseEvents.NONE,
        MouseReportProtocol mouseReportProtocol = MouseReportProtocol.SGR,
        bool                ctrlCForceQuits     = true
    ) : base(
        reportFocus,
        mouseConfig,
        reportResize
    ) {
        BracketedPaste      = bracketedPaste;
        MouseReportProtocol = mouseReportProtocol;
        CtrlCForceQuits     = ctrlCForceQuits;
    }

    public DotnetInputDriverConfig() : this(true) { }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        var old = field;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventTrackingArgs<T>(propertyName, old, value));

        return true;
    }
}