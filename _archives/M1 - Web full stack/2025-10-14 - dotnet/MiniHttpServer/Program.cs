// ==========================
// Program.cs (net8.0)
// Minimal HTTP server using HttpListener
// - Configurable listen prefix via HTTP_PREFIX or HTTP_PORT
// - Basic routing: /health (text), /home (HTML), /api/welcome (GET/POST JSON)
// - Validation via DataAnnotations
// - Structured logging to console + file with per-request duration
// ==========================

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;   // Stopwatch for request duration
using System.IO;            // Directory/File for logs
using System.Net;
using System.Text;
using System.Text.Json;

/// <summary>
/// Resolve the HttpListener prefix to bind on.
/// Priority:
/// 1) HTTP_PREFIX (full URL prefix, must end with '/')
/// 2) HTTP_PORT (port only, e.g. 8081) -> "http://localhost:{port}/"
/// 3) Default -> "http://localhost:8080/"
/// </summary>
/// <remarks>
/// HttpListener requires a *full* scheme+host+port (optional path) and the prefix
/// MUST end with a trailing slash '/'. If not present, the listener will throw.
/// </remarks>
/// <returns>
/// A normalized prefix string suitable for <see cref="HttpListener.Prefixes"/>.
/// </returns>
/// <exception cref="ArgumentException">
/// Thrown when the computed prefix does not start with "http://" or "https://".
/// </exception>
static string ResolvePrefix()
{
    // 1) Read full URL prefix from env (highest priority)
    var fromEnv = Environment.GetEnvironmentVariable("HTTP_PREFIX");
    // 2) Otherwise, accept a simple port (e.g., "8081")
    var portOnly = Environment.GetEnvironmentVariable("HTTP_PORT");

    string prefix;

    if (!string.IsNullOrWhiteSpace(fromEnv))
    {
        prefix = fromEnv.Trim();
    }
    else if (!string.IsNullOrWhiteSpace(portOnly))
    {
        prefix = $"http://localhost:{portOnly.Trim()}/";
    }
    else
    {
        // Fallback if nothing provided
        prefix = "http://localhost:8080/";
    }

    // Normalize: ensure final slash (HttpListener requires it)
    if (!prefix.EndsWith("/")) prefix += "/";

    // Very small sanity check on scheme
    if (!prefix.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
        && !prefix.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
    {
        throw new ArgumentException($"Invalid prefix (missing scheme): {prefix}");
    }

    return prefix;
}

// =========== Top-level application bootstrapping ===========

HttpListener? server = null;
var cts = new CancellationTokenSource();

// Graceful shutdown on Ctrl+C
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;         // don't kill the process immediately
    cts.Cancel();            // signal loops to stop
    try { server?.Stop(); }  // stop listener if running
    catch { /* already stopped */ }
};

server = new HttpListener();
var prefix = ResolvePrefix();
server.Prefixes.Add(prefix);
Log.Info($"Listening prefix: {prefix}");

try
{
    server.Start();
    Log.Info($"Server started on {prefix} (Ctrl+C to stop)");

    while (!cts.IsCancellationRequested)
    {
        HttpListenerContext? context = null;

        // Capture request/response references
        var req = context.Request;
        var res = context.Response;
                   
        // Per-request correlation id (short GUID)
        var correlationId = Guid.NewGuid().ToString("n")[..8];

        try
        {
            // Wait asynchronously for the next request.
            context = await server.GetContextAsync();



            // Measure handling duration for this request
            var sw = Stopwatch.StartNew();

            // Optional: first line to easily spot incoming requests
            // Log.Info($"REQ\t{req.HttpMethod}\t{req.RawUrl}");
            Log.Info($"REQ\t{req.HttpMethod}\t{req.RawUrl}\tcid={correlationId}");


            // ---- Basic routing ----
            if (req.HttpMethod == "GET" && (req.RawUrl == "/" || req.RawUrl == "/health"))
            {
                // Simple liveness probe (text/plain)
                await WriteText(res, 200, "OK", "text/plain; charset=utf-8", correlationId);
            }
            else if (req.HttpMethod == "GET" && req.RawUrl == "/home")
            {
                // Minimal HTML page
                var html = """
                <!doctype html>
                <html lang="fr">
                  <head><meta charset="utf-8"><title>Home</title></head>
                  <body style="font-family:sans-serif">
                    <h1>Bienvenue sur notre serveur HTTP simple</h1>
                    <p>Endpoints : <code>/health</code>, <code>/home</code>, <code>/api/welcome</code> (GET/POST)</p>
                  </body>
                </html>
                """;
                await WriteText(res, 200, html, "text/html; charset=utf-8", correlationId);
            }
            else if (req.HttpMethod == "GET" && req.RawUrl == "/api/welcome")
            {
                // JSON response with a friendly message
                var payload = new { path = req.RawUrl, message = "Bienvenue sur notre serveur HTTP simple" };
                await WriteJson(res, 200, payload, correlationId);
            }
            else if (req.HttpMethod == "POST" && req.RawUrl == "/api/welcome")
            {
                // Read request body as UTF-8
                using var reader = new StreamReader(req.InputStream, Encoding.UTF8);
                var body = await reader.ReadToEndAsync();

                RequestData? data;

                // JSON parsing safety
                try
                {
                    data = JsonSerializer.Deserialize<RequestData>(body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (JsonException jx)
                {
                    await WriteJson(res, 400, new { error = $"JSON invalide: {jx.Message}", correlationId }, correlationId);
                    sw.Stop();
                    var uaBad = req.Headers["User-Agent"] ?? "-";
                    Log.Info($"{req.HttpMethod}\t{req.RawUrl}\t{res.StatusCode}\t{sw.ElapsedMilliseconds}ms\t{uaBad}");
                    continue;
                }

                // Input validation (DataAnnotations)
                try
                {
                    Validator.ValidateObject(data!, new ValidationContext(data!), validateAllProperties: true);
                }
                catch (ValidationException vx)
                {
                    await WriteJson(res, 400, new { error = vx.Message });
                    sw.Stop();
                    var uaBad = req.Headers["User-Agent"] ?? "-";
                    Log.Info($"{req.HttpMethod}\t{req.RawUrl}\t{res.StatusCode}\t{sw.ElapsedMilliseconds}ms\t{uaBad}");
                    continue;
                }

                // Happy path response
                var reply = new
                {
                    path = req.RawUrl,
                    message = $"Bienvenue sur notre serveur HTTP simple, {data!.Name}"
                };
                await WriteJson(res, 200, reply);

                // Finalize per-request log
                sw.Stop();
                var uaOk = req.Headers["User-Agent"] ?? "-";
                Log.Info($"{req.HttpMethod}\t{req.RawUrl}\t{res.StatusCode}\t{sw.ElapsedMilliseconds}ms\t{uaOk}");
            }
            #if DEBUG
            else if (req.HttpMethod == "GET" && req.RawUrl == "/debug/boom")
            {
                // Simulate an unhandled server-side error
                throw new InvalidOperationException("Boom (DEBUG)");
            }
            #endif
            else
            {
                // Unknown route
                await WriteText(res, 404, "Not Found", "text/plain; charset=utf-8", correlationId);

                // Per-request log
                var ua = req.Headers["User-Agent"] ?? "-";
                sw.Stop();
                Log.Info($"{req.HttpMethod}\t{req.RawUrl}\t{res.StatusCode}\t{sw.ElapsedMilliseconds}ms\t{ua}");
            }
        }
        catch (HttpListenerException) when (cts.IsCancellationRequested)
        {
            // Graceful stop in progress — swallow
        }
        catch (Exception ex)
        {
            // Catch-all safeguard for unexpected exceptions
            Log.Error($"Unhandled exception: {ex}");

            if (context is not null)
            {
                await WriteJson(context.Response, 500, new { error = "Internal Server Error", correlationId }, correlationId);
                // No Stopwatch here because an exception might have occurred before we created it
                var ua = context.Request?.Headers["User-Agent"] ?? "-";
                Log.Info($"{context.Request?.HttpMethod ?? "-"}\t{context.Request?.RawUrl ?? "-"}\t500\t- ms\t{ua}");
            }
        }
    }
}
finally
{
    if (server?.IsListening == true) server.Stop();
    server?.Close();
    Log.Info($"{req.HttpMethod}\t{req.RawUrl}\t{res.StatusCode}\t{sw.ElapsedMilliseconds}ms\t{ua}\tcid={correlationId}");
}

// =============== Helpers ===============

/// <summary>
/// Write a plain text HTTP response, with status code and content-type.
/// Closes the response stream when done.
/// </summary>
static async Task WriteText(HttpListenerResponse res, int status, string text, string contentType, string correlationId = "")
{
    res.StatusCode = status;
    res.ContentType = contentType;
    if (!string.IsNullOrEmpty(correlationId))
        res.Headers["X-Correlation-Id"] = correlationId;
    
    var bytes = Encoding.UTF8.GetBytes(text);
    res.ContentLength64 = bytes.Length;
    await res.OutputStream.WriteAsync(bytes);
    res.Close(); // Always close to avoid resource leaks.
}

/// <summary>
/// Serialize <paramref name="obj"/> as JSON and write it as the HTTP response.
/// Uses UTF-8 and sets "application/json; charset=utf-8".
/// </summary>
static async Task WriteJson(HttpListenerResponse res, int status, object obj, string correlationId = "")
{
    var json = JsonSerializer.Serialize(obj);
    await WriteText(res, status, json, "application/json; charset=utf-8", correlationId);
}

/// <summary>
/// Minimal DTO used for POST /api/welcome.
/// </summary>
internal sealed class RequestData
{
    /// <summary>
    /// Required user name (non-empty). The server replies with a personalized message.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Le champ 'name' est requis.")]
    public string Name { get; init; } = default!;
}

// =============== Minimal structured logger ===============

/// <summary>
/// Very small logger that writes to both console and a rolling text file.
/// The format is TSV-like: timestamp(UTC), level, message.
/// Also used to print per-request lines with method, path, status, duration, user-agent.
/// </summary>
static class Log
{
    private static readonly object _lock = new();
    private static readonly string _logDir = Path.Combine(AppContext.BaseDirectory, "logs");
    private static readonly string _logFile = Path.Combine(_logDir, "server.log");

    static Log()
    {
        // Create directory recursively if it does not already exist (idempotent).
        Directory.CreateDirectory(_logDir);
    }

    /// <summary>Write an INFO line.</summary>
    public static void Info(string msg)  => Write("INFO", msg);

    /// <summary>Write an ERROR line.</summary>
    public static void Error(string msg) => Write("ERROR", msg);

    private static void Write(string level, string msg)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        var line = $"{timestamp}\t{level}\t{msg}";

        lock (_lock)
        {
            Console.WriteLine(line);
            using var sw = File.AppendText(_logFile); // Append in UTF-8; creates file if missing
            sw.WriteLine(line);
        }
    }
}
