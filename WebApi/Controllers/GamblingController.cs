using Application.Features.Gambling;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> OneArmedBandit(decimal amount)
    {
        var query = new SpinOneArmedBanditQuery(amount);
        var response = await _mediator.Send(query);
        return response.Match<IActionResult>(
            Ok, NotFound, BadRequest);
    }
}