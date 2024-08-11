using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherForecasting.Contracts.Commands;
using WeatherForecasting.Contracts.Queries;
using WeatherForecasting.Contracts.Responses;

namespace WeatherForecasting.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArchiveController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Create weather archive record
    /// </summary>
    /// <param name="command">Request with values</param>
    /// <returns>Created result for weather archive</returns>
    [HttpPost]
    public async Task<ActionResult<WeatherArchiveResponse>> Create([FromBody] CreateWeatherArchiveCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { result.Id, }, result);
    }

    /// <summary>
    /// Update weather archive record
    /// </summary>
    /// <param name="Id">Weather archive record identificator</param>
    /// <param name="command">Request with values</param>
    /// <returns>No content response</returns>
    [HttpPut("{Id}")]
    public async Task<IActionResult> Update([FromRoute] Guid Id, [FromBody] UpdateWeatherArchiveCommand command)
    {
        command.Id = Id;
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete weather archive record
    /// </summary>
    /// <param name="Id">Weather archive record identificator</param>
    /// <returns>No content response</returns>
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid Id)
    {
        await mediator.Send(new DeleteWeatherArchiveCommand(Id));
        return NoContent();
    }

    /// <summary>
    /// Get weather archive record by id
    /// </summary>
    /// <param name="Id">Weather archive record identificator</param>
    /// <returns>Weather archive record response</returns>
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid Id)
    {
        var query = new GetWeatherArchiveByIdQuery(Id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get weather archive records
    /// </summary>
    /// <returns>Weather archive records</returns>
    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] GetWeatherArchivesQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
}