using WeatherForecasting.Contracts.Responses;

namespace WeatherForecasting.Domain.Interfaces;

public interface ICoordinatesService
{
    Task<CoordinatesResponse> GetCoordinates(string city);
}
