using Domain.Entities;

namespace Application.Services;

public interface IAuthService
{
    public Task<User?> GetCurrentUser(CancellationToken cancellationToken = default);

    public Task<bool> ValidateCredentials(CancellationToken cancellationToken = default);
}