using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherForecasting.Contracts.Queries;
using WeatherForecasting.Contracts.Responses;

namespace WeatherForecasting.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get current weather by city
    /// </summary>
    /// <param name="city">City name</param>
    /// <returns>Current weather based on a city name</returns>
    [HttpGet]
    public async Task<ActionResult<WeatherForecastResponse>> GetCurrentWeather([FromQuery] string city)
    {
        var query = new GetCurrentWeatherQuery { City = city };

        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get weather for the next five days
    /// </summary>
    /// <param name="query">Query with parameters</param>
    /// <returns>Five day weather forecast based on a city name</returns>
    [HttpGet("forecast")]
    public async Task<ActionResult<WeatherForecastResponse>> GetFiveDayWeather([FromQuery] GetFiveDayWeatherQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
}