// NeoKolors.Test
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Tests.Elements;

public class TriggerTests {
    private class DummyElement : Control<string> {
        protected override void RenderCore(ICharCanvas canvas) { }
        protected override Size MeasureOverride(Size availableSize) => Size.Zero;
        public override ElementInfo Info => ElementInfo.Default;
        public override string GetChildNode() => "";
        public override void SetChildNode(string child) { }
    }

    [Fact]
    public void Trigger_ShouldBeActiveWhenConditionMatches() {
        // Arrange
        var element = new DummyElement();
        var trigger = new Trigger { Property = "IsFocused", Value = "True" };

        // Act & Assert
        Assert.False(trigger.IsActive(element));

        element.IsFocused = true;
        Assert.True(trigger.IsActive(element));

        element.IsFocused = false;
        Assert.False(trigger.IsActive(element));
    }

    [Fact]
    public void Trigger_ShouldBeActiveWhenStylePropertyMatches() {
        // Arrange
        var element = new DummyElement();
        var trigger = new Trigger { Property = "Visible", Value = "False" };

        // Act & Assert
        Assert.False(trigger.IsActive(element)); // default is visible = true

        element.Style.Set(new VisibleProperty(false));
        Assert.True(trigger.IsActive(element));
    }

    [Fact]
    public void EvaluateTriggers_ShouldApplyAndRestoreStyles() {
        // Arrange
        var element = new DummyElement();
        
        // Default background should be Inherit
        Assert.True(element.Style.Get<BackgroundColorProperty>().Value.IsInherit);

        var trigger = new Trigger { Property = "IsHovered", Value = "True" };
        var setter = new Setter { Property = "BackgroundColor", Value = "Blue" };
        trigger.Setters.Add(setter);
        
        element.Triggers.Add(trigger);

        // Act - Activate trigger by hovering
        element.IsHovered = true;

        // Assert - Background should be updated to Blue
        var bg = element.Style.Get<BackgroundColorProperty>().Value;
        Assert.False(bg.IsInherit);
        Assert.Equal("BLUE", bg.ToString());

        // Act - Deactivate trigger by unhovering
        element.IsHovered = false;

        // Assert - Background should be restored to Inherit
        Assert.True(element.Style.Get<BackgroundColorProperty>().Value.IsInherit);
    }
}
