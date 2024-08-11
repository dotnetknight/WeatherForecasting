using Newtonsoft.Json;

namespace WeatherForecasting.Contracts.Models;

public class FiveDayForecast
{
    [JsonProperty("dt")]
    public long Date { get; set; }
    public Main Main { get; set; } = null!;
    public List<WeatherData> Weather { get; set; } = [];
}

public class City
{
    public string Name { get; set; } = null!;
}

public class FiveDayWeatherForecast
{
    [JsonProperty("List")]
    public List<FiveDayForecast> Data { get; set; } = [];
    public City City { get; set; } = null!;
}
