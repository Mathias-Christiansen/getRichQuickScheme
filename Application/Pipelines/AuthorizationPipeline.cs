using System.Reflection;
using System.Security.Authentication;
using Application.Services;
using MediatR;

namespace Application.Pipelines;

public class AuthorizationPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    private readonly IAuthService _authService;

    public AuthorizationPipeline(IAuthService authService)
    {
        _authService = authService;
    }
    
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var accessAttribute = request.GetType().GetCustomAttribute<AccessAttribute>();
        if (accessAttribute is null) 
            throw new AuthenticationException(
                $"actually authorization exception but who cares, no access lvl specified on {request.GetType().FullName}");
        var valid = await _authService.ValidateCredentials(cancellationToken);
        switch (accessAttribute.Lvl)
        {
            case AccessLevels.LoggedIn:
                if (!valid)
                    throw new AuthenticationException(
                        $"you must be logged in to access method {request.GetType().FullName}");
                break;
            case AccessLevels.NotLoggedIn:
                if (valid)
                    throw new AuthenticationException(
                        $"you cannot access method {request.GetType().FullName} while logged in");
                break;
            case AccessLevels.AlwaysAvailable:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return await next.Invoke();
    }
}

public enum AccessLevels
{
    LoggedIn = 0, 
    NotLoggedIn = 1,
    AlwaysAvailable = 2,
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AccessAttribute : Attribute
{
    public AccessLevels Lvl { get; }

    public AccessAttribute(AccessLevels lvl)
    {
        Lvl = lvl;
    }
}