using Application;
using Application.Services;
using Contracts.Common;
using Contracts.Errors;
using Domain.Entities;
using Domain.GamblingMachines.Abstractions;
using Domain.ValueObj;
using OneOf;

namespace Infrastructure.Services;

public class GamblingRepository : IGamblingRepository
{
    private readonly IAppDbContext _dbContext;
    private readonly IGamblingMachineService _gmService;

    public GamblingRepository(IAppDbContext dbContext, IGamblingMachineService gmService)
    {
        _dbContext = dbContext;
        _gmService = gmService;
    }
    

    public async Task<OneOf<TResult, InsufficientFundsError, GamblingMachineNotFound>> 
        Gamble<TResult>(decimal amount, User user, CancellationToken cancellationToken) where TResult : IGamblingResult
    {
        if (user.Balance.GetUnits() < amount) return new InsufficientFundsError(amount,user.Balance.GetUnits());
        
        var machineLookUp = _gmService.GetSlotMachine<TResult>();
        if (machineLookUp.IsT1) return new GamblingMachineNotFound();
        var machine = machineLookUp.AsT0;
            
        var result = machine.Spin();
        var totalMultiplier = (decimal)result.TotalMultiplier();
        
        var totalResult = amount * totalMultiplier;
        var resultMoney = Money.Create(totalResult - amount);
        
        var transactionResult = user.AddTransaction(Guid.NewGuid(), resultMoney, TransactionType.Spin);
        
        if(transactionResult.IsT1) return new InsufficientFundsError(amount,user.Balance.GetUnits());
        var transaction = transactionResult.AsT0;
        
        _dbContext.Set<Transaction>().Add(transaction);
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return result;
    }
}