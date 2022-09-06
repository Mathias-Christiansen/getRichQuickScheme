using Application.Pipelines;
using Application.Services;
using Contracts.Errors;
using Domain.Entities;
using Domain.ValueObj;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Features.Transactions;


[Access(AccessLevels.LoggedIn)]
public record TransferFundsCommand(Guid Id, decimal Amount): IRequest<OneOf<Unit, UserNotFound, NegativeAmountTransferError>>;

public class TransferFundsCommandHandler : IRequestHandler<TransferFundsCommand,
    OneOf<Unit, UserNotFound, NegativeAmountTransferError>>
{
    private readonly IAuthService _authService;
    private readonly IAppDbContext _dbContext;

    public TransferFundsCommandHandler(IAuthService authService, IAppDbContext dbContext)
    {
        _authService = authService;
        _dbContext = dbContext;
    }
    public async Task<OneOf<Unit, UserNotFound, NegativeAmountTransferError>> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0) return new NegativeAmountTransferError(request.Amount);
        
        var user = await _authService.GetCurrentUser(cancellationToken);
        if (user is null) return new UserNotFound();
        _dbContext.Users.Attach(user);

        var response = user.AddTransaction(request.Id, Money.Create(request.Amount), TransactionType.Transferal);
        if (response.Value is Error) return new NegativeAmountTransferError(request.Amount);

        _dbContext.Set<Transaction>().Add(response.AsT0);
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}