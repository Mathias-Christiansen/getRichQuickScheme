using Application.Pipelines;
using Application.Services;
using Contracts.Common.SpinDto;
using Contracts.Common.TileSetsDto;
using Contracts.Errors;
using Domain.GamblingMachines.ArrayDisarray;
using MediatR;
using OneOf;

namespace Application.Features.Gambling;

[Access(AccessLevels.LoggedIn)]
public record SpinArrayDisarrayQuery(decimal Amount) : IRequest<OneOf<ArrayDisarraySpinDto, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>;

public class SpinArrayDisarrayQueryHandler : IRequestHandler<SpinArrayDisarrayQuery,
    OneOf<ArrayDisarraySpinDto, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>
{
    private readonly IAuthService _authService;
    private readonly IAppDbContext _dbContext;
    private readonly IGamblingRepository _gamblingRepository;

    public SpinArrayDisarrayQueryHandler(IAuthService authService, IAppDbContext dbContext, IGamblingRepository gamblingRepository)
    {
        _authService = authService;
        _dbContext = dbContext;
        _gamblingRepository = gamblingRepository;
    }

    public async Task<OneOf<ArrayDisarraySpinDto, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>> Handle(SpinArrayDisarrayQuery request, CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUser(cancellationToken);
        if (user is null) return new UserNotFound();
        _dbContext.Users.Attach(user);

        var tempResult = await _gamblingRepository
            .Gamble<ArrayDisarrayResult>(request.Amount, user, cancellationToken);

        if (tempResult.TryPickT0(out var result, out var errors))
        {
            return new ArrayDisarraySpinDto()
            {
                Multipliers = result.GetMultipliers().Select(x => 
                        new MultiplierAndOriginDto(x, Array.Empty<int>())) //TODO add path to vegetable fiesta
                    .ToArray(),
                NewBalance = user.Balance.GetUnits(),
                TotalWon = (decimal)(result.TotalMultiplier()*(double)request.Amount),
                Grid = result.InitialBoard
                    .Select(x => 
                        x.Select(y => (ArrayDisarrayTileSetDto)y).ToArray())
                    .ToArray(),
                States = result
                    .Select(x => new ArrayDisarrayStateDto(x.Grid
                        .Select(y => y
                            .Select(z => (ArrayDisarrayTileSetDto)z)
                            .ToArray())
                        .ToArray(), x.Multipliers))
                    .ToList()
            };
        }

        return errors
            .Match<OneOf<ArrayDisarraySpinDto, UserNotFound, InsufficientFundsError,
                GamblingMachineNotFound>>(
                x => x, y => y);

    }
}