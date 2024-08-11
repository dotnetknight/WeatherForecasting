using MediatR;
using WeatherForecasting.Contracts.Models;
using WeatherForecasting.Contracts.Responses;

namespace WeatherForecasting.Contracts.Queries;

public class GetWeatherArchivesQuery : BaseResourceParameters, IRequest<WeatherArchivePagedResponse>
{
    public string? City { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}