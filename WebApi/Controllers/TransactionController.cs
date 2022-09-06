using Application.Features.Transactions;
using Contracts.Transactions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> CreateMoneyTransfer(TransferRequest request)
    {
        var id = Guid.NewGuid();
        var command = new TransferFundsCommand(id, request.Amount);
        var response = await _mediator.Send(command);
        return response.Match<IActionResult>(
            x => Accepted(), NotFound, BadRequest);
    }
    
    [HttpGet("page/{page:int}")]
    public async Task<IActionResult> GetUserTransactions(int page)
    {
        var query = new GetUserTransactionsPageQuery(page);
        var response = await _mediator.Send(query);
        return response.Match<IActionResult>(
            Ok, NotFound);
    }
}