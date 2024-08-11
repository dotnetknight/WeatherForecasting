namespace WeatherForecasting.Application.Options;

public class OpenWeatherServiceOptions
{
    public string ApiKey { get; set; } = null!;

    public string BaseUrl { get; set; } = null!;

    public string? Unit { get; set; }

    public int CacheTimeInMinutes { get; set; }
}
