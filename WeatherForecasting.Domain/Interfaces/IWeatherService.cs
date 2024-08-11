using WeatherForecasting.Contracts.Queries;
using WeatherForecasting.Contracts.Responses;

namespace WeatherForecasting.Domain.Interfaces;

public interface IWeatherService
{
    Task<WeatherForecastResponse> GetCurrentWeather(CoordinatesResponse coordinates, CancellationToken cancellationToken);

    Task<IEnumerable<FiveDayWeatherForecastResponse>> GetFiveDayWeather(CoordinatesResponse coordinates, GetFiveDayWeatherQuery request, CancellationToken cancellationToken);
}
