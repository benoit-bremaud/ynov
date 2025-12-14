using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// Starts the MiniHttpServer as a child process on a free port,
/// waits until /health responds, and exposes an HttpClient for tests.
/// </summary>
public sealed class ServerHarness : IAsyncLifetime
{
    public string BaseUrl { get; private set; } = default!;
    public HttpClient Client { get; private set; } = default!;
    private Process? _proc;

    public static int GetFreeTcpPort()
    {
        var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        int port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public async Task InitializeAsync()
    {
        var port = GetFreeTcpPort();
        BaseUrl = $"http://localhost:{port}/";

        // Go from .../MiniHttpServer.Tests/bin/Debug/net8.0/ to repo root
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var serverProjectDir = Path.Combine(repoRoot, "MiniHttpServer");

        // Safety check (helps if the path is wrong)
        if (!File.Exists(Path.Combine(serverProjectDir, "MiniHttpServer.csproj")))
            throw new DirectoryNotFoundException($"Could not locate server project at: {serverProjectDir}");


        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --no-build",
            WorkingDirectory = serverProjectDir,
            UseShellExecute = false,              // needed to set env vars
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Pass the listen prefix to the child process
        psi.Environment["HTTP_PREFIX"] = BaseUrl;

        _proc = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start server process");
        _ = Task.Run(() => Console.WriteLine(_proc.StandardOutput.ReadToEnd()));
        _ = Task.Run(() => Console.Error.WriteLine(_proc.StandardError.ReadToEnd()));

        Client = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(2)
        };

        // Wait until the server answers /health (≤ 3s)
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        while (!cts.IsCancellationRequested)
        {
            try
            {
                var resp = await Client.GetAsync("health", cts.Token);
                if (resp.IsSuccessStatusCode) return;
            }
            catch { /* not ready yet */ }
            await Task.Delay(100, cts.Token);
        }

        throw new TimeoutException("Server did not become ready in time.");
    }

    public async Task DisposeAsync()
    {
        try { Client?.Dispose(); } catch { }
        if (_proc is { HasExited: false })
        {
            _proc.Kill(entireProcessTree: true);
            await _proc.WaitForExitAsync();
        }
        _proc?.Dispose();
    }
}
