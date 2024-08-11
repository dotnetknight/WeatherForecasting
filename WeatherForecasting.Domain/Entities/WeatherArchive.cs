namespace WeatherForecasting.Domain.Entities;

public class WeatherArchive
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string City { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double TemperatureC { get; set; }
    public double TemperatureF { get; set; }
    public string Description { get; set; } = null!;
}