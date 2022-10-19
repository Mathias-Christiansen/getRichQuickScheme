using Application.Pipelines;
using Application.Services;
using Contracts.Common;
using Contracts.Common.TileSetsDto;
using Contracts.Errors;
using Domain.GamblingMachines;
using Domain.GamblingMachines.ThreeLeggedBandit;
using Domain.GamblingMachines.TileSets;
using MediatR;
using OneOf;

namespace Application.Features.Gambling;

[Access(AccessLevels.LoggedIn)]
public record SpinThreeLeggedBanditQuery(decimal Amount) : IRequest<OneOf<SpinResultsDto<ThreeLeggedBanditTileSetDto>, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>;

public class SpinThreeLeggedBanditQueryHandler : IRequestHandler<SpinThreeLeggedBanditQuery,
    OneOf<SpinResultsDto<ThreeLeggedBanditTileSetDto>, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>
{
    private readonly IAuthService _authService;
    private readonly IAppDbContext _dbContext;
    private readonly IGamblingRepository _gamblingRepository;

    public SpinThreeLeggedBanditQueryHandler(IAuthService authService, IAppDbContext dbContext, IGamblingRepository gamblingRepository)
    {
        _authService = authService;
        _dbContext = dbContext;
        _gamblingRepository = gamblingRepository;
    }

    public async Task<OneOf<SpinResultsDto<ThreeLeggedBanditTileSetDto>, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>> Handle(SpinThreeLeggedBanditQuery request, CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUser(cancellationToken);
        if (user is null) return new UserNotFound();
        _dbContext.Users.Attach(user);

        var tempResult = await _gamblingRepository
            .Gamble<ThreeLeggedBanditResult>(request.Amount, user, cancellationToken);

        if (tempResult.TryPickT0(out var result, out var errors))
        {
            return new SpinResultsDto<ThreeLeggedBanditTileSetDto>()
            {
                Multipliers = result.GetMultipliers().Select(x => 
                        new MultiplierAndOriginDto(x.Multiplier, x.Path))
                    .ToArray(),
                NewBalance = user.Balance.GetUnits(),
                TotalWon = (decimal)(result.TotalMultiplier()*(double)request.Amount),
                Grid = result
                    .Select(x => 
                        x.Select(y => (ThreeLeggedBanditTileSetDto)y).ToArray())
                    .ToArray()
            };
        }

        return errors
            .Match<OneOf<SpinResultsDto<ThreeLeggedBanditTileSetDto>, UserNotFound, InsufficientFundsError,
                GamblingMachineNotFound>>(
                x => x, y => y);

    }
}