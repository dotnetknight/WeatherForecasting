using MediatR;
using WeatherForecasting.Contracts.Responses;

namespace WeatherForecasting.Contracts.Queries;

public class GetFiveDayWeatherQuery : IRequest<IEnumerable<FiveDayWeatherForecastResponse>>
{
    public string City { get; set; } = null!;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
