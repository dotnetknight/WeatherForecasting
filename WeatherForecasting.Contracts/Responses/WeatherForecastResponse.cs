namespace WeatherForecasting.Contracts.Responses;

public class WeatherForecastResponse
{
    public string City { get; set; } = null!;
    public DateTime Date { get; set; }
    public string WeatherDescription { get; set; } = null!;
    public double TemperatureC { get; set; }
    public double TemperatureF { get; set; }
}
