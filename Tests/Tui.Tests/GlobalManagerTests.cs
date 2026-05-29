//
// NeoKolors.Test
// Copyright (c) 2026 KryKom
//

using System.Reflection;
using NeoKolors.Tui.Global;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Tests;

public class GlobalManagerTests {

    [Fact]
    public void AssemblyManager_ShouldRegisterTuiAssemblyByDefault() {
        var assemblies = AssemblyManager.GetAssemblies();
        Assert.Contains(assemblies, a => a.GetName().Name == "NeoKolors.Tui");
    }

    [Fact]
    public void AssemblyManager_RegisterAssembly_ShouldTriggerEvent() {
        bool triggered = false;
        Assembly? loadedAssembly = null;
        
        AssemblyManager.OnAssemblyLoaded += (a) => {
            triggered = true;
            loadedAssembly = a;
        };
        
        var asm = typeof(string).Assembly; // Just some assembly
        AssemblyManager.RegisterAssembly(asm);
        
        Assert.True(triggered);
        Assert.Equal(asm, loadedAssembly);
        Assert.Contains(asm, AssemblyManager.GetAssemblies());
    }

    [Fact]
    public void StyleManager_ShouldLoadStylesFromRegisteredAssemblies() {
        // Since StyleManager is static and initializes on start, 
        // it should already have styles from NeoKolors.Tui
        
        var styles = StyleManager.GetStyles();
        Assert.NotEmpty(styles);
        
        // Check for a known style, e.g., BackgroundColorProperty or just BackgroundColor
        var type = StyleManager.GetTypeByName("BackgroundColor");
        Assert.NotNull(type);
        
        var type2 = StyleManager.GetTypeByName("BackgroundColorProperty");
        Assert.Equal(type, type2);
    }

    [Fact]
    public void ScreenSizeTracker_ShouldStoreValues() {
        var sizePx = new Size(1920, 1080);
        var sizeCh = new Size(80, 24);
        
        ScreenSizeTracker.SetScreenSizePx(sizePx);
        ScreenSizeTracker.SetScreenSizeCh(sizeCh);
        
        Assert.Equal(sizePx, ScreenSizeTracker.GetScreenSizePx());
        Assert.Equal(sizeCh, ScreenSizeTracker.GetScreenSizeCh());
    }
}
