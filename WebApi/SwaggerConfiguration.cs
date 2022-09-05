using Microsoft.OpenApi.Models;

namespace WebApi;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Version = "v1",
                Title = "Million dollar idea",
                Description = "who cares"
            });
        });
        return services;
    }

    public static WebApplication ConfigureSwag(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(x =>
        {
            x.SwaggerEndpoint("v1/swagger.json", "v1");
        });
        return app;
    }
}