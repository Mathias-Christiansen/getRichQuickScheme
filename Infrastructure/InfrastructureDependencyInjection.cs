using System.Reflection;
using Application;
using Application.Services;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dbString = configuration.GetConnectionString("DbConnection")!;
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(dbString, x =>
            {
                x.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            });
        });
        services.AddScoped<IAppDbContext>(x => x.GetRequiredService<AppDbContext>());
        AddServices(services);
        return services;
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAuthService, AuthorizationService>();
    }
}