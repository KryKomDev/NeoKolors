using System.Diagnostics;

namespace NKChess;

public class UciEngine : IDisposable {
    private Process? _process;
    private StreamWriter? _stdin;
    private Task? _readTask;
    private CancellationTokenSource? _cts;

    public string Path { get; }
    public string Name { get; private set; }
    public bool IsStarted => _process is { HasExited: false };

    public event Action<string>? LineRead;
    public event Action<string>? InfoReceived;
    public event Action? Exited;

    public UciEngine(string path) {
        Path = path;
        Name = System.IO.Path.GetFileNameWithoutExtension(path);
    }

    public bool Start() {
        try {
            _process = new Process();
            _process.StartInfo.FileName = Path;
            var dir = System.IO.Path.GetDirectoryName(Path);

            if (!string.IsNullOrEmpty(dir)) {
                _process.StartInfo.WorkingDirectory = dir;
            }

            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.CreateNoWindow = true;

            if (!_process.Start()) {
                return false;
            }

            _stdin = _process.StandardInput;
            _cts = new CancellationTokenSource();

            var uciokTcs = new TaskCompletionSource<bool>();
            var readyokTcs = new TaskCompletionSource<bool>();

            Action<string> onLine = (line) => {
                if (line == "uciok") {
                    uciokTcs.TrySetResult(true);
                }
                else if (line == "readyok") {
                    readyokTcs.TrySetResult(true);
                }
            };

            LineRead += onLine;
            _readTask = Task.Run(() => ReadOutputAsync(_cts.Token));

            // Initialize UCI
            Send("uci");

            // Wait for uciok with a timeout (e.g. 5 seconds)
            var uciokTask = uciokTcs.Task;
            var completedUciok = Task.WhenAny(uciokTask, Task.Delay(5000)).Result;

            if (completedUciok != uciokTask || !uciokTask.Result) {
                LineRead -= onLine;

                return false;
            }

            // Send isready to ensure NNUE is loaded and engine is fully ready
            Send("isready");

            // Wait for readyok with a timeout (e.g. 10 seconds for NNUE loading)
            var readyokTask = readyokTcs.Task;
            var completedReadyok = Task.WhenAny(readyokTask, Task.Delay(10000)).Result;

            if (completedReadyok != readyokTask || !readyokTask.Result) {
                LineRead -= onLine;

                return false;
            }

            LineRead -= onLine;

            return true;
        }
        catch {
            return false;
        }
    }

    public void Send(string cmd) {
        try {
            if (_stdin != null && _process is { HasExited: false }) {
                _stdin.WriteLine(cmd);
                _stdin.Flush();
            }
        }
        catch {
            // Process might have exited
        }
    }

    private async Task ReadOutputAsync(CancellationToken token) {
        try {
            var reader = _process?.StandardOutput;

            if (reader == null) return;

            while (!token.IsCancellationRequested && _process is { HasExited: false }) {
                var line = await reader.ReadLineAsync(token);

                if (line == null) break;

                LineRead?.Invoke(line);

                if (line.StartsWith("id name ")) {
                    Name = line.Substring(8).Trim();
                }
                else if (line.StartsWith("info ")) {
                    InfoReceived?.Invoke(line);
                }
            }
        }
        catch {
            // Process terminated or reading aborted
        }
        finally {
            Exited?.Invoke();
        }
    }

    public async Task<string> GetBestMoveAsync(string fen, int searchTimeMs, CancellationToken token) {
        if (!IsStarted) return "";

        var readyokTcs = new TaskCompletionSource<bool>();
        Action<string> onLineReady = null!;

        onLineReady = (line) => {
            if (line == "readyok") {
                readyokTcs.TrySetResult(true);
                LineRead -= onLineReady;
            }
        };

        LineRead += onLineReady;

        Send("ucinewgame");
        Send("isready");

        var readyokTask = readyokTcs.Task;
        var completed = await Task.WhenAny(readyokTask, Task.Delay(5000, token));

        if (completed != readyokTask) {
            LineRead -= onLineReady;

            return ""; // Timeout or cancellation
        }

        Send($"position fen {fen}");
        Send($"go movetime {searchTimeMs}");

        var tcs = new TaskCompletionSource<string>();

        Action<string> onLine = null!;
        Action onExited = null!;

        var exited1 = onExited;

        var line1 = onLine;

        onLine = (line) => {
            if (!line.StartsWith("bestmove "))
                return;

            var parts = line.Split(' ');

            tcs.TrySetResult(parts.Length > 1 ? parts[1] : "");

            LineRead -= line1;
            Exited -= exited1;
        };

        var exited = onExited;

        onExited = () => {
            tcs.TrySetResult("");
            LineRead -= onLine;
            Exited -= exited;
        };

        LineRead += onLine;
        Exited += onExited;

        // Monitor engine exit
        var exitRegistration = token.Register(() => {
            LineRead -= onLine;
            Exited -= onExited;
            tcs.TrySetCanceled();
        });

        try {
            return await tcs.Task;
        }
        catch (TaskCanceledException) {
            return "";
        }
        finally {
            await exitRegistration.DisposeAsync();
        }
    }

    public async Task<string> GetBestMoveAsync(string fen, int wtime, int btime, int winc, int binc, CancellationToken token) {
        if (!IsStarted) return "";

        var readyokTcs = new TaskCompletionSource<bool>();
        Action<string> onLineReady = null!;

        onLineReady = (line) => {
            if (line == "readyok") {
                readyokTcs.TrySetResult(true);
                LineRead -= onLineReady;
            }
        };

        LineRead += onLineReady;

        Send("ucinewgame");
        Send("isready");

        var readyokTask = readyokTcs.Task;
        var completed = await Task.WhenAny(readyokTask, Task.Delay(5000, token));

        if (completed != readyokTask) {
            LineRead -= onLineReady;

            return ""; // Timeout or cancellation
        }

        Send($"position fen {fen}");
        Send($"go wtime {wtime} btime {btime} winc {winc} binc {binc}");

        var tcs = new TaskCompletionSource<string>();

        Action<string> onLine = null!;
        Action onExited = null!;

        var exited1 = onExited;

        var line1 = onLine;

        onLine = (line) => {
            if (line.StartsWith("bestmove ")) {
                var parts = line.Split(' ');

                if (parts.Length > 1) {
                    tcs.TrySetResult(parts[1]);
                }
                else {
                    tcs.TrySetResult("");
                }

                LineRead -= line1;
                Exited -= exited1;
            }
        };

        var exited = onExited;

        onExited = () => {
            tcs.TrySetResult("");
            LineRead -= onLine;
            Exited -= exited;
        };

        LineRead += onLine;
        Exited += onExited;

        // Monitor engine exit
        var exitRegistration = token.Register(() => {
            LineRead -= onLine;
            Exited -= onExited;
            tcs.TrySetCanceled();
        });

        try {
            return await tcs.Task;
        }
        catch (TaskCanceledException) {
            return "";
        }
        finally {
            await exitRegistration.DisposeAsync();
        }
    }

    public void Dispose() {
        _cts?.Cancel();

        try {
            if (_process is { HasExited: false }) {
                _process.Kill();
            }
        }
        catch {
            // ignored
        }

        _process?.Dispose();
        _stdin?.Dispose();
        _cts?.Dispose();
    }
}