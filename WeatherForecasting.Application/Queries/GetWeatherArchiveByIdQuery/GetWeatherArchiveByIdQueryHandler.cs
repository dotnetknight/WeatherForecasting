using MediatR;
using Microsoft.Extensions.Logging;
using WeatherForecasting.Contracts.Queries;
using WeatherForecasting.Contracts.Responses;
using WeatherForecasting.Domain.Entities;
using WeatherForecasting.Domain.Exceptions;
using WeatherForecasting.Domain.Extensions;
using WeatherForecasting.Domain.Interfaces;

namespace WeatherForecasting.Application.Queries.GeteWeatherArchiveByIdQuery;

public class GetWeatherArchiveByIdQueryHandler(
    ILogger<GetWeatherArchiveByIdQueryHandler> logger,
    IRepository<WeatherArchive> weatherRepository) : IRequestHandler<GetWeatherArchiveByIdQuery, WeatherArchiveResponse>
{
    private readonly ILogger<GetWeatherArchiveByIdQueryHandler> _logger = logger;
    private readonly IRepository<WeatherArchive> _weatherRepository = weatherRepository;

    public async Task<WeatherArchiveResponse> Handle(GetWeatherArchiveByIdQuery request, CancellationToken cancellationToken)
    {
        var weatherArchive = await _weatherRepository
            .GetById(request.Id, cancellationToken);

        if (weatherArchive.IsNull())
        {
            _logger.LogError("Weather archive with id {request.Id} not found", request.Id);
            throw new WeatherArchiveNotFoundException(request.Id);
        }

        return new WeatherArchiveResponse
            (weatherArchive!.Id,
            weatherArchive.City, weatherArchive.Date,
            weatherArchive.Description, 
            weatherArchive.Latitude, 
            weatherArchive.Longitude, 
            weatherArchive.TemperatureC, 
            weatherArchive.TemperatureF);
    }
}
