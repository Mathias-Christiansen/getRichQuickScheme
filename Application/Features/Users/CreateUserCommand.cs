using System.Drawing;
using Contracts.Errors;
using Domain.Entities;
using Domain.ValueObj;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Application.Features.Users;

public record CreateUserCommand(Guid Id, string Name, string Email, string Password) : IRequest<OneOf<Unit, UserEmailAlreadyExistsError>>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, OneOf<Unit, UserEmailAlreadyExistsError>>
{
    private readonly IAppDbContext _dbContext;

    public CreateUserCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<OneOf<Unit, UserEmailAlreadyExistsError>> Handle(
        CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email.Email == request.Email, cancellationToken);
        if (existingUser is not null)
            return new UserEmailAlreadyExistsError(request.Email);
        
        var password = Password.Create(request.Password);
        var email = EmailAddress.Create(request.Email);
        var user = User.Create(request.Id, email, password, request.Name);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}