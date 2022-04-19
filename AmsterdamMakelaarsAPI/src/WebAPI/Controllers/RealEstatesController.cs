using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/real-estates")]
public class RealEstatesController : ControllerBase
{
    private readonly Serilog.ILogger _logger;
    private readonly IMediator _mediator;

    public RealEstatesController(Serilog.ILogger logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    [HttpGet("tuins/list")]
    [ResponseCache(Duration = 120)] //2 min cache
    public async Task<IActionResult> GetTuinList(CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetAllTuinQuery();
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.Error("An exception occured: {Exception}", e.Message);
            return BadRequest();
        }
    }
    
    [HttpGet("list")]
    [ResponseCache(Duration = 600)] //10 min cache
    public async Task<IActionResult> GetAllList(CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetAllQuery();
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.Error("An exception occured: {Exception}", e.Message);
            return BadRequest();
        }
    }
}