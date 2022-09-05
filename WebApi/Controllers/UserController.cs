using Application.Features.Users;
using Contracts.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[Controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var id = Guid.NewGuid();
        var command = new CreateUserCommand(id, request.Name, request.Email, request.Password);
        var response = await _mediator.Send(command);
        return response.Match<IActionResult>(
            x => Accepted(), NotFound);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var query = new GetUserQuery(id);
        var response = await _mediator.Send(query);
        return response.Match<IActionResult>(
            Ok, NotFound);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> UserLogin(UserLoginRequest request)
    {
        var command = new UserLoginCommand(request.Email, request.Password);
        var response = await _mediator.Send(command);
        return response.Match<IActionResult>(
            Ok, NotFound, Unauthorized);
    }
}