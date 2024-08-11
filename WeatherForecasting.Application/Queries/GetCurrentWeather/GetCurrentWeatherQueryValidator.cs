using FluentValidation;
using WeatherForecasting.Contracts.Queries;

namespace WeatherForecasting.Application.Queries.GetCurrentWeather;

public class GetCurrentWeatherQueryValidator : AbstractValidator<GetCurrentWeatherQuery>
{
    public GetCurrentWeatherQueryValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty();
    }
}
