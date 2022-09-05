using System.Reflection;
using Application;
using Infrastructure.Persistence;
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
        return services;
    }
}