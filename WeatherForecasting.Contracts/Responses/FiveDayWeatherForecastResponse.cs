namespace WeatherForecasting.Contracts.Responses;

public class FiveDayWeatherForecastResponse
{
    public string? City { get; set; }
    public DateTime Date { get; set; }
    public string WeatherDescription { get; set; } = null!;
    public double TemperatureC { get; set; }
    public double TemperatureF { get; set; }
}
