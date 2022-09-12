﻿using Application.Pipelines;
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
public record SpinOneArmedBanditQuery(decimal Amount) : IRequest<OneOf<SpinResultsDto<SlotCardTilesDto>, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>;

public class SpinOneArmedBanditQueryHandler : IRequestHandler<SpinOneArmedBanditQuery,
    OneOf<SpinResultsDto<SlotCardTilesDto>, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>
{
    private readonly IAuthService _authService;
    private readonly IAppDbContext _dbContext;
    private readonly IGamblingRepository _gamblingRepository;

    public SpinOneArmedBanditQueryHandler(IAuthService authService, IAppDbContext dbContext, IGamblingRepository gamblingRepository)
    {
        _authService = authService;
        _dbContext = dbContext;
        _gamblingRepository = gamblingRepository;
    }

    public async Task<OneOf<SpinResultsDto<SlotCardTilesDto>, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>> Handle(SpinOneArmedBanditQuery request, CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUser(cancellationToken);
        if (user is null) return new UserNotFound();
        _dbContext.Users.Attach(user);

        var result = await _gamblingRepository
            .Gamble<SlotCardTiles, SlotCardTilesDto>(request.Amount, user, cancellationToken);
        return result
            .Match<OneOf<SpinResultsDto<SlotCardTilesDto>, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>(
                x => x, 
                x => x, 
                x=> x
            );
    }
}