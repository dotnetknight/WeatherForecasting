namespace WeatherForecasting.Domain.Extensions;

public static class GenericExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? obj) =>
        obj == null || !obj.Any();

    public static bool IsNull<T>(this T? obj) =>
        obj == null;

    public static bool IsNotNull<T>(this T? obj) =>
      obj != null;

    public static DateTime ToDateTimeUtcFromUnixTimestamp(this long unixTimestamp)
    {
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
        return dateTimeOffset.UtcDateTime;
    }

    public static bool IsDefaultValue<T>(this T value)
    {
        if (value is null)
        {
            return true;
        }

        var type = value.GetType();

        if (type.IsValueType)
        {
            return value.Equals(Activator.CreateInstance(value.GetType()));
        }

        return false;
    }

    public static double ToFahrenheit(this double celsiusTemperature)
    {
        return 32 + (celsiusTemperature / 0.5556);
    }
}
