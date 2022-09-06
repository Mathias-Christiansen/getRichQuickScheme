using Application;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class AuthorizationService : IAuthService
{
    private readonly IAppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;
    private const string AuthHeader = "Bearer";

    public AuthorizationService(IAppDbContext dbContext, IHttpContextAccessor httpContext)
    {
        _dbContext = dbContext;
        _httpContext = httpContext;
    }
    
    public async Task<User?> GetCurrentUser(CancellationToken cancellationToken = default)
    {
        if (!_httpContext.HttpContext.Request.Headers.TryGetValue(AuthHeader, out var bearerToken)) return null;
        
        var token = bearerToken.FirstOrDefault();
        if (token is null) return null;
        
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Secret != null && x.Secret.Bearer == token, cancellationToken);
        if (user?.Secret != null && user.Secret.Compare(token)) return user;
        
        return null;

    }

    public async Task<bool> ValidateCredentials(CancellationToken cancellationToken = default)
    {
        return await GetCurrentUser(cancellationToken) is not null;
    }
}