namespace WeatherForecasting.Domain.Exceptions;

public class WeatherArchiveNotFoundException(Guid id) : Exception($"Weather archive with provided id {id} not found.")
{
}
