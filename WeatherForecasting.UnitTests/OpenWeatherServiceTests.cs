using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using WeatherForecasting.Application.Options;
using WeatherForecasting.Contracts.Responses;
using WeatherForecasting.Domain.Exceptions;
using WeatherForecasting.Integrations.OpenWeatherService;

namespace WeatherForecasting.UnitTests;

public class OpenWeatherServiceTests
{
    private const string testApiKey = "test_api_key";

    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IOptions<OpenWeatherServiceOptions>> _optionsMock;
    private readonly OpenWeatherServiceOptions _options;
    private readonly OpenWeatherService _service;
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly Mock<ILogger<OpenWeatherService>> _loggerMock;

    public OpenWeatherServiceTests()
    {
        _options = new OpenWeatherServiceOptions
        {
            ApiKey = "test_api_key",
            Unit = "metric",
            CacheTimeInMinutes = 60,
            BaseUrl = "https://api.openweathermap.org/"
        };

        _optionsMock = new Mock<IOptions<OpenWeatherServiceOptions>>();
        _optionsMock
            .Setup(x => x.Value)
            .Returns(_options);

        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.openweathermap.org/")
        };

        _memoryCacheMock = new Mock<IMemoryCache>();
        _loggerMock = new Mock<ILogger<OpenWeatherService>>();

        _service = new OpenWeatherService(_loggerMock.Object, _httpClient, _optionsMock.Object, _memoryCacheMock.Object);
    }

    [Fact]
    public async Task WhenCurrentWeatherIsRequestedForAValidCity_ShouldReturnWeatherForecast()
    {
        // Arrange
        var latitude = 52.52;
        var longitude = 13.405;
        var coordinates = new CoordinatesResponse(latitude, longitude);
        var cacheKey = $"{coordinates.Latitude}_{coordinates.Longitude}";

        var expectedResponseContent = @"{
                                        ""weather"": [{ ""description"": ""clear sky"" }],
                                        ""main"": { ""temp"": 20.0 },
                                        ""dt"": 1609459200,
                                        ""name"": ""Miami""
                                        }";

        var cachedWeatherData = new WeatherForecastResponse
        {
            WeatherDescription = "clear sky",
            TemperatureC = 20.0,
            TemperatureF = 68.0,
            Date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            City = "Miami",
        };

        object cacheEntry = cachedWeatherData;

        _memoryCacheMock
            .Setup(x => x.TryGetValue(cacheKey, out cacheEntry!))
            .Returns(true);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&units=metric&appid={testApiKey}")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedResponseContent),
            });

        // Act
        var result = await _service.GetCurrentWeather(coordinates, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.WeatherDescription.Should().Be("clear sky");
        result.TemperatureC.Should().Be(20.0);
        result.TemperatureF.Should().BeApproximately(68.0, 0.1);
        result.Date.Should().Be(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        result.City.Should().Be("Miami");
    }

    [Fact]
    public async Task WhenCurrentWeatherIsRequestedForInvalidCity_ShouldThrowWeatherDataNotFoundException()
    {
        // Arrange
        var coordinates = new CoordinatesResponse(0.0, 0.0);

        _httpMessageHandlerMock.SetupRequest(
            HttpMethod.Get,
            $"https://api.openweathermap.org/data/2.5/weather?lat={coordinates.Latitude}&lon={coordinates.Longitude}&units=metric&appid={testApiKey}",
            HttpStatusCode.NotFound,
            content: null
        );

        // Act
        var act = async ()
            => await _service.GetCurrentWeather(coordinates, CancellationToken.None);

        // Assert
        await act
            .Should()
            .ThrowAsync<WeatherDataNotFoundException>();
    }

    [Fact]
    public async Task WhenCurrentWeatherIsCached_ShouldReturnCachedWeatherData()
    {
        // Arrange
        var coordinates = new CoordinatesResponse(52.52, 13.405);
        var cacheKey = $"{coordinates.Latitude}_{coordinates.Longitude}";

        var cachedWeatherData = new WeatherForecastResponse
        {
            WeatherDescription = "clear sky",
            TemperatureC = 20.0,
            TemperatureF = 68.0,
            Date = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            City = "London"
        };

        object cacheEntry = cachedWeatherData;

        _memoryCacheMock
            .Setup(x => x.TryGetValue(cacheKey, out cacheEntry!))
            .Returns(true);

        // Act
        var result = await _service.GetCurrentWeather(coordinates, CancellationToken.None);

        // Assert
        result
            .Should()
            .BeEquivalentTo(cachedWeatherData);
    }
}