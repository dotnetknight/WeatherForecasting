using MediatR;
using WeatherForecasting.Contracts.Queries;
using WeatherForecasting.Contracts.Responses;
using WeatherForecasting.Domain.Interfaces;

namespace WeatherForecasting.Application.Queries.GetWeatherForecast;

public class GetCurrentWeatherQueryHandler(ICoordinatesService coordinatesService, IWeatherService weatherService)
    : IRequestHandler<GetCurrentWeatherQuery, WeatherForecastResponse>
{
    public async Task<WeatherForecastResponse> Handle(GetCurrentWeatherQuery request, CancellationToken cancellationToken)
    {
        var coordinates = await coordinatesService.GetCoordinates(request.City);
        return await weatherService.GetCurrentWeather(coordinates, cancellationToken);
    }
}
