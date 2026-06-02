// NeoKolors.Test
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Tests.Elements;

public class ControlTests {

    // Concrete mock classes for testing abstract base classes
    private class TestControl : Control<string> {
        protected override void RenderCore(ICharCanvas canvas) { }
        protected override Size MeasureOverride(Size availableSize) => Size.Zero;
        public override ElementInfo Info => ElementInfo.Default;
        public override string GetChildNode() => "";
        public override void SetChildNode(string childNode) { }
        
        public void TriggerVisualState() => UpdateVisualState();
    }

    private class TestContentControl : ContentControl {
        protected override void RenderCore(ICharCanvas canvas) { }
        protected override Size MeasureOverride(Size availableSize) => Size.Zero;
        public override ElementInfo Info => ElementInfo.Default;
    }

    private class TestPanel : Panel {
        protected override void RenderCore(ICharCanvas canvas) { }
        protected override Size MeasureOverride(Size availableSize) => Size.Zero;
        public override ElementInfo Info => ElementInfo.Default;
    }

    private class TestRangeControl : RangeBase {
        protected override void RenderCore(ICharCanvas canvas) { }
        protected override Size MeasureOverride(Size availableSize) => Size.Zero;
        public override ElementInfo Info => ElementInfo.Default;
    }

    [Fact]
    public void Control_Focus_ShouldTransitionStateAndFireEvents() {
        var control = new TestControl();
        bool gotFocusFired = false;
        bool lostFocusFired = false;

        control.GotFocus += _ => gotFocusFired = true;
        control.LostFocus += _ => lostFocusFired = true;

        // Act - Focus
        control.Focus();
        Assert.True(control.IsFocused);
        Assert.True(gotFocusFired);

        // Act - Unfocus
        control.Unfocus();
        Assert.False(control.IsFocused);
        Assert.True(lostFocusFired);
    }

    [Fact]
    public void Control_IsEnabled_ShouldFireIsEnabledChanged() {
        var control = new TestControl();
        bool isEnabledChangedFired = false;
        bool expectedState = false;

        control.IsEnabledChanged += (_, state) => {
            isEnabledChangedFired = true;
            expectedState = state;
        };

        control.IsEnabled = false;

        Assert.False(control.IsEnabled);
        Assert.True(isEnabledChangedFired);
        Assert.False(expectedState);
    }

    [Fact]
    public void ContentControl_Content_ShouldTrackAndFireContentChanged() {
        var control = new TestContentControl();
        object? oldContent = null;
        object? newContent = null;
        bool contentChangedFired = false;

        control.ContentChanged += (_, oldVal, newVal) => {
            contentChangedFired = true;
            oldContent = oldVal;
            newContent = newVal;
        };

        var sampleContent = "Hello Content";
        control.Content = sampleContent;

        Assert.Equal(sampleContent, control.Content);
        Assert.True(contentChangedFired);
        Assert.Null(oldContent);
        Assert.Equal(sampleContent, newContent);
    }

    [Fact]
    public void Panel_Children_ShouldManageVisualCollection() {
        var panel = new TestPanel();
        var child1 = new TestControl();
        var child2 = new TestControl();
        bool panelUpdated = false;

        panel.OnElementUpdated += () => panelUpdated = true;

        // Act - Add child
        panel.AddChild(child1);
        Assert.Single(panel.Children);
        Assert.True(panelUpdated);

        // Act - Add duplicate (should be ignored)
        panelUpdated = false;
        panel.AddChild(child1);
        Assert.Single(panel.Children);
        Assert.False(panelUpdated);

        // Act - Add second child
        panel.AddChild(child2);
        Assert.Equal(2, panel.Children.Count);

        // Act - Remove child
        panelUpdated = false;
        panel.RemoveChild(child1);
        Assert.Single(panel.Children);
        Assert.True(panelUpdated);

        // Act - Clear children
        panel.ClearChildren();
        Assert.Empty(panel.Children);
    }

    [Fact]
    public void RangeBase_Value_ShouldClampToBoundaries() {
        var range = new TestRangeControl();
        double newValue = 0;
        bool valChangedFired = false;

        range.ValueChanged += (_, _, newVal) => {
            valChangedFired = true;
            newValue = newVal;
        };

        // Act - Set standard value
        range.Minimum = 10;
        range.Maximum = 50;
        range.Value = 30;

        Assert.Equal(30, range.Value);
        Assert.True(valChangedFired);
        Assert.Equal(30, newValue);

        // Act - Set value above maximum (coerces to Max)
        valChangedFired = false;
        range.Value = 100;
        Assert.Equal(50, range.Value);
        Assert.True(valChangedFired);
        Assert.Equal(50, newValue);

        // Act - Set value below minimum (coerces to Min)
        valChangedFired = false;
        range.Value = 5;
        Assert.Equal(10, range.Value);
        Assert.True(valChangedFired);
        Assert.Equal(10, newValue);
    }
}
