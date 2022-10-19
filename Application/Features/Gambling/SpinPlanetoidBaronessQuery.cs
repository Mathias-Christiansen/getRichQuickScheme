using Application.Pipelines;
using Application.Services;
using Contracts.Common;
using Contracts.Common.TileSetsDto;
using Contracts.Errors;
using Domain.GamblingMachines.PlanetoidBaroness;
using Domain.GamblingMachines.ThreeLeggedBandit;
using MediatR;
using OneOf;

namespace Application.Features.Gambling;

[Access(AccessLevels.LoggedIn)]
public record SpinPlanetoidBaronessQuery(decimal Amount) : IRequest<OneOf<PlanetoidBaronessSpinDto, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>;

public class SpinPlanetoidBaronessQueryHandler : IRequestHandler<SpinPlanetoidBaronessQuery,
    OneOf<PlanetoidBaronessSpinDto, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>
{
    private readonly IAuthService _authService;
    private readonly IAppDbContext _dbContext;
    private readonly IGamblingRepository _gamblingRepository;

    public SpinPlanetoidBaronessQueryHandler(IAuthService authService, IAppDbContext dbContext, IGamblingRepository gamblingRepository)
    {
        _authService = authService;
        _dbContext = dbContext;
        _gamblingRepository = gamblingRepository;
    }

    public async Task<OneOf<PlanetoidBaronessSpinDto, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>> Handle(SpinPlanetoidBaronessQuery request, CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUser(cancellationToken);
        if (user is null) return new UserNotFound();
        _dbContext.Users.Attach(user);

        var tempResult = await _gamblingRepository
            .Gamble<PlanetoidBaronessResult>(request.Amount, user, cancellationToken);

        if (tempResult.TryPickT0(out var result, out var errors))
        {
            return new PlanetoidBaronessSpinDto()
            {
                Multipliers = result.GetMultipliers().Select(x => 
                        new MultiplierAndOriginDto(x, Array.Empty<int>())) //TODO add path to planetoid baroness
                    .ToArray(),
                NewBalance = user.Balance.GetUnits(),
                TotalWon = (decimal)(result.TotalMultiplier()*(double)request.Amount),
                Grid = result.InitialBoard
                    .Select(x => 
                        x.Select(y => (PlanetoidBaronessTileSetDto)y).ToArray())
                    .ToArray(),
                States = result
                    .Select(x => new PlanetoidBaronessStateDto(x.Grid
                        .Select(y => y
                            .Select(z => (PlanetoidBaronessTileSetDto)z)
                            .ToArray())
                        .ToArray(), x.Multipliers))
                    .ToList()
            };
        }

        return errors
            .Match<OneOf<PlanetoidBaronessSpinDto, UserNotFound, InsufficientFundsError,
                GamblingMachineNotFound>>(
                x => x, y => y);

    }
}