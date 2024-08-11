namespace WeatherForecasting.Domain.Exceptions;

public class WeatherArchiveAlreadyExists(string city, DateTime date) 
    : Exception($"Weather archive for '{city}' with provided date {date} already exists.")
{
}