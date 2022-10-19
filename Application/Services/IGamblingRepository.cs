using Contracts.Common;
using Contracts.Errors;
using Domain.Entities;
using Domain.GamblingMachines.Abstractions;
using OneOf;

namespace Application.Services;

public interface IGamblingRepository
{
    public Task<OneOf<TResult, InsufficientFundsError, GamblingMachineNotFound>>
        Gamble<TResult>(decimal amount, User user, CancellationToken cancellationToken) where TResult : IGamblingResult;
}