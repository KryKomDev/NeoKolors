#!/usr/bin/env dotnet

#:property TargetFramework=net11.0
#:property LangVersion=15

#:project ../Src/Common/NeoKolors.Common.csproj
#:project ../Src/Console/NeoKolors.Console.csproj
#:project ../Src/Tui/Core/NeoKolors.Tui.Core.csproj
#:project ../Src/Tui/Elements/NeoKolors.Tui.Elements.csproj
#:project ../Src/Tui/Fonts/NeoKolors.Tui.Fonts.csproj
#:project ../Src/Tui/Lifecycle/Core/NeoKolors.Tui.Lifecycle.Core.csproj
#:project ../Src/Tui/Lifecycle/Game2D/NeoKolors.Tui.Lifecycle.Game2D.csproj

using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Lifecycle.Game2D;

// =============================================================================
//  COSMIC DEFENDER 2D - Powered by NeoKolors.Tui.Lifecycle.Game2D
//  Demonstrating:
//   • Decoupled 60Hz deterministic physics simulation (FixedTimestepAccumulator)
//   • Variable frame-rate logic and particle lifecycle updates (FrameClock)
//   • Smooth inter-frame render interpolation (Alpha)
//   • Multi-layer 2D rendering pipeline (Background, World, Entities, Particles, UI)
//   • 2D Camera viewport tracking and AABB culling
// =============================================================================

// 1. Setup Logging
using var logStream = new FileStream("./logs/example_game.blog", FileMode.Create, FileAccess.Write);

NKDebug.Logger.Writer = new CompositeLogWriter(
    new BinaryLogWriter(logStream),
    TextLogWriter.CreateFromFile("./logs/example_game.log", append: false)
);

Console.OutputEncoding = Encoding.UTF8;

// 2. Initialize Display and Game Loop
var initialSize = NKConsole.WindowSize;
var screenSize  = initialSize is { X: > 20, Y: > 10 } ? initialSize : new Size2D(100, 32);
var screen      = new NKCharScreen(screenSize);

var game = new Game2DLoop {
    TargetFps = 60
};

// Fixed simulation timestep: 60 Hz (~16.666 ms per physics tick)
game.Accumulator.TargetStep          = TimeSpan.FromSeconds(1.0 / 60.0);
game.Accumulator.MaxAccumulatedDelta = TimeSpan.FromSeconds(0.25);

// Configure Camera viewport
game.Camera.ViewportWidth  = screenSize.X;
game.Camera.ViewportHeight = screenSize.Y;

// 3. Game State Definition
var state = new CosmicGameState(arenaWidth: screenSize.X - 2, arenaHeight: screenSize.Y - 4);
game.UserState = state;

// 4. Configure Multi-Stage 2D Pipeline
game.Pipeline.AddStage(new BackgroundStarfieldStage(screen, state));
game.Pipeline.AddStage(new WorldArenaStage(screen, state));
game.Pipeline.AddStage(new EntitiesStage(screen, state));
game.Pipeline.AddStage(new ParticlesStage(screen, state));
game.Pipeline.AddStage(new HudAndPresentStage(screen, state, game));

// 5. Connect Lifecycle Handlers
// 5.1 Fixed Simulation Step (Physics, Collision Detection, Enemy AI)
game.FixedUpdating += ctx => {
    if (state.IsGameOver || state.IsPaused)
        return;

    state.FixedSimulate(ctx.FixedDeltaSecondsF, ctx.FixedTickIndex);
};

// 5.2 Variable Update (Particle decay, timers, starfield parallax, camera tracking)
game.Updating += ctx => {
    state.VariableUpdate(ctx.Time.DeltaSecondsF);

    // Smooth camera tracking
    float targetCamX = state.Player.X;
    game.Camera.X = Math.Clamp(targetCamX, screenSize.X / 2.0f, Math.Max(screenSize.X / 2.0f, state.ArenaWidth - (screenSize.X / 2.0f)));
    game.Camera.Y = screenSize.Y / 2.0f;
};

// 6. Responsive Window Resizing
void HandleResize() {
    var winSize = NKConsole.WindowSize;

    if (winSize != screen.Size && winSize is { X: > 30, Y: > 15 }) {
        screen.Resize(winSize);
        game.Camera.ViewportWidth  = winSize.X;
        game.Camera.ViewportHeight = winSize.Y;
        state.ArenaWidth           = winSize.X - 2;
        state.ArenaHeight          = winSize.Y - 4;
        state.RebalanceStars(winSize.X, winSize.Y);
    }
}

// 7. Input Handling
NKConsole.Key += k => {
    if (k.Down) {
        if (k.Key == KeyCode.ESCAPE || k.Char is 'q' or 'Q' || (k.Key == KeyCode.C && k.Modifiers.HasFlag(KeyModifiers.LEFT_CTRL))) {
            game.Stop();
            return;
        }

        if (k.Key == KeyCode.P || k.Char is 'p' or 'P') {
            state.IsPaused = !state.IsPaused;
            return;
        }

        if (state.IsGameOver && (k.Key == KeyCode.R || k.Char is 'r' or 'R' || k.Key == KeyCode.SPACE)) {
            state.Reset();
            return;
        }

        // Steer left / right
        if (k.Key is KeyCode.ARROW_LEFT or KeyCode.A) {
            state.Player.Vx -= 18.0f;
        }
        else if (k.Key is KeyCode.ARROW_RIGHT or KeyCode.D) {
            state.Player.Vx += 18.0f;
        }

        // Thruster up / down
        if (k.Key is KeyCode.ARROW_UP or KeyCode.W) {
            state.Player.Vy -= 12.0f;
        }
        else if (k.Key is KeyCode.ARROW_DOWN or KeyCode.S) {
            state.Player.Vy += 12.0f;
        }

        // Fire twin lasers
        if (k.Key == KeyCode.SPACE) {
            state.TryFireLaser();
        }
    }
};

Console.CancelKeyPress += (_, e) => {
    e.Cancel = true;
    game.Stop();
};

// 8. Terminal Initialization & Main Loop Execution
try {
    NKConsole.HideCursor();
    NKConsole.EnableAltBuffer();
    NKConsole.ResetModes(EscapeCodes.DecMode.AUTOWRAP);
    NKConsole.InputDriver.Config.MouseReportLevel = ReportedMouseEvents.NONE;
    NKConsole.StartInputInterception();

    game.Start();

    // Check resize prior to each frame
    game.InputPolling += HandleResize;

    // Run the game loop
    game.Run();
}
finally {
    game.Stop();
    NKConsole.StopInputInterception();
    NKConsole.SetModes(EscapeCodes.DecMode.AUTOWRAP);
    NKConsole.DisableAltBuffer();
    NKConsole.ShowCursor();
}

// =============================================================================
//  GAME STATE & SIMULATION LOGIC
// =============================================================================

public class CosmicGameState {
    public float ArenaWidth;
    public float ArenaHeight;

    public PlayerShip     Player     { get; }
    public List<Bullet>   Bullets    { get; } = [];
    public List<Enemy>    Enemies    { get; } = [];
    public List<Particle> Particles  { get; } = [];
    public List<Star>     Stars      { get; } = [];

    public int   Score       { get; set; } = 0;
    public int   HighScore   { get; set; } = 5000;
    public int   Wave        { get; set; } = 1;
    public bool  IsGameOver  { get; set; } = false;
    public bool  IsPaused    { get; set; } = false;

    private float _spawnCooldown = 1.0f;
    private float _waveTimer     = 0.0f;
    private readonly Random _rng = new();

    public CosmicGameState(float arenaWidth, float arenaHeight) {
        ArenaWidth  = arenaWidth;
        ArenaHeight = arenaHeight;
        Player      = new PlayerShip(arenaWidth / 2.0f, arenaHeight - 4.0f);
        InitStars((int)arenaWidth, (int)arenaHeight);
    }

    public void Reset() {
        Player.Reset(ArenaWidth / 2.0f, ArenaHeight - 4.0f);
        Bullets.Clear();
        Enemies.Clear();
        Particles.Clear();
        Score         = 0;
        Wave          = 1;
        IsGameOver    = false;
        IsPaused      = false;
        _spawnCooldown = 1.0f;
        _waveTimer     = 0.0f;
    }

    public void InitStars(int w, int h) {
        Stars.Clear();
        int count = Math.Max(40, (w * h) / 35);

        for (int i = 0; i < count; i++) {
            float speed = (float)(_rng.NextDouble() * 3.5 + 0.8);
            char sym = speed > 3.0f ? '+' : speed > 1.8f ? '*' : '.';
            var col = speed > 3.0f ? NKColor.Cyan : speed > 1.8f ? NKColor.White : NKColor.DarkGray;
            Stars.Add(new Star((float)(_rng.NextDouble() * w), (float)(_rng.NextDouble() * h), speed, sym, col));
        }
    }

    public void RebalanceStars(int w, int h) {
        InitStars(w, h);
    }

    public void TryFireLaser() {
        if (IsGameOver || IsPaused || Player.FireCooldown > 0f)
            return;

        Player.FireCooldown = 0.18f;

        // Twin laser cannons
        Bullets.Add(new Bullet(Player.X - 1.2f, Player.Y - 1.0f, -42.0f, isEnemy: false));
        Bullets.Add(new Bullet(Player.X + 1.2f, Player.Y - 1.0f, -42.0f, isEnemy: false));

        // Muzzle flare particles
        SpawnSparks(Player.X - 1.2f, Player.Y - 1.0f, 2, NKColor.Yellow);
        SpawnSparks(Player.X + 1.2f, Player.Y - 1.0f, 2, NKColor.Yellow);
    }

    public void FixedSimulate(float dt, ulong tick) {
        // 1. Snapshot previous positions for smooth interpolation
        Player.PrevX = Player.X;
        Player.PrevY = Player.Y;

        for (int i = 0; i < Bullets.Count; i++) {
            Bullets[i].PrevX = Bullets[i].X;
            Bullets[i].PrevY = Bullets[i].Y;
        }

        for (int i = 0; i < Enemies.Count; i++) {
            Enemies[i].PrevX = Enemies[i].X;
            Enemies[i].PrevY = Enemies[i].Y;
        }

        // 2. Player Physics Integration (Drag & Bounds)
        Player.X  += Player.Vx * dt;
        Player.Y  += Player.Vy * dt;
        Player.Vx *= MathF.Pow(0.82f, dt * 60.0f);
        Player.Vy *= MathF.Pow(0.82f, dt * 60.0f);

        // Clamp player within arena bounds
        Player.X = Math.Clamp(Player.X, 3.0f, ArenaWidth - 3.0f);
        Player.Y = Math.Clamp(Player.Y, ArenaHeight * 0.55f, ArenaHeight - 2.0f);

        // 3. Advance Bullets
        for (int i = Bullets.Count - 1; i >= 0; i--) {
            var b = Bullets[i];
            b.Y += b.Vy * dt;

            if (b.Y < 1.0f || b.Y > ArenaHeight) {
                Bullets.RemoveAt(i);
            }
        }

        // 4. Enemy Spawning & AI
        _waveTimer += dt;

        if (_waveTimer > 25.0f) {
            Wave++;
            _waveTimer = 0f;
            // Wave announcement particles
            SpawnSparks(ArenaWidth / 2f, 4f, 30, NKColor.Magenta);
        }

        _spawnCooldown -= dt;

        if (_spawnCooldown <= 0f) {
            float spawnRate = Math.Max(0.6f, 2.2f - (Wave * 0.15f));
            _spawnCooldown = spawnRate;
            SpawnEnemy();
        }

        // 5. Advance Enemies
        for (int i = Enemies.Count - 1; i >= 0; i--) {
            var e = Enemies[i];
            e.X += e.Vx * dt;
            e.Y += e.Vy * dt;

            // Bounce off left/right arena walls
            if (e.X < 3.0f) {
                e.X  = 3.0f;
                e.Vx = Math.Abs(e.Vx);
            }
            else if (e.X > ArenaWidth - 3.0f) {
                e.X  = ArenaWidth - 3.0f;
                e.Vx = -Math.Abs(e.Vx);
            }

            // Enemy firing logic
            e.ShootTimer -= dt;

            if (e is { ShootTimer: <= 0f, CanShoot: true }) {
                e.ShootTimer = (float)(_rng.NextDouble() * 2.0 + 1.5);
                Bullets.Add(new Bullet(e.X, e.Y + 1.0f, 18.0f, isEnemy: true));
            }

            // Clean up if reached bottom
            if (e.Y > ArenaHeight) {
                Enemies.RemoveAt(i);
                Player.Shield = Math.Max(0, Player.Shield - 10);

                if (Player.Shield <= 0) {
                    TriggerGameOver();
                }
            }
        }

        // 6. Collision Detection: Player Lasers vs Enemies
        for (int bIdx = Bullets.Count - 1; bIdx >= 0; bIdx--) {
            var b = Bullets[bIdx];

            if (b.IsEnemy)
                continue;

            for (int eIdx = Enemies.Count - 1; eIdx >= 0; eIdx--) {
                var e = Enemies[eIdx];

                if (MathF.Abs(b.X - e.X) < (e.Radius + 0.6f) && MathF.Abs(b.Y - e.Y) < 1.0f) {
                    Bullets.RemoveAt(bIdx);
                    e.Health -= 25;

                    SpawnSparks(b.X, b.Y, 5, NKColor.Yellow);

                    if (e.Health <= 0) {
                        Score += e.ScoreVal;

                        if (Score > HighScore) HighScore = Score;

                        SpawnExplosion(e.X, e.Y, e.Color);
                        Enemies.RemoveAt(eIdx);
                    }

                    break;
                }
            }
        }

        // 7. Collision Detection: Enemy Lasers vs Player
        for (int bIdx = Bullets.Count - 1; bIdx >= 0; bIdx--) {
            var b = Bullets[bIdx];

            if (!b.IsEnemy)
                continue;

            if (MathF.Abs(b.X - Player.X) < 1.8f && MathF.Abs(b.Y - Player.Y) < 1.0f) {
                Bullets.RemoveAt(bIdx);
                Player.Shield = Math.Max(0, Player.Shield - 15);
                SpawnSparks(Player.X, Player.Y, 8, NKColor.Red);

                if (Player.Shield <= 0) {
                    TriggerGameOver();
                }
            }
        }

        // 8. Collision Detection: Enemies ramming Player
        for (int eIdx = Enemies.Count - 1; eIdx >= 0; eIdx--) {
            var e = Enemies[eIdx];

            if (MathF.Abs(e.X - Player.X) < 2.0f && MathF.Abs(e.Y - Player.Y) < 1.2f) {
                Player.Shield = Math.Max(0, Player.Shield - 30);
                SpawnExplosion(e.X, e.Y, e.Color);
                Enemies.RemoveAt(eIdx);

                if (Player.Shield <= 0) {
                    TriggerGameOver();
                }
            }
        }
    }

    public void VariableUpdate(float dt) {
        // Laser cooldown
        if (Player.FireCooldown > 0f) {
            Player.FireCooldown -= dt;
        }

        // Advance stars
        for (int i = 0; i < Stars.Count; i++) {
            var s = Stars[i];
            s.Y += s.Speed * dt * 4.0f;

            if (s.Y >= ArenaHeight) {
                s.Y = 1.0f;
                s.X = (float)(_rng.NextDouble() * ArenaWidth);
            }
        }

        // Advance and fade particles
        for (int i = Particles.Count - 1; i >= 0; i--) {
            var p = Particles[i];
            p.X    += p.Vx * dt;
            p.Y    += p.Vy * dt;
            p.Life -= dt;

            if (p.Life <= 0f) {
                Particles.RemoveAt(i);
            }
        }
    }

    private void SpawnEnemy() {
        int type = _rng.Next(3);
        float x  = (float)(_rng.NextDouble() * (ArenaWidth - 10.0f) + 5.0f);

        switch (type) {
            case 0:
                // Swift Scout Invader
                Enemies.Add(
                    new Enemy(
                        x,
                        1.5f,
                        vx: (float)(_rng.NextDouble() * 12.0 - 6.0),
                        vy: 2.2f + (Wave * 0.4f),
                        health: 25,
                        symbol: 'V',
                        color: NKColor.Cyan,
                        score: 100,
                        radius: 1.0f,
                        canShoot: true
                    )
                );
                break;
            case 1:
                // Armored Asteroid
                Enemies.Add(
                    new Enemy(
                        x,
                        1.5f,
                        vx: (float)(_rng.NextDouble() * 4.0 - 2.0),
                        vy: 1.4f + (Wave * 0.2f),
                        health: 75,
                        symbol: 'O',
                        color: NKColor.Yellow,
                        score: 150,
                        radius: 1.4f,
                        canShoot: false
                    )
                );
                break;
            case 2:
                // Dreadnought Cruiser
                Enemies.Add(
                    new Enemy(
                        x,
                        1.5f,
                        vx: (float)(_rng.NextDouble() * 6.0 - 3.0),
                        vy: 1.0f + (Wave * 0.15f),
                        health: 125,
                        symbol: 'W',
                        color: NKColor.Magenta,
                        score: 250,
                        radius: 2.0f,
                        canShoot: true
                    )
                );
                break;
        }
    }

    public void SpawnSparks(float x, float y, int count, NKColor color) {
        for (int i = 0; i < count; i++) {
            float angle = (float)(_rng.NextDouble() * Math.PI * 2.0);
            float speed = (float)(_rng.NextDouble() * 14.0 + 3.0);
            char sym    = _rng.Next(2) == 0 ? '.' : '*';
            Particles.Add(new Particle(x, y, MathF.Cos(angle) * speed, MathF.Sin(angle) * speed, life: 0.35f, sym, color));
        }
    }

    public void SpawnExplosion(float x, float y, NKColor color) {
        for (int i = 0; i < 18; i++) {
            float angle = (float)(_rng.NextDouble() * Math.PI * 2.0);
            float speed = (float)(_rng.NextDouble() * 20.0 + 4.0);
            char sym    = _rng.Next(3) switch { 0 => '*', 1 => '+', _ => 'o' };
            Particles.Add(new Particle(x, y, MathF.Cos(angle) * speed, MathF.Sin(angle) * speed, life: 0.65f, sym, color));
        }
    }

    private void TriggerGameOver() {
        IsGameOver = true;
        SpawnExplosion(Player.X, Player.Y, NKColor.Red);
        SpawnExplosion(Player.X, Player.Y, NKColor.Yellow);
    }
}

public class PlayerShip {
    public float X, Y, PrevX, PrevY, Vx, Vy;
    public int   Shield       = 100;
    public int   MaxShield    = 100;
    public float FireCooldown = 0.0f;

    public PlayerShip(float x, float y) {
        Reset(x, y);
    }

    public void Reset(float x, float y) {
        X            = x;
        Y            = y;
        PrevX        = x;
        PrevY        = y;
        Vx           = 0f;
        Vy           = 0f;
        Shield       = MaxShield;
        FireCooldown = 0.0f;
    }
}

public class Bullet(float x, float y, float vy, bool isEnemy) {
    public float X     = x;
    public float Y     = y;
    public float PrevX = x;
    public float PrevY = y;
    public float Vy    = vy;
    public bool  IsEnemy = isEnemy;
}

public class Enemy(float x, float y, float vx, float vy, int health, char symbol, NKColor color, int score, float radius, bool canShoot) {
    public float   X          = x;
    public float   Y          = y;
    public float   PrevX      = x;
    public float   PrevY      = y;
    public float   Vx         = vx;
    public float   Vy         = vy;
    public int     Health     = health;
    public int     MaxHealth  = health;
    public char    Symbol     = symbol;
    public NKColor Color      = color;
    public int     ScoreVal   = score;
    public float   Radius     = radius;
    public bool    CanShoot   = canShoot;
    public float   ShootTimer = 1.0f;
}

public class Particle(float x, float y, float vx, float vy, float life, char symbol, NKColor color) {
    public float   X       = x;
    public float   Y       = y;
    public float   Vx      = vx;
    public float   Vy      = vy;
    public float   Life    = life;
    public float   MaxLife = life;
    public char    Symbol  = symbol;
    public NKColor Color   = color;
}

public class Star(float x, float y, float speed, char symbol, NKColor color) {
    public float   X      = x;
    public float   Y      = y;
    public float   Speed  = speed;
    public char    Symbol = symbol;
    public NKColor Color  = color;
}

// =============================================================================
//  2D PIPELINE STAGES
// =============================================================================

// Stage 1: Clean buffer and draw parallax starfield
public class BackgroundStarfieldStage(NKCharScreen screen, CosmicGameState state) : IRender2DStage {
    public string Name => "StarfieldStage";
    public Render2DLayer Layer => Render2DLayer.BACKGROUND;
    public int OrderWithinLayer => 0;
    public bool IsEnabled { get; set; } = true;

    public void Render(Game2DRenderContext context) {
        screen.Clean();
        screen.ResetZ();

        for (int i = 0; i < state.Stars.Count; i++) {
            var star = state.Stars[i];
            int sx   = (int)star.X;
            int sy   = (int)star.Y;

            if (sx >= 0 && sx < screen.Width && sy >= 0 && sy < screen.Height) {
                screen.Draw(new AnsiChar(star.Symbol, new NKStyle(textColor: star.Color)), sx, sy, z: 0);
            }
        }
    }
}

// Stage 2: Draw glowing neon arena boundaries
public class WorldArenaStage(NKCharScreen screen, CosmicGameState state) : IRender2DStage {
    public string Name => "WorldArenaStage";
    public Render2DLayer Layer => Render2DLayer.WORLD;
    public int OrderWithinLayer => 0;
    public bool IsEnabled { get; set; } = true;

    public void Render(Game2DRenderContext context) {
        int w = Math.Min(screen.Width, (int)state.ArenaWidth + 2);
        int h = Math.Min(screen.Height, (int)state.ArenaHeight + 4);

        var wallStyle = new NKStyle(textColor: NKColor.DarkCyan);
        var wallCharH = new AnsiChar('═', wallStyle);
        var wallCharV = new AnsiChar('║', wallStyle);

        // Top & Bottom boundary
        screen.DrawHorizontalLine(wallCharH, 1, 1, w - 2, z: 10);
        screen.DrawHorizontalLine(wallCharH, 1, h - 3, w - 2, z: 10);

        // Left & Right boundary
        screen.DrawVerticalLine(wallCharV, 0, 1, h - 3, z: 10);
        screen.DrawVerticalLine(wallCharV, w - 1, 1, h - 3, z: 10);

        // Stylized corner joints
        screen.Draw(new AnsiChar('╔', wallStyle), 0, 1, z: 11);
        screen.Draw(new AnsiChar('╗', wallStyle), w - 1, 1, z: 11);
        screen.Draw(new AnsiChar('╚', wallStyle), 0, h - 3, z: 11);
        screen.Draw(new AnsiChar('╝', wallStyle), w - 1, h - 3, z: 11);
    }
}

// Stage 3: Render player, bullets, and enemies with smooth sub-tick interpolation (Alpha)
public class EntitiesStage(NKCharScreen screen, CosmicGameState state) : IRender2DStage {
    public string Name => "EntitiesStage";
    public Render2DLayer Layer => Render2DLayer.ENTITIES;
    public int OrderWithinLayer => 0;
    public bool IsEnabled { get; set; } = true;

    public void Render(Game2DRenderContext context) {
        float alpha = (float)context.Alpha;

        // 1. Draw Player (with sub-tick position interpolation)
        if (!state.IsGameOver) {
            float playerRenderX = state.Player.PrevX + ((state.Player.X - state.Player.PrevX) * alpha);
            float playerRenderY = state.Player.PrevY + ((state.Player.Y - state.Player.PrevY) * alpha);

            int px = (int)MathF.Round(playerRenderX);
            int py = (int)MathF.Round(playerRenderY);

            var shipStyle    = new NKStyle(textColor: NKColor.Cyan, styles: NKTextStyles.BOLD);
            var wingStyle    = new NKStyle(textColor: NKColor.Blue);
            var thrusterStyle = new NKStyle(textColor: NKColor.Yellow, styles: NKTextStyles.BOLD);

            // Ship Body: ◄▲►
            screen.Draw(new AnsiChar('◄', wingStyle), px - 1, py, z: 30);
            screen.Draw(new AnsiChar('▲', shipStyle), px,     py, z: 31);
            screen.Draw(new AnsiChar('►', wingStyle), px + 1, py, z: 30);

            // Thruster flame animation
            char flameChar = (DateTime.Now.Millisecond % 200 > 100) ? '▼' : '^';
            screen.Draw(new AnsiChar(flameChar, thrusterStyle), px, py + 1, z: 29);
        }

        // 2. Draw Bullets (interpolated)
        for (int i = 0; i < state.Bullets.Count; i++) {
            var b = state.Bullets[i];
            float bx = b.PrevX + ((b.X - b.PrevX) * alpha);
            float by = b.PrevY + ((b.Y - b.PrevY) * alpha);
            int ibx = (int)MathF.Round(bx);
            int iby = (int)MathF.Round(by);

            screen.Draw(
                b.IsEnemy
                    ? new AnsiChar(
                        'v',
                        new NKStyle(textColor: NKColor.Red, styles: NKTextStyles.BOLD)
                    )
                    : new AnsiChar(
                        '│',
                        new NKStyle(textColor: NKColor.Yellow, styles: NKTextStyles.BOLD)
                    ),
                ibx,
                iby,
                z: 40
            );
        }

        // 3. Draw Enemies (interpolated)
        for (int i = 0; i < state.Enemies.Count; i++) {
            var e = state.Enemies[i];
            float ex = e.PrevX + ((e.X - e.PrevX) * alpha);
            float ey = e.PrevY + ((e.Y - e.PrevY) * alpha);
            int iex = (int)MathF.Round(ex);
            int iey = (int)MathF.Round(ey);

            var enemyStyle = new NKStyle(textColor: e.Color, styles: NKTextStyles.BOLD);

            switch (e.Symbol) {
                case 'V':
                    // Scout: <V>
                    screen.Draw(new AnsiChar('<', enemyStyle), iex - 1, iey, z: 35);
                    screen.Draw(new AnsiChar('V', enemyStyle), iex,     iey, z: 36);
                    screen.Draw(new AnsiChar('>', enemyStyle), iex + 1, iey, z: 35);
                    break;
                case 'O':
                    // Asteroid: (O)
                    screen.Draw(new AnsiChar('(', enemyStyle), iex - 1, iey, z: 35);
                    screen.Draw(new AnsiChar('O', enemyStyle), iex,     iey, z: 36);
                    screen.Draw(new AnsiChar(')', enemyStyle), iex + 1, iey, z: 35);
                    break;
                case 'W':
                    // Cruiser: [=W=]
                    screen.Draw(new AnsiChar('[', enemyStyle), iex - 2, iey, z: 35);
                    screen.Draw(new AnsiChar('=', enemyStyle), iex - 1, iey, z: 35);
                    screen.Draw(new AnsiChar('W', enemyStyle), iex,     iey, z: 37);
                    screen.Draw(new AnsiChar('=', enemyStyle), iex + 1, iey, z: 35);
                    screen.Draw(new AnsiChar(']', enemyStyle), iex + 2, iey, z: 35);
                    break;
            }

            // Health bar above damaged enemy
            if (e.Health < e.MaxHealth && iey > 2) {
                int hpWidth = 3;
                int filled = (int)MathF.Round((e.Health / (float)e.MaxHealth) * hpWidth);
                string hpBar = new string('■', filled) + new string('□', hpWidth - filled);
                screen.Draw(new AnsiString(hpBar, new NKStyle(textColor: NKColor.Green)), iex - 1, iey - 1, z: 38);
            }
        }
    }
}

// Stage 4: Render explosions and spark particles
public class ParticlesStage(NKCharScreen screen, CosmicGameState state) : IRender2DStage {
    public string Name => "ParticlesStage";
    public Render2DLayer Layer => Render2DLayer.PARTICLES;
    public int OrderWithinLayer => 0;
    public bool IsEnabled { get; set; } = true;

    public void Render(Game2DRenderContext context) {
        for (int i = 0; i < state.Particles.Count; i++) {
            var p = state.Particles[i];
            int px = (int)MathF.Round(p.X);
            int py = (int)MathF.Round(p.Y);

            if (px >= 0 && px < screen.Width && py >= 0 && py < screen.Height) {
                // Color fade based on remaining lifetime
                float lifeRatio = p.Life / p.MaxLife;
                var col = lifeRatio > 0.6f ? p.Color : lifeRatio > 0.3f ? NKColor.DarkYellow : NKColor.DarkGray;
                screen.Draw(new AnsiChar(p.Symbol, new NKStyle(textColor: col)), px, py, z: 45);
            }
        }
    }
}

// Stage 5: Render HUD statistics, overlays, and presentation flush
public class HudAndPresentStage(NKCharScreen screen, CosmicGameState state, Game2DLoop game) : IRender2DStage {
    public string Name => "HudAndPresentStage";
    public Render2DLayer Layer => Render2DLayer.UI;
    public int OrderWithinLayer => 0;
    public bool IsEnabled { get; set; } = true;

    public void Render(Game2DRenderContext context) {
        int w = screen.Width;
        int h = screen.Height;

        // Top Status Bar
        int shieldBars = Math.Clamp(state.Player.Shield / 10, 0, 10);
        string shieldStr = new string('█', shieldBars) + new string('░', 10 - shieldBars);
        var shieldCol = state.Player.Shield > 50 ? NKColor.Green : state.Player.Shield > 20 ? NKColor.Yellow : NKColor.Red;

        string leftHud = $" SHIELD: [{shieldStr}] {state.Player.Shield}%  |  SCORE: {state.Score:N0}  |  HIGH: {state.HighScore:N0}  |  WAVE: {state.Wave} ";
        screen.Draw(new AnsiString(leftHud, new NKStyle(textColor: shieldCol, styles: NKTextStyles.BOLD)), 2, 0, z: 100);

        string rightHud = $" FPS: {game.Clock.SmoothedFps:F1} | 60Hz Physics | Ticks: #{game.FixedTickCount:N0} ";
        screen.Draw(new AnsiString(rightHud, new NKStyle(textColor: NKColor.Cyan)), Math.Max(2, w - rightHud.Length - 2), 0, z: 100);

        // Bottom Controls Bar
        string controls = " [A/D or ◄/►]: Steer | [W/S]: Thrusters | [Space]: Fire | [P]: Pause | [Q/Esc]: Quit ";
        screen.Draw(new AnsiString(controls, new NKStyle(textColor: NKColor.Gray)), 2, h - 2, z: 100);

        string alphaTelemetry = $" Render Alpha: {context.Alpha:F2} ";
        screen.Draw(new AnsiString(alphaTelemetry, new NKStyle(textColor: NKColor.DarkCyan)), Math.Max(2, w - alphaTelemetry.Length - 2), h - 2, z: 100);

        // Paused Overlay
        if (state.IsPaused && !state.IsGameOver) {
            string pausedBanner = " ⏸  SIMULATION PAUSED  —  PRESS [P] TO RESUME ";
            int bannerX = Math.Max(2, (w - pausedBanner.Length) / 2);
            int bannerY = h / 2;
            screen.Draw(new AnsiString(pausedBanner, new NKStyle(textColor: NKColor.Black, backgroundColor: NKColor.Yellow, styles: NKTextStyles.BOLD)), bannerX, bannerY, z: 120);
        }

        // Game Over Overlay
        if (state.IsGameOver) {
            string[] gameOverBox = [
                "╔═══════════════════════════════════════════════════╗",
                "║         ✦  M I S S I O N   F A I L E D  ✦         ║",
                "║             H U L L   B R E A C H E D             ║",
                "╠═══════════════════════════════════════════════════╣",
               $"║   FINAL SCORE: {state.Score,-10:N0}  WAVE SURVIVED: {state.Wave,-4} ║",
               $"║   TOTAL TICKS: {game.FixedTickCount,-10:N0}  HIGH SCORE:    {state.HighScore,-4:N0} ║",
                "╠═══════════════════════════════════════════════════╣",
                "║        PRESS [R] / [SPACE] TO RE-ENGAGE           ║",
                "║                PRESS [Q] TO EXIT                  ║",
                "╚═══════════════════════════════════════════════════╝"
            ];

            int startY = Math.Max(2, (h - gameOverBox.Length) / 2);

            for (int line = 0; line < gameOverBox.Length; line++) {
                int startX = Math.Max(2, (w - gameOverBox[line].Length) / 2);
                var style = line switch {
                    1 or 2 => new NKStyle(textColor: NKColor.Red,    styles: NKTextStyles.BOLD),
                    7      => new NKStyle(textColor: NKColor.Yellow, styles: NKTextStyles.BOLD),
                    _      => new NKStyle(textColor: NKColor.Cyan)
                };

                screen.Draw(new AnsiString(gameOverBox[line], style), startX, startY + line, z: 150);
            }
        }

        // 7. Flush buffer onto the terminal screen
        screen.Render();
    }
}
