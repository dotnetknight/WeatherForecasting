using MediatR;
using WeatherForecasting.Contracts.Models;
using WeatherForecasting.Contracts.Queries;
using WeatherForecasting.Contracts.Responses;
using WeatherForecasting.Domain.Entities;
using WeatherForecasting.Domain.Extensions;
using WeatherForecasting.Domain.Interfaces;

namespace WeatherForecasting.Application.Queries.GetWeatherArchives;

public class GetWeatherArchivesQueryHandler(IRepository<WeatherArchive> weatherRepository) : IRequestHandler<GetWeatherArchivesQuery, WeatherArchivePagedResponse>
{
    private readonly IRepository<WeatherArchive> _weatherRepository = weatherRepository;

    public async Task<WeatherArchivePagedResponse> Handle(GetWeatherArchivesQuery request, CancellationToken cancellationToken)
    {
        var paginationParameters = new BaseResourceParameters
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
        };

        var weatherArchivesRecords = await _weatherRepository.GetPage(paginationParameters, cancellationToken);
        var filteredRecords = ApplyFilters(weatherArchivesRecords, request);

        return new WeatherArchivePagedResponse
        {
            TotalCount = weatherArchivesRecords.TotalCount,
            TotalPages = weatherArchivesRecords.TotalPages,
            CurrentPage = weatherArchivesRecords.CurrentPage,
            PageSize = weatherArchivesRecords.PageSize,
            Archive = MapWeatherArchives(filteredRecords)
        };
    }

    private static IEnumerable<WeatherArchive> ApplyFilters(IEnumerable<WeatherArchive> weatherArchivesRecords, GetWeatherArchivesQuery request)
    {
        if (request.DateFrom.IsNotNull() && !request.DateFrom.IsDefaultValue())
        {
            weatherArchivesRecords = weatherArchivesRecords
                .Where(x => x.Date.Date >= request.DateFrom!.Value.Date)
                .ToList();
        }

        if (request.DateTo.IsNotNull() && !request.DateTo.IsDefaultValue())
        {
            weatherArchivesRecords = weatherArchivesRecords
                .Where(x => x.Date.Date < request.DateTo!.Value.Date)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            weatherArchivesRecords = weatherArchivesRecords
                .Where(x => x.City.Contains(request.City, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }

        return weatherArchivesRecords;
    }

    private static IEnumerable<WeatherArchiveResponse> MapWeatherArchives(IEnumerable<WeatherArchive> weatherArchivesRecords)
    {
        foreach (var weatherArchive in weatherArchivesRecords)
        {
            yield return new WeatherArchiveResponse(
                weatherArchive.Id,
                weatherArchive.City,
                weatherArchive.Date,
                weatherArchive.Description,
                weatherArchive.Latitude,
                weatherArchive.Longitude,
                weatherArchive.TemperatureC,
                weatherArchive.TemperatureF);
        }
    }
}
