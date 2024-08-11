namespace WeatherForecasting.Domain.Exceptions;

public class FiveDayWeatherNotFoundException(string city) 
    : Exception($"Five day weather data wasn't found for '{city}'")
{
}
