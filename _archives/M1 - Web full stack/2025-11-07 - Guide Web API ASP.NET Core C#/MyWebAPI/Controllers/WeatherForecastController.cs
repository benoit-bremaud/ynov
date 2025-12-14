using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;

namespace MyWebAPI.Controllers;

/// <summary>
/// Weather Forecast API Controller.
/// Provides endpoints for retrieving weather forecast data.
/// </summary>
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    /// <summary>
    /// Array of possible weather conditions.
    /// </summary>
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    /// <summary>
    /// Gets a 5-day weather forecast.
    /// </summary>
    /// <returns>Collection of weather forecasts</returns>
    /// <response code="200">Returns the weather forecast data</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        (
            Date: DateTime.Now.AddDays(index),
            TemperatureC: Random.Shared.Next(-20, 55),
            Summary: Summaries[Random.Shared.Next(Summaries.Length)]
        ));
    }
}
