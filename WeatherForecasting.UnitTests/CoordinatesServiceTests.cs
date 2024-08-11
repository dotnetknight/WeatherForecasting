using Moq.Protected;
using WeatherForecasting.Application.Options;
using WeatherForecasting.Domain.Exceptions;
using WeatherForecasting.Integrations.GetCoordinates;

namespace WeatherForecasting.UnitTests;

public class CoordinatesServiceTests
{
    private const string testApiKey = "test_api_key";

    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IOptions<OpenWeatherServiceOptions>> _optionsMock;
    private readonly OpenWeatherServiceOptions _options;
    private readonly CoordinatesService _service;

    public CoordinatesServiceTests()
    {
        _options = new OpenWeatherServiceOptions
        {
            ApiKey = testApiKey,
            BaseUrl = "https://api.openweathermap.org/"
        };

        _optionsMock = new Mock<IOptions<OpenWeatherServiceOptions>>();
        _optionsMock
            .Setup(x => x.Value)
            .Returns(_options);

        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(_options.BaseUrl)
        };

        _service = new CoordinatesService(_httpClient, _optionsMock.Object);
    }

    [Fact]
    public async Task WhenValidCityIsProvided_ShouldReturnCoordinates()
    {
        // Arrange
        var city = "Berlin";
        var expectedResponseContent = @"[
            { 
                ""lat"": 52.52, 
                ""lon"": 13.405,
                ""name"": ""Berlin""
            }
        ]";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=5&appid={testApiKey}")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedResponseContent),
            });

        // Act
        var result = await _service.GetCoordinates(city);

        // Assert
        result.Should().NotBeNull();
        result.Latitude.Should().Be(52.52);
        result.Longitude.Should().Be(13.405);
    }

    [Fact]
    public async Task WhenInvalidCityIsProvided_ShouldThrowCoordinatesNotFoundException()
    {
        // Arrange
        var city = "InvalidCity";
        var expectedResponseContent = "[]";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=5&appid={testApiKey}")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedResponseContent),
            });

        // Act
        var act = async () => await _service.GetCoordinates(city);

        // Assert
        await act
            .Should()
            .ThrowAsync<CoordinatesNotFoundException>();
    }

    [Fact]
    public async Task WhenHttpClientReturnsNonSuccessStatusCode_ShouldThrowHttpRequestException()
    {
        // Arrange
        var city = "Berlin";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=5&appid={testApiKey}")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        // Act
        var act = async () => await _service.GetCoordinates(city);

        // Assert
        await act
            .Should()
            .ThrowAsync<CoordinatesNotFoundException>();
    }
}
