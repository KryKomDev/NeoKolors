using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace NKChess;

public class EngineConfig {
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public override string ToString() => Name;
}

public class AppPreferences {
    public bool EnableSound { get; set; } = true;
    public bool EnableAnimations { get; set; } = true;
    public bool UseLetterPieces { get; set; } = false;
    public bool UseCompactBoard { get; set; } = false;
    public string WhiteEnginePath { get; set; } = "stockfish.exe";
    public string BlackEnginePath { get; set; } = "stockfish.exe";
    public List<EngineConfig> AddedEngines { get; set; } = new() { new EngineConfig { Name = "Stockfish", Path = "stockfish.exe" } };
    public int WhiteTimeLimit { get; set; } = 1000;
    public int BlackTimeLimit { get; set; } = 1000;

    private static string GetFilePath() {
        return Path.Combine(AppContext.BaseDirectory, "preferences.json");
    }

    public static AppPreferences Load() {
        string path = GetFilePath();
        if (!File.Exists(path)) {
            return new AppPreferences();
        }

        try {
            string json = File.ReadAllText(path);
            var prefs = JsonSerializer.Deserialize<AppPreferences>(json) ?? new AppPreferences();
            if (prefs.AddedEngines == null || prefs.AddedEngines.Count == 0) {
                prefs.AddedEngines = new List<EngineConfig> {
                    new EngineConfig { Name = "Stockfish", Path = "stockfish.exe" }
                };
            }
            if (!string.IsNullOrEmpty(prefs.WhiteEnginePath) && !prefs.AddedEngines.Any(e => e.Path == prefs.WhiteEnginePath)) {
                string name = System.IO.Path.GetFileNameWithoutExtension(prefs.WhiteEnginePath);
                if (string.IsNullOrEmpty(name)) name = "White Engine";
                prefs.AddedEngines.Add(new EngineConfig { Name = name, Path = prefs.WhiteEnginePath });
            }
            if (!string.IsNullOrEmpty(prefs.BlackEnginePath) && !prefs.AddedEngines.Any(e => e.Path == prefs.BlackEnginePath)) {
                string name = System.IO.Path.GetFileNameWithoutExtension(prefs.BlackEnginePath);
                if (string.IsNullOrEmpty(name)) name = "Black Engine";
                prefs.AddedEngines.Add(new EngineConfig { Name = name, Path = prefs.BlackEnginePath });
            }
            return prefs;
        }
        catch {
            return new AppPreferences();
        }
    }

    public void Save() {
        string path = GetFilePath();
        try {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
        catch {
            // ignore failure to write settings
        }
    }
}
