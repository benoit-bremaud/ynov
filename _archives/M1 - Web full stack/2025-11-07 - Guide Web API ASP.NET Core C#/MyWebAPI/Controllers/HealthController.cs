using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;
using System.Reflection;

namespace MyWebAPI.Controllers
{
    /// <summary>
    /// Health check controller for monitoring API availability.
    /// Provides basic health status, detailed diagnostics, and ping endpoint.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        // Track application start time (static = shared across all requests)
        private static readonly DateTime _startTime = DateTime.UtcNow;

        /// <summary>
        /// Basic health check endpoint.
        /// Returns API status with uptime and version information.
        /// </summary>
        /// <returns>Health status object</returns>
        /// <remarks>
        /// GET /Health
        /// </remarks>
        [HttpGet]
        public ActionResult<Health> Get()
        {
            // Calculate how long the API has been running
            var uptime = DateTime.UtcNow - _startTime;

            // Get version from assembly (defaults to "1.0.0" if not found)
            var version = Assembly.GetExecutingAssembly()
                .GetName()
                .Version?
                .ToString() ?? "1.0.0";

            // Create health response with details
            var health = new Health
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = version,
                Details = new Dictionary<string, object>
                {
                    { "uptime", $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s" },
                    { "environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development" }
                }
            };

            return Ok(health);
        }

        /// <summary>
        /// Detailed health check with full system information.
        /// </summary>
        /// <returns>Health status with detailed diagnostics</returns>
        /// <remarks>
        /// GET /Health/detailed
        /// </remarks>
        [HttpGet("detailed")]
        public ActionResult<Health> GetDetailed()
        {
            var uptime = DateTime.UtcNow - _startTime;
            var version = Assembly.GetExecutingAssembly()
                .GetName()
                .Version?
                .ToString() ?? "1.0.0";

            // Comprehensive system information
            var details = new Dictionary<string, object>
            {
                { "uptime", $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s" },
                { "environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development" },
                { "machineName", Environment.MachineName },
                { "processorCount", Environment.ProcessorCount },
                { "osVersion", Environment.OSVersion.ToString() },
                { "workingSetMB", Environment.WorkingSet / 1024 / 1024 },
                { "is64BitProcess", Environment.Is64BitProcess }
            };

            var health = new Health
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = version,
                Details = details
            };

            return Ok(health);
        }

        /// <summary>
        /// Simple ping endpoint for connectivity testing.
        /// </summary>
        /// <returns>Pong response with timestamp</returns>
        /// <remarks>
        /// GET /Health/ping
        /// </remarks>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new 
            { 
                message = "pong", 
                timestamp = DateTime.UtcNow 
            });
        }
    }
}
