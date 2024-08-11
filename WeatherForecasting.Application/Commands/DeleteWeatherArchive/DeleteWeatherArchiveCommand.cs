using MediatR;
using Microsoft.Extensions.Logging;
using WeatherForecasting.Contracts.Commands;
using WeatherForecasting.Domain.Entities;
using WeatherForecasting.Domain.Exceptions;
using WeatherForecasting.Domain.Extensions;
using WeatherForecasting.Domain.Interfaces;

namespace WeatherForecasting.Application.Commands.DeleteWeatherArchive;

public class DeleteWeatherArchiveCommandHandler(
    ILogger<DeleteWeatherArchiveCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IRepository<WeatherArchive> weatherRepository) : IRequestHandler<DeleteWeatherArchiveCommand>
{
    private readonly ILogger<DeleteWeatherArchiveCommandHandler> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IRepository<WeatherArchive> _weatherRepository = weatherRepository;

    public async Task Handle(DeleteWeatherArchiveCommand request, CancellationToken cancellationToken)
    {
        var weatherArchive = await _weatherRepository
            .GetById(request.Id, cancellationToken);

        if (weatherArchive.IsNull())
        {
            _logger.LogError("Weather archive with id {request.Id} not found", request.Id);
            throw new WeatherArchiveNotFoundException(request.Id);
        }

        _weatherRepository.Delete(weatherArchive!);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
