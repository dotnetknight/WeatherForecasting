using MediatR;
using Microsoft.Extensions.Logging;
using WeatherForecasting.Contracts.Commands;
using WeatherForecasting.Contracts.Responses;
using WeatherForecasting.Domain.Entities;
using WeatherForecasting.Domain.Exceptions;
using WeatherForecasting.Domain.Extensions;
using WeatherForecasting.Domain.Interfaces;

namespace WeatherForecasting.Application.Commands.CreateWeatherArchive;

public class CreateWeatherArchiveCommandHandler(
    ILogger<CreateWeatherArchiveCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IRepository<WeatherArchive> weatherRepository) : IRequestHandler<CreateWeatherArchiveCommand, WeatherArchiveResponse>
{
    private readonly ILogger<CreateWeatherArchiveCommandHandler> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IRepository<WeatherArchive> _weatherRepository = weatherRepository;

    public async Task<WeatherArchiveResponse> Handle(CreateWeatherArchiveCommand request, CancellationToken cancellationToken)
    {
        var weatherArchives = await _weatherRepository.GetFirst(x => x.City == request.City &&
                      x.Longitude == request.Longitude &&
                      x.Latitude == request.Latitude &&
                      x.Date == request.Date, cancellationToken: cancellationToken);

        var isWeatherArchived = weatherArchives.IsNotNull();
        if (isWeatherArchived)
        {
            _logger.LogError("Weather archive for {request.City} and provided date {request.Date} already exists", request.City, request.Date);
            throw new WeatherArchiveAlreadyExists(request.City, request.Date);
        }

        var weatherArchive = new WeatherArchive()
        {
            Id = new Guid(),
            City = request.City,
            Date = request.Date,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            TemperatureC = request.TemperatureC,
            TemperatureF = request.TemperatureC.ToFahrenheit(),
        };

        await _weatherRepository.Add(weatherArchive, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
