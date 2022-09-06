using Application.Pipelines;
using Contracts.Errors;
using Contracts.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Application.Features.Users;

[Access(AccessLevels.LoggedIn)]
public record GetUserQuery(Guid UserId): IRequest<OneOf<UserDto, UserNotFound>>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, OneOf<UserDto, UserNotFound>>
{
    private readonly IAppDbContext _dbContext;

    public GetUserQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<OneOf<UserDto, UserNotFound>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        if (user is null)
        {
            return new UserNotFound();
        }

        return new UserDto()
        {
            Email = user.Email.Email,
            Id = user.Id,
            Name = user.Name
        };
    }
}