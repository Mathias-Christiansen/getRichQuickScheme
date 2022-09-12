using Contracts.Common;
using Contracts.Errors;
using Domain.Entities;
using OneOf;

namespace Application.Services;

public interface IGamblingRepository
{
    public Task<OneOf<SpinResultsDto<TTileDto>, InsufficientFundsError, GamblingMachineNotFound>>
        Gamble<TTileSet, TTileDto>(decimal amount, User user, CancellationToken cancellationToken)
        where TTileSet : struct, Enum
        where TTileDto : struct, Enum;
}