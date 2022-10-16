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
public record SpinThreeArmedBanditQuery(decimal Amount) : IRequest<OneOf<SpinResultsDto<ThreeArmedBanditTileSetDto>, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>;

public class SpinThreeArmedBanditQueryHandler : IRequestHandler<SpinThreeArmedBanditQuery,
    OneOf<SpinResultsDto<ThreeArmedBanditTileSetDto>, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>
{
    private readonly IAuthService _authService;
    private readonly IAppDbContext _dbContext;
    private readonly IGamblingRepository _gamblingRepository;

    public SpinThreeArmedBanditQueryHandler(IAuthService authService, IAppDbContext dbContext, IGamblingRepository gamblingRepository)
    {
        _authService = authService;
        _dbContext = dbContext;
        _gamblingRepository = gamblingRepository;
    }

    public async Task<OneOf<SpinResultsDto<ThreeArmedBanditTileSetDto>, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>> Handle(SpinThreeArmedBanditQuery request, CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUser(cancellationToken);
        if (user is null) return new UserNotFound();
        _dbContext.Users.Attach(user);

        var result = await _gamblingRepository
            .Gamble<ThreeLeggedBanditTileSet, ThreeArmedBanditTileSetDto>(request.Amount, user, cancellationToken);
        return result
            .Match<OneOf<SpinResultsDto<ThreeArmedBanditTileSetDto>, UserNotFound, InsufficientFundsError, GamblingMachineNotFound>>(
                x => x, 
                x => x, 
                x=> x
            );
    }
}