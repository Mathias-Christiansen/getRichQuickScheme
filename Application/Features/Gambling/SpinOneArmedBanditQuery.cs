using Application.Pipelines;
using Application.Services;
using Contracts.Common;
using Contracts.Errors;
using Domain.Entities;
using Domain.GamblingMachines;
using Domain.ValueObj;
using MediatR;
using OneOf;

namespace Application.Features.Gambling;
[Access(AccessLevels.LoggedIn)]
public record SpinOneArmedBanditQuery(decimal Amount) : IRequest<OneOf<SpinResultsDto<SlotCardTilesDto>, UserNotFound, InsufficientFundsError>>;

public class SpinOneArmedBanditQueryHandler : IRequestHandler<SpinOneArmedBanditQuery,
    OneOf<SpinResultsDto<SlotCardTilesDto>, UserNotFound, InsufficientFundsError>>
{
    private readonly IAuthService _authService;
    private readonly IAppDbContext _dbContext;

    public SpinOneArmedBanditQueryHandler(IAuthService authService, IAppDbContext dbContext)
    {
        _authService = authService;
        _dbContext = dbContext;
    }

    public async Task<OneOf<SpinResultsDto<SlotCardTilesDto>, UserNotFound, InsufficientFundsError>> Handle(SpinOneArmedBanditQuery request, CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUser(cancellationToken);
        if (user is null) return new UserNotFound();
        _dbContext.Users.Attach(user);
        
        if (user.Balance.GetUnits() < request.Amount) return new InsufficientFundsError(request.Amount,user.Balance.GetUnits());
        
        var machine = new OneArmedBanditSlotMachine();
        var result = machine.Spin();
        var totalMultiplier = (decimal)result.SumMultiplier();
        
        var totalResult = request.Amount * totalMultiplier;
        var resultMoney = Money.Create(totalResult - request.Amount);
        
        var transactionResult = user.AddTransaction(Guid.NewGuid(), resultMoney, TransactionType.Spin);
        
        if(transactionResult.IsT1) return new InsufficientFundsError(request.Amount,user.Balance.GetUnits());
        var transaction = transactionResult.AsT0;
        
        _dbContext.Set<Transaction>().Add(transaction);
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SpinResultsDto<SlotCardTilesDto>()
        {
            Multipliers = result.GetMultipliers().Select(x => new MultiplierAndOriginDto(x.Multiplier, x.Path))
                .ToArray(),
            NewBalance = user.Balance.GetUnits(),
            TotalWon = totalResult,
            Grid = result.Select(x => x.Select(y => (SlotCardTilesDto)y).ToArray()).ToArray()
        };
    }
}