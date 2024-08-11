using MediatR;
using Microsoft.Extensions.Logging;
using WeatherForecasting.Contracts.Commands;
using WeatherForecasting.Domain.Entities;
using WeatherForecasting.Domain.Exceptions;
using WeatherForecasting.Domain.Extensions;
using WeatherForecasting.Domain.Interfaces;

namespace WeatherForecasting.Application.Commands.UpdateWeatherArchive;

public class UpdateWeatherArchiveCommandHandler(
    ILogger<UpdateWeatherArchiveCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IRepository<WeatherArchive> weatherRepository) : IRequestHandler<UpdateWeatherArchiveCommand>
{
    private readonly ILogger<UpdateWeatherArchiveCommandHandler> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IRepository<WeatherArchive> _weatherRepository = weatherRepository;

    public async Task Handle(UpdateWeatherArchiveCommand request, CancellationToken cancellationToken)
    {
        var weatherArchive = await _weatherRepository
            .GetById(request.Id, cancellationToken);

        if (weatherArchive.IsNull())
        {
            _logger.LogError("Weather archive with id {request.Id} not found", request.Id);
            throw new WeatherArchiveNotFoundException(request.Id);
        }

        weatherArchive!.City = request.City;
        weatherArchive.Date = request.Date;
        weatherArchive.Description = request.Description;
        weatherArchive.Latitude = request.Latitude;
        weatherArchive.Longitude = request.Longitude;
        weatherArchive.TemperatureC = request.TemperatureC;
        weatherArchive.TemperatureF = request.TemperatureC.ToFahrenheit();

        _weatherRepository.Update(weatherArchive);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
