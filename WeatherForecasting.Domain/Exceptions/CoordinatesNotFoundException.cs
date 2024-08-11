namespace WeatherForecasting.Domain.Exceptions;

public class CoordinatesNotFoundException(string city) 
    : Exception($"Coordinates for specified city '{city}' not found.")
{
}
