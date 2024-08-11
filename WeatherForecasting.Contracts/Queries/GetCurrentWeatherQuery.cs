using MediatR;
using WeatherForecasting.Contracts.Responses;

namespace WeatherForecasting.Contracts.Queries;

public class GetCurrentWeatherQuery : IRequest<WeatherForecastResponse>
{
    public string City { get; set; } = null!;
}
