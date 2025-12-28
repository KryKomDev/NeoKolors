//
// NeoKolors.Test
// Copyright (c) 2025 KryKom

using NeoKolors.Tui;

namespace NeoKolors.Tui.Tests;

public class AppConfigTests {
    
    [Fact]
    public void DefaultConstructor_ShouldSetDefaultValues() {
        try {
            var config = new AppConfig();
        
            Assert.True(config.CtrlCForceQuits);
            Assert.True(config.LazyRender);
            Assert.False(config.EnableDebugLogging);
            Assert.Equal(20, config.FpsLimit.Framerate);
            Assert.True(config.FpsLimit.IsLimited);
        } catch (IOException) {
            // Ignore IOException (handle is invalid) in test environments without console
        }
    }

    [Fact]
    public void ParameterizedConstructor_ShouldSetValues() {
        try {
            var config = new AppConfig(
                ctrlCForceQuits: false,
                lazyRender: false,
                fpsLimit: 60,
                enableDebugLogging: true
            );
            
            Assert.False(config.CtrlCForceQuits);
            Assert.False(config.LazyRender);
            Assert.True(config.EnableDebugLogging);
            Assert.Equal(60, config.FpsLimit.Framerate);
        } catch (IOException) {
            // Ignore IOException (handle is invalid) in test environments without console
        }
    }

    [Fact]
    public void FramerateLimit_ShouldThrow_WhenValueIsLessThanOne() {
        Assert.Throws<ArgumentException>(() => new AppConfig.FramerateLimit(0));
        Assert.Throws<ArgumentException>(() => new AppConfig.FramerateLimit(-5));
    }
    
    [Fact]
    public void FramerateLimit_ImplicitConversion_ShouldWork() {
        AppConfig.FramerateLimit limit = 30;
        Assert.Equal(30, limit.Framerate);
        Assert.True(limit.IsLimited);
    }

    [Fact]
    public void FramerateLimit_Unlimited_ShouldBeUnlimited() {
        var limit = new AppConfig.FramerateLimit(10).Unlimited;
        Assert.False(limit.IsLimited);
    }
}