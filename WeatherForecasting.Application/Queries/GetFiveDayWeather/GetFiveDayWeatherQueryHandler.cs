using MediatR;
using WeatherForecasting.Contracts.Queries;
using WeatherForecasting.Contracts.Responses;
using WeatherForecasting.Domain.Interfaces;

namespace WeatherForecasting.Application.Queries.GetFiveDayWeather;

public class GetFiveDayWeatherQueryHandler(ICoordinatesService coordinatesService, IWeatherService weatherService)
    : IRequestHandler<GetFiveDayWeatherQuery, IEnumerable<FiveDayWeatherForecastResponse>>
{
    public async Task<IEnumerable<FiveDayWeatherForecastResponse>> Handle(GetFiveDayWeatherQuery request, CancellationToken cancellationToken)
    {
        var coordinates = await coordinatesService.GetCoordinates(request.City);
        return await weatherService.GetFiveDayWeather(coordinates, request, cancellationToken);
    }
}
