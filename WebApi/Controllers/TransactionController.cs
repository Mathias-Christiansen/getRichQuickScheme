using Application.Features.Transactions;
using Application.Features.Users;
using Contracts.Transactions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Properties;

namespace WebApi.Controllers;


[ApiController]
[Route("[Controller]")]
public class TransactionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("")]
    [MapResponses(typeof(TransferFundsCommand))]
    public async Task<IActionResult> CreateMoneyTransfer(TransferRequest request)
    {
        var id = Guid.NewGuid();
        var command = new TransferFundsCommand(id, request.Amount);
        var response = await _mediator.Send(command);
        return response.MatchResponse();

    }
    
    [HttpGet("page/{page:int}")]
    [MapResponses(typeof(GetUserTransactionsPageQuery))]
    public async Task<IActionResult> GetUserTransactions(int page)
    {
        var query = new GetUserTransactionsPageQuery(page);
        var response = await _mediator.Send(query);
        return response.MatchResponse();
    }
}