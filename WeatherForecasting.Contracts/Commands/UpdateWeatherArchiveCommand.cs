using MediatR;

namespace WeatherForecasting.Contracts.Commands;

public class UpdateWeatherArchiveCommand : IRequest
{
    public Guid Id { get; set; }
    public string City { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Date { get; set; }
    public double TemperatureC { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
