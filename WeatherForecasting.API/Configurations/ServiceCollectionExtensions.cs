using Microsoft.Extensions.Options;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using WeatherForecasting.Application.Options;
using WeatherForecasting.Application.Queries.GetFiveDayWeather;
using WeatherForecasting.Application.Queries.GetWeatherForecast;
using WeatherForecasting.Domain.Interfaces;
using FluentValidation.AspNetCore;
using WeatherForecasting.Infrastructure;
using Microsoft.EntityFrameworkCore;
using WeatherForecasting.Integrations.OpenWeatherService;
using WeatherForecasting.Integrations.GetCoordinates;

namespace WeatherForecasting.API.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        services.AddControllers(optiuons =>
        {
            optiuons.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.AllowTrailingCommas = true;
                options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
            })
            .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<GetFiveDayWeatherQueryValidator>()); ;
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning restore CS0618 // Type or member is obsolete

        services.AddEndpointsApiExplorer();

        services.AddRouting(x =>
        {
            x.LowercaseUrls = true;
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IWeatherService, OpenWeatherService>();
        services.AddScoped<ICoordinatesService, CoordinatesService>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(GetCurrentWeatherQueryHandler).Assembly));
        services.AddMemoryCache();

        var openWeatherServiceOptions = new OpenWeatherServiceOptions();
        services.Configure<OpenWeatherServiceOptions>(options =>
        {
            configuration.GetSection(nameof(OpenWeatherServiceOptions)).Bind(options);
        });

        services.AddSingleton(openWeatherServiceOptions);
        services.AddHttpClient<IWeatherService, OpenWeatherService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<OpenWeatherServiceOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddHttpClient<ICoordinatesService, CoordinatesService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<OpenWeatherServiceOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddDbContext<WeatherDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}