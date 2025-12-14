namespace MyWebAPI.Models;

/// <summary>
/// Represents a weather forecast for a single day.
/// </summary>
public record WeatherForecast(
    DateTime Date,
    int TemperatureC,
    string? Summary)
{
    /// <summary>
    /// Temperature in Fahrenheit (computed).
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC * 9.0 / 5.0);
}
