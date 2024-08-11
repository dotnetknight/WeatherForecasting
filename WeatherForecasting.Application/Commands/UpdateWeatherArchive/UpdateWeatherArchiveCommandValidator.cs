using FluentValidation;
using WeatherForecasting.Contracts.Commands;

namespace WeatherForecasting.Application.Commands.UpdateWeatherArchive;

public class UpdateWeatherArchiveCommandValidator : AbstractValidator<UpdateWeatherArchiveCommand>
{
    public UpdateWeatherArchiveCommandValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required.");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required.")
            .MaximumLength(100)
            .WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.Latitude)
            .NotEmpty()
            .WithMessage("Latitude is required.");

        RuleFor(x => x.Longitude)
            .NotEmpty()
            .WithMessage("Longitude is required.");

        RuleFor(x => x.TemperatureC)
            .NotEmpty()
            .WithMessage("Temperature in Celsius is required.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(255)
            .WithMessage("Description cannot exceed 255 characters.");
    }
}
