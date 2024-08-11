using Moq.Protected;

namespace WeatherForecasting.UnitTests;

public static class HttpMessageHandlerMockExtensions
{
    public static void SetupRequest(this Mock<HttpMessageHandler> mockHandler, HttpMethod method, string url, HttpStatusCode statusCode, string? content = null)
    {
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == method &&
                    req.RequestUri == new Uri(url)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = content != null ? new StringContent(content) : null
            });
    }
}