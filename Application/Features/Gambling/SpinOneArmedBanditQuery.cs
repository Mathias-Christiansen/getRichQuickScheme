using Application.Pipelines;
using Application.Services;
using Contracts.Common;
using Contracts.Common.SpinDto;
using Contracts.Common.TileSetsDto;
using Contracts.Errors;
using Domain.Entities;
using Domain.GamblingMachines;
using Domain.GamblingMachines.OneArmedBandit;
using Domain.GamblingMachines.TileSets;
using Domain.ValueObj;
using MediatR;
using OneOf;

namespace Application.Features.Gambling;
[Access(AccessLevels.LoggedIn)]
public record SpinOneArmedBanditQuery(decimal Amount) : IRequest<OneOf<SpinResultsDto<OneArmedBanditTileSetDto>, 
    UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>;

public class SpinOneArmedBanditQueryHandler : IRequestHandler<SpinOneArmedBanditQuery,
    OneOf<SpinResultsDto<OneArmedBanditTileSetDto>, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>
{
    private readonly IAuthService _authService;
    private readonly IAppDbContext _dbContext;
    private readonly IGamblingRepository _gamblingRepository;

    public SpinOneArmedBanditQueryHandler(IAuthService authService, IAppDbContext dbContext, 
        IGamblingRepository gamblingRepository)
    {
        _authService = authService;
        _dbContext = dbContext;
        _gamblingRepository = gamblingRepository;
    }

    public async Task<OneOf<SpinResultsDto<OneArmedBanditTileSetDto>, UserNotFound, InsufficientFundsError, 
        GamblingMachineNotFound>> Handle(SpinOneArmedBanditQuery request, CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUser(cancellationToken);
        if (user is null) return new UserNotFound();
        _dbContext.Users.Attach(user);

        var tempResult = await _gamblingRepository
            .Gamble<OneArmedBanditResult>(request.Amount, user, cancellationToken);

        if (tempResult.TryPickT0(out var result, out var errors))
        {
            return new SpinResultsDto<OneArmedBanditTileSetDto>()
            {
                Multipliers = result.GetMultipliers().Select(x => 
                        new MultiplierAndOriginDto(x.Multiplier, x.Path))
                    .ToArray(),
                NewBalance = user.Balance.GetUnits(),
                TotalWon = (decimal)(result.TotalMultiplier()*(double)request.Amount),
                Grid = result
                    .Select(x => 
                        x.Select(y => (OneArmedBanditTileSetDto)y).ToArray())
                    .ToArray()
            };
        }

        return errors
            .Match<OneOf<SpinResultsDto<OneArmedBanditTileSetDto>, UserNotFound, InsufficientFundsError,
                GamblingMachineNotFound>>(
                x => x, y => y);

    }
}