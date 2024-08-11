namespace WeatherForecasting.Domain.Exceptions;

public class WeatherDataNotFoundException(double latitude, double longitude) 
    : Exception($"Weather data for specified coordinates '{latitude}', '{longitude}' not found.")
{
}
