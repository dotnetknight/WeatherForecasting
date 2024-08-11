using Newtonsoft.Json;

namespace WeatherForecasting.Contracts.Models;

public class WeatherData
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
}

public class Main
{
    [JsonProperty("temp")]
    public double Temperature { get; set; }

    [JsonProperty("feels_like")]
    public double FeelsLike { get; set; }
}

public class CurrentWeatherData
{
    public List<WeatherData> Weather { get; set; } = null!;
    public Main Main { get; set; } = null!;

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("dt")]
    public long Date { get; set; }
}
