namespace WeatherForecasting.Contracts.Responses;

public class CoordinatesResponse
{
    public double Latitude { get; init; }

    public double Longitude { get; init; }

    public CoordinatesResponse(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}