using MediatR;
using WeatherForecasting.Contracts.Responses;

namespace WeatherForecasting.Contracts.Queries;

public class GetWeatherArchiveByIdQuery(Guid id) : IRequest<WeatherArchiveResponse>
{
    public Guid Id { get; set; } = id;
}
