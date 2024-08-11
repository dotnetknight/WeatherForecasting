using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WeatherForecasting.Application.Options;
using WeatherForecasting.Contracts.Models;
using WeatherForecasting.Contracts.Responses;
using WeatherForecasting.Domain.Exceptions;
using WeatherForecasting.Domain.Extensions;
using WeatherForecasting.Domain.Interfaces;

namespace WeatherForecasting.Integrations.GetCoordinates;

public class CoordinatesService(HttpClient httpClient, IOptions<OpenWeatherServiceOptions> options) : ICoordinatesService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly OpenWeatherServiceOptions _options = options.Value;

    public async Task<CoordinatesResponse> GetCoordinates(string city)
    {
        var apiKey = _options.ApiKey;
        var coordinatesResponse = await _httpClient.GetAsync($"/geo/1.0/direct?q={city}&limit=5&appid={apiKey}");
        var coordinatesData = await coordinatesResponse.Content.ReadAsStringAsync();
        var coordinatesList = JsonConvert.DeserializeObject<List<Coordinates>>(coordinatesData);

        var isCoordinatesCollectionEmpty = coordinatesList.IsNullOrEmpty();
        if (isCoordinatesCollectionEmpty)
        {
            throw new CoordinatesNotFoundException(city);
        }

        var latitude = coordinatesList![0].Latitude;
        var longitude = coordinatesList![0].Longitude;

        return new CoordinatesResponse(latitude, longitude);
    }
}
