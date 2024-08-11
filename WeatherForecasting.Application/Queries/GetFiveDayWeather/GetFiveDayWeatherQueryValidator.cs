using FluentValidation;
using WeatherForecasting.Contracts.Queries;

namespace WeatherForecasting.Application.Queries.GetFiveDayWeather;

public class GetFiveDayWeatherQueryValidator : AbstractValidator<GetFiveDayWeatherQuery>
{
    public GetFiveDayWeatherQueryValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty();

        RuleFor(x => x.DateFrom)
            .Must(BeWithinFiveDaysFromToday).WithMessage("DateFrom must be within 5 days of the current date.");

        RuleFor(x => x.DateTo)
            .Must(BeWithinFiveDaysFromToday).WithMessage("DateTo must be within 5 days of the current date.");
    }

    private bool BeWithinFiveDaysFromToday(DateTime? date)
    {
        if (!date.HasValue)
        {
            return true;
        }

        var today = DateTime.Today;
        var maxDate = today.AddDays(4);
        return date >= today && date <= maxDate;
    }
}