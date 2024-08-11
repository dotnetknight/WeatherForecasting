using Newtonsoft.Json;

namespace WeatherForecasting.Contracts.Models;

public class Coordinates
{
    [JsonProperty("lat")]
    public double Latitude { get; set; }

    [JsonProperty("lon")]
    public double Longitude { get; set; }
}