using MediatR;

namespace WeatherForecasting.Contracts.Commands;

public class DeleteWeatherArchiveCommand(Guid id) : IRequest
{
    public Guid Id { get; init; } = id;
}
