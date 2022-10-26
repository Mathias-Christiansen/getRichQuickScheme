using Application.Pipelines;
using Application.Services;
using Contracts.Common;
using Contracts.Common.SpinDto;
using Contracts.Common.TileSetsDto;
using Contracts.Errors;
using Domain.GamblingMachines.VegetableFiesta;
using Domain.GamblingMachines.ThreeLeggedBandit;
using MediatR;
using OneOf;

namespace Application.Features.Gambling;

[Access(AccessLevels.LoggedIn)]
public record SpinVegetableFiestaQuery(decimal Amount) : IRequest<OneOf<VegetableFiestaSpinDto, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>;

public class SpinVegetableFiestaQueryHandler : IRequestHandler<SpinVegetableFiestaQuery,
    OneOf<VegetableFiestaSpinDto, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>
{
    private readonly IAuthService _authService;
    private readonly IAppDbContext _dbContext;
    private readonly IGamblingRepository _gamblingRepository;

    public SpinVegetableFiestaQueryHandler(IAuthService authService, IAppDbContext dbContext, IGamblingRepository gamblingRepository)
    {
        _authService = authService;
        _dbContext = dbContext;
        _gamblingRepository = gamblingRepository;
    }

    public async Task<OneOf<VegetableFiestaSpinDto, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>> Handle(SpinVegetableFiestaQuery request, CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUser(cancellationToken);
        if (user is null) return new UserNotFound();
        _dbContext.Users.Attach(user);

        var tempResult = await _gamblingRepository
            .Gamble<VegetableFiestaResult>(request.Amount, user, cancellationToken);

        if (tempResult.TryPickT0(out var result, out var errors))
        {
            return new VegetableFiestaSpinDto()
            {
                Multipliers = result.GetMultipliers().Select(x => 
                        new MultiplierAndOriginDto(x, Array.Empty<int>())) //TODO add path to vegetable fiesta
                    .ToArray(),
                NewBalance = user.Balance.GetUnits(),
                TotalWon = (decimal)(result.TotalMultiplier()*(double)request.Amount),
                Grid = result.InitialBoard
                    .Select(x => 
                        x.Select(y => (VegetableFiestaTileSetDto)y).ToArray())
                    .ToArray(),
                States = result
                    .Select(x => new VegetableFiestaStateDto(x.Grid
                        .Select(y => y
                            .Select(z => (VegetableFiestaTileSetDto)z)
                            .ToArray())
                        .ToArray(), x.Multipliers))
                    .ToList()
            };
        }

        return errors
            .Match<OneOf<VegetableFiestaSpinDto, UserNotFound, InsufficientFundsError,
                GamblingMachineNotFound>>(
                x => x, y => y);

    }
}