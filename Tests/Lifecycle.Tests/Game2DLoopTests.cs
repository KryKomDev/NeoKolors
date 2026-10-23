// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Lifecycle.Game2D;

namespace NeoKolors.Tui.Lifecycle.Tests;

public class Game2DLoopTests {
    private sealed class MockStage(string name, Render2DLayer layer, int order = 0) : IRender2DStage {
        public string Name { get; } = name;
        public Render2DLayer Layer { get; } = layer;
        public int OrderWithinLayer { get; } = order;
        public bool IsEnabled { get; set; } = true;
        public List<double> RenderAlphas { get; } = [];

        public void Render(Game2DRenderContext context) {
            RenderAlphas.Add(context.Alpha);
        }
    }

    private sealed class MockGame2DScene : IGame2DScene {
        public string Name => "Mock2DScene";
        public bool IsActive { get; private set; }
        public int FixedCount { get; private set; }
        public int UpdateCount { get; private set; }
        public int RenderCount { get; private set; }

        public void OnEnter(Game2DUpdateContext context) => IsActive = true;
        public void OnExit(Game2DUpdateContext context) => IsActive = false;
        public void OnPause(Game2DUpdateContext context) => IsActive = false;
        public void OnResume(Game2DUpdateContext context) => IsActive = true;
        public void OnUpdate(Game2DUpdateContext context, DeltaTime time) => UpdateCount++;
        public void FixedUpdate(Game2DFixedContext context) => FixedCount++;
        public void Render(Game2DRenderContext context) => RenderCount++;
    }

    [Fact]
    public void Camera2D_TransformAndCull_WorksAccurately() {
        var camera = new Camera2D {
            X = 100,
            Y = 50,
            Zoom = 1.0f,
            ViewportWidth = 80,
            ViewportHeight = 40
        };

        // Center should map to (40, 20)
        camera.WorldToScreen(100, 50, out float sx, out float sy);
        Assert.Equal(40.0f, sx);
        Assert.Equal(20.0f, sy);

        // Screen center should map back to world center
        camera.ScreenToWorld(40, 20, out float wx, out float wy);
        Assert.Equal(100.0f, wx);
        Assert.Equal(50.0f, wy);

        // Bounding box overlapping camera viewport
        Assert.True(camera.IsAabbVisible(90, 45, 110, 55));

        // Bounding box completely outside viewport
        Assert.False(camera.IsAabbVisible(500, 500, 510, 510));
    }

    [Fact]
    public void Render2DPipeline_ExecutesInStrictLayerOrder() {
        var pipeline = new Render2DPipeline();
        var executionOrder = new List<string>();

        var uiStage = new MockStage("UI", Render2DLayer.UI);
        var bgStage = new MockStage("BG", Render2DLayer.BACKGROUND);
        var spriteStage = new MockStage("Sprites", Render2DLayer.ENTITIES);

        // Add out of order
        pipeline.AddStage(uiStage);
        pipeline.AddStage(bgStage);
        pipeline.AddStage(spriteStage);

        var context = new Game2DRenderContext(0.5, DeltaTime.Zero, new Camera2D());
        pipeline.Execute(context);

        Assert.Single(bgStage.RenderAlphas);
        Assert.Single(spriteStage.RenderAlphas);
        Assert.Single(uiStage.RenderAlphas);
    }

    [Fact]
    public void Game2DLoop_StepSimulation_ExecutesFixedAndRenderSteps() {
        var loop = new Game2DLoop();
        loop.Accumulator.TargetStep = TimeSpan.FromMilliseconds(20);
        var scene = new MockGame2DScene();

        loop.SceneStack.PushScene(scene, new Game2DUpdateContext(DeltaTime.Zero));

        // Advance by 50ms -> should run 2 fixed ticks, 1 variable update, 1 render pass
        loop.StepSimulation(TimeSpan.FromMilliseconds(50));

        Assert.Equal(2, scene.FixedCount);
        Assert.Equal(2UL, loop.FixedTickCount);
        Assert.Equal(1, scene.UpdateCount);
        Assert.Equal(1, scene.RenderCount);
    }
}
