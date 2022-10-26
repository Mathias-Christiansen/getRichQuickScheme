using Application.Features.Gambling;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Properties;

namespace WebApi.Controllers;


[ApiController]
[Route("[Controller]")]
public class GamblingController : ControllerBase
{
    private readonly IMediator _mediator;

    public GamblingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("OneArmedBandit/{amount:decimal}")]
    [MapResponses(typeof(SpinOneArmedBanditQuery))]
    public async Task<IActionResult> OneArmedBandit(decimal amount)
    {
        var query = new SpinOneArmedBanditQuery(amount);
        var response = await _mediator.Send(query);
        return response.MatchResponse();
    }
    
    
    [HttpGet("ThreeArmedBandit/{amount:decimal}")]
    [MapResponses(typeof(SpinThreeLeggedBanditQuery))]
    public async Task<IActionResult> ThreeArmedBandit(decimal amount)
    {
        var query = new SpinThreeLeggedBanditQuery(amount);
        var response = await _mediator.Send(query);
        return response.MatchResponse();
    }
    
    [HttpGet("PlanetoidBaroness/{amount:decimal}")]
    [MapResponses(typeof(SpinPlanetoidBaronessQuery))]
    public async Task<IActionResult> PlanetoidBaroness(decimal amount)
    {
        var query = new SpinPlanetoidBaronessQuery(amount);
        var response = await _mediator.Send(query);
        return response.MatchResponse();
    }
    
    [HttpGet("VegetableFiesta/{amount:decimal}")]
    [MapResponses(typeof(SpinPlanetoidBaronessQuery))]
    public async Task<IActionResult> VegetableFiesta(decimal amount)
    {
        var query = new SpinVegetableFiestaQuery(amount);
        var response = await _mediator.Send(query);
        return response.MatchResponse();
    }
}