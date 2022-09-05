using Contracts.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Application.Features.Users;

public record UserLoginCommand(string Email, string Password) : IRequest<OneOf<string, UserNotFound, IncorrectPasswordError>>;

public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, OneOf<string, UserNotFound, IncorrectPasswordError>>
{
    private readonly IAppDbContext _dbContext;

    public UserLoginCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OneOf<string, UserNotFound, IncorrectPasswordError>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email.Email == request.Email, cancellationToken);
        if (user is null)
        {
            return new UserNotFound();
        }

        if (!user.Password.Compare(request.Password)) return new IncorrectPasswordError();
        var secret = user.GenerateSecret();
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return secret.Bearer;

    }
}