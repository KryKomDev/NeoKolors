//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoKolors.Common;

namespace NeoKolors.Console.Tests;

[Collection("ConsoleTests")]
public class WindowsDriverTests {

    [WindowsFact]
    public unsafe void WindowsImports_StructSizes_MatchWindowsAbi() {
        Assert.Equal(4, sizeof(WinImports.WinFocusEvent));
        Assert.Equal(16, sizeof(WinImports.WinKeyEvent));
        Assert.Equal(4, sizeof(WinImports.WinMenuEvent));
        Assert.Equal(16, sizeof(WinImports.WinCoord));
        Assert.Equal(16, sizeof(WinImports.WinMouseEvent));
        Assert.Equal(16, sizeof(WinImports.WinResizeEvent));
        Assert.Equal(4, sizeof(WinImports.WinColorRef));
        Assert.Equal(20, Marshal.SizeOf<WinImports.WinInputRecord>());
    }

    [WindowsTheory]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.VK_RETURN, KeyCode.RETURN)]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.VK_BACK, KeyCode.BACKSPACE)]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.VK_TAB, KeyCode.TAB)]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.VK_ESCAPE, KeyCode.ESCAPE)]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.VK_SPACE, KeyCode.SPACE)]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.VK_LEFT, KeyCode.ARROW_LEFT)]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.VK_UP, KeyCode.ARROW_UP)]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.VK_RIGHT, KeyCode.ARROW_RIGHT)]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.VK_DOWN, KeyCode.ARROW_DOWN)]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.A, KeyCode.A)]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.Z, KeyCode.Z)]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.D0, KeyCode.D0)]
    [InlineData((ushort)WinImports.WinVirtualKeyCode.D9, KeyCode.D9)]
    public void WindowsImports_VirtualKeyCodes_MatchKeyCodeValues(ushort winKey, KeyCode expectedKey) {
        Assert.Equal((int)expectedKey, winKey);
    }

    [WindowsTheory]
    [InlineData((uint)WinImports.WinKeyModifiers.NONE, KeyModifiers.NONE)]
    [InlineData((uint)WinImports.WinKeyModifiers.RIGHT_ALT, KeyModifiers.RIGHT_ALT)]
    [InlineData((uint)WinImports.WinKeyModifiers.LEFT_ALT, KeyModifiers.LEFT_ALT)]
    [InlineData((uint)WinImports.WinKeyModifiers.RIGHT_CTRL, KeyModifiers.RIGHT_CTRL)]
    [InlineData((uint)WinImports.WinKeyModifiers.LEFT_CTRL, KeyModifiers.LEFT_CTRL)]
    [InlineData((uint)WinImports.WinKeyModifiers.SHIFT, KeyModifiers.SHIFT)]
    [InlineData((uint)WinImports.WinKeyModifiers.NUMLOCK, KeyModifiers.NUMLOCK)]
    [InlineData((uint)WinImports.WinKeyModifiers.SCROLL_LOCK, KeyModifiers.SCROLL_LOCK)]
    [InlineData((uint)WinImports.WinKeyModifiers.CAPS_LOCK, KeyModifiers.CAPS_LOCK)]
    [InlineData((uint)WinImports.WinKeyModifiers.ENHANCED, KeyModifiers.ENHANCED)]
    public void WindowsImports_KeyModifiers_MatchKeyModifiersEnum(uint winMod, KeyModifiers expectedMod) {
        Assert.Equal((uint)expectedMod, winMod);
    }

    [WindowsFact]
    public void WinCoord_PropertiesAndDeconstruction_WorkCorrectly() {
        var coord = new WinImports.WinCoord(15, 42);

        Assert.Equal(15, coord.X);
        Assert.Equal(42, coord.Y);

        coord.Deconstruct(out var x, out var y);
        Assert.Equal(15, x);
        Assert.Equal(42, y);

        Assert.Equal("[15, 42]", coord.ToString());
    }

    [WindowsFact]
    public void WinColorRef_RgbChannelsAndValue_AreAccurate() {
        var color = new WinImports.WinColorRef(0x12, 0x34, 0x56);
        Assert.Equal(0x12, color.R);
        Assert.Equal(0x34, color.G);
        Assert.Equal(0x56, color.B);

        var colorFromValue = new WinImports.WinColorRef(0xFF112233);
        Assert.Equal(0x00112233u, colorFromValue.Value);
    }

    [WindowsFact]
    public void WinInputRecord_ToString_FormatsCorrectly() {
        var focus = new WinImports.WinFocusEvent(true);
        Assert.Equal("FocusEvent(True)", focus.ToString());

        var focusOff = new WinImports.WinFocusEvent(false);
        Assert.Equal("FocusEvent(False)", focusOff.ToString());

        var key = new WinImports.WinKeyEvent(
            true,
            1,
            WinImports.WinVirtualKeyCode.A,
            0x1E,
            'a',
            WinImports.WinKeyModifiers.NONE
        );
        var keyStr = key.ToString();
        Assert.Contains("down", keyStr);
        Assert.Contains("1x", keyStr);
        Assert.Contains("A", keyStr);
    }

    [WindowsFact]
    public void WinInputDriverConfig_Defaults_AreValid() {
        var config = new WinInputDriverConfig();

        Assert.True(config.ReportFocus);
        Assert.Equal(ReportedMouseEvents.ALL, config.MouseReportLevel);
        Assert.True(config.ReportResize);
        Assert.Equal(TimeSpan.FromMilliseconds(10), config.RefreshInterval);
        Assert.Equal(TimeSpan.FromMilliseconds(100), config.VTRequestTimeout);
        Assert.True(config.CtrlCForceQuits);
    }

    [WindowsFact]
    public void WinInputDriverConfig_CustomParameters_AreApplied() {
        var config = new WinInputDriverConfig(
            reportFocus: false,
            mouseConfig: ReportedMouseEvents.NONE,
            reportResize: false,
            refreshInterval: TimeSpan.FromMilliseconds(25),
            vtRequestTimeout: TimeSpan.FromMilliseconds(250),
            ctrlCForceQuits: false
        );

        Assert.False(config.ReportFocus);
        Assert.Equal(ReportedMouseEvents.NONE, config.MouseReportLevel);
        Assert.False(config.ReportResize);
        Assert.Equal(TimeSpan.FromMilliseconds(25), config.RefreshInterval);
        Assert.Equal(TimeSpan.FromMilliseconds(250), config.VTRequestTimeout);
        Assert.False(config.CtrlCForceQuits);
    }

    [WindowsFact]
    public void WinExceptionSuppressor_MuteSafe_NeverThrows() {
        // Safe mute must never throw on any platform
        WinExceptionSuppressor.Mute(safe: true);
    }

    [WindowsFact]
    public void WinExceptionSuppressor_MuteUnsafe_SucceedsOnWindows() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        WinExceptionSuppressor.Mute(safe: false);
    }

    [WindowsFact]
    public void Win32ExceptionExtensions_Throw_ThrowsWin32ExceptionWithCode() {
        const uint errorCode = 5; // ERROR_ACCESS_DENIED
        var ex = Assert.Throws<Win32Exception>(() => Win32ExceptionExtensions.Throw(errorCode));
        Assert.Equal(5, ex.NativeErrorCode);
    }

    [WindowsFact]
    public void WindowsHandles_GetStdHandles_DoNotThrow() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        var stdIn = WinImports.GetStdIn();
        var stdOut = WinImports.GetStdOut();
        var stdErr = WinImports.GetStdErr();

        // Valid Win32 handles or 0 if unattached
        Assert.True(stdIn != -1);
        Assert.True(stdOut != -1);
        Assert.True(stdErr != -1);
    }

    [WindowsFact]
    public void WindowsInputDriver_ConstructsAndDisposes_IfConsoleAvailable() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        // If running in a redirected/non-console test environment on Windows, skip terminal mode calls
        if (!WinImports.GetStdInMode(WinImports.GetStdIn(), out _))
            return;

        using var driver = new WindowsInputDriver();
        Assert.NotNull(driver);
        Assert.False(driver.IsRunning);
    }

    [WindowsFact]
    public void WindowsOutputDriver_ConstructsAndDisposes_IfConsoleAvailable() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        // If running in a redirected/non-console test environment on Windows, skip terminal mode calls
        if (!WinImports.GetStdOutMode(WinImports.GetStdOut(), out _))
            return;

        using var driver = new WindowsOutputDriver();
        Assert.NotNull(driver);
    }
}
