using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WeatherForecasting.Application.Options;
using WeatherForecasting.Contracts.Models;
using WeatherForecasting.Contracts.Queries;
using WeatherForecasting.Contracts.Responses;
using WeatherForecasting.Domain.Exceptions;
using WeatherForecasting.Domain.Extensions;
using WeatherForecasting.Domain.Interfaces;

namespace WeatherForecasting.Integrations.OpenWeatherService;

public class OpenWeatherService(
    ILogger<OpenWeatherService> logger,
    HttpClient httpClient,
    IOptions<OpenWeatherServiceOptions> options,
    IMemoryCache memoryCache) : IWeatherService
{
    private readonly ILogger<OpenWeatherService> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly OpenWeatherServiceOptions _options = options.Value;

    public async Task<WeatherForecastResponse> GetCurrentWeather(CoordinatesResponse coordinates, CancellationToken cancellationToken)
    {
        var weatherData = await GetCachedData(coordinates, cancellationToken);
        if (weatherData.IsNull())
        {
            weatherData = await GetWeather(coordinates, cancellationToken);
        }

        return weatherData!;
    }

    public async Task<IEnumerable<FiveDayWeatherForecastResponse>> GetFiveDayWeather(CoordinatesResponse coordinates, GetFiveDayWeatherQuery request, CancellationToken cancellationToken)
    {
        var apiKey = _options.ApiKey;
        var unit = _options.Unit;

        var fiveDayForecastResponse = await _httpClient.GetAsync($"data/2.5/forecast?lat={coordinates.Latitude}&lon={coordinates.Longitude}&units={unit}&appid={apiKey}", cancellationToken);
        var fiveDayForecastContent = await fiveDayForecastResponse.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("Received response from external weather API: {fiveDayForecastContent}", fiveDayForecastContent);

        var fiveDayForecast = JsonConvert.DeserializeObject<FiveDayWeatherForecast>(fiveDayForecastContent);
        if (fiveDayForecast.IsNull() && fiveDayForecast!.Data.IsNullOrEmpty())
        {
            throw new FiveDayWeatherNotFoundException(request.City);
        }

        if (request.DateFrom.IsNotNull() && !request.DateFrom.IsDefaultValue())
        {
            fiveDayForecast!.Data = fiveDayForecast.Data
                .Where(x => x.Date.ToDateTimeUtcFromUnixTimestamp().Date >= request.DateFrom!.Value.Date)
                .ToList();
        }

        if (request.DateTo.IsNotNull() && !request.DateTo.IsDefaultValue())
        {
            fiveDayForecast!.Data = fiveDayForecast.Data
                .Where(x => x.Date.ToDateTimeUtcFromUnixTimestamp().Date < request.DateTo!.Value.Date)
                .ToList();
        }

        return GenerateFiveDayWeatherForecastResponse(fiveDayForecast!.Data);
    }

    private async Task<WeatherForecastResponse?> GetCachedData(CoordinatesResponse coordinates, CancellationToken cancellationToken)
    {
        var cacheTimeInMinutes = _options.CacheTimeInMinutes;
        var cacheKey = $"{coordinates.Latitude}_{coordinates.Longitude}";

        var isWeatherCached = _memoryCache.TryGetValue(cacheKey, out WeatherForecastResponse? weatherData);
        if (!isWeatherCached)
        {
            weatherData = await GetWeather(coordinates, cancellationToken);
            _memoryCache.Set(cacheKey, weatherData, TimeSpan.FromMinutes(cacheTimeInMinutes));
        }

        _logger.LogInformation("Returning cached value");
        return weatherData;
    }

    private async Task<WeatherForecastResponse> GetWeather(CoordinatesResponse coordinates, CancellationToken cancellationToken)
    {
        var apiKey = _options.ApiKey;
        var unit = _options.Unit;

        var currentWeatherResponse = await _httpClient.GetAsync($"data/2.5/weather?lat={coordinates.Latitude}&lon={coordinates.Longitude}&units={unit}&appid={apiKey}", cancellationToken);
        var currentWeatherContent = await currentWeatherResponse.Content.ReadAsStringAsync(cancellationToken);

        var weatherData = JsonConvert.DeserializeObject<CurrentWeatherData>(currentWeatherContent);
        if (weatherData.IsNull())
        {
            throw new WeatherDataNotFoundException(coordinates.Latitude, coordinates.Longitude);
        }

        return new WeatherForecastResponse()
        {
            WeatherDescription = weatherData!.Weather.First().Description,
            TemperatureC = weatherData.Main.Temperature,
            TemperatureF = weatherData.Main.Temperature.ToFahrenheit(),
            Date = weatherData.Date.ToDateTimeUtcFromUnixTimestamp(),
            City = weatherData.Name
        };
    }

    private static IEnumerable<FiveDayWeatherForecastResponse> GenerateFiveDayWeatherForecastResponse(IEnumerable<FiveDayForecast> weatherList)
    {
        foreach (var weather in weatherList)
        {
            var response = new FiveDayWeatherForecastResponse()
            {
                WeatherDescription = weather.Weather.First().Description,
                TemperatureC = weather.Main.Temperature,
                TemperatureF = weather.Main.Temperature.ToFahrenheit(),
                Date = weather.Date.ToDateTimeUtcFromUnixTimestamp(),
            };

            yield return response;
        }
    }
}