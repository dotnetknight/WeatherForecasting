namespace WeatherForecasting.Domain.Exceptions;

public class CityNotFoundException(string city) : Exception($"City '{city}' not found.")
{
}
