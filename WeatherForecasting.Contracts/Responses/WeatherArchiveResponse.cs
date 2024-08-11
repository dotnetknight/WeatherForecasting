namespace WeatherForecasting.Contracts.Responses;

public class WeatherArchivePagedResponse: BasePagedResponse
{
    public IEnumerable<WeatherArchiveResponse> Archive { get; set; }
}

public record WeatherArchiveResponse(
    Guid Id,
    string City,
    DateTime Date,
    string Description,
    double Latitude,
    double Longitude,
    double TemperatureC,
    double TemperatureF);