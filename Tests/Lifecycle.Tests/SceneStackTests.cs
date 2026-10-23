// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Lifecycle.Core;

namespace NeoKolors.Tui.Lifecycle.Tests;

public class SceneStackTests {
    private sealed class TestScene(string name) : IScene<string> {
        public string Name { get; } = name;
        public bool IsActive { get; private set; }
        public List<string> History { get; } = [];

        public void OnEnter(string context) {
            IsActive = true;
            History.Add($"Enter:{context}");
        }

        public void OnExit(string context) {
            IsActive = false;
            History.Add($"Exit:{context}");
        }

        public void OnPause(string context) {
            IsActive = false;
            History.Add($"Pause:{context}");
        }

        public void OnResume(string context) {
            IsActive = true;
            History.Add($"Resume:{context}");
        }

        public void OnUpdate(string context, DeltaTime time) {
            History.Add($"Update:{context}");
        }
    }

    [Fact]
    public void SceneStack_PushAndPop_NotifiesProperly() {
        var stack = new SceneStack<string>();
        var scene1 = new TestScene("Menu");
        var scene2 = new TestScene("Gameplay");

        stack.PushScene(scene1, "ctx1");
        Assert.Equal(scene1, stack.CurrentScene);
        Assert.Equal(1, stack.SceneCount);
        Assert.True(scene1.IsActive);
        Assert.Contains("Enter:ctx1", scene1.History);

        // Push scene2: scene1 pauses, scene2 enters
        stack.PushScene(scene2, "ctx2");
        Assert.Equal(scene2, stack.CurrentScene);
        Assert.Equal(2, stack.SceneCount);
        Assert.False(scene1.IsActive);
        Assert.True(scene2.IsActive);
        Assert.Contains("Pause:ctx2", scene1.History);
        Assert.Contains("Enter:ctx2", scene2.History);

        // Pop scene2: scene2 exits, scene1 resumes
        var popped = stack.PopScene("ctx3");
        Assert.Equal(scene2, popped);
        Assert.Equal(scene1, stack.CurrentScene);
        Assert.Equal(1, stack.SceneCount);
        Assert.False(scene2.IsActive);
        Assert.True(scene1.IsActive);
        Assert.Contains("Exit:ctx3", scene2.History);
        Assert.Contains("Resume:ctx3", scene1.History);
    }

    [Fact]
    public void SceneStack_SwitchScene_ReplacesActiveScene() {
        var stack = new SceneStack<string>();
        var scene1 = new TestScene("Level1");
        var scene2 = new TestScene("Level2");

        stack.PushScene(scene1, "ctx");
        stack.SwitchScene(scene2, "ctx");

        Assert.Equal(scene2, stack.CurrentScene);
        Assert.Equal(1, stack.SceneCount);
        Assert.False(scene1.IsActive);
        Assert.True(scene2.IsActive);
    }

    [Fact]
    public void SceneStack_Update_DispatchesToCurrentScene() {
        var stack = new SceneStack<string>();
        var scene = new TestScene("Game");
        stack.PushScene(scene, "ctx");

        stack.Update("ctx", DeltaTime.Zero);
        Assert.Contains("Update:ctx", scene.History);
    }
}
