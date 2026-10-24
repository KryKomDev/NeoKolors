//
// NeoKolors
// Copyright (c) 2026 KryKom
//

#if NK_ENABLE_NATIVE_IO

using System.ComponentModel;
using System.Runtime.CompilerServices;
using NeoKolors.Console.Ansi;

namespace NeoKolors.Console;

/// <summary>
/// Represents the configuration options for the native LinuxInputDriver.
/// </summary>
public sealed class LinuxInputDriverConfig : InputDriverConfig, INotifyPropertyChanged {
    private ReportedMouseEvents _mouseReportLevel;
    private MouseReportProtocol _mouseReportProtocol;

    public bool BracketedPaste { get; set; }

    public MouseReportProtocol MouseReportProtocol {
        get => _mouseReportProtocol;
        set => SetField(ref _mouseReportProtocol, value);
    }

    public override ReportedMouseEvents MouseReportLevel {
        get => _mouseReportLevel;
        set => SetField(ref _mouseReportLevel, value);
    }

    public bool CtrlCForceQuits { get; set; }

    public TimeSpan RefreshInterval { get; }
    public TimeSpan VTRequestTimeout { get; }

    public LinuxInputDriverConfig(
        bool reportFocus = true,
        bool reportResize = true,
        bool bracketedPaste = false,
        ReportedMouseEvents mouseConfig = ReportedMouseEvents.NONE,
        MouseReportProtocol mouseReportProtocol = MouseReportProtocol.SGR,
        bool ctrlCForceQuits = true,
        TimeSpan? refreshInterval = null,
        TimeSpan? vtRequestTimeout = null
    ) : base(reportFocus, mouseConfig, reportResize) {
        BracketedPaste = bracketedPaste;
        _mouseReportLevel = mouseConfig;
        _mouseReportProtocol = mouseReportProtocol;
        CtrlCForceQuits = ctrlCForceQuits;
        RefreshInterval = refreshInterval ?? TimeSpan.FromMilliseconds(10);
        VTRequestTimeout = vtRequestTimeout ?? TimeSpan.FromMilliseconds(100);
    }

    public LinuxInputDriverConfig() : this(true) { }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        var old = field;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventTrackingArgs<T>(propertyName, old, value));
        return true;
    }
}

#endif
